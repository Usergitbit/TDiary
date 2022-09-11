using System;
using System.Collections.Generic;
using System.Text;
using TDiary.Common.Models.Base;
using TDiary.Common.Models.Entities;
using TDiary.Common.Models.Entities.Enums;

namespace TDiary.Common.ServiceContracts
{
    public interface IEventFactory
    {
        Event CreateInsertEvent<T>(T entity, Guid userId) where T : EntityBase;
        Event CreateUpdateEvent<T>(T entity, T initialEntity, IReadOnlyDictionary<string, object> changes) where T : EntityBase;
        Event CreateDeleteEvent<T>(T entity) where T : EntityBase;
    }
}