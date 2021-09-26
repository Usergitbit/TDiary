using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using TDiary.Common.Models.Entities;
using TDiary.Common.ServiceContracts;

namespace TDiary.Web.Services
{
    public class UpdateEventMergerService : IUpdateEventMergerService
    {
        //TODO: merging for all entities, some props shouldn't be merged like navigation ones
        public Event Merge(Event serverEvent, Event localEvent)
        {
            var eventEntity = new Event
            {
                CreatedAt = DateTime.Now,
                CreatedAtUtc = DateTime.UtcNow,
                Entity = serverEvent.Entity,
                EntityId = serverEvent.EntityId,
                EventType = Common.Models.Entities.Enums.EventType.Update,
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

            eventEntity.Data = JsonSerializer.Serialize(mergedBrand);
            eventEntity.Changes = JsonSerializer.Serialize(changes);

            return eventEntity;
        }
    }
}
