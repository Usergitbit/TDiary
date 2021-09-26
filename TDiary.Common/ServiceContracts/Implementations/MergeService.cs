using System;
using System.Collections.Generic;
using System.Linq;
using TDiary.Common.Models.Domain;
using TDiary.Common.Models.Domain.Enums;
using TDiary.Common.Models.Entities;
using TDiary.Common.Extensions;
using TDiary.Common.Models.Entities.Enums;

namespace TDiary.Common.ServiceContracts.Implementations
{
    public class MergeService : IMergeService
    {
        public MergeResult Merge(IEnumerable<Event> incomingEvents, IEnumerable<Event> outgoingEvents)
        {
            var result = new MergeResult();
            var unresolvedIncomingEvents = new List<Event>(incomingEvents);
            var unresolvedfOutgoingEvents = new List<Event>(outgoingEvents);
            var remoteEventResolutions = ResolveRemoteEvents(unresolvedIncomingEvents, unresolvedfOutgoingEvents);
            foreach (var remoteEventResolution in remoteEventResolutions)
            {
                result.EventResolutions.Enqueue(remoteEventResolution);
            }

            return result;
        }

        // TOO COMPLICATED?
        // maybe simpler flow: undo all local events, order everything by date and go one by one depending on date updating the remaining depending on operation?

        /// <summary>
        /// General merge strategy: server is source of truth for conflicts that can not be merged.<br/>
        /// Cases:<br/>
        /// 1. inserted - pull incoming, push outgoing <br/>
        /// 2. updated on server only pull incoming
        /// 3. updated locally only push outgoing
        /// 4. updated on server and updated locally - created merge event (play locally, push to server) <br/>
        /// 5. updated on server and deleted locally: <br/>
        /// --a. updated on server newer than deleted locally -> undo locally, pull incoming <br/>
        /// --b. updated on server older than deleted locally -> push to server <br/>
        /// 6. updated locally and deleted on server: <br/>
        /// --a. deleted on server newer than updated locally -> remove local event, pull incoming <br/>
        /// --b. deleted on server older than updated locally -> remove local event, pull incoming  <br/>
        /// 7. deleted on both -> pull incoming (should be no-op, but should be removed from local) <br/>
        /// 8. deleted on server
        /// 9. deleted locally
        /// </summary>
        /// <param name="unresolvedIncomingEvents"></param>
        /// <param name="unresolvedfOutgoingEvents"></param>
        /// <returns></returns>
        private IEnumerable<EventResolution> ResolveRemoteEvents(List<Event> unresolvedIncomingEvents, List<Event> unresolvedfOutgoingEvents)
        {
            var result = new List<EventResolution>();

            // 1.
            var insertResolutions = HandleInsertedBrands(unresolvedIncomingEvents, unresolvedfOutgoingEvents);
            result.AddRange(insertResolutions);

            // 2. && 3.
            var nonConflictingUpdatesResolutions = HandleNonConflictingUpdatedBrands(unresolvedIncomingEvents, unresolvedfOutgoingEvents);
            result.AddRange(nonConflictingUpdatesResolutions);

            // 4.
            var updatedServerUpdatedLocallyResolutions = HandleUpdatedServerUpdatedLocallyBrands(unresolvedIncomingEvents, unresolvedfOutgoingEvents);
            result.AddRange(updatedServerUpdatedLocallyResolutions);

            // 5.
            var updatedOnServerDeletedLocallyResolutions = HandleUpdatedOnServerDeletedLocallyBrands(unresolvedIncomingEvents, unresolvedfOutgoingEvents);
            result.AddRange(updatedOnServerDeletedLocallyResolutions);

            // 6.
            var updatedLocallyDeletedOnServerResolutions = HandleUpdatedLocallyDeletedOnServerBrands(unresolvedIncomingEvents, unresolvedfOutgoingEvents);
            result.AddRange(updatedLocallyDeletedOnServerResolutions);

            // 7.
            var deletedInBothResolutions = HandleDeletedInBothBrands(unresolvedIncomingEvents, unresolvedfOutgoingEvents);
            result.AddRange(deletedInBothResolutions);

            // 8. 
            var deletedOnServer = HandleDeletedInServerBrands(unresolvedIncomingEvents, unresolvedfOutgoingEvents);
            result.AddRange(deletedOnServer);

            // 9. 
            // WHAT TO DO IF DELTED LOCALLY HAVE UPDATES REMOTE?

            return result;
        }

