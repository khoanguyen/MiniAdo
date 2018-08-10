using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace C3R.MiniAdo.Mapping
{
    public class AutoMapperProvider : IMapperProvider
    {
        private static Dictionary<string, object> _mappers = new Dictionary<string, object>();

        /// <summary>
        /// Gets mapper of given generic type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public virtual IMapper<T> GetMapper<T>()
        {
            var type = typeof(T);
            var key = type.FullName;

            if (_mappers.ContainsKey(key)) return (IMapper<T>)_mappers[key];

            var mapper = new AutoMapper<T>();
            _mappers[key] = mapper;

            return mapper;
        }

        /// <summary>
        /// Registers given mapper for given generic type
        /// </summary>
        /// <typeparam name="T">Type that the registered mapper for</typeparam>
        /// <param name="mapper"></param>
        public virtual void RegisterMapper<T>(IMapper<T> mapper)
        {
            _mappers[typeof(T).FullName] = mapper;
        }
    }
}
