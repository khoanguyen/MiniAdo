using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace C3R.MiniAdo.Mapping
{
    public static class Mapper
    {
        private static Dictionary<string, object> _mapper = new Dictionary<string, object>();

        
        public static T Map<T>(DataRow row)
        {
            return GetMapper<T>().Map(row);                
        }
        
        public static IEnumerable<T> Map<T>(DataTable table)
        {
            for(var i = 0; i< table.Rows.Count; i++)
            {
                var row = table.Rows[i];
                yield return Map<T>(row);
            }
        }

        public static void PopulateRow<T>(T obj, DataRow target)
        {
            GetMapper<T>().PopulateRow(obj, target);
        }
        

        public static void PopulateObject<T>(DataRow row, T target)
        {
            GetMapper<T>().Populate(row, target);
        }

        public static void RegisterMapper<T>(IMapper<T> mapper)
        {
            _mapper[typeof(T).FullName] = mapper;
        }

        private static IMapper<T> GetMapper<T>()
        {
            var key = typeof(T).FullName;
            if (_mapper.ContainsKey(key))
            {
                return (IMapper<T>)_mapper[key];
            }
            else
            {
                throw new NotSupportedException($"No mapper was registered for {typeof(T).Name}");
            }
        }        
    }
}
