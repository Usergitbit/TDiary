using System;
using System.Collections.Generic;
using System.Text;

namespace TDiary.Common.Models.Domain
{
    public class MergeResult
    {
        public Queue<EventResolution> EventResolutions { get; set; } = new Queue<EventResolution>();
    }
}
