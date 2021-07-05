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
using Blazored.LocalStorage;
using TG.Blazor.IndexedDB;
using TDiary.Common.ServiceContracts;
using TDiary.Web.IndexedDB.SchemaBuilder;
using TDiary.Common.Models.Entities;
using TDiary.Web.IndexedDB;

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
            builder.Services.AddScoped<LocalUserStore>();

            builder.Services.AddBlazoredLocalStorage();

            builder.Services.AddOidcAuthentication(options =>
            {
                builder.Configuration.Bind("oidc", options.ProviderOptions);
            }).AddAccountClaimsPrincipalFactory<OfflineAccountClaimsPrincipalFactory>();

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
            builder.Services.AddScoped<IBrandService, BrandService>();


            builder.Services.AddIndexedDB(dbStore =>
            {
                dbStore.DbName = "TDiary"; 
                dbStore.Version = 1;

                var eventsSchema = new SchemaBuilder<Event>()
                    .StoreName(StoreNameConstants.Events)
                    .BaseProperties()
                    .Property("entity")
                    .Property("eventType")
                    .Property("version")
                    .Property("data")
                    .Build();

                var brandsSchema = new SchemaBuilder<Brand>()
                    .StoreName(StoreNameConstants.Brands)
                    .BaseProperties()
                    .Property("name")
                    .Build();

                dbStore.Stores.AddRange(new[] { eventsSchema, brandsSchema });
            });

            await builder.Build().RunAsync();

        }
    }
}
