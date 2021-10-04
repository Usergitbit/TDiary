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
using TDiary.Grpc.ServiceContracts;

namespace TDiary.Api.Grpc
{
    [Authorize]
    public class EventRpc : EventProto.EventProtoBase
    {
        private readonly ILogger<EventRpc> logger;
        private readonly IEventService eventService;
        private readonly EventValidator eventValidator;
        private readonly IManualGrpcMapper manualGrpcMapper;

        public EventRpc(ILogger<EventRpc> logger,
            IEventService eventService,
            EventValidator eventValidator,
            IManualGrpcMapper manualGrpcMapper)
        {
            this.logger = logger;
            this.eventService = eventService;
            this.eventValidator = eventValidator;
            this.manualGrpcMapper = manualGrpcMapper;
        }

        public override async Task<AddEventReply> AddEvent(AddEventRequest request, ServerCallContext context)
        {
            try
            {
                if (!eventValidator.IsValid(request.EventData, out var propertyValidationFailures))
                {
                    var validationError = new Error(request.EventData, propertyValidationFailures);
                    var errorInformation = new ErrorInformation(validationError);
                    return new AddEventReply(errorInformation);
                }

                var userId = context.GetUserId();
                var eventEntity = manualGrpcMapper.Map(request.EventData);
                if (eventEntity.UserId != userId)
                {
                    // TODO: what to do if an event is sent for a different user?
                }
                else
                {
                    await eventService.Add(eventEntity);
                }

                return new AddEventReply();
            }
            catch (DuplicateIdException ex)
            {
                logger.LogError(ex, "Dupliate id exception adding event.");
                var error = new ErrorInformation(ex);

                return new AddEventReply(error);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Exception adding event.");
                var error = new ErrorInformation(ex);

                return new AddEventReply(error);
            }
        }

        public override async Task<BulkAddEventReply> BulkAddEvent(BulkAddEventRequest request, ServerCallContext context)
        {
            try
            {
                var validationErrors = new List<Error>();
                foreach (var eventData in request.EventData)
                {
                    if (!eventValidator.IsValid(eventData, out var propertyValidationFailures))
                    {
                        var validationError = new Error(eventData, propertyValidationFailures);
                        validationErrors.Add(validationError);
                    }
                }

                if (validationErrors.Any())
                {
                    var errorInformation = new ErrorInformation(validationErrors);
                    return new BulkAddEventReply(errorInformation);
                }

                var userId = context.GetUserId();
                var eventEntities = manualGrpcMapper.Map(request.EventData);
                // TODO: check if user ids match up?

                await eventService.BulkAdd(eventEntities);

                return new BulkAddEventReply();

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Exception adding event.");
                var error = new ErrorInformation(ex);

                return new BulkAddEventReply(error);
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
                    var eventData = manualGrpcMapper.Map(eventEntity);
                    eventDataList.Add(eventData);
                }
                var response = new GetEventsReply(eventDataList);

                return response;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Exception adding event.");
                var error = new ErrorInformation(ex);

                return new GetEventsReply(error);
            }
        }
    }
}
