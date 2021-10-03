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
using TDiary.Grpc.ServiceContracts;

namespace TDiary.Web.Services
{
    public class EventService : IEventService
    {
        private readonly EventProto.EventProtoClient eventClient;
        private readonly NetworkStateService networkStateService;
        private readonly IndexedDBManager dbManager;
        private readonly IEventPlayerService eventPlayerService;
        private readonly IManualGrpcMapper manualGrpcMapper;

        public EventService(EventProto.EventProtoClient eventClient,
            NetworkStateService networkStateService,
            IndexedDBManager dbManager,
            IEventPlayerService eventPlayerService,
            IManualGrpcMapper manualGrpcMapper)
        {
            this.eventClient = eventClient;
            this.networkStateService = networkStateService;
            this.dbManager = dbManager;
            this.eventPlayerService = eventPlayerService;
            this.manualGrpcMapper = manualGrpcMapper;
        }

        public async Task Add(Event eventEntity)
        {
            var isOnline = await networkStateService.IsOnline();
            var isApiAvailable = await networkStateService.IsApiOnline();
            if (isOnline && isApiAvailable)
            {
                try
                {
                    // TODO: check with ping if api available maybe move ping to network state service
                    var eventData = manualGrpcMapper.Map(eventEntity);
                    var reply = await eventClient.AddEventAsync(new AddEventRequest
                    {
                        EventData = eventData
                    });

                    if (reply.ResultCase == AddEventReply.ResultOneofCase.ErrorInfo)
                    {
                        throw new Exception(reply.ErrorInfo.Reason);
                    }
                    else
                    {
                        await dbManager.AddRecord(new StoreRecord<Event> { Storename = StoreNameConstants.Events, Data = eventEntity });
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    await dbManager.AddRecord(new StoreRecord<Event> { Storename = StoreNameConstants.UnsynchronizedEvents, Data = eventEntity });
                }
            }
            else
            {
                await dbManager.AddRecord(new StoreRecord<Event>
                {
                    Storename = StoreNameConstants.UnsynchronizedEvents,
                    Data = eventEntity
                });
            }
            await eventPlayerService.PlayEvent(eventEntity);
        }
    }
}
