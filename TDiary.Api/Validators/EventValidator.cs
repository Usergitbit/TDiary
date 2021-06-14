using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TDiary.Api.Protos;

namespace TDiary.Api.Validators
{
    public class EventValidator
    {
        public bool IsValid(EventData eventData)
        {
            var result = true;

            if (!Guid.TryParse(eventData.Id, out var id) || id == Guid.Empty)
                return false;
            if (eventData.AuditData == null)
                return false;
            if (string.IsNullOrWhiteSpace(eventData.AuditData.TimeZone))
                return false;
            if (string.IsNullOrWhiteSpace(eventData.Data))
                return false;
            if (string.IsNullOrWhiteSpace(eventData.Entity))
                return false;
            if (eventData.EventType == EventType.Unknown)
                return false;

            return result;
        }
    }
}
