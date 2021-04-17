using System;
using System.Collections.Generic;
using System.Text;
using TDiary.Common.Models.Base;
using TDiary.Common.Models.Enums;

namespace TDiary.Common.Models.Entities
{
    public class Profile : EntityBase
    {
        public double Height { get; set; }
        public double Weight { get; set; }
        public DateTime BirthDate { get; set; }
        public Sex Sex{ get; set; }
        public DietFormula DietFormula { get; set; }
        public double ActivityLevel { get; set; }
        public double WaistCircumference { get; set; }
        public double NeckCircumference { get; set; }
        public double BodyFatPercentage { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public ProfileType ProfileType { get; set; }
        public int MaximumCalories { get; set; }
        public int TargetCalories { get; set; }
    }
}
