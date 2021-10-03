using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TDiary.Common.Models.Entities;

namespace TDiary.Common.ServiceContracts
{
    public interface IEntityQueryService
    {
        Task<List<Brand>> GetBrands(Guid userId);
        Task<Brand> GetBrand(Guid brandId);
    }
}
