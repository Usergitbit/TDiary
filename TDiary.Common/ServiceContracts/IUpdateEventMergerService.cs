using System;
using System.Collections.Generic;
using System.Text;
using TDiary.Common.Models.Entities;

namespace TDiary.Common.ServiceContracts
{
    public interface IUpdateEventMergerService
    {
        Event Merge(Event serverEvent, Event localEvent);
    }
}
