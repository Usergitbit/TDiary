using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TDiary.Api.Protos;
using TDiary.Common.Models.Entities;
using TDiary.Common.Models.Entities.Enums;
using TDiary.Database;

namespace TDiary.Api.Grpc
{
    [Authorize]
    public class EventRpc : EventProto.EventProtoBase
    {
        private readonly ILogger<EventRpc> logger;
        private readonly TDiaryDatabaseContext tdiaryDatabaseContext;

        public EventRpc(ILogger<EventRpc> logger, TDiaryDatabaseContext tdiaryDatabaseContext)
        {
            this.logger = logger;
            this.tdiaryDatabaseContext = tdiaryDatabaseContext;
        }

        public override async Task<AddEventReply> AddEvent(AddEventRequest request, ServerCallContext context)
        {
            try
            {
                var eventEntity = new Event
                {
                    Entity = "test",
                    CreatedAt = DateTime.Now,
                    CreatedBy = Guid.NewGuid(),
                    EventType = EventType.Insert,
                    LocallyCreatedAt = DateTime.Now,
                    TimeZone = TimeZoneInfo.Local,
                    UserId = Guid.NewGuid(),
                    Version = 1,
                    Data = request.Name
                };
                await tdiaryDatabaseContext.Events.AddAsync(eventEntity);
                await tdiaryDatabaseContext.SaveChangesAsync();
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
