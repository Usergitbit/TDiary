using AutoMapper;
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

namespace TDiary.Web.Services
{
    public class EventService : IEventService
    {
        private readonly EventProto.EventProtoClient eventClient;
        private readonly IMapper mapper;
        private readonly NetworkStateService networkStateService;
        private readonly IndexedDBManager dbManager;
        private readonly IEventPlayerService eventPlayerService;

        public EventService(EventProto.EventProtoClient eventClient,
            IMapper mapper,
            NetworkStateService networkStateService,
            IndexedDBManager dbManager,
            IEventPlayerService eventPlayerService)
        {
            this.eventClient = eventClient;
            this.mapper = mapper;
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
                var succesful = true;
                try
                {
                    var eventData = mapper.Map<EventData>(eventEntity);
                    var reply = await eventClient.AddEventAsync(new AddEventRequest
                    {
                        EventData = eventData
                    });

                }
                catch (Exception ex)
                {
                    succesful = false;
                    Console.WriteLine(ex.Message);
                }
                await AddLocally(eventEntity, succesful);
            }
            {
                await AddLocally(eventEntity, false);
            }
        }

        private async Task AddLocally(Event eventEntity, bool synchronized)
        {
            if(synchronized)
            {
                await dbManager.AddRecord(new StoreRecord<Event> { Storename = StoreNameConstants.Events, Data = eventEntity });
            }
            else
            {
                await dbManager.AddRecord(new StoreRecord<Event> { Storename = StoreNameConstants.UnsynchronizedEvents, Data = eventEntity });
            }
            await eventPlayerService.PlayEvent(eventEntity);
        }
    }
}
