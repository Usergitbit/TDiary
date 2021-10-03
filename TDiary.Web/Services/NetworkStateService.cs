using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TDiary.Grpc.Protos;

namespace TDiary.Web.Services
{
    public class NetworkStateService
    {
        private readonly IJSRuntime jsRuntime;
        private readonly PingProto.PingProtoClient pingClient;

        public NetworkStateService(IJSRuntime jsRuntime,
            PingProto.PingProtoClient pingClient)
        {
            this.jsRuntime = jsRuntime;
            this.pingClient = pingClient;
        }

        public async Task<bool> IsOnline()
        {
            var result = await jsRuntime.InvokeAsync<bool>("tdiaryFunctions.isOnline");

            return result;

        }

        public async Task<bool> IsApiOnline()
        {
            try
            {
                // TODO: make this configurable
                await pingClient.PingAsync(new PingRequest(), deadline: DateTime.UtcNow.AddSeconds(0.25));
                return true;
            }
            catch(Exception Ex)
            {
                return false;
            }
        }
    }
}
