using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using TDiary.Common.Models.Entities;
using TDiary.Common.Models.Entities.Enums;
using TDiary.Common.ServiceContracts;
using TDiary.Web.Services;
using TDiary.Web.Services.Interfaces;

namespace TDiary.Web.Pages
{
    public partial class Brands
    {
        [Inject]
        IEventService EventService { get; set; }
        [Inject]
        IEntityQueryService EntityQueryService { get; set; }
        [Inject]
        AuthenticationStateProvider authenticationStateProvider { get; set; }
        [Inject]
        NetworkStateService NetworkStateService { get; set; }
        [Inject]
        ISynchronizationService SynchronizationService { get; set; }
        [Inject]
        NavigationManager NavigationManager { get; set; }
        public Brand Brand { get; set; } = new();
        public List<Brand> BrandsList { get; set; } = new List<Brand>();
        public bool IsBusy { get; set; }
        public bool IsLoadingBrands { get; set; } = true;

        protected override async Task OnInitializedAsync()
        {
            var isOnline = await NetworkStateService.IsOnline();
            if (isOnline)
            {
                var userId = await GetUserId();
                IsBusy = true;
                await SynchronizationService.Synchronize(userId);
                IsBusy = false;
            }
            await Get();
        }

        public async Task Add()
        {
            var userId = await GetUserId();
            Brand.UserId = userId;
            Brand.Id = Guid.NewGuid();
            var addBrandEvent = new Event
            {
                CreatedAt = DateTime.Now,
                CreatedAtUtc = DateTime.UtcNow,
                Data = JsonSerializer.Serialize(Brand),
                Entity = "Brand",
                EventType = EventType.Insert,
                Id = Guid.NewGuid(),
                TimeZone = TimeZoneInfo.Local.Id,
                UserId = userId,
                Version = 1,
                EntityId = Brand.Id
            };
            await EventService.Add(addBrandEvent);
            Brand = new();
            await Get();
        }

        public async Task Add1000()
        {
            //TODO: add bulk event rpc
            var brandEvents = new List<Event>();
            for (var i = 0; i < 1000; i++)
            {
                var userId = await GetUserId();
                Brand.UserId = userId;
                Brand.Id = Guid.NewGuid();
                Brand.Name = $"Brand {Brand.Id}";
                var addBrandEvent = new Event
                {
                    CreatedAt = DateTime.Now,
                    CreatedAtUtc = DateTime.UtcNow,
                    Data = JsonSerializer.Serialize(Brand),
                    Entity = "Brand",
                    EventType = EventType.Insert,
                    Id = Guid.NewGuid(),
                    TimeZone = TimeZoneInfo.Local.Id,
                    UserId = userId,
                    Version = 1,
                    EntityId = Brand.Id
                };
                brandEvents.Add(addBrandEvent);
                Brand = new();
            }
            await EventService.BulkAdd(brandEvents);
            await Get();
        }

        public async Task Get()
        {
            try
            {
                var userId = await GetUserId();
                var result = await EntityQueryService.GetBrands(userId);
                BrandsList = new List<Brand>(result);
            }
            catch(Exception ex)
            {
                Console.WriteLine($"{ex.Message}\n{ex.StackTrace}");
            }
            finally
            {
                IsLoadingBrands = false;
            }
        }

        private async Task<Guid> GetUserId()
        {
            var authState = await authenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;
            var claims = user.Claims;
            var userIdClaim = claims.FirstOrDefault(c => c.Type == "id");
            var userIdClaimValue = userIdClaim.Value;
            var userId = Guid.Parse(userIdClaimValue);

            return userId;
        }

        private void RowClickEvent(TableRowClickEventArgs<Brand> tableRowClickEventArgs)
        {
            var clickedBrand = tableRowClickEventArgs.Item;
            if (clickedBrand != null)
            {
                NavigationManager.NavigateTo($"brand/{clickedBrand.Id}");
            }
        }

        private void OnAddBrandClick()
        {
            NavigationManager.NavigateTo($"brand/{Guid.Empty}");
        }
    }
}
