using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using System.Threading.Tasks;
using TDiary.Web.Services;

namespace TDiary.Web.Shared
{
    public partial class LoginDisplay
    {
        [Inject]
        public SignOutSessionStateManager SignOutSessionStateManager { get; set; }
        [Inject]
        public NavigationManager NavigationManager { get; set; }
        [Inject]
        public LocalUserStore LocalUserStore { get; set; }

        private async Task BeginSignOut(MouseEventArgs args)
        {
            await SignOutSessionStateManager.SetSignOutState();
            await LocalUserStore.SaveUserAccountAsync(null);
            NavigationManager.NavigateTo("authentication/logout");
        }

        private void BeginSignIn(MouseEventArgs args)
        {
            NavigationManager.NavigateTo("authentication/login");
        }
    }
}
