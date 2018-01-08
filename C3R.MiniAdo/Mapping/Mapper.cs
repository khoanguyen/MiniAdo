using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace C3R.MiniAdo.Mapping
{
    /// <summary>
    /// Static class for setting MapperProvider and 
    /// provides helpper method for mapping and populating objects
    /// </summary>
    public static class Mapper
    {
        /// <summary>
        /// Gets and Sets global MapperProvider
        /// </summary>
        public static IMapperProvider MapperProvider { get; set; }

        /// <summary>
        /// Static Constructor
        /// </summary>
        static Mapper()
        {
            /// Use DefaultMapperProvider by default
            MapperProvider = new DefaultMapperProvider();
        }

        /// <summary>
        /// Maps Datarow to entity of type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="row"></param>
        /// <returns></returns>
        public static T Map<T>(DataRow row)
        {
            return GetMapper<T>().Map(row);
        }

        /// <summary>
        /// Maps datatable into collection of entities of type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <returns></returns>
        public static IEnumerable<T> Map<T>(DataTable table)
        {
            for (var i = 0; i < table.Rows.Count; i++)
            {
                var row = table.Rows[i];
                yield return Map<T>(row);
            }
        }

        /// <summary>
        /// Populates values of entity to datarow
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="target"></param>
        public static void PopulateRow<T>(T entity, DataRow target)
        {
            GetMapper<T>().PopulateRow(entity, target);
        }

        /// <summary>
        /// Populates values of datarow to target entity
        /// </summary>
        /// <param name="row"></param>
        /// <param name="target"></param>
        public static void PopulateObject<T>(DataRow row, T target)
        {
            GetMapper<T>().Populate(row, target);
        }

        /// <summary>
        /// Gets mapper of given generic type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private static IMapper<T> GetMapper<T>()
        {
            if (MapperProvider == null) throw new InvalidOperationException("No MapperProvider was set");

            var mapper = MapperProvider.GetMapper<T>();

            if (mapper == null) throw new NotSupportedException($"No mapper was registered for {typeof(T).Name}");

            return mapper;
        }
    }
}