        private IEnumerable<EventResolution> HandleInsertedBrands(List<Event> unresolvedIncomingEvents, List<Event> unresolvedfOutgoingEvents)
        {
            var result = new List<EventResolution>();

            var brandsToPull = unresolvedIncomingEvents.Brands()
                .Inserts()
                .ToArray();
            foreach (var brand in brandsToPull)
            {
                unresolvedIncomingEvents.Remove(brand);
            }

            var brandsToPullResolutions = brandsToPull.Select(e => new EventResolution
            {
                Event = e,
                EventResolutionOperation = EventResolutionOperation.Pull,
                Reason = "Server Brand insert is independent."
            });

            result.AddRange(brandsToPullResolutions);

            var brandsToPush = unresolvedfOutgoingEvents.Brands()
                .Inserts()
                .ToArray();
            foreach (var brand in brandsToPush)
            {
                unresolvedfOutgoingEvents.Remove(brand);
            }

            var brandsToPushResolutions = brandsToPush.Select(e => new EventResolution
            {
                Event = e,
                EventResolutionOperation = EventResolutionOperation.Push,
                Reason = "Local Brand insert is independent."
            });

            result.AddRange(brandsToPushResolutions);

            return result;
        }

        private IEnumerable<EventResolution> HandleUpdatedServerUpdatedLocallyBrands(List<Event> unresolvedIncomingEvents, List<Event> unresolvedfOutgoingEvents)
        {
            var result = new List<EventResolution>();
            var deletedLocallyIds = unresolvedfOutgoingEvents.Brands()
                .Deletes()
                .Select(e => e.EntityId)
                .ToArray();
            var serverUpdatedBrandEvents = unresolvedIncomingEvents.Brands()
                .Updates()
                .Where(e => !deletedLocallyIds.Contains(e.EntityId))
                .ToArray();
            foreach (var updatedBrandEvent in serverUpdatedBrandEvents)
            {
                var outgoingUpdateEvent = unresolvedfOutgoingEvents.Brands()
                    .Updates()
                    .FirstOrDefault(e => e.EntityId == updatedBrandEvent.EntityId);
                var updatedLocally = outgoingUpdateEvent != null;
                if (updatedLocally)
                {
                    result.Add(new EventResolution
                    {
                        Event = outgoingUpdateEvent,
                        ServerEvent = updatedBrandEvent,
                        EventResolutionOperation = EventResolutionOperation.Merge,
                        Reason = "Brand updated both locally and on server, merge update event needs to be created."
                    });

                    unresolvedIncomingEvents.Remove(updatedBrandEvent);
                    unresolvedfOutgoingEvents.Remove(outgoingUpdateEvent);
                }
            }

            return result;
        }

        private IEnumerable<EventResolution> HandleNonConflictingUpdatedBrands(List<Event> unresolvedIncomingEvents, List<Event> unresolvedfOutgoingEvents)
        {
            var result = new List<EventResolution>();
            var incomingUpdatedBrands = GetIncomingUpdatedNotTouchedLocally(unresolvedIncomingEvents, unresolvedfOutgoingEvents);
            foreach (var incomingUpdatedBrand in incomingUpdatedBrands)
            {
                result.Add(new EventResolution
                {
                    Event = incomingUpdatedBrand,
                    EventResolutionOperation = EventResolutionOperation.Pull,
                    Reason = "Updated on only server must be pulled."
                });
                unresolvedIncomingEvents.Remove(incomingUpdatedBrand);
            }

            var outgoingUpdatedBrands = GetOutgoingUpdatedNotTouchedOnServer(unresolvedIncomingEvents, unresolvedfOutgoingEvents);
            foreach (var outgoingUpdatedBrand in outgoingUpdatedBrands)
            {
                result.Add(new EventResolution
                {
                    Event = outgoingUpdatedBrand,
                    EventResolutionOperation = EventResolutionOperation.Push,
                    Reason = "Updated only locally must be pushed."
                });
                unresolvedfOutgoingEvents.Remove(outgoingUpdatedBrand);
            }

            return result;
        }

