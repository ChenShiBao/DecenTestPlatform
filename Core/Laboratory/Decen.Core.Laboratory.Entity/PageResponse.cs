using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decen.Core.Laboratory.Entity
{
    public class PageResponse<T> where T : class, new()
    {
        public PageResponse(List<T> list, int pageIndex, int pageSize, int totalCount)
        {
            List = list;
            PageIndex = pageIndex;
            PageSize = pageSize;
            TotalCount = totalCount;
            TotalPage = (totalCount / pageSize) + 1;
        }

        public List<T> List { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPage { get; set; }
    }
}
