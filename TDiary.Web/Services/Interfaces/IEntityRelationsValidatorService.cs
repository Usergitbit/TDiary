using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TDiary.Common.Models.Entities;

namespace TDiary.Web.Services.Interfaces
{
    public interface IEntityRelationsValidatorService
    {
        Task<bool> Validate(Event eventEntity);
    }
}
