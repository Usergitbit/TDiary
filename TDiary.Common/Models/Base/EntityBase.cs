using System;
using System.Collections.Generic;
using System.Text;

namespace TDiary.Common.Models.Base
{
    public class EntityBase
    {
        public Guid Id { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime InsertedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime LocallyCreatedAt { get; set; }
        public DateTime LocallyCreatedAtUtc { get; set; }
        public DateTime? LocallyModifiedAt { get; set; }
        public DateTime? LocallyModifiedAtUtc { get; set; }
        public TimeZoneInfo TimeZone { get; set; }
        public Guid UserId { get; set; }
    }
}
