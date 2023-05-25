using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Decen.Core.Laboratory.Entity.DB
{
    [SugarTable("Company")]
    public class Company
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int id { get; set; }
        [SugarColumn(ColumnName = "name")]
        public string name { get; set; }
        [SugarColumn(ColumnName = "age")]
        public int age { get; set; }
        [SugarColumn(ColumnName = "address")]
        public string address { get; set; }
        [SugarColumn(ColumnName = "salary")]
        public double salary { get; set; }

        [SugarColumn(IsIgnore = true)]
        public int PageIndex { get; set; } = 1;
        [SugarColumn(IsIgnore = true)]
        public int PageSize { get; set; } = 10;
    }
}
