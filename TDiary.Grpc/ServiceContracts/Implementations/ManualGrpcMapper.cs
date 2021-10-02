using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections.Generic;
using System.Text;
using TDiary.Common.Extensions;
using TDiary.Common.Models.Entities;
using TDiary.Grpc.Protos;

namespace TDiary.Grpc.ServiceContracts.Implementations
{
    public class ManualGrpcMapper : IManualGrpcMapper
    {
        public EventData Map(Event eventEntity)
        {
            var eventData = new EventData
            {
                Data = eventEntity.Data,
                Entity = eventEntity.Entity,
                EventType = (EventType)eventEntity.EventType,
                Id = eventEntity.Id.ToString(),
                UserId = eventEntity.UserId.ToString(),
                Version = eventEntity.Version,
                AuditData = new AuditData
                {
                    CreatedAt = Timestamp.FromDateTime(eventEntity.CreatedAt.AsUtc()),
                    CreatedAtUtc = Timestamp.FromDateTime(eventEntity.CreatedAtUtc.AsUtc()),
                    ModifiedAt = eventEntity.ModifiedtAt == null ? null : Timestamp.FromDateTime(eventEntity.ModifiedtAt.AsUtcNullMinimum()),
                    ModifiedAtUtc = eventEntity.ModifiedAtUtc == null ? null : Timestamp.FromDateTime(eventEntity.ModifiedAtUtc.AsUtcNullMinimum()),
                    TimeZone = eventEntity.TimeZone
                },
                EntityId = eventEntity.EntityId.ToString(),
                Changes = eventEntity.Changes,
                InitialData = eventEntity.InitialData
            };

            return eventData;
        }

        public Event Map(EventData eventData)
        {
            var eventEntity = new Event
            {
                UserId = Guid.Parse(eventData.UserId),
                CreatedAt = eventData.AuditData.CreatedAt.ToDateTime(),
                CreatedAtUtc = eventData.AuditData.CreatedAtUtc.ToDateTime(),
                Data = eventData.Data,
                Entity = eventData.Entity,
                EventType = (Common.Models.Entities.Enums.EventType)eventData.EventType,
                Id = Guid.Parse(eventData.Id),
                ModifiedtAt = eventData.AuditData.ModifiedAt?.ToDateTime(),
                ModifiedAtUtc = eventData.AuditData.ModifiedAtUtc?.ToDateTime(),
                TimeZone = eventData.AuditData.TimeZone,
                Version = eventData.Version,
                EntityId = Guid.Parse(eventData.EntityId),
                Changes = eventData.Changes,
                InitialData = eventData.InitialData
            };

            return eventEntity;
        }
    }
}
