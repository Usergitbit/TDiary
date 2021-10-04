using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TDiary.Common.Models.Entities;
using TDiary.Common.Models.Entities.Enums;
using TDiary.Common.ServiceContracts;
using TDiary.Common.ServiceContracts.Implementations;
using Xunit;

namespace TDiary.UnitTests
{
    public class UpdateEventMergerServiceTests
    {
        private readonly IUpdateEventMergerService updateEventMergerService = new UpdateEventMergerService();

        [Fact(DisplayName = "Merging different entities should throw")]
        public void MergingDifferentEntitiesShouldThrow()
        {
            // Arrange
            var localEvent = new Event
            {
                Entity = "Brand"
            };
            var serverEvent = new Event
            {
                Entity = "FoodItem"
            };

            // Act and Assert
            Assert.Throws<InvalidOperationException>(() => updateEventMergerService.Merge(localEvent, serverEvent));
        }

        [Fact(DisplayName = "Non conflicting local changes should apply")]
        public void NonConflictingLocalChangesShouldApply()
        {
            // Arrange
            var createdAtUc = DateTime.UtcNow;
            var entityId = Guid.NewGuid();
            var localInitialBrand = new Brand
            {
                Id = entityId,
                Name = "localInitialBrand",
                CreatedAtUtc = createdAtUc
            };
            var localUpdatedBrand = new Brand
            {
                Id = entityId,
                Name = "updatedBrand",
                CreatedAtUtc = createdAtUc
            };
            var localChanges = new Dictionary<string, object>
            {
                { nameof(Brand.Name), "updatedBrand"}
            };
            var localEvent = new Event
            {
                Entity = "Brand",
                EventType = EventType.Update,
                EntityId = entityId,
                CreatedAt = createdAtUc,
                Data = JsonSerializer.Serialize(localUpdatedBrand),
                Changes = JsonSerializer.Serialize(localChanges),
                InitialData = JsonSerializer.Serialize(localInitialBrand)
            };

            var serverInitialBrand = new Brand
            {
                Id = entityId,
                Name = "serverBrand",
                CreatedAtUtc = createdAtUc.AddDays(1)
            };
            var serverUpdatedBrand = new Brand
            {
                Id = entityId,
                Name = "serverBrand",
                CreatedAtUtc = createdAtUc.AddDays(1)
            };
            var serverChanges = new Dictionary<string, object>
            {
                { nameof(Brand.ModifiedAtUtc),  DateTime.UtcNow }
            };
            var serverEvent = new Event
            {
                Entity = "Brand",
                EventType = EventType.Update,
                EntityId = entityId,
                CreatedAt = createdAtUc.AddDays(1),
                Data = JsonSerializer.Serialize(serverUpdatedBrand),
                InitialData = JsonSerializer.Serialize(serverInitialBrand),
                Changes = JsonSerializer.Serialize(serverChanges)
            };

            // Act
            var result = updateEventMergerService.Merge(serverEvent, localEvent);

            // Assert
            var resultingBrand = JsonSerializer.Deserialize<Brand>(result.Data);
            resultingBrand.Name.Should().Be("updatedBrand");
            result.InitialData.Should().Be(serverEvent.InitialData);
            result.Changes.Should().Be(localEvent.Changes);
        }

