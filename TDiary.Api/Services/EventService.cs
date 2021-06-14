﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        public async Task Add(Guid userId, Event eventEntity)
        {
            eventEntity.UserId = userId;
            eventEntity.CreatedBy = userId;
            eventEntity.InsertedAt = DateTime.UtcNow;
            await tdiaryDatabaseContext.AddAsync(eventEntity);
            await tdiaryDatabaseContext.SaveChangesAsync();
        }
    }
}
