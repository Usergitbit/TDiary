using System;
using System.Collections.Generic;
using System.Text;
using TDiary.Common.Models.Base;

namespace TDiary.Common.Models.Entities
{
    public class Event : EntityBase
    {
        public string Name { get; set; }
        public int Version { get; set; }
        public string Data { get; set; }
    }
}
