using Microsoft.AspNetCore.Hosting;
using System;
using System.Net;

namespace TDiary.Api.Extensions
{
    public static class EnvironmentExtensions
    {
        public static bool IsDevelopment()
        {
            var environmentVariable = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var isDev = !string.IsNullOrWhiteSpace(environmentVariable) && environmentVariable.ToLower() == "development";

            return isDev;
        }

        public static IWebHostBuilder SetupDevelopment(this IWebHostBuilder webHostBuilder, AppSettings.AppSettings appSettings)
        {
            webHostBuilder.UseUrls($"https://localhost:{appSettings.Server.Port}");

            return webHostBuilder;
        }

        public static IWebHostBuilder SetupProduction(this IWebHostBuilder webHostBuilder, AppSettings.AppSettings appSettings)
        {
            var ipString = appSettings.Server.Ip;
            if (string.IsNullOrWhiteSpace(ipString) || ipString == "locahost")
            {
                webHostBuilder.UseKestrel((context, options) =>
                {
                    options.ListenLocalhost(appSettings.Server.Port, listenOptions =>
                    {
                        listenOptions.UseHttps(appSettings.Server.SSLCertificateName, appSettings.Server.SSLCertificatePassword);
                    });
                });
            }
            else
            {
                var ip = IPAddress.Parse(ipString);
                webHostBuilder.UseKestrel((context, options) =>
                {
                    options.Listen(ip, appSettings.Server.Port, listenOptions =>
                    {
                        listenOptions.UseHttps(appSettings.Server.SSLCertificateName, appSettings.Server.SSLCertificatePassword);
                    });

                });
            }

            return webHostBuilder;
        }

    }
}
