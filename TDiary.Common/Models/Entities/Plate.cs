using System;
using System.Collections.Generic;
using System.Text;
using TDiary.Common.Models.Base;

namespace TDiary.Common.Models.Entities
{
    public class Plate : EntityBase
    {
        public string Name { get; set; }
        public double Weight { get; set; }
    }
}
