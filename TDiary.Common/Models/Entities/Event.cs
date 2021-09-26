using System;
using System.Collections.Generic;
using System.Text;
using TDiary.Common.Models.Base;
using TDiary.Common.Models.Entities.Enums;

namespace TDiary.Common.Models.Entities
{
    public class Event : EntityBase
    {
        public string Entity { get; set; }
        public EventType EventType { get; set; }
        public int Version { get; set; }
        public string Changes { get; set; }
        public string Data { get; set; }
        public Guid EntityId { get; set; }
    }
}
