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
        private readonly AuthenticationStateProvider authenticationStateProvider;

        public BrandService(IndexedDBManager dbManager, AuthenticationStateProvider authenticationStateProvider)
        {
            this.dbManager = dbManager;
            this.authenticationStateProvider = authenticationStateProvider;
        }
        public async Task Add(Brand brand)
        {
            brand.Id = Guid.NewGuid();
            brand.UserId = await GetUserId();

            await dbManager.AddRecord(new StoreRecord<Brand> { Storename = StoreNameConstants.Brands, Data = brand });
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

        private async Task<Guid> GetUserId()
        {
            var authState = await authenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;
            var claims = user.Claims;
            var userIdClaim = claims.FirstOrDefault(c => c.Type == "id").Value;
            var userId = Guid.Parse(userIdClaim);

            return userId;
        }
    }
}
