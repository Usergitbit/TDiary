using System;

namespace TDiary.Web.ViewModels
{
    public class FoodItemViewModel
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public double Calories { get; set; }
        public double Carbohydrates { get; set; }
        public double Proteins { get; set; }
        public double Fats { get; set; }
        public double SaturatedFats { get; set; }
        public string Brand { get; set; }
        public Guid? BrandId { get; set; }
        public bool NewBrand { get; set; }
    }
}