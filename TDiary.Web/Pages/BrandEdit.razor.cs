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
using TDiary.Common.ServiceContracts.Implementations;
using TDiary.Web.Pages.Base;
using TDiary.Web.Services;

namespace TDiary.Web.Pages
{
    public partial class BrandEdit : EditBase<Brand>, IDisposable
    {
        protected override string AfterSubmitRoute => "brands";
        protected override async Task<Brand> Get()
        {
            var brand = await EntityQueryService.GetBrand(Id);

            return brand;
        }
        protected override async Task<Brand> GetInitial()
        {
            var brand = await EntityQueryService.GetBrand(Id);

            return brand;
        }
        protected override Event CreateInsertEvent()
        {
            var insertEvent = DefaultEventFactory.CreateInsertEvent(Model, UserId);

            return insertEvent;
        }
        protected override Event CreateUpdateEvent()
        {
            var updateEvent = DefaultEventFactory.CreateUpdateEvent(Model, InitialModel, Changes);

            return updateEvent;
        }
        protected override void OnFieldChanged(object sender, FieldChangedEventArgs e)
        {
            // TODO: maybe abstract this into a service
            switch (e.FieldIdentifier.FieldName)
            {
                case nameof(Model.Name):
                    if (!Changes.ContainsKey(nameof(Model.Name)))
                    {
                        Changes.Add(nameof(Model.Name), Model.Name);
                    }
                    else
                    {
                        Changes[nameof(Model.Name)] = Model.Name;
                    }
                    break;
                default:
                    Console.WriteLine($"Change for field {e.FieldIdentifier.FieldName} not handled.");
                    break;
            }
        }
    }
}
