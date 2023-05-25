using Decen.Core.Laboratory.Entity.DB;
using Decen.Core.Laboratory.Entity.RequestDtos;
using Decen.Core.Laboratory.Entity.ResponseDtos;
using Decen.Core.Laboratory.Entity;
using Decen.Core.Laboratory.Service.IRepository;
using Decen.Core.Laboratory.Service.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Decen.Common.Extensions;

namespace Decen.Core.Laboratory.Service.Impl
{  
    public class CompanyServiceImpl : ICompanyService
    {
        private readonly ICompanyRepository CompanyRepository;
        public CompanyServiceImpl(ICompanyRepository companyRepository)
        {
            CompanyRepository = companyRepository;
        }

        public async Task<bool> DeleteCompanyAsync(List<int> Ids)
        {
            return await CompanyRepository.DeleteCompanyByIdsAsync(Ids);
        }

        public async Task<Company> GetCompanyAsync(CompanyRequestDto entity)
        {
            return await CompanyRepository.GetCompanyAsync(entity);
        }

        public async Task<List<Company>> GetCompanyListAsync(Company entity)
        {
            return await CompanyRepository.GetCompanyListAsync(entity);
        }

        public async Task<PageResponse<Company>> GetCompanyPageListAsync(CompanyRequestDto entity)
        {
            return await CompanyRepository.GetCompanyPageListAsync(entity);
        }

        public List<CompanyResponeDto> ToGetCompanyPageList(CompanyRequestDto entity)
        {
            var list = GetCompanyPageListAsync(entity);
            List<CompanyResponeDto> resultDto = new List<CompanyResponeDto>();
            foreach (var item in list.Result.List)
            {
                CompanyResponeDto Dto = item.Adapt<CompanyResponeDto>();
                resultDto.Add(Dto);
            }
            return resultDto;
        }


        public async Task<bool> InsertCompanyAsync(Company entity)
        {
            return await CompanyRepository.InsertCompanyAsync(entity);
        }

        public Task<bool> UpdateCompanyAsync(Company entity)
        {
            throw new NotImplementedException();
        }
    }
}
