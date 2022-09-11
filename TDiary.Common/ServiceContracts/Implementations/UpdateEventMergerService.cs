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
                case "FoodItem":
                    result = MergeFoodItem(serverEvent, localEvent);
                    break;
                default:
                    throw new NotImplementedException($"Merging not implemented for entity {serverEvent.Entity}.");
            }

            return result;

        }

        private Event MergeFoodItem(Event serverEvent, Event localEvent)
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
            var serverFoodItem = JsonSerializer.Deserialize<FoodItem>(serverEvent.Data);
            var localChanges = JsonSerializer.Deserialize<Dictionary<string, object>>(localEvent.Changes);
            var localFoodItem = JsonSerializer.Deserialize<FoodItem>(localEvent.Data);
            var mergedFoodItem = serverFoodItem;
            var changes = new Dictionary<string, object>();
            // TODO: refactor, kinda hard to understand at first glance that it only updates with local value if the server value is older
            foreach (var changedProp in localChanges.Keys)
            {
                if (serverChanges.ContainsKey(changedProp))
                {
                    if (serverEvent.CreatedAtUtc < localEvent.CreatedAtUtc)
                    {
                        UpdatePropertyValue(changedProp, mergedFoodItem, localFoodItem, changes);
                    }
                }
                else
                {
                    UpdatePropertyValue(changedProp, mergedFoodItem, localFoodItem, changes);
                }
            }

            eventEntity.InitialData = serverEvent.Data;
            eventEntity.Data = JsonSerializer.Serialize(mergedFoodItem);
            if (changes.Any())
            {
                eventEntity.Changes = JsonSerializer.Serialize(changes);
            }

            return eventEntity;
        }

        private Event MergeBrand(Event serverEvent, Event localEvent)
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
            // TODO: refactor, kinda hard to understand at first glance that it only updates with local value if the server value is older
            foreach (var changedProp in localChanges.Keys)
            {
                if (serverChanges.ContainsKey(changedProp))
                {
                    if (serverEvent.CreatedAtUtc < localEvent.CreatedAtUtc)
                    {
                        UpdatePropertyValue(changedProp, mergedBrand, localBrand, changes);
                    }
                }
                else
                {
                    UpdatePropertyValue(changedProp, mergedBrand, localBrand, changes);
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

        private void UpdatePropertyValue(string changedProp, Brand mergedBrand, Brand localBrand, Dictionary<string, object> changes)
        {
            object localBrandPropertyValue;
            switch (changedProp)
            {
                case nameof(Brand.Name):
                    localBrandPropertyValue = localBrand.Name;
                    mergedBrand.Name = localBrandPropertyValue as string;
                    break;
                default: throw new NotImplementedException($"Unhandled property {changedProp} for entity {nameof(Brand)}");
            }
            changes.Add(changedProp, localBrandPropertyValue);
        }

        private void UpdatePropertyValue(string changedProp, FoodItem mergedFoodItem, FoodItem localFoodItem, Dictionary<string, object> changes)
        {
            object localFoodItemPropertyValue;
            switch (changedProp)
            {
                case nameof(FoodItem.Name):
                    localFoodItemPropertyValue = localFoodItem.Name;
                    mergedFoodItem.Name = localFoodItemPropertyValue as string;
                    break;
                case nameof(FoodItem.BrandId):
                    localFoodItemPropertyValue = localFoodItem.BrandId;
                    mergedFoodItem.BrandId = (Guid)localFoodItemPropertyValue;
                    break;
                case nameof(FoodItem.Calories):
                    localFoodItemPropertyValue = localFoodItem.Calories;
                    mergedFoodItem.Calories = (double)localFoodItemPropertyValue;
                    break;
                case nameof(FoodItem.Carbohydrates):
                    localFoodItemPropertyValue = localFoodItem.Carbohydrates;
                    mergedFoodItem.Carbohydrates = (double)localFoodItemPropertyValue;
                    break;
                case nameof(FoodItem.Fats):
                    localFoodItemPropertyValue = localFoodItem.Fats;
                    mergedFoodItem.Fats = (double)localFoodItemPropertyValue;
                    break;
                case nameof(FoodItem.Proteins):
                    localFoodItemPropertyValue = localFoodItem.Proteins;
                    mergedFoodItem.Proteins = (double)localFoodItemPropertyValue;
                    break;
                case nameof(FoodItem.SaturatedFats):
                    localFoodItemPropertyValue = localFoodItem.SaturatedFats;
                    mergedFoodItem.SaturatedFats = (double)localFoodItemPropertyValue;
                    break;
                default: throw new NotImplementedException($"Unhandled property {changedProp} for entity {nameof(FoodItem)}");
            }
            changes.Add(changedProp, localFoodItemPropertyValue);
        }
    }
}
