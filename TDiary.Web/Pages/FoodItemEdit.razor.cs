using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System;
using TDiary.Common.Models.Entities;
using TDiary.Common.Models.Entities.Enums;
using TDiary.Web.ViewModels;
using TDiary.Web.Services;

namespace TDiary.Web.Pages
{
    public partial class FoodItemEdit : IDisposable
    {
        private readonly IDictionary<string, object> Changes = new Dictionary<string, object>();
        private FoodItemViewModel initialFoodItemViewModel;
        private FoodItemViewModel foodItemViewModel = new();
        private EditContext EditContext;
        private bool isBusy;
        private Guid UserId;

        [Parameter]
        public Guid Id { get; set; }

        [Inject]
        public WebMapper WebMapper { get; set; }

        protected override async Task OnInitializedAsync()
        {
            isBusy = true;
            EditContext = new(foodItemViewModel);
            UserId = await GetUserId();
            var isOnline = await NetworkStateService.IsOnline();
            if (isOnline)
            {
                await SynchronizationService.Synchronize(UserId);
            }

            if (Id != Guid.Empty)
            {
                var foodItem = await EntityQueryService.GetFoodItem(Id);
                foodItemViewModel = WebMapper.Map(foodItem);
                initialFoodItemViewModel = WebMapper.Map(foodItem);
            }

            EditContext.OnFieldChanged += OnFieldChanged;
            isBusy = false;
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

        public Task OnValidSubmit()
        {
            if (foodItemViewModel.Id != Guid.Empty)
            {
                return Update();
            }
            else
            {
                return Add();
            }
        }

        private async Task Add()
        {
            foodItemViewModel.Id = Guid.NewGuid();
            foodItemViewModel.UserId = UserId;
            var eventEntity = new Event
            {
                Id = Guid.NewGuid(),
                UserId = UserId,
                CreatedAt = DateTime.Now,
                CreatedAtUtc = DateTime.UtcNow,
                Data = JsonSerializer.Serialize(foodItemViewModel),
                Entity = "FoodItem",
                EntityId = foodItemViewModel.Id,
                EventType = EventType.Insert,
                TimeZone = TimeZoneInfo.Local.Id,
                Version = 1,
            };

            await EventService.Add(eventEntity);
            NavigationManager.NavigateTo("foodItems");
        }

        private async Task Update()
        {
            var eventEntity = new Event
            {
                Id = Guid.NewGuid(),
                UserId = UserId,
                CreatedAt = DateTime.Now,
                CreatedAtUtc = DateTime.UtcNow,
                Data = JsonSerializer.Serialize(foodItemViewModel),
                InitialData = JsonSerializer.Serialize(initialFoodItemViewModel),
                Entity = "FoodItem",
                EntityId = foodItemViewModel.Id,
                EventType = EventType.Update,
                TimeZone = TimeZoneInfo.Local.Id,
                Version = 1,
                Changes = JsonSerializer.Serialize(Changes)
            };

            await EventService.Add(eventEntity);
            NavigationManager.NavigateTo("foodItems");
        }


        private void OnFieldChanged(object sender, FieldChangedEventArgs e)
        {
            // TODO: maybe abstract this into a service
            switch (e.FieldIdentifier.FieldName)
            {
                case nameof(foodItemViewModel.Name):
                    if (!Changes.ContainsKey(nameof(foodItemViewModel.Name)))
                    {
                        Changes.Add(nameof(foodItemViewModel.Name), foodItemViewModel.Name);
                    }
                    else
                    {
                        Changes[nameof(foodItemViewModel.Name)] = foodItemViewModel.Name;
                    }
                    break;
                default:
                    Console.WriteLine($"Change for field {e.FieldIdentifier.FieldName} not handled.");
                    break;
            }
        }

        public void Dispose()
        {
            EditContext.OnFieldChanged -= OnFieldChanged;
        }
    }
}
