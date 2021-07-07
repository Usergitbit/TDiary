using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TDiary.Common.ServiceContracts
{
    public interface ISynchronizationService
    {
        Task Synchronize();
    }
}
