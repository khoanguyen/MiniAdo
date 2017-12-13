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

        public static IMapperProvider MapperProvider { get; set; }

        static Mapper()
        {
            MapperProvider = new DefaultMapperProvider();
        }

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

        private static IMapper<T> GetMapper<T>()
        {
            if (MapperProvider == null) throw new InvalidOperationException("No MapperProvider was set");

            var mapper = MapperProvider.GetMapper<T>();            

            if (mapper == null) throw new NotSupportedException($"No mapper was registered for {typeof(T).Name}");

            return mapper;
        }        
    }
}
