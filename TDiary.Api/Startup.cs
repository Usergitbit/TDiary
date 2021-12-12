using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using TDiary.Api.AppSettings;
using TDiary.Api.Grpc;
using TDiary.Api.Services;
using TDiary.Api.Services.Interfaces;
using TDiary.Api.Validators;
using TDiary.Common.ServiceContracts;
using TDiary.Database;
using TDiary.Grpc.ServiceContracts;
using TDiary.Grpc.ServiceContracts.Implementations;

namespace TDiary.Api
{
    public class Startup
    {
        private readonly IConfiguration configuration;


        public Startup(IConfiguration configuration)
        {
            this.configuration = configuration;
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var appSettings = configuration.Get<AppSettings.AppSettings>();
            var connection = appSettings.ConnectionStrings.DefaultConnection;
            services.AddDbContext<TDiaryDatabaseContext>(options =>
            {
                options.UseNpgsql(connection);
                options.EnableSensitiveDataLogging();
                options.EnableDetailedErrors();
            });

            services.AddScoped<IEventService, EventService>();
            services.AddScoped<IManualGrpcMapper, ManualGrpcMapper>();

            services.AddSingleton<EventValidator>();

            services.AddCors(options =>
            {
                options.AddPolicy("TDiary",
                                  builder =>
                                  {
                                      builder.WithOrigins(appSettings.Cors.Origins)
                                        .AllowAnyMethod()
                                        .AllowAnyHeader()
                                        .AllowCredentials()
                                        .WithExposedHeaders("Grpc-Status", "Grpc-Message", "Grpc-Encoding", "Grpc-Accept-Encoding");
                                  });
            });


            services.AddControllers();
            services.AddGrpc();
            services.AddGrpcReflection();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.Authority = appSettings.Authorization.Authority;
                    options.TokenValidationParameters.ValidTypes = new[] { "at+jwt" };
                    options.TokenValidationParameters.ValidateAudience = false;

                    #region DANGER: Ignore SSL erros
                    //var handler = new HttpClientHandler
                    //{
                    //    ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, cetChain, policyErrors) =>
                    //    {
                    //        return true;
                    //    },
                    //};
                    //var backchannel = new HttpClient(handler);
                    //backchannel.DefaultRequestHeaders.UserAgent.ParseAdd("Microsoft ASP.NET Core JwtBearer handler");
                    //backchannel.Timeout = options.BackchannelTimeout;
                    //backchannel.MaxResponseContentBufferSize = 1024 * 1024 * 10; // 10 MB
                    //var configurationManager = new ConfigurationManager<OpenIdConnectConfiguration>($"{options.Authority}/.well-known/openid-configuration", new OpenIdConnectConfigurationRetriever(),
                    //    new HttpDocumentRetriever(backchannel) { RequireHttps = options.RequireHttpsMetadata });
                    //options.ConfigurationManager = configurationManager;
                    #endregion

                    #region Custom configuration manager to solve SSL issues without ignoring
                    //options.ConfigurationManager = new CustomConfigurationManager(appSettings.Authorization);
                    #endregion
                });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "TDiary API", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    { new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            services.AddHsts(options =>
            {
                options.Preload = true;
                options.IncludeSubDomains = true;
                options.MaxAge = TimeSpan.FromDays(365);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseSwagger(c =>
            {
                //where to put the swagger generated json, the document name MUST be there and indicates the version
                c.RouteTemplate = "tdiary/swagger/{documentName}/swagger.json";
            });
            app.UseSwaggerUI(c =>
            {
                //by default /swagger to open swagger ui, this is used to change it
                c.RoutePrefix = "tdiary";
                // this must pint to the above route tempalte but with a given document name
                c.SwaggerEndpoint("/tdiary/swagger/v1/swagger.json", "TDiary.Api v1");
            });

            var context = serviceProvider.GetRequiredService<TDiaryDatabaseContext>();
            context?.Database.Migrate();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseGrpcWeb(new GrpcWebOptions { DefaultEnabled = true });

            app.UseCors("TDiary");

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapGrpcService<EventRpc>().EnableGrpcWeb().RequireCors("TDiary");
                endpoints.MapGrpcService<PingRpc>().EnableGrpcWeb().RequireCors("TDiary");
                if (env.IsDevelopment())
                {
                    endpoints.MapGrpcReflectionService();
                }
            });
        }
    }
}
