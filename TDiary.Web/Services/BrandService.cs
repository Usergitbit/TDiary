using Microsoft.AspNetCore.Components.Authorization;
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
    public class BrandService : IBrandService
    {
        private readonly IndexedDBManager dbManager;

        public BrandService(IndexedDBManager dbManager)
        {
            this.dbManager = dbManager;
        }
        public async Task Add(Brand brand)
        {
            await dbManager.AddRecord(new StoreRecord<Brand> { Storename = StoreNameConstants.Brands, Data = brand });
        }

        public async Task Update(Brand brand)
        {
            await dbManager.UpdateRecord(new StoreRecord<Brand> { Storename = StoreNameConstants.Brands, Data = brand });
        }

        public async Task Delete(Guid brandId)
        {
            await dbManager.DeleteRecord(StoreNameConstants.Brands, brandId);
        }

        public async Task<List<Brand>> Get(Guid userId)
        {
            var indexSearch = new StoreIndexQuery<string>
            {
                Storename = StoreNameConstants.Brands,
                IndexName = "userId",
                QueryValue = userId.ToString(),
            };
            var results = await dbManager.GetAllRecordsByIndex<string, Brand>(indexSearch);

            return results.ToList();
        }
    }
}
