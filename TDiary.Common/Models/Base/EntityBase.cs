using System;
using System.Collections.Generic;
using System.Text;

namespace TDiary.Common.Models.Base
{
    public class EntityBase
    {
        public Guid Id { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public TimeZoneInfo TimeZone { get; set; }
        public Guid UserId { get; set; }
    }
}
