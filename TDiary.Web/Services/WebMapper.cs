using System;
using TDiary.Common.Models.Entities;
using TDiary.Web.ViewModels;

namespace TDiary.Web.Services
{
    public class WebMapper
    {
        public FoodItemViewModel Map(FoodItem foodItem)
        {
            if(foodItem.Brand == null && foodItem.BrandId != null)
            {
                throw new ArgumentException($"FoodItem {foodItem.Name} has a brand id but no related entity.");
            }

            return new FoodItemViewModel
            {
                Brand = foodItem.Brand?.Name,
                Calories = foodItem.Calories,
                Carbohydrates = foodItem.Carbohydrates,
                Fats = foodItem.Fats,
                Name = foodItem.Name,
                Id = foodItem.Id,
                Proteins = foodItem.Proteins,
                SaturatedFats = foodItem.SaturatedFats,
                UserId = foodItem.UserId,
                BrandId= foodItem.Brand?.Id
            };
        }
    }
}
