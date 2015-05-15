using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Helper
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class Column : Attribute
    {
        public string Name { get; set; }

        public Column(string name)
        {
            if (String.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException();
            }

            this.Name = name;
        }
    }

    public partial class PropertyHelper<T> where T : class, new()
    {
        private PropertyHelper()
        {
        }

        private static readonly Dictionary<string, Func<T, object>> GetDelegateDict;
        private static readonly Dictionary<string, Action<T, object>> SetDelegateDict;

        static PropertyHelper()
        {
            var type = typeof(T);
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            var propertyCount = properties.Count();
            GetDelegateDict = new Dictionary<string, Func<T, object>>(propertyCount);
            SetDelegateDict = new Dictionary<string, Action<T, object>>(propertyCount);

            foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                Func<T, object> getDelegate = null;
                if (property.GetGetMethod() != null)
                {
                    // p => (object)p.XXX;
                    var getParameter = Expression.Parameter(typeof (T));
                    var getBody = Expression.Convert(Expression.Property(getParameter, property.Name), typeof (object));
                    getDelegate = Expression.Lambda<Func<T, object>>(getBody, getParameter).Compile();
                    GetDelegateDict.Add(property.Name, getDelegate);
                }

                Action<T, object> setDelegate = null;
                if (property.GetSetMethod() != null)
                {
                    // (p,v) => p.XXX = (object)v;
                    var setParameter1 = Expression.Parameter(typeof (T));
                    var setParameter2 = Expression.Parameter(typeof (object));
                    var setBody = Expression.Assign(Expression.Property(setParameter1, property.Name), Expression.Convert(setParameter2, property.PropertyType));
                    setDelegate = Expression.Lambda<Action<T, object>>(setBody, setParameter1, setParameter2).Compile();
                    SetDelegateDict.Add(property.Name, setDelegate);
                }

                var columnAttribute = property.GetCustomAttribute<Column>();
                if (columnAttribute == null || columnAttribute.Name == property.Name)
                {
                    continue;
                }
                if (getDelegate != null)
                {
                    GetDelegateDict.Add(columnAttribute.Name, getDelegate);
                }
                if (setDelegate != null)
                {
                    SetDelegateDict.Add(columnAttribute.Name, setDelegate);
                }
            }
        }

        public static void SetProperty(T obj, string propertyName, object value)
        {
            if (!SetDelegateDict.ContainsKey(propertyName))
            {
                return;
            }

            SetDelegateDict[propertyName].Invoke(obj, value);
        }

        public static object GetProperty(T obj, string propertyName)
        {
            return GetDelegateDict[propertyName].Invoke(obj);
        }

        public static T FillEntity(DataRow row)
        {
            if (row == null)
            {
                return default(T);
            }

            T entity = new T();
            foreach (var column in row.Table.Columns)
            {
                var value = row[column.ToString()];

                if (value == DBNull.Value)
                {
                    continue;
                }

                SetProperty(entity, column.ToString(), row[column.ToString()]);
            }
            return entity;
        }

        public static void FillDataRow(T entity, ref DataRow dataRow)
        {
            if (entity == null)
            {
                return;
            }

            foreach (var column in dataRow.Table.Columns)
            {
                var columnName = column.ToString();
                if (GetDelegateDict.ContainsKey(columnName))
                {
                    dataRow[columnName] = GetProperty(entity, columnName) ?? DBNull.Value;
                }
            }
        }
    }
}
