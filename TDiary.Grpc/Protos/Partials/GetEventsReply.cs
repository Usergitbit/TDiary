using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TDiary.Grpc.Protos
{
    public partial class GetEventsReply
    {
        public GetEventsReply(IEnumerable<EventData> eventData) : base()
        {
            EventData.AddRange(eventData);
            if (!eventData.Any())
                ResultCode = ResultCode.NoData;
            else
                ResultCode = ResultCode.Ok;
        }

        public GetEventsReply(ErrorInfo errorInfo)
        {
            ErrorInfo = errorInfo;
        }
    }
}
