using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Collections.Generic;
using TDiary.Common.Models.Entities;
using TDiary.Common.ServiceContracts;
using TDiary.Web.Services;
using TDiary.Web.Services.Interfaces;
using System.Text.Json;
using System.Threading.Tasks;
using System;
using TDiary.Common.Models.Entities.Enums;
using System.Linq;

namespace TDiary.Web.Pages
{
    public partial class FoodItems
    {
        [Inject]
        IEventService EventService { get; set; }
        [Inject]
        IEntityQueryService EntityQueryService { get; set; }
        [Inject]
        AuthenticationStateProvider AuthenticationStateProvider { get; set; }
        [Inject]
        NetworkStateService NetworkStateService { get; set; }
        [Inject]
        ISynchronizationService SynchronizationService { get; set; }
        [Inject]
        NavigationManager NavigationManager { get; set; }
        [Inject]
        IDialogService DialogService { get; set; }
        public FoodItem FoodItem { get; set; } = new();
        public List<FoodItem> FoodItemsList { get; set; } = new List<FoodItem>();
        public bool IsBusy { get; set; }
        public bool IsLoadingFoodItems { get; set; } = true;

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
            FoodItem.UserId = userId;
            FoodItem.Id = Guid.NewGuid();
            var addBrandEvent = new Event
            {
                CreatedAt = DateTime.Now,
                CreatedAtUtc = DateTime.UtcNow,
                Data = JsonSerializer.Serialize(FoodItem),
                Entity = "FoodItem",
                EventType = EventType.Insert,
                Id = Guid.NewGuid(),
                TimeZone = TimeZoneInfo.Local.Id,
                UserId = userId,
                Version = 1,
                EntityId = FoodItem.Id
            };
            await EventService.Add(addBrandEvent);
            FoodItem = new();
            await Get();
        }

        public async Task Add1000()
        {
            //TODO: add bulk event rpc
            var brandEvents = new List<Event>();
            for (var i = 0; i < 1000; i++)
            {
                var userId = await GetUserId();
                FoodItem.UserId = userId;
                FoodItem.Id = Guid.NewGuid();
                FoodItem.Name = $"Brand {FoodItem.Id}";
                var addBrandEvent = new Event
                {
                    CreatedAt = DateTime.Now,
                    CreatedAtUtc = DateTime.UtcNow,
                    Data = JsonSerializer.Serialize(FoodItem),
                    Entity = "Brand",
                    EventType = EventType.Insert,
                    Id = Guid.NewGuid(),
                    TimeZone = TimeZoneInfo.Local.Id,
                    UserId = userId,
                    Version = 1,
                    EntityId = FoodItem.Id
                };
                brandEvents.Add(addBrandEvent);
                FoodItem = new();
            }
            await EventService.BulkAdd(brandEvents);
            await Get();
        }

        public async Task Get()
        {
            try
            {
                var userId = await GetUserId();
                var result = await EntityQueryService.GetFoodItems(userId);
                FoodItemsList = new List<FoodItem>(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}\n{ex.StackTrace}");
            }
            finally
            {
                IsLoadingFoodItems = false;
            }
        }

        private async Task<Guid> GetUserId()
        {
            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;
            var claims = user.Claims;
            var userIdClaim = claims.FirstOrDefault(c => c.Type == "id");
            var userIdClaimValue = userIdClaim.Value;
            var userId = Guid.Parse(userIdClaimValue);

            return userId;
        }

        private void RowClickEvent(TableRowClickEventArgs<FoodItem> tableRowClickEventArgs)
        {
            var clickedFoodItem = tableRowClickEventArgs.Item;
            if (clickedFoodItem != null)
            {
                NavigationManager.NavigateTo($"foodItems/{clickedFoodItem.Id}");
            }
        }

        private void OnAddBrandClick()
        {
            NavigationManager.NavigateTo($"foodItems/{Guid.Empty}");
        }

        private async Task OnDeleteFoodItemClick(FoodItem foodItem)
        {
            try
            {
                //TODO validate deletion
                var result = await DialogService.ShowMessageBox(
                    "Warning",
                    "Deleting can not be undone! Continue?",
                    yesText: "Delete!", cancelText: "Cancel");

                if (result == true)
                {
                    var deleteEvent = new Event
                    {
                        CreatedAt = DateTime.Now,
                        CreatedAtUtc = DateTime.UtcNow,
                        Entity = "FoodItem",
                        EventType = EventType.Delete,
                        Data = JsonSerializer.Serialize(foodItem),
                        EntityId = foodItem.Id,
                        Id = Guid.NewGuid(),
                        TimeZone = TimeZoneInfo.Local.Id,
                        UserId = await GetUserId(),
                        Version = 1
                    };

                    await EventService.Add(deleteEvent);
                    await Get();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}\n{ex.StackTrace}");
            }
        }
    }
}
