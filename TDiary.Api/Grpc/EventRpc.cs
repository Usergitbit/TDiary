using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TDiary.Api.Validators;
using TDiary.Common.Models.Entities;
using TDiary.Common.ServiceContracts;
using TDiary.Common.Extensions;
using TDiary.Api.Extensions;
using TDiary.Common.Exceptions;
using TDiary.Grpc.Protos;
using TDiary.Api.Services.Interfaces;
using Google.Protobuf.WellKnownTypes;

namespace TDiary.Api.Grpc
{
    [Authorize]
    public class EventRpc : EventProto.EventProtoBase
    {
        private readonly ILogger<EventRpc> logger;
        private readonly IEventService eventService;
        private readonly EventValidator eventValidator;

        public EventRpc(ILogger<EventRpc> logger, IEventService eventService, EventValidator eventValidator)
        {
            this.logger = logger;
            this.eventService = eventService;
            this.eventValidator = eventValidator;
        }

        public override async Task<AddEventReply> AddEvent(AddEventRequest request, ServerCallContext context)
        {
            try
            {
                if (!eventValidator.IsValid(request.EventData, out var propertyValidationFailures))
                {
                    var error = new ErrorInfo(propertyValidationFailures);

                    return new AddEventReply(error);
                }

                var userId = context.GetUserId();
                var eventEntity = new Event
                {
                    Id = Guid.Parse(request.EventData.Id),
                    CreatedAt = request.EventData.AuditData.CreatedAt.ToDateTime(),
                    CreatedAtUtc = request.EventData.AuditData.CreatedAtUtc.ToDateTime(),
                    Data = request.EventData.Data,
                    Entity = request.EventData.Entity,
                    EventType = (Common.Models.Entities.Enums.EventType)request.EventData.EventType,
                    ModifiedtAt = request.EventData.AuditData.ModifiedAt.ToNullMinimumDateTime(),
                    ModifiedAtUtc = request.EventData.AuditData.ModifiedAtUtc.ToNullMinimumDateTime(),
                    TimeZone = request.EventData.AuditData.TimeZone,
                    UserId = userId,
                    Version = request.EventData.Version
                };
                await eventService.Add(userId, eventEntity);

                return new AddEventReply();
            }
            catch (DuplicateIdException ex)
            {
                logger.LogError(ex, "Dupliate id exception adding event.");
                var error = new ErrorInfo(ex);

                return new AddEventReply(error);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Exception adding event.");
                var error = new ErrorInfo(ex);

                return new AddEventReply(error);
            }
        }

        public override async Task<GetEventsReply> GetEvents(GetEventsRequest request, ServerCallContext context)
        {
            try
            {
                var userId = context.GetUserId();
                var events = await eventService.Get(userId, request.LastEventDateUtc.ToDateTime());
                var eventDataList = new List<EventData>();
                foreach (var eventEntity in events)
                {
                    var eventData = new EventData
                    {
                        AuditData = new AuditData
                        {
                            CreatedAt = Timestamp.FromDateTime(eventEntity.CreatedAt.AsUtc()),
                            CreatedAtUtc = Timestamp.FromDateTime(eventEntity.CreatedAtUtc.AsUtc()),
                            ModifiedAt = Timestamp.FromDateTime(eventEntity.ModifiedtAt.AsUtcNullMinimum()),
                            ModifiedAtUtc = Timestamp.FromDateTime(eventEntity.ModifiedAtUtc.AsUtcNullMinimum()),
                            TimeZone = eventEntity.TimeZone
                        },
                        Entity = eventEntity.Entity,
                        Data = eventEntity.Data,
                        EventType = (EventType)eventEntity.EventType,
                        Id = eventEntity.Id.ToString(),
                        Version = eventEntity.Version,
                        UserId = eventEntity.UserId.ToString()
                    };

                    eventDataList.Add(eventData);
                }
                var response = new GetEventsReply(eventDataList);

                return response;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Exception adding event.");
                var error = new ErrorInfo(ex);

                return new GetEventsReply(error);
            }
        }
    }
}