        private Event[] GetIncomingUpdatedNotTouchedLocally(List<Event> unresolvedIncomingEvents, List<Event> unresolvedfOutgoingEvents)
        {
            var touchedLocallyIds = unresolvedfOutgoingEvents.Brands()
                .Where(e => e.EventType == EventType.Update || e.EventType == EventType.Delete)
                .Select(e => e.EntityId)
                .ToArray();

            var updatedIncomingNotDeletedLocally = unresolvedIncomingEvents.Brands()
                .Updates()
                .Where(e => !touchedLocallyIds.Contains(e.EntityId))
                .ToArray();

            return updatedIncomingNotDeletedLocally;
        }

        private Event[] GetOutgoingUpdatedNotTouchedOnServer(List<Event> unresolvedIncomingEvents, List<Event> unresolvedfOutgoingEvents)
        {
            var touchedOnServerIds = unresolvedIncomingEvents.Brands()
                .Where(e => e.EventType == EventType.Update || e.EventType == EventType.Delete)
                .Select(e => e.EntityId)
                .ToArray();

            var updatedOutgoingNotDeletedOnServer = unresolvedfOutgoingEvents.Brands()
                .Updates()
                .Where(e => !touchedOnServerIds.Contains(e.EntityId))
                .ToArray();

            return updatedOutgoingNotDeletedOnServer;
        }

        private IEnumerable<EventResolution> HandleUpdatedOnServerDeletedLocallyBrands(List<Event> unresolvedIncomingEvents, List<Event> unresolvedfOutgoingEvents)
        {
            var result = new List<EventResolution>();
            var deletedLocallyIds = unresolvedfOutgoingEvents.Brands()
                .Deletes()
                .Select(e => e.EntityId)
                .ToArray();
            var updatedOnServerDeletedLocallyEvents = unresolvedIncomingEvents.Brands()
                .Updates()
                .Where(e => deletedLocallyIds.Contains(e.EntityId))
                .ToArray();
            foreach (var updatedOnServerDeletedLocally in updatedOnServerDeletedLocallyEvents)
            {
                var deletedLocallyEvent = unresolvedfOutgoingEvents.FirstOrDefault(e => e.EntityId == updatedOnServerDeletedLocally.EntityId);
                if (updatedOnServerDeletedLocally.CreatedAtUtc > deletedLocallyEvent.CreatedAtUtc)
                {
                    result.Add(new EventResolution
                    {
                        Event = deletedLocallyEvent,
                        EventResolutionOperation = EventResolutionOperation.UndoAndRemove,
                        Reason = "Local deletion must be undone when updated on server at a later date."
                    });
                    unresolvedfOutgoingEvents.Remove(deletedLocallyEvent);
                    result.Add(new EventResolution
                    {
                        Event = updatedOnServerDeletedLocally,
                        EventResolutionOperation = EventResolutionOperation.Pull,
                        Reason = "Updated on server should be pulled when newer than local deletion."
                    });
                    unresolvedIncomingEvents.Remove(updatedOnServerDeletedLocally);
                }
                else
                {
                    result.Add(new EventResolution
                    {
                        Event = deletedLocallyEvent,
                        EventResolutionOperation = EventResolutionOperation.Push,
                        Reason = "Local deletion should be pushed to server if newer than update on server."
                    });
                    unresolvedfOutgoingEvents.Remove(deletedLocallyEvent);
                    unresolvedIncomingEvents.Remove(updatedOnServerDeletedLocally);
                }
            }

            return result;
        }

