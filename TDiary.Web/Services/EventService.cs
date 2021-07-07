using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using TDiary.Common.Models.Entities;
using TDiary.Common.ServiceContracts;
using TDiary.Grpc.Protos;
using TDiary.Web.IndexedDB;
using TG.Blazor.IndexedDB;

namespace TDiary.Web.Services
{
    public class EventService : IEventService
    {
        private readonly EventProto.EventProtoClient eventClient;
        private readonly IMapper mapper;
        private readonly AuthenticationStateProvider authenticationStateProvider;
        private readonly NetworkStateService networkStateService;
        private readonly IndexedDBManager dbManager;
        private readonly IBrandService brandService;

        public EventService(EventProto.EventProtoClient eventClient,
            IMapper mapper,
            AuthenticationStateProvider authenticationStateProvider,
            NetworkStateService networkStateService,
            IndexedDBManager dbManager,
            IBrandService brandService)
        {
            this.eventClient = eventClient;
            this.mapper = mapper;
            this.authenticationStateProvider = authenticationStateProvider;
            this.networkStateService = networkStateService;
            this.dbManager = dbManager;
            this.brandService = brandService;
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
            else
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
            await PlayEvent(eventEntity);
        }

        private async Task PlayEvent(Event eventEntity)
        {
            var brand = JsonSerializer.Deserialize<Brand>(eventEntity.Data);
            brand.UserId = eventEntity.UserId;
            brand.Id = Guid.NewGuid();
            await brandService.Add(brand);
        }

        public async Task<List<Event>> Get(Guid userId, DateTime lastEventDateUtc)
        {
            //sync flow:
            // if access token OK => start SYNC process => log last sync date
            // if access token exception => 3. if sts online => go to login
            //                                 if sts offline => skip and check last sync date to show warning if it was a long time ago

            // repository flow
            // if online => sync
            // return data from cache
            var authState = await authenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;
            var claims = user.Claims;
            var userIdClaim = claims.FirstOrDefault(c => c.Type == "id").Value;
            userId = Guid.Parse(userIdClaim);

            var reply = await eventClient.GetEventsAsync(new GetEventsRequest
            {
                LastEventDateUtc = Timestamp.FromDateTime(DateTime.UtcNow.AddYears(-100))
            });
            var events = mapper.Map<List<Event>>(reply.EventData);

            return events;
        }
    }
}
