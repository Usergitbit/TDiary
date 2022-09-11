using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using TDiary.Common.Models.Base;
using TDiary.Common.Models.Entities;
using TDiary.Common.Models.Entities.Enums;

namespace TDiary.Common.ServiceContracts
{
    public abstract class AbstractEventFactory : IEventFactory
    {
        public virtual Event CreateDeleteEvent<T>(T entity) where T : EntityBase
        {
            var deleteEvent = new Event
            {
                CreatedAt = DateTime.Now,
                CreatedAtUtc = DateTime.UtcNow,
                Entity = entity.GetType().Name,
                EventType = EventType.Delete,
                Data = JsonSerializer.Serialize(entity),
                EntityId = entity.Id,
                Id = Guid.NewGuid(),
                TimeZone = TimeZoneInfo.Local.Id,
                UserId = entity.Id,
                Version = 1
            };

            return deleteEvent;
        }

        public virtual Event CreateInsertEvent<T>(T entity, Guid userId) where T : EntityBase
        {
            entity.UserId = userId;
            entity.Id = Guid.NewGuid();
            var insertEvent = new Event
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                CreatedAt = DateTime.Now,
                CreatedAtUtc = DateTime.UtcNow,
                Data = JsonSerializer.Serialize(entity),
                Entity = entity.GetType().Name,
                EntityId = entity.Id,
                EventType = EventType.Insert,
                TimeZone = TimeZoneInfo.Local.Id,
                Version = 1,
            };

            return insertEvent;
        }

        public virtual Event CreateUpdateEvent<T>(T entity, T initialEntity, IReadOnlyDictionary<string, object> changes) where T : EntityBase
        {
            var updateEvent = new Event
            {
                Id = Guid.NewGuid(),
                UserId = entity.UserId,
                CreatedAt = DateTime.Now,
                CreatedAtUtc = DateTime.UtcNow,
                Data = JsonSerializer.Serialize(entity),
                InitialData = JsonSerializer.Serialize(initialEntity),
                Entity = entity.GetType().Name,
                EntityId = entity.Id,
                EventType = EventType.Update,
                TimeZone = TimeZoneInfo.Local.Id,
                Version = 1,
                Changes = JsonSerializer.Serialize(changes)
            };

            return updateEvent;
        }
    }
}