        private IEnumerable<EventResolution> HandleUpdatedLocallyDeletedOnServerBrands(List<Event> unresolvedIncomingEvents, List<Event> unresolvedfOutgoingEvents)
        {
            var result = new List<EventResolution>();
            var deletedOnServerIds = unresolvedIncomingEvents.Brands()
                .Deletes()
                .Select(e => e.EntityId)
                .ToArray();
            var updatedLocallyDeletedOnServerBrands = unresolvedfOutgoingEvents
                .Brands()
                .Updates()
                .Where(e => deletedOnServerIds.Contains(e.EntityId))
                .ToArray();
            foreach (var updatedLocallyDeletedOnServer in updatedLocallyDeletedOnServerBrands)
            {
                var deletedOnServerEvent = unresolvedIncomingEvents.Brands()
                    .Deletes()
                    .FirstOrDefault(e => e.EntityId == updatedLocallyDeletedOnServer.EntityId);
                result.Add(new EventResolution
                {
                    Event = updatedLocallyDeletedOnServer,
                    EventResolutionOperation = EventResolutionOperation.UndoAndRemove,
                    Reason = "Local deleted must be undone when updated on server at later date."
                });
                unresolvedfOutgoingEvents.Remove(updatedLocallyDeletedOnServer);
                result.Add(new EventResolution
                {
                    Event = deletedOnServerEvent,
                    EventResolutionOperation = EventResolutionOperation.Pull,
                    Reason = "Server deleted must be pulled when newer than local deleted"
                });
                unresolvedIncomingEvents.Remove(deletedOnServerEvent);
            }

            return result;
        }

        private IEnumerable<EventResolution> HandleDeletedInBothBrands(List<Event> unresolvedIncomingEvents, List<Event> unresolvedfOutgoingEvents)
        {
            var result = new List<EventResolution>();
            var deletedOnServer = unresolvedIncomingEvents.Brands()
                .Deletes()
                .Select(e => e.EntityId)
                .ToArray();
            var deletedLocally = unresolvedfOutgoingEvents.Brands()
                .Deletes()
                .Select(e => e.EntityId)
                .ToArray();
            var deletedInBothIds = deletedOnServer.Intersect(deletedLocally);
            foreach(var deletedInBoth in deletedInBothIds)
            {
                var locallyDeleted = unresolvedfOutgoingEvents.Brands()
                    .FirstOrDefault(e => e.EntityId == deletedInBoth);
                result.Add(new EventResolution
                {
                    Event = locallyDeleted,
                    EventResolutionOperation = EventResolutionOperation.Remove,
                    Reason = "Deleted in both event can be removed from local."
                });
                unresolvedfOutgoingEvents.Remove(locallyDeleted);

                var serverDeleted = unresolvedIncomingEvents.Brands()
                    .FirstOrDefault(e => e.EntityId == deletedInBoth);
                result.Add(new EventResolution
                {
                    Event = serverDeleted,
                    EventResolutionOperation = EventResolutionOperation.Pull,
                    Reason = "Deleted in both event be pulled as deletion is idempotent."
                });
                unresolvedIncomingEvents.Remove(serverDeleted);
            }

            return result;
        }


        private IEnumerable<EventResolution> HandleDeletedInServerBrands(List<Event> unresolvedIncomingEvents, List<Event> unresolvedfOutgoingEvents)
        {
            var result = new List<EventResolution>();
            var deletedInServer = unresolvedIncomingEvents.Brands()
                .Deletes()
                .ToArray();
            foreach(var deletedBrand in deletedInServer)
            {
                result.Add(new EventResolution
                {
                    Event = deletedBrand,
                    EventResolutionOperation = EventResolutionOperation.Pull,
                    Reason = "Deleted only in server can be pulled."
                });
                // TODO: DELETED RELATED ENTITIES + EVENTS?
            }

            return result;

        }
    }
}
