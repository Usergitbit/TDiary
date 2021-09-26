using Blazored.LocalStorage;
using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TDiary.Common.Models.Entities;
using TDiary.Common.ServiceContracts;
using TDiary.Grpc.Protos;
using TDiary.Web.IndexedDB;
using TG.Blazor.IndexedDB;
using TDiary.Common.Extensions;
using TDiary.Common.Models.Domain.Enums;
using TDiary.Web.Services.Interfaces;

namespace TDiary.Web.Services
{
    public class SynchronizationService : ISynchronizationService
    {
        private const string lastSuccesfullSyncDatUtcKey = "lastSuccessfullSyncDateUtc";

        private readonly IndexedDBManager dbManager;
        private readonly IEventPlayerService eventPlayerService;
        private readonly EventProto.EventProtoClient eventClient;
        private readonly ILocalStorageService localStorageService;
        private readonly IMergeService mergeService;
        private readonly IEntityRelationsValidatorService entityRelationsValidator;
        private readonly IUpdateEventMergerService updateEventMergerService;

        public SynchronizationService(IndexedDBManager dbManager,
            IEventPlayerService eventPlayerService,
            EventProto.EventProtoClient eventClient,
            ILocalStorageService localStorageService,
            IMergeService mergeService,
            IEntityRelationsValidatorService entityRelationsValidator,
            IUpdateEventMergerService updateEventMergerService)
        {
            this.dbManager = dbManager;
            this.eventPlayerService = eventPlayerService;
            this.eventClient = eventClient;
            this.localStorageService = localStorageService;
            this.mergeService = mergeService;
            this.entityRelationsValidator = entityRelationsValidator;
            this.updateEventMergerService = updateEventMergerService;
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

            // TODO: create downloadable backup of unsynchronized events
            var mergeResult = mergeService.Merge(incomingEvents, unsynchronizedEvents);
            while(mergeResult.EventResolutions.TryDequeue(out var eventResolution))
            {
                switch (eventResolution.EventResolutionOperation)
                {

                    case EventResolutionOperation.UndoAndRemove:
                        await eventPlayerService.UndoEvent(eventResolution.Event);
                        await dbManager.DeleteRecord(StoreNameConstants.UnsynchronizedEvents, eventResolution.Event.Id);
                        break;
                    case EventResolutionOperation.Pull:
                        await dbManager.AddRecord(new StoreRecord<Event> 
                        { 
                            Storename = StoreNameConstants.Events, 
                            Data = eventResolution.Event 
                        });
                        await eventPlayerService.PlayEvent(eventResolution.Event);
                        break;
                    case EventResolutionOperation.Push:
                        await Push(eventResolution.Event);
                        break;
                    case EventResolutionOperation.PushIfValid:
                        if (await entityRelationsValidator.Validate(eventResolution.Event))
                        {
                            await Push(eventResolution.Event);
                        }
                        break;
                    case EventResolutionOperation.Merge:
                        var mergeEvent = updateEventMergerService.Merge(eventResolution.ServerEvent, eventResolution.Event);
                        await Push(mergeEvent);
                        await dbManager.AddRecord(new StoreRecord<Event>
                        {
                            Storename = StoreNameConstants.Events,
                            Data = mergeEvent
                        });
                        await eventPlayerService.PlayEvent(mergeEvent);
                        break;
                    case EventResolutionOperation.NoOp:
                        break;
                    default:
                        throw new NotImplementedException($"Event resolution {eventResolution.EventResolutionOperation} not implemented.");
                }
            }

            await localStorageService.SetItemAsync(lastSuccesfullSyncDatUtcKey, DateTime.UtcNow);
        }

        private async Task Push(Event eventEntity)
        {
            await eventClient.AddEventAsync(new AddEventRequest
            {
                EventData = new EventData
                {
                    AuditData = new AuditData
                    {
                        CreatedAt = Timestamp.FromDateTime(eventEntity.CreatedAt.AsUtc()),
                        CreatedAtUtc = Timestamp.FromDateTime(eventEntity.CreatedAtUtc.AsUtc()),
                        ModifiedAt = Timestamp.FromDateTime(eventEntity.ModifiedtAt.AsUtcNullMinimum()),
                        ModifiedAtUtc = Timestamp.FromDateTime(eventEntity.ModifiedAtUtc.AsUtcNullMinimum()),
                        TimeZone = eventEntity.TimeZone
                    },
                    Data = eventEntity.Data,
                    Entity = eventEntity.Entity,
                    EventType = (EventType)eventEntity.EventType,
                    Id = eventEntity.Id.ToString(),
                    UserId = eventEntity.UserId.ToString(),
                    Version = eventEntity.Version
                }
            });
            await dbManager.AddRecord(new StoreRecord<Event>
            {
                Storename = StoreNameConstants.Events,
                Data = eventEntity
            });
        }
        private async Task<List<Event>> GetRemoteEvents(Guid userId, DateTime lastEventDateUtc)
        {
            //sync flow:
            // if access token OK => start SYNC process => log last sync date
            // if access token exception => 3. if sts online => go to login
            //                                 if sts offline => skip and check last sync date to show warning if it was a long time ago

            // maybe simpler sync flow:
            // if get remotes events OK => sync
            // if get remote events fails => skip and log reason for failure
            // if the fail is access token related it will get refreshed when navigating to some other page at some point and the sync will run there


            // repository flow
            // if online => sync
            // return data from cache

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
