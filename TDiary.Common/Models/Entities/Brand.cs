using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using TDiary.Common.Models.Base;

namespace TDiary.Common.Models.Entities
{
    public class Brand : EntityBase
    {
        public string Name { get; set; }
        [JsonIgnore]
        public List<FoodItem> FoodItems { get; set; } = new List<FoodItem>();
    }
}