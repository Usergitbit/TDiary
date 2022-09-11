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
using TDiary.Common.ServiceContracts.Implementations;
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
        AuthenticationStateProvider AuthenticationStateProvider { get; set; }
        [Inject]
        NetworkStateService NetworkStateService { get; set; }
        [Inject]
        ISynchronizationService SynchronizationService { get; set; }
        [Inject]
        NavigationManager NavigationManager { get; set; }
        [Inject]
        IDialogService DialogService { get; set; }
        [Inject]
        ISnackbar Snackbar { get; set; }
        [Inject] DefaultEventFactory DefaultEventFactory {get; set;}
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
            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
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
                NavigationManager.NavigateTo($"brands/{clickedBrand.Id}");
            }
        }

        private void OnAddBrandClick()
        {
            NavigationManager.NavigateTo($"brands/{Guid.Empty}");
        }

        private async Task OnDeleteBrandClick(Brand brand)
        {
            try
            {
                var brandFoodItems = await EntityQueryService.GetFoodItemsByBrandId(brand.Id);
                if(brandFoodItems.Count != 0)
                {
                    var usedBy = string.Join(", ", brandFoodItems.Select(bfi => bfi.Name));
                    Snackbar.Add($"Can't delete: brand in use by: {usedBy}", Severity.Error);
                    return;
                }

                var result = await DialogService.ShowMessageBox(
                    "Warning",
                    "Deleting can not be undone! Continue?",
                    yesText: "Delete!", cancelText: "Cancel");

                if (result == true)
                {
                    var deleteEvent = DefaultEventFactory.CreateDeleteEvent(brand);
                    await EventService.Add(deleteEvent);
                    await Get();
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"{ex.Message}\n{ex.StackTrace}");
            }
        }
    }
}
