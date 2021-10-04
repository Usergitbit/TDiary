using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TDiary.Api.Services.Interfaces;
using TDiary.Common.Exceptions;
using TDiary.Common.Models.Entities;
using TDiary.Common.ServiceContracts;
using TDiary.Database;

namespace TDiary.Api.Services
{
    public class EventService : IEventService
    {
        private readonly TDiaryDatabaseContext tdiaryDatabaseContext;

        public EventService(TDiaryDatabaseContext tdiaryDatabaseContext)
        {
            this.tdiaryDatabaseContext = tdiaryDatabaseContext;
        }
        public async Task Add(Event eventEntity)
        {
            var existingEvent = await tdiaryDatabaseContext.Events
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == eventEntity.Id);

            if (existingEvent != null)
                throw new DuplicateIdException(eventEntity.Id, nameof(Event));

            tdiaryDatabaseContext.Add(eventEntity);
            await tdiaryDatabaseContext.SaveChangesAsync();
        }

        public async Task BulkAdd(IReadOnlyList<Event> eventEntities)
        {
            // TODO: maybe optimize, ef sucks at bulk operations
            tdiaryDatabaseContext.AddRange(eventEntities);
            await tdiaryDatabaseContext.SaveChangesAsync();
        }

        public async Task<List<Event>> Get(Guid userId, DateTime lastEventDateUtc)
        {
            var events = await tdiaryDatabaseContext.Events
                .Where(e => e.CreatedAtUtc > lastEventDateUtc && e.UserId == userId)
                .OrderBy(e => e.CreatedAtUtc)
                .ToListAsync();

            return events;
        }
    }
}
