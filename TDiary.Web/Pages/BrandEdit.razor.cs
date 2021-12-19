using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using TDiary.Common.Models.Entities;
using TDiary.Common.Models.Entities.Enums;

namespace TDiary.Web.Pages
{
    public partial class BrandEdit
    {
        public EditContext EditContext { get; set; }
        public Brand InitialBrand { get; set; }
        public Brand Brand { get; set; }
        public bool IsBusy { get; set; }
        private string name = "";
        public string Name
        {
            get => name;
            set
            {
                name = value;
                OnNameChange();
            }
        }
        private IDictionary<string, object> Changes = new Dictionary<string, object>();

        [Parameter]
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        protected override async Task OnInitializedAsync()
        {
            EditContext = new EditContext(Name);
            UserId = await GetUserId();
            var isOnline = await NetworkStateService.IsOnline();
            if (isOnline)
            {
                IsBusy = true;
                await SynchronizationService.Synchronize(UserId);
                IsBusy = false;
            }
            if (Id == Guid.Empty)
            {
                Brand = new Brand
                {
                    Id = Guid.NewGuid(),
                    UserId = UserId
                };
            }
            else
            {
                Brand = await EntityQueryService.GetBrand(Id);
                InitialBrand = await EntityQueryService.GetBrand(Id);
                name = Brand.Name;
                StateHasChanged();
            }

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

        public async Task Update()
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

        public void OnNameChange()
        {
            if (name != InitialBrand.Name)
            {
                if (!Changes.ContainsKey(nameof(Name)))
                {
                    Changes.Add(nameof(Name), Name);
                }
                else
                {
                    Changes[nameof(Name)] = name;
                }
            }
            Brand.Name = name;
        }
    }
}
