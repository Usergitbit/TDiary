using System;
using System.Collections.Generic;
using System.Text;

namespace TDiary.Common.Models.Base
{
    public abstract class EntityBase
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime? ModifiedtAt { get; set; }
        public DateTime? ModifiedAtUtc { get; set; }
        public string TimeZone { get; set; }
        public Guid UserId { get; set; }
    }
}
