using SqlSugar;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decen.Core.Basic.Service.RepositoryImpl
{
    public class BaseRepository<T>
    {
        private static SQLiteConnectionStringBuilder stringBuilder = new SQLiteConnectionStringBuilder { DataSource = ConfigurationManager.ConnectionStrings["SQLiteConnectionString"].ConnectionString.ToString() };
        //private static SQLiteConnectionStringBuilder stringBuilder = new SQLiteConnectionStringBuilder { DataSource = @"D:\SoftWare\sqlite\demo.db" };
        protected SqlSugarClient db = new SqlSugarClient(new ConnectionConfig()
        {
            ConnectionString = stringBuilder.ToString(),
            DbType = DbType.Sqlite,
            IsAutoCloseConnection = true,//自动释放
            InitKeyType = InitKeyType.Attribute
        });
    }
}
