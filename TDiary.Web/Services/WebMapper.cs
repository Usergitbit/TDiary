using System;
using System.Xml.Linq;
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

        public FoodItem Map(FoodItemViewModel foodItemViewModel)
        {
            var foodItem = new FoodItem
            {
                BrandId = foodItemViewModel.BrandId,
                Calories = foodItemViewModel.Calories,
                Carbohydrates = foodItemViewModel.Carbohydrates,
                Fats = foodItemViewModel.Fats,
                Id = foodItemViewModel.Id,
                UserId = foodItemViewModel.UserId,
                SaturatedFats = foodItemViewModel.SaturatedFats,
                Proteins = foodItemViewModel.Proteins,
                Name = foodItemViewModel.Name,
            };


            return foodItem;
        }
    }
}
