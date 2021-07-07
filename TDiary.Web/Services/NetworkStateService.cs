using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TDiary.Web.Services
{
    public class NetworkStateService
    {
        private readonly IJSRuntime jsRuntime;

        public NetworkStateService(IJSRuntime jsRuntime)
        {
            this.jsRuntime = jsRuntime;
        }
        public async Task<bool> IsOnline()
        {
            var result = await jsRuntime.InvokeAsync<bool>("tdiaryFunctions.isOnline");

            return result;

        }
    }
}
