using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Exceptions;
using Serilog.Formatting.Elasticsearch;

namespace frontend.values.web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    var env = hostingContext.HostingEnvironment;
                    config.AddJsonFile($"appsettings.json", true, true); // First, AppSettings (general)
                    config.AddJsonFile($"appsettings.{env.EnvironmentName}.json", true, true); // Then, AppSettings (specific environment)
                    config.AddEnvironmentVariables(); // Then, Environment variables
                    if (args != null)
                        config.AddCommandLine(args); // Then, CommandLine arguments
                })
                .UseSerilog((ctx, config) =>
                {
                    var shouldFormatElastic = ctx.Configuration.GetValue<bool>("LOG_ELASTICFORMAT", false);
                    config
                        .MinimumLevel.Information()
                        .Enrich.FromLogContext() // TODO: See what difference this makes!
                        .Enrich.WithExceptionDetails()
                        ;

                    if (shouldFormatElastic)
                        config.WriteTo.Console(new ExceptionAsObjectJsonFormatter(renderMessage: true));
                    else
                        config.WriteTo.Console();

                })
                .UseStartup<Startup>();
    }
}
