using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using TDiary.Common.Models.Entities;
using TDiary.Common.ServiceContracts;
using TDiary.Grpc.Protos;
using TDiary.Web.IndexedDB;
using TDiary.Web.Services.Interfaces;
using TG.Blazor.IndexedDB;
using TDiary.Common.Extensions;

namespace TDiary.Web.Services
{
    public class EventService : IEventService
    {
        private readonly EventProto.EventProtoClient eventClient;
        private readonly NetworkStateService networkStateService;
        private readonly IndexedDBManager dbManager;
        private readonly IEventPlayerService eventPlayerService;

        public EventService(EventProto.EventProtoClient eventClient,
            NetworkStateService networkStateService,
            IndexedDBManager dbManager,
            IEventPlayerService eventPlayerService)
        {
            this.eventClient = eventClient;
            this.networkStateService = networkStateService;
            this.dbManager = dbManager;
            this.eventPlayerService = eventPlayerService;
        }

        public async Task Add(Guid userId, Event eventEntity)
        {
            var isOnline = await networkStateService.IsOnline();
            // TODO: check if token valid, if not valid then get from sts somehow
            // if sts offline then don't call api
            if (isOnline)
            {
                try
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
                            ModifiedAt = Timestamp.FromDateTime(eventEntity.ModifiedtAt.AsUtcNullMinimum()),
                            ModifiedAtUtc = Timestamp.FromDateTime(eventEntity.ModifiedAtUtc.AsUtcNullMinimum()),
                            TimeZone = eventEntity.TimeZone
                        }
                    };

                    var reply = await eventClient.AddEventAsync(new AddEventRequest
                    {
                        EventData = eventData
                    });
                    await dbManager.AddRecord(new StoreRecord<Event> { Storename = StoreNameConstants.Events, Data = eventEntity });
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    await dbManager.AddRecord(new StoreRecord<Event> { Storename = StoreNameConstants.UnsynchronizedEvents, Data = eventEntity });
                }
            }
            else
            {
                await dbManager.AddRecord(new StoreRecord<Event> { Storename = StoreNameConstants.UnsynchronizedEvents, Data = eventEntity });
            }
            await eventPlayerService.PlayEvent(eventEntity);
        }
    }
}
