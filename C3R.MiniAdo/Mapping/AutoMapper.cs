using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace C3R.MiniAdo.Mapping
{
    class AutoMapper<T> : IMapper<T>
    {
        private static Dictionary<string, PropertyInfo> _propertyCache;

        public T Map(DataRow row)
        {
            var result = Activator.CreateInstance<T>();

            Populate(row, result);

            return result;
        }

        public void Populate(DataRow row, T target)
        {
            var type = typeof(T);
            var props = GetProperties(typeof(T));

            foreach (var col in row.Table.Columns.OfType<DataColumn>())
            {
                var propName = ResolveColumnName(type.Name, col.ColumnName);

                if (props.ContainsKey(propName) &&
                    props[propName].CanWrite)
                {
                    var value = row[col] is DBNull ? null : row[col];
                    props[propName].SetValue(target, value, null);
                }
            }
        }

        public void PopulateRow(T entity, DataRow target)
        {
            var type = typeof(T);
            var props = GetProperties(typeof(T));

            foreach (var col in target.Table.Columns.OfType<DataColumn>())
            {
                var propName = ResolveColumnName(type.Name, col.ColumnName);

                if (props.ContainsKey(propName) &&
                    props[propName].CanRead)
                {
                    var value = props[propName].GetValue(entity, null);
                    target[col] = value == null ? DBNull.Value : value;
                }
            }
        }

        private static Dictionary<string, PropertyInfo> GetProperties(Type type)
        {
            return _propertyCache ??
                   (_propertyCache = new Dictionary<string, PropertyInfo>(type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                                                                              .ToDictionary(pInfo => pInfo.Name, pInfo => pInfo),
                                                                          StringComparer.OrdinalIgnoreCase));
        }

        private static string ResolveColumnName(string entityName, string colName)
        {
            var builder = new StringBuilder();
            string[] tokens = colName.Split(new[] { '_' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var token in tokens)
            {
                var lower = new Regex("^[A-Z0-9]+$").IsMatch(token) ? token.ToLower() : token;
                builder.Append(char.ToUpper(lower[0]));
                if (lower.Length > 1) builder.Append(lower.ToCharArray(), 1, lower.Length - 1);
            }

            var result = builder.ToString();
            return result.Equals(entityName, StringComparison.OrdinalIgnoreCase) ? result + "_" : result;
        }

    }
}
