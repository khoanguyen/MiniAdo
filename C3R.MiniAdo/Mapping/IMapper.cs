using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace C3R.MiniAdo.Mapping
{
    /// <summary>
    /// Mapper for given genetic type T. 
    /// Provide mapping between entity of type T and System.Data.Datarow
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IMapper<T>
    {
        /// <summary>
        /// Maps datarow to object of type T
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        T Map(DataRow row);

        /// <summary>
        /// Populates values of entity to datarow
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="target"></param>
        void PopulateRow(T entity, DataRow target);

        /// <summary>
        /// Populates values of datarow to target entity
        /// </summary>
        /// <param name="row"></param>
        /// <param name="target"></param>
        void Populate(DataRow row, T target);
    }
}
