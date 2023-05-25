using Decen.Core.Laboratory.Entity.DB;
using Decen.Core.Laboratory.Entity.RequestDtos;
using Decen.Core.Laboratory.Entity.ResponseDtos;
using Decen.Core.Laboratory.Entity;
using Decen.Core.Laboratory.Service.IRepository;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Decen.Common.Extensions;

namespace Decen.Core.Laboratory.Service.RepositoryImpl
{
    public class CompanyRepository : BaseRepository<Company>, ICompanyRepository
    {
        public async Task<bool> DeleteCompanyByIdsAsync(List<int> Ids)
        {
            return await db.Deleteable<Company>(Ids.ToArray()).ExecuteCommandAsync() > 0;
        }

        public async Task<Company> GetCompanyAsync(CompanyRequestDto entity)
        {
            return await db.Queryable<Company>()
                .WhereIF(!String.IsNullOrEmpty(entity.name), d => d.name == entity.name).FirstAsync();
        }

        public async Task<List<Company>> GetCompanyListAsync(Company entity)
        {
            return await db.Queryable<Company>()
                .WhereIF(!String.IsNullOrEmpty(entity.name), d => d.name == entity.name).ToListAsync();
        }

        public async Task<PageResponse<Company>> GetCompanyPageListAsync(CompanyRequestDto entity)
        {
            RefAsync<int> totalCount = 0;
            List<Company> list = new List<Company>();
            if (entity != null)
            {
                list = db.Queryable<Company>()
                   .WhereIF(!String.IsNullOrEmpty(entity.name), d => d.name == entity.name)
                   .ToPageListAsync(entity.PageIndex, entity.PageSize, totalCount).Result;
            }
            return new PageResponse<Company>(list, entity.PageIndex, entity.PageSize, totalCount);
        }
      
        public async Task<bool> InsertCompanyAsync(Company entity)
        {
            return await db.Insertable<Company>(entity).ExecuteCommandAsync() > 0;
        }

        public async Task<bool> UpdateCompanyAsync(Company entity)
        {
            return await db.Updateable<Company>().ExecuteCommandAsync() > 0;
        }
    }
}
