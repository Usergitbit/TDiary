using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using TDiary.Grpc.Protos;
using TDiary.Web.Services;
using Grpc.Net.Client.Web;
using Blazored.LocalStorage;
using TG.Blazor.IndexedDB;
using TDiary.Common.ServiceContracts;
using TDiary.Web.IndexedDB.SchemaBuilder;
using TDiary.Common.Models.Entities;
using TDiary.Web.IndexedDB;
using TDiary.Web.Services.Interfaces;
using TDiary.Common.ServiceContracts.Implementations;
using TDiary.Grpc.ServiceContracts;
using TDiary.Grpc.ServiceContracts.Implementations;

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

            builder.Services.AddGrpcClient<PingProto.PingProtoClient>(o =>
            {
                o.Address = new Uri("https://localhost:5002");
            }).ConfigurePrimaryHttpMessageHandler(() =>
            {
                var grcpWebHandler = new GrpcWebHandler(GrpcWebMode.GrpcWeb);
                grcpWebHandler.InnerHandler = new HttpClientHandler();

                return grcpWebHandler;
            });

            // scoped doesn't do anything and functions as single but the indexDb library registers itself as scoped 
            // so we need to make anything that uses it scoped as well
            builder.Services.AddScoped<IEventService, EventService>();
            builder.Services.AddScoped<IEntityQueryService, EntityQueryService>();
            builder.Services.AddScoped<IEventPlayerService, EventPlayerService>();
            builder.Services.AddSingleton<NetworkStateService>();
            builder.Services.AddScoped<ISynchronizationService, SynchronizationService>();
            builder.Services.AddSingleton<IUpdateEventMergerService, UpdateEventMergerService>();
            builder.Services.AddScoped<IEntityRelationsValidatorService, EntityRelationsValidatorService>();
            builder.Services.AddSingleton<IMergeService, MergeService>();
            builder.Services.AddSingleton<IManualGrpcMapper, ManualGrpcMapper>();


            builder.Services.AddIndexedDB(dbStore =>
            {
                dbStore.DbName = "TDiary";
                //todo: get from somewhere to do comparisons for recreation
                dbStore.Version = 1;

                var eventsSchema = new SchemaBuilder<Event>()
                    .StoreName(StoreNameConstants.Events)
                    .BaseProperties()
                    .Property("entity")
                    .Property("eventType")
                    .Property("version")
                    .Property("data")
                    .Property("entityId")
                    .Property("initialData")
                    .Property("changes")
                    .Build();

                var unsynchronizedEventsSchema = new SchemaBuilder<Event>()
                    .StoreName(StoreNameConstants.UnsynchronizedEvents)
                    .BaseProperties()
                    .Property("entity")
                    .Property("eventType")
                    .Property("version")
                    .Property("data")
                    .Property("entityId")
                    .Property("initialData")
                    .Property("changes")
                    .Build();

                var brandsSchema = new SchemaBuilder<Brand>()
                    .StoreName(StoreNameConstants.Brands)
                    .BaseProperties()
                    .Property("name")
                    .Build();

                dbStore.Stores.AddRange(new[] { eventsSchema, brandsSchema, unsynchronizedEventsSchema });
            });

            await builder.Build().RunAsync();

        }
    }
}
