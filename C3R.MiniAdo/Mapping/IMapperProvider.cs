using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace C3R.MiniAdo.Mapping
{
    /// <summary>
    /// MapperProvider provides mappers for entity types. 
    /// </summary>
    public interface IMapperProvider
    {
        /// <summary>
        /// Gets mapper of given generic type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IMapper<T> GetMapper<T>();
    }
}
