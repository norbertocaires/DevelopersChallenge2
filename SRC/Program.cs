using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nibo.Data;

namespace Nibo {
    public class Program {
        public static void Main(string[] args) {
            var host = BuildWebHost(args);
            using (var scope = host.Services.CreateScope()) {
                var services = scope.ServiceProvider;
                var context = services.GetRequiredService<ApplicationDbContext>();
            }
            host.Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .ConfigureAppConfiguration((WebHostBuilderContext, ConfigurationBuilder) => {
                    var env = WebHostBuilderContext.HostingEnvironment;
                    ConfigurationBuilder
                        .AddJsonFile("appsettings.json", true)
                        .AddEnvironmentVariables();
                })
                .ConfigureLogging(logging => {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                })
                .Build();
    }
}
