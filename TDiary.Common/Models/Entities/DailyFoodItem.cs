using System;
using System.Collections.Generic;
using System.Text;
using TDiary.Common.Models.Base;

namespace TDiary.Common.Models.Entities
{
    public class DailyFoodItem : EntityBase
    {
        public double Quantity { get; set; }
        public double Calories { get; set; }
        public double Carbohydrates { get; set; }
        public double Proteins { get; set; }
        public double Fats { get; set; }
        public Guid? PlateId { get; set; }
        public Plate Plate { get; set; }
        public Guid FoodItemId { get; set; }
        public FoodItem FoodItem { get; set; }
    }
}
