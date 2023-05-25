using Decen.Core.Laboratory.Entity.DB;
using Decen.Core.Laboratory.Entity.RequestDtos;
using Decen.Core.Laboratory.Entity.ResponseDtos;
using Decen.Core.Laboratory.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decen.Core.Laboratory.Service.IRepository
{
    public interface ICompanyRepository
    {
        Task<List<Company>> GetCompanyListAsync(Company entity);
        Task<bool> UpdateCompanyAsync(Company entity);
        Task<bool> InsertCompanyAsync(Company entity);
        Task<bool> DeleteCompanyByIdsAsync(List<int> userIds);
        Task<PageResponse<Company>> GetCompanyPageListAsync(CompanyRequestDto entity);
        Task<Company> GetCompanyAsync(CompanyRequestDto entity);
    }
}
