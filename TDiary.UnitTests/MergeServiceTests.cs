using FluentAssertions;
using System;
using System.Collections.Generic;
using TDiary.Common.Models.Domain;
using TDiary.Common.Models.Domain.Enums;
using TDiary.Common.Models.Entities;
using TDiary.Common.Models.Entities.Enums;
using TDiary.Common.ServiceContracts;
using TDiary.Common.ServiceContracts.Implementations;
using Xunit;

namespace TDiary.UnitTests
{

    public class MergeServiceTests
    {
        private readonly IMergeService mergeService = new MergeService();

        [Fact(DisplayName = "Incoming events only")]
        public void IncomingEventsOnly()
        {
            // Arrange
            var incomingEvents = new List<Event>
            {
                new()
                {
                    EventType = EventType.Insert,
                    Data = "data",
                    CreatedAt = DateTime.Now,
                    CreatedAtUtc = DateTime.UtcNow,
                    Entity = "Brand",
                    Id = Guid.NewGuid(),
                    EntityId = Guid.NewGuid(),
                    TimeZone = "timezone",
                    UserId = Guid.NewGuid(),
                    Version = 1
                },
                new()
                {
                    EventType = EventType.Update,
                    Data = "data2",
                    CreatedAt = DateTime.Now,
                    CreatedAtUtc = DateTime.UtcNow,
                    Entity = "Brand",
                    Id = Guid.NewGuid(),
                    EntityId = Guid.NewGuid(),
                    TimeZone = "timezone",
                    UserId = Guid.NewGuid(),
                    Changes = "changes",
                    Version = 1
                },
                new()
                {
                    EventType = EventType.Delete,
                    Data = "data3",
                    CreatedAt = DateTime.Now,
                    CreatedAtUtc = DateTime.UtcNow,
                    Entity = "Brand",
                    Id = Guid.NewGuid(),
                    EntityId = Guid.NewGuid(),
                    TimeZone = "timezone",
                    UserId = Guid.NewGuid(),
                    Version = 1
                }
            };
            var outGoingEvents = new List<Event>();

            // Act
            var result = mergeService.Merge(incomingEvents, outGoingEvents);

            // Assert
            var eventResolutions = result.EventResolutions;
            eventResolutions.Should().HaveCount(incomingEvents.Count);
            eventResolutions.Should().OnlyContain(e => e.EventResolutionOperation == EventResolutionOperation.Pull);
        }

        [Fact(DisplayName = "Outgoing events only")]
        public void OutgoingEventsOnly()
        {
            // Arrange
            var outgoingEvents = new List<Event>
            {
                new()
                {
                    EventType = EventType.Insert,
                    Data = "data",
                    CreatedAt = DateTime.Now,
                    CreatedAtUtc = DateTime.UtcNow,
                    Entity = "Brand",
                    Id = Guid.NewGuid(),
                    EntityId = Guid.NewGuid(),
                    TimeZone = "timezone",
                    UserId = Guid.NewGuid(),
                    Version = 1
                },
                new()
                {
                    EventType = EventType.Update,
                    Data = "data2",
                    CreatedAt = DateTime.Now,
                    CreatedAtUtc = DateTime.UtcNow,
                    Entity = "Brand",
                    Id = Guid.NewGuid(),
                    EntityId = Guid.NewGuid(),
                    TimeZone = "timezone",
                    UserId = Guid.NewGuid(),
                    Changes = "changes",
                    Version = 1
                },
                new()
                {
                    EventType = EventType.Delete,
                    Data = "data3",
                    CreatedAt = DateTime.Now,
                    CreatedAtUtc = DateTime.UtcNow,
                    Entity = "Brand",
                    Id = Guid.NewGuid(),
                    EntityId = Guid.NewGuid(),
                    TimeZone = "timezone",
                    UserId = Guid.NewGuid(),
                    Version = 1
                }
            };
            var incomingEvents = new List<Event>();

            // Act
            var result = mergeService.Merge(incomingEvents, outgoingEvents);

            // Assert
            var eventResolutions = result.EventResolutions;
            eventResolutions.Should().HaveCount(outgoingEvents.Count);
            eventResolutions.Should().OnlyContain(e => e.EventResolutionOperation == EventResolutionOperation.Push);
        }

