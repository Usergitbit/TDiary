using System;
using System.Collections.Generic;
using System.Text;
using TDiary.Common.Models.Base;
using TDiary.Common.Models.Entities.Enums;

namespace TDiary.Common.Models.Entities
{
    public class DietProfile : EntityBase
    {
        public double Height { get; set; }
        public double Weight { get; set; }
        public DateTime BirthDate { get; set; }
        public Sex Sex{ get; set; }
        public DietFormula DietFormula { get; set; }
        public double ActivityLevel { get; set; } = 1.2;
        public double WaistCircumference { get; set; }
        public double NeckCircumference { get; set; }
        public double BodyFatPercentage { get; set; }
        public bool BodyFatPercentageDefinedByUser { get; set; }
        public string Name { get; set; }
        public DateTime StartDateUtc { get; set; }
        public DateTime EndDateUtc { get; set; }
        public ProfileType ProfileType { get; set; }
        public int TargetCalories { get; set; }
        public bool TargetDefinedByUser { get; set; }
    }
}
