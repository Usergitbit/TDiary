using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TDiary.Grpc.Protos;

namespace TDiary.Api.Grpc
{
    public class PingRpc : PingProto.PingProtoBase
    {
        public override Task<PingReply> Ping(PingRequest request, ServerCallContext context)
        {
            return Task.FromResult(new PingReply());
        }
    }
}