        [Fact(DisplayName = "Incoming inserts and outgoing inserts")]
        public void IncomingInsertsAndOutgoingInserts()
        {
            // Arrange
            var incomingEvents = new List<Event>
            {
                new()
                {
                    EventType = EventType.Insert,
                    Data = "data1",
                    CreatedAt = DateTime.Now,
                    CreatedAtUtc = DateTime.UtcNow,
                    Entity = "Brand",
                    Id = Guid.NewGuid(),
                    EntityId = Guid.NewGuid(),
                    TimeZone = "timezone",
                    UserId = Guid.NewGuid(),
                    Version = 1
                },
                new()
                {
                    EventType = EventType.Insert,
                    Data = "data2",
                    CreatedAt = DateTime.Now,
                    CreatedAtUtc = DateTime.UtcNow,
                    Entity = "Brand",
                    Id = Guid.NewGuid(),
                    EntityId = Guid.NewGuid(),
                    TimeZone = "timezone",
                    UserId = Guid.NewGuid(),
                    Version = 1
                },
                new()
                {
                    EventType = EventType.Insert,
                    Data = "data3",
                    CreatedAt = DateTime.Now,
                    CreatedAtUtc = DateTime.UtcNow,
                    Entity = "Brand",
                    Id = Guid.NewGuid(),
                    EntityId = Guid.NewGuid(),
                    TimeZone = "timezone",
                    UserId = Guid.NewGuid(),
                    Version = 1
                },
            };

            var outgoingEvents = new List<Event>
            {
                new()
                {
                    EventType = EventType.Insert,
                    Data = "data4",
                    CreatedAt = DateTime.Now,
                    CreatedAtUtc = DateTime.UtcNow,
                    Entity = "Brand",
                    Id = Guid.NewGuid(),
                    EntityId = Guid.NewGuid(),
                    TimeZone = "timezone",
                    UserId = Guid.NewGuid(),
                    Version = 1
                },
                new()
                {
                    EventType = EventType.Insert,
                    Data = "data5",
                    CreatedAt = DateTime.Now,
                    CreatedAtUtc = DateTime.UtcNow,
                    Entity = "Brand",
                    Id = Guid.NewGuid(),
                    EntityId = Guid.NewGuid(),
                    TimeZone = "timezone",
                    UserId = Guid.NewGuid(),
                    Version = 1
                },
                new()
                {
                    EventType = EventType.Insert,
                    Data = "data6",
                    CreatedAt = DateTime.Now,
                    CreatedAtUtc = DateTime.UtcNow,
                    Entity = "Brand",
                    Id = Guid.NewGuid(),
                    EntityId = Guid.NewGuid(),
                    TimeZone = "timezone",
                    UserId = Guid.NewGuid(),
                    Version = 1
                },
            };

            // Act
            var result = mergeService.Merge(incomingEvents, outgoingEvents);

            // Assert
            var eventResolutions = result.EventResolutions;
            eventResolutions.Count.Should().Be(incomingEvents.Count + outgoingEvents.Count + incomingEvents.Count);
            var resolution1 = eventResolutions.Dequeue();
            var resolution2 = eventResolutions.Dequeue();
            var resolution3 = eventResolutions.Dequeue();
            resolution1.EventResolutionOperation.Should().Be(EventResolutionOperation.UndoAndRemove);
            resolution2.EventResolutionOperation.Should().Be(EventResolutionOperation.UndoAndRemove);
            resolution3.EventResolutionOperation.Should().Be(EventResolutionOperation.UndoAndRemove);
            var resolution4 = eventResolutions.Dequeue();
            var resolution5 = eventResolutions.Dequeue();
            var resolution6 = eventResolutions.Dequeue();
            resolution4.EventResolutionOperation.Should().Be(EventResolutionOperation.Pull);
            resolution5.EventResolutionOperation.Should().Be(EventResolutionOperation.Pull);
            resolution6.EventResolutionOperation.Should().Be(EventResolutionOperation.Pull);
            var resolution7 = eventResolutions.Dequeue();
            var resolution8 = eventResolutions.Dequeue();
            var resolution9 = eventResolutions.Dequeue();
            resolution7.EventResolutionOperation.Should().Be(EventResolutionOperation.PushIfValid);
            resolution8.EventResolutionOperation.Should().Be(EventResolutionOperation.PushIfValid);
            resolution9.EventResolutionOperation.Should().Be(EventResolutionOperation.PushIfValid);
        }

