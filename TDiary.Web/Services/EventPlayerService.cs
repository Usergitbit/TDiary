using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using TDiary.Common.Models.Entities;
using TDiary.Common.Models.Entities.Enums;
using TDiary.Common.ServiceContracts;

namespace TDiary.Web.Services
{
    public class EventPlayerService : IEventPlayerService
    {
        private readonly IBrandService brandService;

        public EventPlayerService(IBrandService brandService)
        {
            this.brandService = brandService;
        }

        public async Task PlayEvent(Event eventEntity)
        {
            switch (eventEntity.Entity)
            {
                case nameof(Brand):
                    await PlayBrandEvent(eventEntity);
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
                    await brandService.Delete(eventEntity.EntityId);
                    break;
                case EventType.Update:
                    brand = JsonSerializer.Deserialize<Brand>(eventEntity.InitialData);
                    await brandService.Update(brand);
                    break;
                case EventType.Delete:
                    brand = JsonSerializer.Deserialize<Brand>(eventEntity.Data);
                    brand.CreatedAt = DateTime.Now;
                    brand.CreatedAtUtc = DateTime.UtcNow;
                    brand.TimeZone = TimeZoneInfo.Local.Id;
                    await brandService.Add(brand);
                    break;
                default:
                    throw new NotImplementedException($"Play for event type {eventEntity.EventType} entity {eventEntity.Entity} not implemented.");
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
                    await brandService.Add(brand);
                    break;
                case EventType.Update:
                    brand = JsonSerializer.Deserialize<Brand>(eventEntity.Data);
                    brand.ModifiedtAt = DateTime.Now;
                    brand.ModifiedAtUtc = DateTime.UtcNow;
                    await brandService.Update(brand);
                    break;
                case EventType.Delete:
                    await brandService.Delete(eventEntity.EntityId);
                    break;
                default:
                    throw new NotImplementedException($"Play for event type {eventEntity.EventType} entity {eventEntity.Entity} not implemented.");
            }
        }
    }
}
