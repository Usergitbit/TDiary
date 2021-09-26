using System;
using System.Collections.Generic;
using System.Text;
using TDiary.Common.Models.Domain.Enums;
using TDiary.Common.Models.Entities;

namespace TDiary.Common.Models.Domain
{
    public class EventResolution
    {
        /// <summary>
        /// Used for merging.
        /// </summary>
        public Event ServerEvent { get; set; }
        public Event Event { get; set; }
        public EventResolutionOperation EventResolutionOperation { get; set; }
        public string Reason { get; set; }
    }
}
