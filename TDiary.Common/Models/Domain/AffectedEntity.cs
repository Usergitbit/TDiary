using System;
using System.Collections.Generic;
using System.Text;
using TDiary.Common.Models.Domain.Enums;
using TDiary.Common.Models.Entities;

namespace TDiary.Common.Models.Domain
{
    public class AffectedEntity
    {
        public Guid AffectedEntityId { get; set; }
        public EntityState EntityState { get; set; }
        public Event LastAffectingEvent { get; set; }
    }
}
