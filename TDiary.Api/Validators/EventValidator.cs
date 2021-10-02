using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TDiary.Grpc.Protos;

namespace TDiary.Api.Validators
{
    public class EventValidator
    {
        public bool IsValid(EventData eventData, out IReadOnlyDictionary<string, IEnumerable<string>> propertyValidationFailures)
        {
            var propertyValidationFailuresInternal = new Dictionary<string, IEnumerable<string>>();

            var idValidationMessages = new List<string>();
            if (!Guid.TryParse(eventData.Id, out var id))
                idValidationMessages.Add("Could not parse event id string into Guid.");
            if(id == Guid.Empty)
                idValidationMessages.Add("Id can not be empty.");
            if (idValidationMessages.Any())
                propertyValidationFailuresInternal.Add(nameof(EventData.Id), idValidationMessages);

            var entityIdValidationMessages = new List<string>();
            if (!Guid.TryParse(eventData.EntityId, out var entityId))
                entityIdValidationMessages.Add("Could not parse entity id string into Guid.");
            if (entityId == Guid.Empty)
                entityIdValidationMessages.Add("Entity id can not be empty");
            if (entityIdValidationMessages.Any())
                propertyValidationFailuresInternal.Add(nameof(EventData.EntityId), entityIdValidationMessages);

            if (eventData.AuditData == null)
                propertyValidationFailuresInternal.Add(nameof(EventData.AuditData), new List<string> { "AuditData can not be empty." });

            if (string.IsNullOrWhiteSpace(eventData.AuditData?.TimeZone))
                propertyValidationFailuresInternal.Add(nameof(AuditData.TimeZone), new List<string> { "AuditData Timezone can not be empty." });

            if (string.IsNullOrWhiteSpace(eventData.Data))
                propertyValidationFailuresInternal.Add(nameof(EventData.Data), new List<string> { "Data can not be empty." });

            if (string.IsNullOrWhiteSpace(eventData.Entity))
                propertyValidationFailuresInternal.Add(nameof(EventData.Entity), new List<string> { "Entity can not be empty." });

            if (eventData.EventType == EventType.Unknown)
                propertyValidationFailuresInternal.Add(nameof(EventData.EventType), new List<string> { "EventType can not be Unknown." });

            propertyValidationFailures = propertyValidationFailuresInternal;
            var result = !propertyValidationFailures.Any();

            return result;
        }
    }
}
