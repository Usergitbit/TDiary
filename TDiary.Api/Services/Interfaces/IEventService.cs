using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TDiary.Common.Models.Entities;

namespace TDiary.Api.Services.Interfaces
{
    public interface IEventService
    {
        Task Add(Guid userId, Event eventEntity);
        Task<List<Event>> Get(Guid userId, DateTime lastEventDateUtc);
    }
}
