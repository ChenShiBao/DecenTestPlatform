using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace Decen.Common.Helpers
{
    public static class CovertToListOrDataTableHelper
    {
        #region DataTable 1
        /// <summary>
        /// 将datatable转换为泛型集合
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="inputDataTable"></param>
        /// <returns></returns>
        public static List<TEntity> ToList<TEntity>(this DataTable inputDataTable) where TEntity : class, new()
        {
            if (inputDataTable == null)
            {
                throw new ArgumentNullException("input datatable is null");
            }
            Type type = typeof(TEntity);
            PropertyInfo[] propertyInfos = type.GetProperties();
            List<TEntity> lstEntitys = new List<TEntity>();
            foreach (DataRow row in inputDataTable.Rows)
            {
                object obj = Activator.CreateInstance(type);
                foreach (PropertyInfo pro in propertyInfos)
                {
                    foreach (DataColumn col in inputDataTable.Columns)
                    {
                        //如果直接查询的数据库，数据库是不区别大小写的，所以转换为小写忽略大小写的问题
                        if (col.ColumnName.ToLower().Equals(pro.Name.ToLower()))
                        {
                            //属性是否是可写的，如果是只读的属性跳过。
                            if (pro.CanWrite)
                            {
                                //判断类型，基本类型，如果是其他的类属性
                                if (pro.PropertyType == typeof(System.Int32))
                                {
                                    pro.SetValue(obj, Convert.ToInt32(row[pro.Name.ToLower()]));
                                }
                                else if (pro.PropertyType == typeof(System.String))
                                {
                                    pro.SetValue(obj, row[pro.Name.ToLower()].ToString());
                                }
                                else if (pro.PropertyType == typeof(System.Boolean))
                                {
                                    pro.SetValue(obj, Convert.ToBoolean(row[pro.Name.ToLower()]));
                                }
                                else if (pro.PropertyType == typeof(System.DateTime))
                                {
                                    pro.SetValue(obj, Convert.ToDateTime(row[pro.Name.ToLower()]));
                                }
                                else if (pro.PropertyType == typeof(System.Int64))
                                {
                                    pro.SetValue(obj, Convert.ToInt64(row[pro.Name.ToLower()]));
                                }
                                else
                                {
                                    pro.SetValue(obj, row[pro.Name.ToLower()]);
                                }

                            }
                        }
                    }
                }
                TEntity tEntity = obj as TEntity;
                lstEntitys.Add(tEntity);
            }
            return lstEntitys;
        }

        /// <summary>
        /// 将list转换为datatable
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="inputList"></param>
        /// <returns></returns>
        public static DataTable ToDataTable<TEntity>(this List<TEntity> inputList) where TEntity : class, new()
        {
            if (inputList == null)
            {
                throw new ArgumentNullException("inputList");
            }
            DataTable dt = null;
            Type type = typeof(TEntity);
            if (inputList.Count == 0)
            {
                dt = new DataTable(type.Name);
                return dt;
            }
            else { dt = new DataTable(); }
            PropertyInfo[] propertyInfos = type.GetProperties();
            foreach (var item in propertyInfos)
            {
                dt.Columns.Add(new DataColumn() { ColumnName = item.Name, DataType = item.PropertyType });
            }
            foreach (var item in inputList)
            {
                DataRow row = dt.NewRow();
                foreach (var pro in propertyInfos)
                {
                    row[pro.Name] = pro.GetValue(item);
                }
                dt.Rows.Add(row);
            }
            return dt;
        }
        #endregion

        #region DataTable 2

        /// <summary>
        /// DataTable转成List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static List<T> ToDataList<T>(this DataTable dt)
        {
            var list = new List<T>();
            var plist = new List<PropertyInfo>(typeof(T).GetProperties());
            foreach (DataRow item in dt.Rows)
            {
                T s = Activator.CreateInstance<T>();
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    PropertyInfo info = plist.Find(p => p.Name == dt.Columns[i].ColumnName);
                    if (info != null)
                    {
                        try
                        {
                            if (!Convert.IsDBNull(item[i]))
                            {
                                object v = null;
                                if (info.PropertyType.ToString().Contains("System.Nullable"))
                                {
                                    v = Convert.ChangeType(item[i], Nullable.GetUnderlyingType(info.PropertyType));
                                }
                                else
                                {
                                    v = Convert.ChangeType(item[i], info.PropertyType);
                                }
                                info.SetValue(s, v, null);
                            }
                        }
                        catch (Exception ex)
                        {
                            throw new Exception("字段[" + info.Name + "]转换出错," + ex.Message);
                        }
                    }
                }
                list.Add(s);
            }
            return list;
        }
        #endregion
    }
}
