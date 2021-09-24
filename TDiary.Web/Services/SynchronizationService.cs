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
using TDiary.Common.Extensions;

namespace TDiary.Web.Services
{
    public class SynchronizationService : ISynchronizationService
    {
        private readonly IndexedDBManager dbManager;
        private readonly IEventPlayerService eventPlayerService;
        private readonly AuthenticationStateProvider authenticationStateProvider;
        private readonly EventProto.EventProtoClient eventClient;

        public SynchronizationService(IndexedDBManager dbManager,
            IEventPlayerService eventPlayerService,
            AuthenticationStateProvider authenticationStateProvider,
            EventProto.EventProtoClient eventClient)
        {
            this.dbManager = dbManager;
            this.eventPlayerService = eventPlayerService;
            this.authenticationStateProvider = authenticationStateProvider;
            this.eventClient = eventClient;
        }

        public async Task Synchronize(Guid userId)
        {
            DateTime lastEventDate;
            try
            {
                lastEventDate = await GetLastEventDateTime(userId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Aborting sync. Failed to get last event date time: {ex.Message}\n{ex.StackTrace}");
                return;
            }

            List<Event> incomingEvents;
            try
            {
                incomingEvents = await GetRemoteEvents(userId, lastEventDate);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Aborting sync. Failed to get incoming events: {ex.Message}\n{ex.StackTrace}");
                return;
            }
            var unsynchronizedEvents = await GetUnsynchronized(userId);
            var mergeResult = MergeConflicts(incomingEvents, unsynchronizedEvents);
            foreach (var eventEntity in mergeResult.EventsToBePushed)
            {
                await dbManager.DeleteRecord(StoreNameConstants.UnsynchronizedEvents, eventEntity.Id);
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
            foreach (var eventEntity in mergeResult.EventsToBePulled)
            {
                await dbManager.AddRecord(new StoreRecord<Event> { Storename = StoreNameConstants.Events, Data = eventEntity });
                await eventPlayerService.PlayEvent(eventEntity);
            }
        }

        private static MergeResult MergeConflicts(List<Event> incomingEvents, List<Event> unsynchronizedEvents)
        {
            // TODO: implement conflict solving;
            var mergeResult = new MergeResult
            {
                EventsToBePulled = incomingEvents,
                EventsToBePushed =  unsynchronizedEvents
            };

            return mergeResult;
        }

        private class MergeResult
        {
            public List<Event> EventsToBePushed { get; set; } = new();
            public List<Event> EventsToBePulled { get; set; } = new();
        }

        private async Task<List<Event>> GetRemoteEvents(Guid userId, DateTime lastEventDateUtc)
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
                LastEventDateUtc = Timestamp.FromDateTime(lastEventDateUtc.AsUtc())
            });

            var events = new List<Event>(); 
            foreach(var eventData in reply.EventData)
            {
                var eventEntity = new Event
                {
                    UserId = Guid.Parse(eventData.UserId),
                    CreatedAt = eventData.AuditData.CreatedAt.ToDateTime(),
                    CreatedAtUtc = eventData.AuditData.CreatedAtUtc.ToDateTime(),
                    Data = eventData.Data,
                    Entity = eventData.Entity,
                    EventType = (Common.Models.Entities.Enums.EventType)eventData.EventType,
                    Id = Guid.Parse(eventData.Id),
                    ModifiedtAt = eventData.AuditData.ModifiedAt.ToNullMinimumDateTime(),
                    ModifiedAtUtc = eventData.AuditData.ModifiedAtUtc.ToNullMinimumDateTime(),
                    TimeZone = eventData.AuditData.TimeZone,
                    Version = eventData.Version
                };

                events.Add(eventEntity);
            }

            return events;
        }

        private async Task<DateTime> GetLastEventDateTime(Guid userId)
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

        private async Task<List<Event>> GetUnsynchronized(Guid userId)
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
