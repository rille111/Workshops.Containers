using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

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
                .UseStartup<Startup>()
                .ConfigureLogging((ctx, l) =>
                {
                    l.ClearProviders();
                    l.AddConfiguration(ctx.Configuration.GetSection("Logging"));
                    l.AddConsole();
                    l.AddDebug();
                })
        ;
    }
}
