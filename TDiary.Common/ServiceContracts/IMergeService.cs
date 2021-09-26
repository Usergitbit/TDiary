using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TDiary.Common.Models.Domain;
using TDiary.Common.Models.Entities;

namespace TDiary.Common.ServiceContracts
{
    public interface IMergeService
    {
        MergeResult Merge(IEnumerable<Event> incomingEvents, IEnumerable<Event> outgoingEvents);
    }
}
