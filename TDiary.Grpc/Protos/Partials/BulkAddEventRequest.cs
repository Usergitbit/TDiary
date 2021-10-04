using System;
using System.Collections.Generic;
using System.Text;

namespace TDiary.Grpc.Protos
{
    public partial class BulkAddEventRequest
    {
        public BulkAddEventRequest(IEnumerable<EventData> eventDataList) : base()
        {
            EventData.AddRange(eventDataList);
        }
    }
}
