using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;
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
                        .ReadFrom.Configuration(ctx.Configuration) // Read from appsettings and env, cmdline
                        .Enrich.FromLogContext()
                        .Enrich.WithExceptionDetails();

                    var logFormatter = new ExceptionAsObjectJsonFormatter(renderMessage: true);
                    var logMessageTemplate = "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}";

                    if (shouldFormatElastic)
                        config.WriteTo.Console(logFormatter, standardErrorFromLevel: LogEventLevel.Error);
                    else
                        config.WriteTo.Console(standardErrorFromLevel: LogEventLevel.Error, outputTemplate: logMessageTemplate);

                })
                .UseStartup<Startup>();
    }
}