        [Fact(DisplayName = "Incoming updates and outgoing inserts")]
        public void IncomingUpdatesOutgoingInserts()
        {
            // Arrange
            var incomingEvents = new List<Event>
            {
                new()
                {
                    EventType = EventType.Update,
                    Data = "data1",
                    CreatedAt = DateTime.Now,
                    CreatedAtUtc = DateTime.UtcNow,
                    Entity = "Brand",
                    Id = Guid.NewGuid(),
                    EntityId = Guid.NewGuid(),
                    TimeZone = "timezone",
                    UserId = Guid.NewGuid(),
                    Version = 1
                },
                new()
                {
                    EventType = EventType.Update,
                    Data = "data2",
                    CreatedAt = DateTime.Now,
                    CreatedAtUtc = DateTime.UtcNow,
                    Entity = "Brand",
                    Id = Guid.NewGuid(),
                    EntityId = Guid.NewGuid(),
                    TimeZone = "timezone",
                    UserId = Guid.NewGuid(),
                    Version = 1
                },
                new()
                {
                    EventType = EventType.Update,
                    Data = "data3",
                    CreatedAt = DateTime.Now,
                    CreatedAtUtc = DateTime.UtcNow,
                    Entity = "Brand",
                    Id = Guid.NewGuid(),
                    EntityId = Guid.NewGuid(),
                    TimeZone = "timezone",
                    UserId = Guid.NewGuid(),
                    Version = 1
                },
            };

            var outgoingEvents = new List<Event>
            {
                new()
                {
                    EventType = EventType.Insert,
                    Data = "data4",
                    CreatedAt = DateTime.Now,
                    CreatedAtUtc = DateTime.UtcNow,
                    Entity = "Brand",
                    Id = Guid.NewGuid(),
                    EntityId = Guid.NewGuid(),
                    TimeZone = "timezone",
                    UserId = Guid.NewGuid(),
                    Version = 1
                },
                new()
                {
                    EventType = EventType.Insert,
                    Data = "data5",
                    CreatedAt = DateTime.Now,
                    CreatedAtUtc = DateTime.UtcNow,
                    Entity = "Brand",
                    Id = Guid.NewGuid(),
                    EntityId = Guid.NewGuid(),
                    TimeZone = "timezone",
                    UserId = Guid.NewGuid(),
                    Version = 1
                },
                new()
                {
                    EventType = EventType.Insert,
                    Data = "data6",
                    CreatedAt = DateTime.Now,
                    CreatedAtUtc = DateTime.UtcNow,
                    Entity = "Brand",
                    Id = Guid.NewGuid(),
                    EntityId = Guid.NewGuid(),
                    TimeZone = "timezone",
                    UserId = Guid.NewGuid(),
                    Version = 1
                },
            };

            // Act
            var result = mergeService.Merge(incomingEvents, outgoingEvents);

            // Assert
            var eventResolutions = result.EventResolutions;
            eventResolutions.Count.Should().Be(incomingEvents.Count + outgoingEvents.Count + incomingEvents.Count);
            var resolution1 = eventResolutions.Dequeue();
            var resolution2 = eventResolutions.Dequeue();
            var resolution3 = eventResolutions.Dequeue();
            resolution1.EventResolutionOperation.Should().Be(EventResolutionOperation.UndoAndRemove);
            resolution2.EventResolutionOperation.Should().Be(EventResolutionOperation.UndoAndRemove);
            resolution3.EventResolutionOperation.Should().Be(EventResolutionOperation.UndoAndRemove);
            var resolution4 = eventResolutions.Dequeue();
            var resolution5 = eventResolutions.Dequeue();
            var resolution6 = eventResolutions.Dequeue();
            resolution4.EventResolutionOperation.Should().Be(EventResolutionOperation.Pull);
            resolution5.EventResolutionOperation.Should().Be(EventResolutionOperation.Pull);
            resolution6.EventResolutionOperation.Should().Be(EventResolutionOperation.Pull);
            var resolution7 = eventResolutions.Dequeue();
            var resolution8 = eventResolutions.Dequeue();
            var resolution9 = eventResolutions.Dequeue();
            resolution7.EventResolutionOperation.Should().Be(EventResolutionOperation.PushIfValid);
            resolution8.EventResolutionOperation.Should().Be(EventResolutionOperation.PushIfValid);
            resolution9.EventResolutionOperation.Should().Be(EventResolutionOperation.PushIfValid);
        }

