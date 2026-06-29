using Serilog;
using Serilog.Enrichers.Span;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Filters;

namespace Loteria.API.Extension
{
    public static class SerilogExtension
    {
        public static void AddSerilogApi(IConfiguration configuration)
        {
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";
            var seqUrl = Environment.GetEnvironmentVariable("SEQ_URL");
            var seqApiKey = Environment.GetEnvironmentVariable("SEQ_API_KEY");

            var logCfg = new LoggerConfiguration()
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .Enrich.WithExceptionDetails()
                .Enrich.WithMachineName()
                .Enrich.WithSpan()
                .Enrich.WithCorrelationId()
                .Enrich.WithProperty("Service", "Loteria.API")
                .Enrich.WithProperty("ApplicationName", $"API Loteria - {environmentName}")
                .Filter.ByExcluding(Matching.FromSource("Microsoft.AspNetCore.StaticFiles"))
                .Filter.ByExcluding(logEvent => logEvent.MessageTemplate.Text.Contains("Business error"))
                .ReadFrom.Configuration(configuration)
                .WriteTo.Async(wt => wt.Console(
                    outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}"));

            if (!string.IsNullOrWhiteSpace(seqUrl))
            {
                logCfg = logCfg.WriteTo.Async(wt => wt.Seq(
                    serverUrl: seqUrl,
                    apiKey: string.IsNullOrWhiteSpace(seqApiKey) ? null : seqApiKey));
            }

            Log.Logger = logCfg.CreateLogger();

            if (string.IsNullOrWhiteSpace(seqUrl))
            {
                Log.Warning("SEQ_URL nao foi configurada. Logs nao serao enviados para o Seq.");
            }
        }
    }
}
