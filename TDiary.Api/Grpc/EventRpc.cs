using AutoMapper;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TDiary.Api.Protos;
using TDiary.Api.Validators;
using TDiary.Common.Models.Entities;
using TDiary.Common.ServiceContracts;
using TDiary.Common.Extensions;
using TDiary.Api.Extensions;
using TDiary.Api.Exceptions;

namespace TDiary.Api.Grpc
{
    [Authorize]
    public class EventRpc : EventProto.EventProtoBase
    {
        private readonly ILogger<EventRpc> logger;
        private readonly IEventService eventService;
        private readonly IMapper mapper;
        private readonly EventValidator eventValidator;

        public EventRpc(ILogger<EventRpc> logger, IEventService eventService, IMapper mapper, EventValidator eventValidator)
        {
            this.logger = logger;
            this.eventService = eventService;
            this.mapper = mapper;
            this.eventValidator = eventValidator;
        }

        public override async Task<AddEventReply> AddEvent(AddEventRequest request, ServerCallContext context)
        {
            try
            {
                if (!eventValidator.IsValid(request.EventData, out var propertyValidationFailiures))
                {
                    var error = new ErrorInfo(propertyValidationFailiures);
                    return new AddEventReply(error);
                }

                var userId = context.GetUserId();
                var eventEntity = mapper.Map<Event>(request.EventData);
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
                var eventDataList = mapper.Map<List<EventData>>(events);
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
