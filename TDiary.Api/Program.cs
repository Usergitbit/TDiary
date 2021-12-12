using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using TDiary.Api.Extensions;

namespace TDiary.Api
{
    public class Program
    {
        public static string DefaultConnection { get; set; }
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    var configuration = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json", false, true)
                        .AddJsonFile($"appsettings.Development.json", optional: true, true)
                        .AddEnvironmentVariables()
                        .Build();

                    var appSettings = configuration.Get<AppSettings.AppSettings>();
                    var envvar = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                    var isDev = envvar != null && envvar.ToLower() == "development";
                    if (isDev)
                    {
                        webBuilder.SetupDevelopment(appSettings);
                    }
                    else
                    {
                        webBuilder.SetupProduction(appSettings);
                    }
                    webBuilder.UseStartup<Startup>();
                });

            return host;
        }
    }
}
