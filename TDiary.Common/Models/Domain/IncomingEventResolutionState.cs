using System;
using System.Collections.Generic;
using System.Text;
using TDiary.Common.Models.Domain.Enums;
using TDiary.Common.Models.Entities;

namespace TDiary.Common.Models.Domain
{
    public class IncomingEventResolutionState
    {
        public List<AffectedEntity> AffectedEntities { get; set; } = new List<AffectedEntity>();
        public List<EventResolution> EventResolutions { get; set; } = new List<EventResolution>();
    }
}
