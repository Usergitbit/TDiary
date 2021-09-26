using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TDiary.Common.Models.Entities;
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
            bool result;
            switch (eventEntity.Entity)
            {
                case "Brand":
                    result =  await HasRelatingEntites(eventEntity.EntityId);
                    break;
                default:
                    throw new NotImplementedException($"Entity relations validation for {eventEntity.Entity} not implemented.");

            }

            return result;
        }

        private Task<bool> HasRelatingEntites(Guid entityId)
        {
            // TODO: get food items that have brand id == entity id
            return Task.FromResult(false);
        }
    }
}
