using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TDiary.Common.Models.Entities;

namespace TDiary.Web.Services.Interfaces
{
    public interface IEventService
    {
        Task Add(Event eventEntity);
        Task BulkAdd(IEnumerable<Event> eventEntities);
    }
}