        [Fact(DisplayName = "Incoming deletes and outgoing inserts")]
        public void IncomingDeletesOutgoingInserts()
        {
            // Arrange
            var incomingEvents = new List<Event>
            {
                new()
                {
                    EventType = EventType.Delete,
                    Data = "data1",
                    CreatedAt = DateTime.Now,
                    CreatedAtUtc = DateTime.UtcNow,
                    Entity = "Brand",
                    Id = Guid.NewGuid(),
                    EntityId = Guid.NewGuid(),
                    TimeZone = "timezone",
                    UserId = Guid.NewGuid(),
                    Version = 1
                },
                new()
                {
                    EventType = EventType.Delete,
                    Data = "data2",
                    CreatedAt = DateTime.Now,
                    CreatedAtUtc = DateTime.UtcNow,
                    Entity = "Brand",
                    Id = Guid.NewGuid(),
                    EntityId = Guid.NewGuid(),
                    TimeZone = "timezone",
                    UserId = Guid.NewGuid(),
                    Version = 1
                },
                new()
                {
                    EventType = EventType.Delete,
                    Data = "data3",
                    CreatedAt = DateTime.Now,
                    CreatedAtUtc = DateTime.UtcNow,
                    Entity = "Brand",
                    Id = Guid.NewGuid(),
                    EntityId = Guid.NewGuid(),
                    TimeZone = "timezone",
                    UserId = Guid.NewGuid(),
                    Version = 1
                },
            };

            var outgoingEvents = new List<Event>
            {
                new()
                {
                    EventType = EventType.Insert,
                    Data = "data4",
                    CreatedAt = DateTime.Now,
                    CreatedAtUtc = DateTime.UtcNow,
                    Entity = "Brand",
                    Id = Guid.NewGuid(),
                    EntityId = Guid.NewGuid(),
                    TimeZone = "timezone",
                    UserId = Guid.NewGuid(),
                    Version = 1
                },
                new()
                {
                    EventType = EventType.Insert,
                    Data = "data5",
                    CreatedAt = DateTime.Now,
                    CreatedAtUtc = DateTime.UtcNow,
                    Entity = "Brand",
                    Id = Guid.NewGuid(),
                    EntityId = Guid.NewGuid(),
                    TimeZone = "timezone",
                    UserId = Guid.NewGuid(),
                    Version = 1
                },
                new()
                {
                    EventType = EventType.Insert,
                    Data = "data6",
                    CreatedAt = DateTime.Now,
                    CreatedAtUtc = DateTime.UtcNow,
                    Entity = "Brand",
                    Id = Guid.NewGuid(),
                    EntityId = Guid.NewGuid(),
                    TimeZone = "timezone",
                    UserId = Guid.NewGuid(),
                    Version = 1
                },
            };

            // Act
            var result = mergeService.Merge(incomingEvents, outgoingEvents);

            // Assert
            var eventResolutions = result.EventResolutions;
            eventResolutions.Count.Should().Be(incomingEvents.Count + outgoingEvents.Count + incomingEvents.Count);
            var resolution1 = eventResolutions.Dequeue();
            var resolution2 = eventResolutions.Dequeue();
            var resolution3 = eventResolutions.Dequeue();
            resolution1.EventResolutionOperation.Should().Be(EventResolutionOperation.UndoAndRemove);
            resolution2.EventResolutionOperation.Should().Be(EventResolutionOperation.UndoAndRemove);
            resolution3.EventResolutionOperation.Should().Be(EventResolutionOperation.UndoAndRemove);
            var resolution4 = eventResolutions.Dequeue();
            var resolution5 = eventResolutions.Dequeue();
            var resolution6 = eventResolutions.Dequeue();
            resolution4.EventResolutionOperation.Should().Be(EventResolutionOperation.Pull);
            resolution5.EventResolutionOperation.Should().Be(EventResolutionOperation.Pull);
            resolution6.EventResolutionOperation.Should().Be(EventResolutionOperation.Pull);
            var resolution7 = eventResolutions.Dequeue();
            var resolution8 = eventResolutions.Dequeue();
            var resolution9 = eventResolutions.Dequeue();
            resolution7.EventResolutionOperation.Should().Be(EventResolutionOperation.PushIfValid);
            resolution8.EventResolutionOperation.Should().Be(EventResolutionOperation.PushIfValid);
            resolution9.EventResolutionOperation.Should().Be(EventResolutionOperation.PushIfValid);
        }

