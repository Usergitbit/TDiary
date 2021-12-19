using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TDiary.Common.Models.Entities;
using TDiary.Common.ServiceContracts;
using TDiary.Web.IndexedDB;
using TG.Blazor.IndexedDB;

namespace TDiary.Web.Services
{
    public class EntityQueryService : IEntityQueryService
    {
        private readonly IndexedDBManager dbManager;

        public EntityQueryService(IndexedDBManager dbManager)
        {
            this.dbManager = dbManager;
        }

        public async Task<Brand> GetBrand(Guid brandId)
        {
            var brand = await dbManager.GetRecordById<Guid, Brand>(StoreNameConstants.Brands, brandId);

            return brand;
        }

        public async Task<List<Brand>> GetBrands(Guid userId)
        {
            var indexSearch = new StoreIndexQuery<string>
            {
                Storename = StoreNameConstants.Brands,
                IndexName = "userId",
                QueryValue = userId.ToString(),
            };
            var results = await dbManager.GetAllRecordsByIndex<string, Brand>(indexSearch);

            return results.OrderBy(b => b.CreatedAtUtc).ToList();
        }
    }
}
