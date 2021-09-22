using AutoMapper;
using Blazored.LocalStorage;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TDiary.Common.Models.Entities;
using TDiary.Common.ServiceContracts;
using TDiary.Grpc.Protos;
using TDiary.Web.IndexedDB;
using TDiary.Web.Services.Interfaces;
using TG.Blazor.IndexedDB;

namespace TDiary.Web.Services
{
    public class SynchronizationService : ISynchronizationService
    {
        private readonly IndexedDBManager dbManager;
        private readonly IEventPlayerService eventPlayerService;
        private readonly AuthenticationStateProvider authenticationStateProvider;
        private readonly EventProto.EventProtoClient eventClient;
        private readonly IMapper mapper;

        public SynchronizationService(IndexedDBManager dbManager,
            IEventPlayerService eventPlayerService,
            AuthenticationStateProvider authenticationStateProvider,
            EventProto.EventProtoClient eventClient,
            IMapper mapper)
        {
            this.dbManager = dbManager;
            this.eventPlayerService = eventPlayerService;
            this.authenticationStateProvider = authenticationStateProvider;
            this.eventClient = eventClient;
            this.mapper = mapper;
        }

        public async Task Synchronize(Guid userId)
        {
            var lastEventDate = await GetLastEventDateTime(userId);
            var incomingEvents = await GetRemote(userId, lastEventDate);
            var unsynchronizedEvents = await GetUnsynchronized(userId);
            var mergeResult = MergeConflicts(incomingEvents, unsynchronizedEvents);
            foreach(var eventEntity in mergeResult.EventsToUndo)
            {
                await eventPlayerService.UndoEvent(eventEntity);
            }
            foreach(var eventEntity in mergeResult.EventsToPlay)
            {
                await eventPlayerService.PlayEvent(eventEntity);
            }

        }

        private static MergeResult MergeConflicts(List<Event> incomingEvents, List<Event> unsynchronizedEvents)
        {
            // TODO: implement conflict solving;
            var mergeResult = new MergeResult
            {
                EventsToPlay = incomingEvents
            };

            return mergeResult;
        }

        private class MergeResult
        {
            public List<Event> EventsToUndo { get; set; }
            public List<Event> EventsToPlay { get; set; }
        }

        public async Task<List<Event>> GetRemote(Guid userId, DateTime lastEventDateUtc)
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
                LastEventDateUtc = Timestamp.FromDateTime(lastEventDateUtc)
            });
            var events = mapper.Map<List<Event>>(reply.EventData);

            return events;
        }

        public async Task<DateTime> GetLastEventDateTime(Guid userId)
        {
            //TODO: optimize to query by max somehow (upper bound in index db)
            var lastEventDate = DateTime.MinValue;
            var indexSearch = new StoreIndexQuery<string>
            {
                Storename = StoreNameConstants.Events,
                IndexName = "userId",
                QueryValue = userId.ToString(),
            };
            var events = await dbManager.GetAllRecordsByIndex<string, Brand>(indexSearch);
            if (events.Any())
            {
                lastEventDate = events.Max(e => e.CreatedAtUtc);
            }

            return lastEventDate;
        }

        public async Task<List<Event>> GetUnsynchronized(Guid userId)
        {
            var indexSearch = new StoreIndexQuery<string>
            {
                Storename = StoreNameConstants.UnsynchronizedEvents,
                IndexName = "userId",
                QueryValue = userId.ToString(),
            };
            var events = await dbManager.GetAllRecordsByIndex<string, Event>(indexSearch);

            return events.ToList();
        }
    }


}
