using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TDiary.Api.Protos
{
    public partial class GetEventsReply
    {
        public GetEventsReply(string message, IEnumerable<EventData> eventData) : base()
        {
            Message = message;
            EventData.AddRange(eventData);
        }
    }
}
