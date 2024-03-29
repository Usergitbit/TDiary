﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TDiary.Common.Models.Entities;

namespace TDiary.Common.ServiceContracts
{
    public interface IEventPlayerService
    {
        Task PlayEvent(Event eventEntity);
        Task UndoEvent(Event eventEntity);
    }
}
