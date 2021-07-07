using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TDiary.Common.ServiceContracts;

namespace TDiary.Web.Services
{
    public class SynchronizationService : ISynchronizationService
    {
        private readonly IEventService eventService;

        public SynchronizationService(IEventService eventService)
        {
            this.eventService = eventService;
        }
        public async Task Synchronize()
        {

        }
    }
}
