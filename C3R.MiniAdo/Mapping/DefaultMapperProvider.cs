using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace C3R.MiniAdo.Mapping
{
    /// <summary>
    /// Default MapperProvider Implementation
    /// </summary>
    public class DefaultMapperProvider : IMapperProvider
    {
        /// <summary>
        /// Mapper Dictionary
        /// </summary>
        private Dictionary<string, object> _mappers = new Dictionary<string, object>();

        /// <summary>
        /// Registers given mapper for given generic type
        /// </summary>
        /// <typeparam name="T">Type that the registered mapper for</typeparam>
        /// <param name="mapper"></param>
        public virtual void RegisterMapper<T>(IMapper<T> mapper)
        {
            _mappers[typeof(T).FullName] = mapper;
        }

        /// <summary>
        /// Gets mapper of given generic type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public virtual IMapper<T> GetMapper<T>()
        {
            var key = typeof(T).FullName;
            if (_mappers.ContainsKey(key)) return _mappers[key] as IMapper<T>;
            else throw new NotSupportedException($"No mapper was registered for {typeof(T).Name}");
        }
    }
}
