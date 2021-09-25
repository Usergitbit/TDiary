using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using TDiary.Common.Models.Entities;
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

        public Task UndoEvent(Event eventEntity)
        {
            return Task.CompletedTask;
        }

        private async Task PlayBrandEvent(Event eventEntity)
        {
            var brand = JsonSerializer.Deserialize<Brand>(eventEntity.Data);
            brand.CreatedAt = DateTime.Now;
            brand.CreatedAtUtc = DateTime.UtcNow;
            brand.TimeZone = TimeZoneInfo.Local.Id;
            await brandService.Add(brand);
        }
    }
}
