using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using TDiary.Common.Models.Entities;
using TDiary.Common.Models.Entities.Enums;

namespace TDiary.Web.Pages
{
    public partial class BrandEdit : IDisposable
    {
        private readonly IDictionary<string, object> Changes = new Dictionary<string, object>();
        private Brand InitialBrand;
        private Brand Brand = new();
        private EditContext EditContext;
        private bool isBusy;
        private Guid UserId;

        [Parameter]
        public Guid Id { get; set; }

        protected override async Task OnInitializedAsync()
        {
            isBusy = true;
            EditContext = new(Brand);
            UserId = await GetUserId();
            var isOnline = await NetworkStateService.IsOnline();
            if (isOnline)
            {
                await SynchronizationService.Synchronize(UserId);
            }

            if (Id != Guid.Empty)
            {
                Brand = await EntityQueryService.GetBrand(Id);
                InitialBrand = await EntityQueryService.GetBrand(Id);
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
            if (Brand.Id != Guid.Empty)
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
            Brand.Id = Guid.NewGuid();
            Brand.UserId = UserId;
            var eventEntity = new Event
            {
                Id = Guid.NewGuid(),
                UserId = UserId,
                CreatedAt = DateTime.Now,
                CreatedAtUtc = DateTime.UtcNow,
                Data = JsonSerializer.Serialize(Brand),
                Entity = "Brand",
                EntityId = Brand.Id,
                EventType = EventType.Insert,
                TimeZone = TimeZoneInfo.Local.Id,
                Version = 1,
            };

            await EventService.Add(eventEntity);
            NavigationManager.NavigateTo("brands");
        }

        private async Task Update()
        {
            var eventEntity = new Event
            {
                Id = Guid.NewGuid(),
                UserId = UserId,
                CreatedAt = DateTime.Now,
                CreatedAtUtc = DateTime.UtcNow,
                Data = JsonSerializer.Serialize(Brand),
                InitialData = JsonSerializer.Serialize(InitialBrand),
                Entity = "Brand",
                EntityId = Brand.Id,
                EventType = EventType.Update,
                TimeZone = TimeZoneInfo.Local.Id,
                Version = 1,
                Changes = JsonSerializer.Serialize(Changes)
            };

            await EventService.Add(eventEntity);
            NavigationManager.NavigateTo("brands");
        }


        private void OnFieldChanged(object sender, FieldChangedEventArgs e)
        {
            // TODO: maybe abstract this into a service
            switch (e.FieldIdentifier.FieldName)
            {
                case nameof(Brand.Name):
                    if (!Changes.ContainsKey(nameof(Brand.Name)))
                    {
                        Changes.Add(nameof(Brand.Name), Brand.Name);
                    }
                    else
                    {
                        Changes[nameof(Brand.Name)] = Brand.Name;
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
