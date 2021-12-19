using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication.Internal;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using TDiary.Web.Services;

namespace TDiary.Web
{
    public class OfflineAccountClaimsPrincipalFactory : AccountClaimsPrincipalFactory<RemoteUserAccount>
    {
        private readonly IServiceProvider services;

        public OfflineAccountClaimsPrincipalFactory(IServiceProvider services, IAccessTokenProviderAccessor accessor) : base(accessor)
        {
            this.services = services;
        }

        public override async ValueTask<ClaimsPrincipal> CreateUserAsync(RemoteUserAccount account, RemoteAuthenticationUserOptions options)
        {
            var localUserStore = services.GetRequiredService<LocalUserStore>();

            var result = await base.CreateUserAsync(account, options);
            if (result.Identity.IsAuthenticated)
            {
                await localUserStore.SaveUserAccountAsync(result);
            }
            else
            {
                result = await localUserStore.LoadUserAccountAsync();
            }


            return result;
        }
    }
}