        [Fact(DisplayName = "When changes conflict latest should apply - server latest")]
        public void WhenChangesConflictLatestShouldApplyServerLatest()
        {
            // Arrange
            var createdAtUc = DateTime.UtcNow;
            var entityId = Guid.NewGuid();
            var localInitialBrand = new Brand
            {
                Id = entityId,
                Name = "localInitialBrand",
                CreatedAtUtc = createdAtUc
            };
            var localUpdatedBrand = new Brand
            {
                Id = entityId,
                Name = "updatedBrand",
                CreatedAtUtc = createdAtUc
            };
            var localChanges = new Dictionary<string, object>
            {
                { nameof(Brand.Name), "updatedBrand"}
            };
            var localEvent = new Event
            {
                Entity = "Brand",
                EventType = EventType.Update,
                EntityId = entityId,
                CreatedAt = createdAtUc,
                Data = JsonSerializer.Serialize(localUpdatedBrand),
                Changes = JsonSerializer.Serialize(localChanges),
                InitialData = JsonSerializer.Serialize(localInitialBrand)
            };

            var serverInitialBrand = new Brand
            {
                Id = entityId,
                Name = "serverBrand",
                CreatedAtUtc = createdAtUc.AddDays(1)
            };
            var serverUpdatedBrand = new Brand
            {
                Id = entityId,
                Name = "serverBrand",
                CreatedAtUtc = createdAtUc.AddDays(1)
            };
            var serverChanges = new Dictionary<string, object>
            {
                { nameof(Brand.Name),  "serverBrand" }
            };
            var serverEvent = new Event
            {
                Entity = "Brand",
                EventType = EventType.Update,
                EntityId = entityId,
                CreatedAt = createdAtUc.AddDays(1),
                Data = JsonSerializer.Serialize(serverUpdatedBrand),
                InitialData = JsonSerializer.Serialize(serverInitialBrand),
                Changes = JsonSerializer.Serialize(serverChanges)
            };

            // Act
            var result = updateEventMergerService.Merge(serverEvent, localEvent);

            // Assert
            var resultingBrand = JsonSerializer.Deserialize<Brand>(result.Data);
            resultingBrand.Name.Should().Be("serverBrand");
            result.InitialData.Should().Be(serverEvent.InitialData);
            result.Changes.Should().BeNull();
        }

        [Fact(DisplayName = "When changes conflict latest should apply - local latest")]
        public void WhenChangesConflictLatestShouldApplyLocalLatest()
        {
            // Arrange
            var createdAtUc = DateTime.UtcNow;
            var entityId = Guid.NewGuid();
            var localInitialBrand = new Brand
            {
                Id = entityId,
                Name = "localInitialBrand",
                CreatedAtUtc = createdAtUc
            };
            var localUpdatedBrand = new Brand
            {
                Id = entityId,
                Name = "updatedLocalBrand",
                CreatedAtUtc = createdAtUc
            };
            var localChanges = new Dictionary<string, object>
            {
                { nameof(Brand.Name), "updatedLocalBrand"}
            };
            var localEvent = new Event
            {
                Entity = "Brand",
                EventType = EventType.Update,
                EntityId = entityId,
                CreatedAtUtc = createdAtUc,
                Data = JsonSerializer.Serialize(localUpdatedBrand),
                Changes = JsonSerializer.Serialize(localChanges),
                InitialData = JsonSerializer.Serialize(localInitialBrand)
            };

            var serverInitialBrand = new Brand
            {
                Id = entityId,
                Name = "serverBrand",
                CreatedAtUtc = createdAtUc.AddDays(-1)
            };
            var serverUpdatedBrand = new Brand
            {
                Id = entityId,
                Name = "serverBrand",
                CreatedAtUtc = createdAtUc.AddDays(-1)
            };
            var serverChanges = new Dictionary<string, object>
            {
                { nameof(Brand.Name),  "serverBrand" }
            };
            var serverEvent = new Event
            {
                Entity = "Brand",
                EventType = EventType.Update,
                EntityId = entityId,
                CreatedAtUtc = createdAtUc.AddDays(-1),
                Data = JsonSerializer.Serialize(serverUpdatedBrand),
                InitialData = JsonSerializer.Serialize(serverInitialBrand),
                Changes = JsonSerializer.Serialize(serverChanges)
            };

            // Act
            var result = updateEventMergerService.Merge(serverEvent, localEvent);

            // Assert
            var resultingBrand = JsonSerializer.Deserialize<Brand>(result.Data);
            resultingBrand.Name.Should().Be("updatedLocalBrand");
            result.InitialData.Should().Be(serverEvent.InitialData);
            result.Changes.Should().Be(localEvent.Changes);
        }
    }
}
