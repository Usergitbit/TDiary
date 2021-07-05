using Blazored.LocalStorage;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace TDiary.Web.Services
{
    public class LocalUserStore
    {
        private readonly ILocalStorageService localStorage;

        public LocalUserStore(ILocalStorageService localStorage)
        {
            this.localStorage = localStorage;
        }
        public ValueTask SaveUserAccountAsync(ClaimsPrincipal user)
        {
            return user != null
                ? PutAsync("userAccount", user.Claims.Select(c => new ClaimData { Type = c.Type, Value = c.Value }))
                : DeleteAsync("userAccount");
        }

        public async Task<ClaimsPrincipal> LoadUserAccountAsync()
        {
            var storedClaims = (await GetAsync<ClaimData[]>("userAccount"))?.ToList();
            var nameClaim = storedClaims?.FirstOrDefault(x => x.Type == "name");
            if (nameClaim != null)
            {
                storedClaims.Add(new ClaimData
                {
                    Type = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name",
                    Value = nameClaim.Value
                });
            }
            return storedClaims != null
                ? new ClaimsPrincipal(new ClaimsIdentity(storedClaims.Select(c => new Claim(c.Type, c.Value)), "oidc"))
                : new ClaimsPrincipal(new ClaimsIdentity());
        }

        ValueTask<T> GetAsync<T>(string key)
        {
            return localStorage.GetItemAsync<T>(key);
        }

        ValueTask PutAsync<T>(string key, T value)
        {
            return localStorage.SetItemAsync(key, value);
        }

        ValueTask DeleteAsync(string key)
        {
            return localStorage.RemoveItemAsync(key);
        }

        class ClaimData
        {
            public string Type { get; set; }
            public string Value { get; set; }
        }
    }
}
