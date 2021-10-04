using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using TDiary.Common.Models.Entities;
using TDiary.Common.Models.Entities.Enums;
using TDiary.Common.ServiceContracts;

namespace TDiary.Common.ServiceContracts.Implementations
{
    public class UpdateEventMergerService : IUpdateEventMergerService
    {
        //TODO: merging for all entities, some props shouldn't be merged like navigation ones
        //TODO: version handling?
        public Event Merge(Event serverEvent, Event localEvent)
        {
            if (serverEvent.Entity != localEvent.Entity)
            {
                throw new InvalidOperationException($"Attempt to merge server entity {serverEvent.Entity} with local entity {localEvent.Entity}");
            }

            if (serverEvent.EntityId != localEvent.EntityId)
            {
                throw new InvalidOperationException($"Attempt to merge diferent entities: {serverEvent.Entity} {serverEvent.EntityId} with {serverEvent.Entity} {localEvent.EntityId}");
            }

            Event result;
            switch (serverEvent.Entity)
            {
                case "Brand":
                    result = MergeBrand(serverEvent, localEvent);
                    break;
                default:
                    throw new NotImplementedException($"Merging not implemented for entity {serverEvent.Entity}.");
            }

            return result;

        }

        private static Event MergeBrand(Event serverEvent, Event localEvent)
        {
            var eventEntity = new Event
            {
                CreatedAt = DateTime.Now,
                CreatedAtUtc = DateTime.UtcNow,
                Entity = serverEvent.Entity,
                EntityId = serverEvent.EntityId,
                EventType = EventType.Update,
                Id = Guid.NewGuid(),
                TimeZone = TimeZoneInfo.Local.Id,
                UserId = serverEvent.UserId,
                Version = serverEvent.Version
            };

            var serverChanges = JsonSerializer.Deserialize<Dictionary<string, object>>(serverEvent.Changes);
            var serverBrand = JsonSerializer.Deserialize<Brand>(serverEvent.Data);
            var localChanges = JsonSerializer.Deserialize<Dictionary<string, object>>(localEvent.Changes);
            var localBrand = JsonSerializer.Deserialize<Brand>(localEvent.Data);
            var mergedBrand = serverBrand;
            var changes = new Dictionary<string, object>();
            foreach (var changedProp in localChanges.Keys)
            {
                if (serverChanges.ContainsKey(changedProp))
                {
                    // TODO: maybe not use reflection?
                    if (serverEvent.CreatedAtUtc < localEvent.CreatedAtUtc)
                    {
                        // TODO: DateTime and Guid will incorrectly deserialize to string and will fail this, most primitive types should be ok
                        // TODO: check if enums are ok
                        var localBrandPropertyValue = localBrand.GetType().GetProperty(changedProp).GetValue(localBrand);
                        mergedBrand.GetType().GetProperty(changedProp).SetValue(mergedBrand, localBrandPropertyValue);
                        changes.Add(changedProp, localBrandPropertyValue);
                    }
                }
                else
                {
                    var localBrandPropertyValue = localBrand.GetType().GetProperty(changedProp).GetValue(localBrand);
                    mergedBrand.GetType().GetProperty(changedProp).SetValue(mergedBrand, localBrandPropertyValue);
                    changes.Add(changedProp, localBrandPropertyValue);
                }
            }

            eventEntity.InitialData = serverEvent.Data;
            eventEntity.Data = JsonSerializer.Serialize(mergedBrand);
            if (changes.Any())
            {
                eventEntity.Changes = JsonSerializer.Serialize(changes);
            }

            return eventEntity;
        }
    }
}
