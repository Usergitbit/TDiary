using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TDiary.Api.Extensions
{
    public static class ServerCallContextExtensions
    {
        public static Guid GetUserId(this ServerCallContext serverCallContext)
        {
            var userIdString = serverCallContext.GetHttpContext().User.Claims.FirstOrDefault(c => c.Type == "id").Value;
            var userId = Guid.Parse(userIdString);

            return userId;
        }
    }
}
