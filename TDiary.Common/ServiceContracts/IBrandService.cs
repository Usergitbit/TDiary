using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TDiary.Common.Models.Entities;

namespace TDiary.Common.ServiceContracts
{
    public interface IBrandService
    {
        Task Add(Brand brand);
        Task<List<Brand>> Get(Guid userId);
    }
}
