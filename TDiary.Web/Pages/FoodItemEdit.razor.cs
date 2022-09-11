using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;
using System;
using TDiary.Common.Models.Entities;
using TDiary.Web.ViewModels;
using TDiary.Web.Services;

namespace TDiary.Web.Pages
{
    public partial class FoodItemEdit : IDisposable
    {
        [Inject]
        public WebMapper WebMapper { get; set; }

        protected override string AfterSubmitRoute => "foodItems";
        protected override async Task<FoodItemViewModel> Get()
        {
            var foodItem = await EntityQueryService.GetFoodItem(Id);
            var foodItemViewModel = WebMapper.Map(foodItem);

            return foodItemViewModel;
        }
        protected override async Task<FoodItemViewModel> GetInitial()
        {
            var foodItem = await EntityQueryService.GetFoodItem(Id);
            var foodItemViewModel = WebMapper.Map(foodItem);

            return foodItemViewModel;
        }
        protected override Event CreateInsertEvent()
        {
            var foodItem = WebMapper.Map(Model);
            var insertEvent = DefaultEventFactory.CreateInsertEvent(foodItem, UserId);

            return insertEvent;
        }
        protected override Event CreateUpdateEvent()
        {
            var foodItem = WebMapper.Map(Model);
            var initialFoodItem = WebMapper.Map(InitialModel);
            var updateEvent = DefaultEventFactory.CreateUpdateEvent(foodItem, initialFoodItem, Changes);

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