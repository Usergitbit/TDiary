using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TDiary.Common.Models.Entities;
using TDiary.Web.IndexedDB;
using TDiary.Web.Services.Interfaces;
using TG.Blazor.IndexedDB;

namespace TDiary.Web.Services
{
    public class EntityRelationsValidatorService : IEntityRelationsValidatorService
    {
        private readonly IndexedDBManager dbManager;

        public EntityRelationsValidatorService(IndexedDBManager dbManager)
        {
            this.dbManager = dbManager;
        }

        public async Task<bool> Validate(Event eventEntity)
        {
            bool hasDependantEntities;
            switch (eventEntity.Entity)
            {
                case "Brand":
                    hasDependantEntities = await BrandHasDependantEntites(eventEntity.EntityId);
                    break;
                case "FoodItem":
                    hasDependantEntities = await FoodItemsHasDependantEntities(eventEntity.EntityId);
                    break;
                default:
                    throw new NotImplementedException($"Entity relations validation for {eventEntity.Entity} not implemented.");

            }

            return !hasDependantEntities;
        }

        private Task<bool> FoodItemsHasDependantEntities(Guid entityId)
        {
            return Task.FromResult(false);
        }

        private async Task<bool> BrandHasDependantEntites(Guid entityId)
        {
            var indexSearch = new StoreIndexQuery<string>
            {
                Storename = StoreNameConstants.FoodItems,
                IndexName = "brandId",
                QueryValue = entityId.ToString(),
            };
            var relatedentites = await dbManager.GetAllRecordsByIndex<string, FoodItem>(indexSearch) ?? new List<FoodItem>();

            return relatedentites.Count != 0;
        }
    }
}
