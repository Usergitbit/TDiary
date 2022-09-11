using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using TDiary.Common.Models.Base;
using TDiary.Common.Models.Entities;
using TDiary.Common.Models.Entities.Enums;
using TDiary.Common.ServiceContracts;
using TDiary.Common.ServiceContracts.Implementations;
using TDiary.Web.Services;
using TDiary.Web.Services.Interfaces;

namespace TDiary.Web.Pages.Base
{
    public abstract class EditBase<T> : ComponentBase, IDisposable where T : new()
    {
        [Inject] protected AuthenticationStateProvider authenticationStateProvider { get; set; }
        [Inject] protected ISynchronizationService SynchronizationService { get; set; }
        [Inject] protected NetworkStateService NetworkStateService { get; set; }
        [Inject] protected IEntityQueryService EntityQueryService { get; set; }
        [Inject] protected IEventService EventService { get; set; }
        [Inject] protected NavigationManager NavigationManager { get; set; }
        [Inject] protected DefaultEventFactory DefaultEventFactory { get; set; }

        protected readonly Dictionary<string, object> Changes = new Dictionary<string, object>();
        protected T InitialEntity;
        protected T Entity = new();
        protected EditContext EditContext;
        protected bool isBusy;
        protected Guid UserId;

        [Parameter] public Guid Id { get; set; }

        protected override async Task OnInitializedAsync()
        {
            isBusy = true;
            EditContext = new(Entity);
            UserId = await GetUserId();
            var isOnline = await NetworkStateService.IsOnline();
            if (isOnline)
            {
                await SynchronizationService.Synchronize(UserId);
            }

            if (Id != Guid.Empty)
            {
                Entity = await Get();
                InitialEntity = await GetInitial();
            }

            EditContext.OnFieldChanged += OnFieldChanged;
            isBusy = false;
        }

        protected abstract string AfterSubmitRoute { get; }
        protected abstract Task<T> Get();
        protected abstract Task<T> GetInitial();
        protected abstract Event CreateInsertEvent();
        protected abstract Event CreateUpdateEvent();
        protected abstract void OnFieldChanged(object sender, FieldChangedEventArgs e);

        protected virtual async Task<Guid> GetUserId()
        {
            var authState = await authenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;
            var claims = user.Claims;
            var userIdClaim = claims.FirstOrDefault(c => c.Type == "id").Value;
            var userId = Guid.Parse(userIdClaim);

            return userId;
        }


        public virtual async Task OnValidSubmit()
        {
            Event result;
            if (Id == Guid.Empty)
            {
                result = CreateInsertEvent();
            }
            else
            {
                result = CreateUpdateEvent();
            }

            await EventService.Add(result);
            NavigationManager.NavigateTo(AfterSubmitRoute);
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                EditContext.OnFieldChanged -= OnFieldChanged;
            }
        }
    }
}
