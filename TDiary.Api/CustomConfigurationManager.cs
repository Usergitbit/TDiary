using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using TDiary.Api.AppSettings;

namespace TDiary.Api
{
    public class CustomConfigurationManager : IConfigurationManager<OpenIdConnectConfiguration>
    {
        private readonly Authorization authorization;

        public CustomConfigurationManager(Authorization authorization)
        {
            this.authorization = authorization;
        }
        public async Task<OpenIdConnectConfiguration> GetConfigurationAsync(CancellationToken cancel)
        {
            var httpClient = new HttpClient();

            var request = new HttpRequestMessage
            {
                RequestUri = new Uri($"{authorization.Authority}/.well-known/openid-configuration"),
                Method = HttpMethod.Get
            };

            var configurationResult = await httpClient.SendAsync(request, cancel);
            var resultContent = await configurationResult.Content.ReadAsStringAsync(cancel);
            if (configurationResult.IsSuccessStatusCode)
            {
                var config = OpenIdConnectConfiguration.Create(resultContent);
                var jwks = config.JwksUri.Replace(authorization.AuthorityOrigin, authorization.Authority);
                var keyRequest = new HttpRequestMessage
                {
                    RequestUri = new Uri(jwks),
                    Method = HttpMethod.Get
                };
                var keysResposne = await httpClient.SendAsync(keyRequest, cancel);
                if (keysResposne.IsSuccessStatusCode)
                {
                    var keysJson = await keysResposne.Content.ReadAsStringAsync(cancel);
                    config.JsonWebKeySet = new JsonWebKeySet(keysJson);
                    var signingKeys = config.JsonWebKeySet.GetSigningKeys();
                    foreach (var key in signingKeys)
                    {
                        config.SigningKeys.Add(key);
                    }
                }

                return config;
            }
            else
            {
                throw new Exception($"Failed to get jwt configuration: {configurationResult.StatusCode}: {resultContent}");
            }
        }

        public void RequestRefresh()
        {
            // if you are caching the configuration this is probably where you should invalidate it
        }
    }
}
