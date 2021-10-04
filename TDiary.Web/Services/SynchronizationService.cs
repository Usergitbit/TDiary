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
using TDiary.Grpc.ServiceContracts;

namespace TDiary.Web.Services
{
    public class SynchronizationService : ISynchronizationService
    {
        private const string lastSuccesfullSyncDateUtcKey = "lastSuccessfullSyncDateUtc";
        private const string fullSyncRequired = "fullSyncRequired";
        private const string unsynchronizedEventsBackup = "unsynchronizedEventsBackup";

        private readonly IndexedDBManager dbManager;
        private readonly IEventPlayerService eventPlayerService;
        private readonly EventProto.EventProtoClient eventClient;
        private readonly ILocalStorageService localStorageService;
        private readonly IMergeService mergeService;
        private readonly IEntityRelationsValidatorService entityRelationsValidator;
        private readonly IUpdateEventMergerService updateEventMergerService;
        private readonly NetworkStateService networkStateService;
        private readonly IManualGrpcMapper manualGrpcMapper;

        public SynchronizationService(IndexedDBManager dbManager,
            IEventPlayerService eventPlayerService,
            EventProto.EventProtoClient eventClient,
            ILocalStorageService localStorageService,
            IMergeService mergeService,
            IEntityRelationsValidatorService entityRelationsValidator,
            IUpdateEventMergerService updateEventMergerService,
            NetworkStateService networkStateService,
            IManualGrpcMapper manualGrpcMapper)
        {
            this.dbManager = dbManager;
            this.eventPlayerService = eventPlayerService;
            this.eventClient = eventClient;
            this.localStorageService = localStorageService;
            this.mergeService = mergeService;
            this.entityRelationsValidator = entityRelationsValidator;
            this.updateEventMergerService = updateEventMergerService;
            this.networkStateService = networkStateService;
            this.manualGrpcMapper = manualGrpcMapper;
        }

        //TODO: test the access token thing to be sure, maybe find a way to refresh token from here if the handler doesn't do it?

        /// <summary>
        /// sync flow:
        /// if ping fails => skip <br/>
        /// if get remotes events OK => sync <br/>
        /// if get remote events fails => skip and log reason for failure <br/>
        /// if the fail is access token related it will get refreshed when navigating to some other page at some point and the sync will run there <br/>
        /// flow of an operation: <br/>
        /// if online => sync <br/>
        /// perform local operations <br/>
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task Synchronize(Guid userId)
        {
            var apiAvailable = await networkStateService.IsApiOnline();
            if (!apiAvailable)
            {
                Console.WriteLine($"Aborting sync. Api is not available.");
                return;
            }

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
                incomingEvents = await GetRemoteEvents(lastEventDate);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Aborting sync. Failed to get incoming events: {ex.Message}\n{ex.StackTrace}");
                return;
            }

            // TODO: put all local operations in a transaction (not supported indexdb library atm), group all push operations and send to server in bulk
            // if push successful -> commit transaction
            // if push fails -> revert transaction
            // local transactions are unlikely to fail except maybe due to low memory but if pushes to server were successful it's okay
            // if local transaction fails after push -> critical error and full resync needed 
            try
            {
                var unsynchronizedEvents = await GetUnsynchronized(userId);
                var backupDate = DateTime.UtcNow;
                await localStorageService.SetItemAsync($"{unsynchronizedEventsBackup}_{backupDate:yyyy-MM-dd-HH-mm-ss}", unsynchronizedEvents);
                var unsynced = await localStorageService.GetItemAsync<List<Event>>($"{unsynchronizedEventsBackup}_{backupDate:yyyy-MM-dd-HH-mm-ss}");
                var mergeResult = mergeService.Merge(incomingEvents, unsynchronizedEvents);
                while (mergeResult.EventResolutions.TryDequeue(out var eventResolution))
                {
                    switch (eventResolution.EventResolutionOperation)
                    {

                        case EventResolutionOperation.Undo:
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
                            await dbManager.DeleteRecord(StoreNameConstants.UnsynchronizedEvents, eventResolution.Event.Id);
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
                        case EventResolutionOperation.None:
                            break;
                        default:
                            throw new NotImplementedException($"Event resolution {eventResolution.EventResolutionOperation} not implemented.");
                    }
                }

                await localStorageService.SetItemAsync(lastSuccesfullSyncDateUtcKey, DateTime.UtcNow);
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Critical sync failure: {ex.Message}\n{ex.StackTrace}. Full sync required to recover.");
                await localStorageService.SetItemAsync(fullSyncRequired, true);
            }
        }

        private async Task Push(Event eventEntity)
        {
            var eventData = manualGrpcMapper.Map(eventEntity);
            await eventClient.AddEventAsync(new AddEventRequest
            {
                EventData = eventData
            });

            await dbManager.AddRecord(new StoreRecord<Event>
            {
                Storename = StoreNameConstants.Events,
                Data = eventEntity
            });
        }
        private async Task<List<Event>> GetRemoteEvents(DateTime lastEventDateUtc)
        {
            var reply = await eventClient.GetEventsAsync(new GetEventsRequest
            {
                LastEventDateUtc = Timestamp.FromDateTime(lastEventDateUtc.AsUtc())
            });

            var events = new List<Event>(); 
            foreach(var eventData in reply.EventData)
            {
                var eventEntity = manualGrpcMapper.Map(eventData);
                events.Add(eventEntity);
            }

            return events;
        }

        private async Task<DateTime> GetLastEventDateTime(Guid userId)
        {
            //TODO: optimize to query by max somehow (upper bound in index db not supported by library)
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

            return events.OrderBy(e => e.CreatedAtUtc).ToList();
        }
    }


}
