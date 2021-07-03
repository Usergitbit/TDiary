using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using TDiary.Grpc.Protos;
using TDiary.Automapper;
using TDiary.Web.Services;
using Grpc.Core.Interceptors;
using Grpc.Core;
using Grpc.Net.Client.Web;

namespace TDiary.Web
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            // We register a named HttpClient here for the API
            builder.Services.AddHttpClient("api")
                .AddHttpMessageHandler(sp =>
                {
                    var handler = sp.GetService<AuthorizationMessageHandler>()
                        .ConfigureHandler(new[] { "https://localhost:5002" }, new[] { "tdiaryapi.full" });
                    return handler;
                });

            builder.Services.AddScoped(sp => sp.GetService<IHttpClientFactory>().CreateClient("api"));

            builder.Services.AddOidcAuthentication(options =>
            {
                builder.Configuration.Bind("oidc", options.ProviderOptions);
            });

            builder.Services.AddAutoMapper(typeof(EventProfile).Assembly);

            builder.Services.AddGrpcClient<EventProto.EventProtoClient>(o =>
            {
                o.Address = new Uri("https://localhost:5002");
            }).ConfigurePrimaryHttpMessageHandler((services) =>
            {
                var handler = services.GetService<AuthorizationMessageHandler>()
                  .ConfigureHandler(new[] { "https://localhost:5002" }, new[] { "tdiaryapi.full" });
                handler.InnerHandler = new HttpClientHandler();
                return new GrpcWebHandler(GrpcWebMode.GrpcWeb, handler);
            });
            builder.Services.AddScoped<EventService>();
            await builder.Build().RunAsync();

        }
    }
}
