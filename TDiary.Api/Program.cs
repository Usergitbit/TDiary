using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace TDiary.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseKestrel((context, options) =>
                    {
                        var ipVariable = Environment.GetEnvironmentVariable("Ip");
                        var portVariable = Environment.GetEnvironmentVariable("Port");

                        if(string.IsNullOrWhiteSpace(portVariable))
                        {
                            portVariable = "5002";
                        }
                        var port = int.Parse(portVariable);

                        if (ipVariable == "localhost")
                        {
                            options.ListenLocalhost(port);
                        }
                        else
                        {
                            var ip = IPAddress.Parse(ipVariable);
                            options.Listen(ip, port);
                        }

                    });
                });
    }
}
