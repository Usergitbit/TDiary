﻿using System.Collections.Generic;
using System.Linq;
using TDiary.Common.Models.Domain;
using TDiary.Common.Models.Domain.Enums;
using TDiary.Common.Models.Entities;
using TDiary.Common.Models.Entities.Enums;

namespace TDiary.Common.ServiceContracts.Implementations
{
    public class MergeService : IMergeService
    {
        public MergeResult Merge(IReadOnlyList<Event> incomingEvents, IReadOnlyList<Event> outgoingEvents)
        {
            var mergeResult = new MergeResult
            {
                EventResolutions = ResolveEvents(incomingEvents, outgoingEvents)
            };

            return mergeResult;
        }

        private Queue<EventResolution> ResolveEvents(IReadOnlyList<Event> incomingEvents, IReadOnlyList<Event> outgoingEvents)
        {
            var eventResolutions = new Queue<EventResolution>();
            if (incomingEvents.Count == 0)
            {
                foreach (var outgoingEvent in outgoingEvents)
                {
                    var eventResolution = new EventResolution
                    {
                        Event = outgoingEvent,
                        EventResolutionOperation = EventResolutionOperation.Push,
                        Reason = "Local events can be pushed when no incoming events."
                    };
                    eventResolutions.Enqueue(eventResolution);
                }

                return eventResolutions;
            }

            var reversedOrderOutgoingEvents = outgoingEvents.OrderByDescending(e => e.CreatedAtUtc).ToList();
            foreach (var outgoingEvent in reversedOrderOutgoingEvents)
            {
                var eventResolution = new EventResolution
                {
                    Event = outgoingEvent,
                    EventResolutionOperation = EventResolutionOperation.Undo,
                    Reason = "Undo all local events before playing remote events."
                };
                eventResolutions.Enqueue(eventResolution);
            }

            var incomingEventsResolutionState = ResolveRemoteEvents(incomingEvents);
            foreach (var eventResolution in incomingEventsResolutionState.EventResolutions)
            {
                eventResolutions.Enqueue(eventResolution);
            }

            var localEventsResolutions = ReconcileLocalEvents(incomingEventsResolutionState, outgoingEvents);
            foreach (var localEventResolution in localEventsResolutions)
            {
                eventResolutions.Enqueue(localEventResolution);
            }

            return eventResolutions;
        }

        private IncomingEventsResolutionState ResolveRemoteEvents(IReadOnlyList<Event> incomingEvents)
        {
            var result = new IncomingEventsResolutionState();
            foreach (var incomingEvent in incomingEvents)
            {
                result.EventResolutions.Add(new EventResolution
                {
                    Event = incomingEvent,
                    EventResolutionOperation = EventResolutionOperation.Pull,
                    Reason = "Remote events must be played after undoing local ones."
                });
                var entityState = EntityState.Untouched;
                switch (incomingEvent.EventType)
                {
                    case EventType.Insert:
                        entityState = EntityState.Inserted;
                        break;
                    case EventType.Update:
                        entityState = EntityState.Updated;
                        break;
                    case EventType.Delete:
                        entityState = EntityState.Deleted;
                        break;
                    default:
                        break;
                }
                var affectedEntity = result.AffectedEntities.FirstOrDefault(e => e.AffectedEntityId == incomingEvent.EntityId);
                if (affectedEntity == null)
                {
                    affectedEntity = new AffectedEntity
                    {
                        AffectedEntityId = incomingEvent.EntityId,
                        LastAffectingEvent = incomingEvent,
                    };
                    result.AffectedEntities.Add(affectedEntity);
                }
                affectedEntity.EntityState = entityState;
            }

            return result;
        }

        private IEnumerable<EventResolution> ReconcileLocalEvents(IncomingEventsResolutionState incomingEventResolutionState, IReadOnlyList<Event> outgoingEvents)
        {
            var result = new List<EventResolution>();
            foreach (var outgoingEvent in outgoingEvents)
            {
                // TODO: need to check relations as well
                var isAffectedByIncoming = incomingEventResolutionState.AffectedEntities.FirstOrDefault(e => e.AffectedEntityId == outgoingEvent.EntityId) != null;
                if (!isAffectedByIncoming)
                {
                    result.Add(new EventResolution
                    {
                        Event = outgoingEvent,
                        EventResolutionOperation = EventResolutionOperation.PushIfValid,
                        Reason = "Local event is not directly affected by pulled changes but relationships could be and should only be pushed if valid else it will be dropped."
                    });
                }
                else
                {
                    var eventResolution = ReconcileLocalEvent(outgoingEvent, incomingEventResolutionState);
                    result.Add(eventResolution);
                }
            }

            return result;
        }

        private EventResolution ReconcileLocalEvent(Event outgoingEvent, IncomingEventsResolutionState incomingEventResolutionState)
        {
            var eventResolution = new EventResolution
            {
                Event = outgoingEvent,
                EventResolutionOperation = EventResolutionOperation.None,
                Reason = $"Could not determine course of action for event type {outgoingEvent.EventType} with id {outgoingEvent.Id}, entity {outgoingEvent.Entity} with entityId {outgoingEvent.EntityId}"
            };

            var affectedEntity = incomingEventResolutionState.AffectedEntities.FirstOrDefault(ae => ae.AffectedEntityId == outgoingEvent.EntityId);
            switch (affectedEntity.EntityState)
            {
                case EntityState.Updated:
                    if (outgoingEvent.EventType == EventType.Update)
                    {
                        eventResolution = new EventResolution
                        {
                            Event = outgoingEvent,
                            EventResolutionOperation = EventResolutionOperation.Merge,
                            Reason = "Entity was updated both locally and on server, changes must be merged.",
                            ServerEvent = affectedEntity.LastAffectingEvent
                        };
                    }
                    if (outgoingEvent.EventType == EventType.Delete)
                    {
                        eventResolution = new EventResolution
                        {
                            Event = outgoingEvent,
                            EventResolutionOperation = EventResolutionOperation.None,
                            // TODO: at a later date? there is no date check? server is always the driver?
                            Reason = "Entity was updated on the server at a later date than it was deleted locally, no change will be performed."
                        };
                    }
                    break;
                case EntityState.Deleted:
                    eventResolution = new EventResolution
                    {
                        Event = outgoingEvent,
                        EventResolutionOperation = EventResolutionOperation.None,
                        Reason = $"Entity was deleted on the server, local event type {outgoingEvent.EventType} will be dropped.",
                    };
                    break;
            }

            return eventResolution;
        }

    }
}
