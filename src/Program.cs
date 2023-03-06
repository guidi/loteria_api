
using Loteria.API.Data.Context;
using Loteria.API.Extension;
using Loteria.API.Service;
using Microsoft.EntityFrameworkCore;
using Polly;
using Serilog;

namespace Loteria.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<LoteriaContext>(opt =>
            {
                var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

                var retryPolicy = Policy.Handle<MySql.Data.MySqlClient.MySqlException>()
                    .WaitAndRetry(new[]
                    {
                        TimeSpan.FromSeconds(1),
                        TimeSpan.FromSeconds(2),
                        TimeSpan.FromSeconds(3)
                    });

                retryPolicy.Execute(() => opt.UseMySQL(connectionString));
            });

            SerilogExtension.AddSerilogApi(builder.Configuration);
            builder.Host.UseSerilog(Log.Logger);
            builder.Services.AddHttpClient<ILoteriaService, LoteriaService>();
            var app = builder.Build();
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

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
    }
}