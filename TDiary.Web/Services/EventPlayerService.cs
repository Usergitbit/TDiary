using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using TDiary.Common.Models.Entities;
using TDiary.Common.Models.Entities.Enums;
using TDiary.Common.ServiceContracts;
using TDiary.Web.IndexedDB;
using TG.Blazor.IndexedDB;

namespace TDiary.Web.Services
{
    public class EventPlayerService : IEventPlayerService
    {
        private readonly IndexedDBManager dbManager;

        public EventPlayerService(IndexedDBManager dbManager)
        {
            this.dbManager = dbManager;
        }

        public async Task PlayEvent(Event eventEntity)
        {
            switch (eventEntity.Entity)
            {
                case nameof(Brand):
                    await PlayBrandEvent(eventEntity);
                    break;
                case nameof(FoodItem):
                    await PlayFoodItemEvent(eventEntity);
                    break;
                default:
                    throw new NotImplementedException($"{nameof(PlayEvent)} for entity {eventEntity.Entity} not implemeneted in {typeof(EventPlayerService).FullName}.");
            }
        }


        public async Task UndoEvent(Event eventEntity)
        {
            switch (eventEntity.Entity)
            {
                case nameof(Brand):
                    await UndoBrandEvent(eventEntity);
                    break;
                case nameof(FoodItem):
                    await UndoFoodItemEvent(eventEntity);
                    break;
                default:
                    throw new NotImplementedException($"{nameof(UndoEvent)} for entity {eventEntity.Entity} not implemeneted in {typeof(EventPlayerService).FullName}.");
            }
        }

        private async Task UndoBrandEvent(Event eventEntity)
        {
            Brand brand;
            switch (eventEntity.EventType)
            {
                case EventType.Insert:
                    await dbManager.DeleteRecord(StoreNameConstants.Brands, eventEntity.EntityId);
                    break;
                case EventType.Update:
                    brand = JsonSerializer.Deserialize<Brand>(eventEntity.InitialData);
                    await dbManager.UpdateRecord(new StoreRecord<Brand> { Storename = StoreNameConstants.Brands, Data = brand });
                    break;
                case EventType.Delete:
                    brand = JsonSerializer.Deserialize<Brand>(eventEntity.Data);
                    brand.CreatedAt = DateTime.Now;
                    brand.CreatedAtUtc = DateTime.UtcNow;
                    brand.TimeZone = TimeZoneInfo.Local.Id;
                    await dbManager.AddRecord(new StoreRecord<Brand> { Storename = StoreNameConstants.Brands, Data = brand });
                    break;
                default:
                    throw new NotImplementedException($"Undo for event type {eventEntity.EventType} entity {eventEntity.Entity} not implemented.");
            }
        }

        private async Task UndoFoodItemEvent(Event eventEntity)
        {
            FoodItem foodItem;
            switch (eventEntity.EventType)
            {
                case EventType.Insert:
                    await dbManager.DeleteRecord(StoreNameConstants.FoodItems, eventEntity.EntityId);
                    break;
                case EventType.Update:
                    foodItem = JsonSerializer.Deserialize<FoodItem>(eventEntity.Data);
                    await dbManager.UpdateRecord(new StoreRecord<FoodItem> { Storename = StoreNameConstants.FoodItems, Data = foodItem });
                    break;
                case EventType.Delete:
                    foodItem = JsonSerializer.Deserialize<FoodItem>(eventEntity.Data);
                    foodItem.CreatedAt = DateTime.Now;
                    foodItem.CreatedAtUtc = DateTime.UtcNow;
                    foodItem.TimeZone = TimeZoneInfo.Local.Id;
                    await dbManager.AddRecord(new StoreRecord<FoodItem> { Storename = StoreNameConstants.FoodItems, Data = foodItem });
                    break;
                default:
                    throw new NotImplementedException($"Undo for event type {eventEntity.EventType} entity {eventEntity.Entity} not implemented.");
            }
        }


        private async Task PlayBrandEvent(Event eventEntity)
        {
            Brand brand;
            switch (eventEntity.EventType)
            {
                case EventType.Insert:
                    brand = JsonSerializer.Deserialize<Brand>(eventEntity.Data);
                    brand.CreatedAt = DateTime.Now;
                    brand.CreatedAtUtc = DateTime.UtcNow;
                    //TODO: timezone for creation and modification?
                    brand.TimeZone = TimeZoneInfo.Local.Id;
                    await dbManager.AddRecord(new StoreRecord<Brand> { Storename = StoreNameConstants.Brands, Data = brand });
                    break;
                case EventType.Update:
                    brand = JsonSerializer.Deserialize<Brand>(eventEntity.Data);
                    brand.ModifiedtAt = DateTime.Now;
                    brand.ModifiedAtUtc = DateTime.UtcNow;
                    await dbManager.UpdateRecord(new StoreRecord<Brand> { Storename = StoreNameConstants.Brands, Data = brand });
                    break;
                case EventType.Delete:
                    await dbManager.DeleteRecord(StoreNameConstants.Brands, eventEntity.EntityId);
                    break;
                default:
                    throw new NotImplementedException($"Play for event type {eventEntity.EventType} entity {eventEntity.Entity} not implemented.");
            }
        }

        private async Task PlayFoodItemEvent(Event eventEntity)
        {
            FoodItem foodItem;
            switch (eventEntity.EventType)
            {
                case EventType.Insert:
                    foodItem = JsonSerializer.Deserialize<FoodItem>(eventEntity.Data);
                    foodItem.CreatedAt = DateTime.Now;
                    foodItem.CreatedAtUtc = DateTime.UtcNow;
                    foodItem.TimeZone = TimeZoneInfo.Local.Id;
                    await dbManager.AddRecord(new StoreRecord<FoodItem> { Storename = StoreNameConstants.FoodItems, Data = foodItem });
                    break;
                case EventType.Update:
                    foodItem = JsonSerializer.Deserialize<FoodItem>(eventEntity.Data);
                    foodItem.ModifiedtAt = DateTime.Now;
                    foodItem.ModifiedAtUtc = DateTime.UtcNow;
                    await dbManager.UpdateRecord(new StoreRecord<FoodItem> { Storename = StoreNameConstants.FoodItems, Data = foodItem });
                    break;
                case EventType.Delete:
                    await dbManager.DeleteRecord(StoreNameConstants.FoodItems, eventEntity.EntityId);
                    break;
                default:
                    throw new NotImplementedException($"Play for event type {eventEntity.EventType} entity {eventEntity.Entity} not implemented.");
            }
        }
    }
}
