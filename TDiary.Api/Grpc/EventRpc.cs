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
using TDiary.Common.Models.Entities.Enums;
using TDiary.Common.ServiceContracts;
using TDiary.Database;

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
                if(!eventValidator.IsValid(request.EventData))
                    return new AddEventReply
                    {
                        Message = "Invalid"
                    };

                var userIdString = context.GetHttpContext().User.Claims.FirstOrDefault(c => c.Type == "id").Value;
                var userId = Guid.Parse(userIdString);
                var eventEntity = mapper.Map<Event>(request.EventData);
                await eventService.Add(userId, eventEntity);

                return new AddEventReply
                {
                    Message = "Success"
                };
            }
            catch(Exception ex)
            {
                logger.LogError(ex, "Exception adding event.");
                return new AddEventReply
                {
                    Message = "Error"
                };
            }
        }
    }
}