        [Fact(DisplayName = "Incoming update outgoing update to the same entity")]
        public void IncomingUpdateOutgoingUpdateToTheSameEntity()
        {
            // Arrange
            var entityId = Guid.NewGuid();
            var incomingEvents = new List<Event>
            {
                new()
                {
                    EventType = EventType.Update,
                    Data = "data1",
                    CreatedAt = DateTime.Now,
                    CreatedAtUtc = DateTime.UtcNow,
                    Entity = "Brand",
                    Changes = "changes",
                    Id = Guid.NewGuid(),
                    EntityId =entityId,
                    TimeZone = "timezone",
                    UserId = Guid.NewGuid(),
                    Version = 1
                },
            };

            var outgoingEvents = new List<Event>
            {
                new()
                {
                    EventType = EventType.Update,
                    Data = "data2",
                    CreatedAt = DateTime.Now,
                    CreatedAtUtc = DateTime.UtcNow,
                    Entity = "Brand",
                    Changes = "changes2",
                    Id = Guid.NewGuid(),
                    EntityId =entityId,
                    TimeZone = "timezone",
                    UserId = Guid.NewGuid(),
                    Version = 1
                },
            };

            // Act
            var result = mergeService.Merge(incomingEvents, outgoingEvents);

            // Assert
            var eventResolutions = result.EventResolutions;
            eventResolutions.Count.Should().Be(incomingEvents.Count + outgoingEvents.Count + incomingEvents.Count);
            var resolution1 = eventResolutions.Dequeue();
            var resolution2 = eventResolutions.Dequeue();
            var resolution3 = eventResolutions.Dequeue();
            resolution1.EventResolutionOperation.Should().Be(EventResolutionOperation.UndoAndRemove);
            resolution2.EventResolutionOperation.Should().Be(EventResolutionOperation.Pull);
            resolution3.EventResolutionOperation.Should().Be(EventResolutionOperation.Merge);
            resolution3.ServerEvent.Should().Be(resolution2.Event);
        }

        [Fact(DisplayName = "Incoming update outgoing delete to the same entity")]
        public void IncomingUpdateOutgoingDeleteToTheSameEntity()
        {
            // Arrange
            var entityId = Guid.NewGuid();
            var incomingEvents = new List<Event>
            {
                new()
                {
                    EventType = EventType.Update,
                    Data = "data1",
                    CreatedAt = DateTime.Now,
                    CreatedAtUtc = DateTime.UtcNow,
                    Entity = "Brand",
                    Changes = "changes",
                    Id = Guid.NewGuid(),
                    EntityId =entityId,
                    TimeZone = "timezone",
                    UserId = Guid.NewGuid(),
                    Version = 1
                },
            };

            var outgoingEvents = new List<Event>
            {
                new()
                {
                    EventType = EventType.Delete,
                    Data = "data2",
                    CreatedAt = DateTime.Now,
                    CreatedAtUtc = DateTime.UtcNow,
                    Entity = "Brand",
                    Changes = "changes2",
                    Id = Guid.NewGuid(),
                    EntityId =entityId,
                    TimeZone = "timezone",
                    UserId = Guid.NewGuid(),
                    Version = 1
                },
            };

            // Act
            var result = mergeService.Merge(incomingEvents, outgoingEvents);

            // Assert
            var eventResolutions = result.EventResolutions;
            eventResolutions.Count.Should().Be(incomingEvents.Count + outgoingEvents.Count + incomingEvents.Count);
            var resolution1 = eventResolutions.Dequeue();
            var resolution2 = eventResolutions.Dequeue();
            var resolution3 = eventResolutions.Dequeue();
            resolution1.EventResolutionOperation.Should().Be(EventResolutionOperation.UndoAndRemove);
            resolution2.EventResolutionOperation.Should().Be(EventResolutionOperation.Pull);
            resolution3.EventResolutionOperation.Should().Be(EventResolutionOperation.NoOp);
        }

