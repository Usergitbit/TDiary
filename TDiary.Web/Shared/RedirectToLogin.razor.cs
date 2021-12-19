using Microsoft.AspNetCore.Components;
using System;

namespace TDiary.Web.Shared
{
    public partial class RedirectToLogin
    {
        [Inject]
        public NavigationManager NavigationManager { get; set; }
        protected override void OnInitialized()
        {
            NavigationManager.NavigateTo($"authentication/login?returnUrl={Uri.EscapeDataString(NavigationManager.Uri)}");
        }
    }
}
