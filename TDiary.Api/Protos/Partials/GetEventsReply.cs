using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TDiary.Api.Protos
{
    public partial class GetEventsReply
    {
        public GetEventsReply(IEnumerable<EventData> eventData) : base()
        {
            EventData.AddRange(eventData);
            if (!eventData.Any())
                Message = "No data.";
            else
                Message = "OK";
        }

        public GetEventsReply(ErrorInfo errorInfo)
        {
            ErrorInfo = errorInfo;
        }
    }
}