        [Fact(DisplayName = "Incoming delete outgoing update to the same entity")]
        public void IncomingDeleteOutgoingUpdateToTheSameEntity()
        {
            // Arrange
            var entityId = Guid.NewGuid();
            var incomingEvents = new List<Event>
            {
                new()
                {
                    EventType = EventType.Delete,
                    Data = "data1",
                    CreatedAt = DateTime.Now,
                    CreatedAtUtc = DateTime.UtcNow,
                    Entity = "Brand",
                    Changes = "changes",
                    Id = Guid.NewGuid(),
                    EntityId =entityId,
                    TimeZone = "timezone",
                    UserId = Guid.NewGuid(),
                    Version = 1
                },
            };

            var outgoingEvents = new List<Event>
            {
                new()
                {
                    EventType = EventType.Update,
                    Data = "data2",
                    CreatedAt = DateTime.Now,
                    CreatedAtUtc = DateTime.UtcNow,
                    Entity = "Brand",
                    Changes = "changes2",
                    Id = Guid.NewGuid(),
                    EntityId =entityId,
                    TimeZone = "timezone",
                    UserId = Guid.NewGuid(),
                    Version = 1
                },
            };

            // Act
            var result = mergeService.Merge(incomingEvents, outgoingEvents);

            // Assert
            var eventResolutions = result.EventResolutions;
            eventResolutions.Count.Should().Be(incomingEvents.Count + outgoingEvents.Count + incomingEvents.Count);
            var resolution1 = eventResolutions.Dequeue();
            var resolution2 = eventResolutions.Dequeue();
            var resolution3 = eventResolutions.Dequeue();
            resolution1.EventResolutionOperation.Should().Be(EventResolutionOperation.UndoAndRemove);
            resolution2.EventResolutionOperation.Should().Be(EventResolutionOperation.Pull);
            resolution3.EventResolutionOperation.Should().Be(EventResolutionOperation.NoOp);
        }

        [Fact(DisplayName = "Undo resolutions should be in reverse order")]
        public void UndoResolutionsShouldBeInReverseOrder()
        {
            // Arrange
            var createdAtUtc = DateTime.UtcNow;
            var outgoing1 = new Event
            {
                EventType = EventType.Insert,
                Data = "data",
                CreatedAt = DateTime.Now,
                CreatedAtUtc = createdAtUtc,
                Entity = "Brand",
                Id = Guid.NewGuid(),
                EntityId = Guid.NewGuid(),
                TimeZone = "timezone",
                UserId = Guid.NewGuid(),
                Version = 1
            };
            var outgoing2 = new Event
            {
                EventType = EventType.Update,
                Data = "data2",
                CreatedAt = DateTime.Now,
                CreatedAtUtc = createdAtUtc.AddDays(1),
                Entity = "Brand",
                Id = Guid.NewGuid(),
                EntityId = Guid.NewGuid(),
                TimeZone = "timezone",
                UserId = Guid.NewGuid(),
                Changes = "changes",
                Version = 1
            };
            var outgoing3 = new Event
            {
                EventType = EventType.Delete,
                Data = "data3",
                CreatedAt = DateTime.Now,
                CreatedAtUtc = createdAtUtc.AddDays(2),
                Entity = "Brand",
                Id = Guid.NewGuid(),
                EntityId = Guid.NewGuid(),
                TimeZone = "timezone",
                UserId = Guid.NewGuid(),
                Version = 1
            };
            var outgoingEvents = new List<Event>
            {
                outgoing1,
                outgoing2,
                outgoing3
            };

            var incomingEvents = new List<Event>
            {
                new()
                {
                    EventType = EventType.Delete,
                    Data = "data1",
                    CreatedAt = DateTime.Now,
                    CreatedAtUtc = DateTime.UtcNow,
                    Entity = "Brand",
                    Id = Guid.NewGuid(),
                    EntityId = Guid.NewGuid(),
                    TimeZone = "timezone",
                    UserId = Guid.NewGuid(),
                    Version = 1
                },
                new()
                {
                    EventType = EventType.Delete,
                    Data = "data2",
                    CreatedAt = DateTime.Now,
                    CreatedAtUtc = DateTime.UtcNow,
                    Entity = "Brand",
                    Id = Guid.NewGuid(),
                    EntityId = Guid.NewGuid(),
                    TimeZone = "timezone",
                    UserId = Guid.NewGuid(),
                    Version = 1
                },
                new()
                {
                    EventType = EventType.Delete,
                    Data = "data3",
                    CreatedAt = DateTime.Now,
                    CreatedAtUtc = DateTime.UtcNow,
                    Entity = "Brand",
                    Id = Guid.NewGuid(),
                    EntityId = Guid.NewGuid(),
                    TimeZone = "timezone",
                    UserId = Guid.NewGuid(),
                    Version = 1
                },
            };

            // Act
            var result = mergeService.Merge(incomingEvents, outgoingEvents);

            // Assert
            var eventResolutions = result.EventResolutions;
            var result1 = eventResolutions.Dequeue();
            var result2 = eventResolutions.Dequeue();
            var result3 = eventResolutions.Dequeue();
            var list = new List<EventResolution> { result1, result2, result3 };
            list.Should().OnlyContain(x => x.EventResolutionOperation == EventResolutionOperation.UndoAndRemove);
            list.Should().BeInDescendingOrder(x => x.Event.CreatedAtUtc);
        }
    }
}
