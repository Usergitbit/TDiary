using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using TDiary.Common.Models.Base;

namespace TDiary.Common.Models.Entities
{
    public class FoodItem : EntityBase 
    {
        public string Name { get; set; }
        public double Calories { get; set; }
        public double Carbohydrates { get; set; }
        public double Proteins { get; set; }
        public double Fats { get; set; }
        public double SaturatedFats { get; set; }
        public Guid? BrandId { get; set; }
        [JsonIgnore]
        public Brand Brand { get; set; }
        [JsonIgnore]
        public List<DailyFoodItem> DailyFoodItems { get; set; } = new List<DailyFoodItem>();
    }
}
