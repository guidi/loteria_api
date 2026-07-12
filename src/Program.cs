using Loteria.API.Data.Context;
using Loteria.API.Exceptions;
using Loteria.API.Extension;
using Loteria.API.Service;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Polly;
using Serilog;
using Serilog.Context;
using Serilog.Events;
using System.Net;
using System.Threading.RateLimiting;

namespace Loteria.API
{
    public class Program
    {
        private const string RequestLogLevelItemKey = "RequestLogLevel";

        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddProblemDetails();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddRateLimiter(options =>
            {
                options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
                {
                    var clientIp = context.ObterIpOrigem();
                    var partitionKey = string.IsNullOrWhiteSpace(clientIp) ? "unknown" : clientIp;

                    return RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey,
                        _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 5,
                            Window = TimeSpan.FromSeconds(10),
                            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                            QueueLimit = 0,
                            AutoReplenishment = true
                        });
                });

                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
                options.OnRejected = async (context, cancellationToken) =>
                {
                    var clientIp = context.HttpContext.ObterIpOrigem();

                    Log.Warning(
                        "Rate limit exceeded for IP {ClientIp} on {RequestPath}.",
                        clientIp,
                        context.HttpContext.Request.Path);

                    await context.HttpContext.Response.WriteAsync("Too Many Requests", cancellationToken);
                };
            });
            builder.Services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedHost;
                options.ForwardLimit = 1;

                var forwardedNetworks = Environment.GetEnvironmentVariable("FORWARDED_NETWORKS")
                    ?? "172.18.0.0/16,172.19.0.0/16";

                foreach (var cidr in forwardedNetworks.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                {
                    var parts = cidr.Split('/', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                    if (parts.Length == 2 && IPAddress.TryParse(parts[0], out var ip) && int.TryParse(parts[1], out var prefix))
                    {
                        options.KnownIPNetworks.Add(new System.Net.IPNetwork(ip, prefix));
                    }
                }
            });

            builder.Services.AddDbContext<LoteriaContext>(opt =>
            {
                var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                    ?? throw new InvalidOperationException("ConnectionStrings:DefaultConnection nao foi configurada.");

                var retryPolicy = Policy.Handle<MySql.Data.MySqlClient.MySqlException>()
                    .WaitAndRetry(new[]
                    {
                        TimeSpan.FromSeconds(1),
                        TimeSpan.FromSeconds(2),
                        TimeSpan.FromSeconds(3)
                    });

                retryPolicy.Execute(() => opt.UseMySQL(connectionString));
            });

            builder.Services.AddHttpClient<ILoteriaService, LoteriaService>(client =>
            {
                client.Timeout = TimeSpan.FromSeconds(12);
            });

            SerilogExtension.AddSerilogApi(builder.Configuration);
            builder.Host.UseSerilog(Log.Logger);
            var app = builder.Build();
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseForwardedHeaders();
            app.Use(async (context, next) =>
            {
                using (LogContext.PushProperty("ClientIp", context.ObterIpOrigem()))
                using (LogContext.PushProperty("CloudflareConnectingIp", context.Request.Headers["CF-Connecting-IP"].ToString()))
                using (LogContext.PushProperty("XForwardedFor", context.Request.Headers["X-Forwarded-For"].ToString()))
                using (LogContext.PushProperty("CloudflareCountry", ObterPaisCloudflare(context)))
                {
                    await next();
                }
            });
            app.UseSerilogRequestLogging(options =>
            {
                options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
                {
                    diagnosticContext.Set("ClientIp", httpContext.ObterIpOrigem());
                    diagnosticContext.Set("UserAgent", httpContext.Request.Headers.UserAgent.ToString());
                    diagnosticContext.Set("CloudflareCountry", ObterPaisCloudflare(httpContext));
                };
                options.GetLevel = (httpContext, _, exception) =>
                {
                    if (httpContext.Items.TryGetValue(RequestLogLevelItemKey, out var requestLogLevel)
                        && requestLogLevel is LogEventLevel logEventLevel)
                    {
                        return logEventLevel;
                    }

                    if (exception != null || httpContext.Response.StatusCode >= StatusCodes.Status500InternalServerError)
                    {
                        return LogEventLevel.Error;
                    }

                    return LogEventLevel.Information;
                };
            });
            app.Use(async (context, next) =>
            {
                try
                {
                    await next();
                }
                catch (Exception exception)
                {
                    var erroTratado = MapearErroTratado(exception);

                    context.Items[RequestLogLevelItemKey] = erroTratado.RequestLogLevel;

                    if (exception is LoteriaApiException)
                    {
                        Log.Warning(
                            exception,
                            "Erro tratado ao processar {RequestPath}. Status {StatusCode}.",
                            context.Request.Path,
                            erroTratado.StatusCode);
                    }
                    else if (exception != null)
                    {
                        Log.Error(
                            exception,
                            "Erro nao tratado ao processar {RequestPath}. Status {StatusCode}.",
                            context.Request.Path,
                            erroTratado.StatusCode);
                    }

                    if (!context.Response.HasStarted)
                    {
                        context.Response.StatusCode = erroTratado.StatusCode;
                        context.Response.ContentType = "application/problem+json";

                        await context.Response.WriteAsJsonAsync(new ProblemDetails
                        {
                            Status = erroTratado.StatusCode,
                            Title = erroTratado.Title,
                            Detail = erroTratado.Detail,
                            Instance = context.Request.Path
                        });
                    }
                }
            });
            app.UseRateLimiter();
            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                var context = services.GetRequiredService<LoteriaContext>();
                context.Database.Migrate();
            }

            app.Run();
        }

        private static string ObterPaisCloudflare(HttpContext context)
        {
            return context.Request.Headers["CF-IPCountry"].ToString();
        }

        private static (int StatusCode, string Title, string Detail, LogEventLevel RequestLogLevel) MapearErroTratado(Exception? exception)
        {
            if (exception is LoteriaApiException loteriaApiException)
            {
                return (
                    loteriaApiException.StatusCode,
                    loteriaApiException.Title,
                    loteriaApiException.Message,
                    LogEventLevel.Warning);
            }

            if (exception is BadHttpRequestException)
            {
                return (
                    StatusCodes.Status400BadRequest,
                    "Requisicao invalida",
                    "A requisicao enviada e invalida.",
                    LogEventLevel.Warning);
            }

            return (
                StatusCodes.Status500InternalServerError,
                "Erro interno",
                "Ocorreu um erro interno ao processar a requisicao.",
                LogEventLevel.Error);
        }
    }
}
