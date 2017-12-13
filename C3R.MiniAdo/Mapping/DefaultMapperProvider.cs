using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace C3R.MiniAdo.Mapping
{
    public class DefaultMapperProvider : IMapperProvider
    {
        private static Dictionary<string, object> _mappers = new Dictionary<string, object>();

        public static void RegisterMapper<T>(IMapper<T> mapper)
        {
            _mappers[typeof(T).FullName] = mapper;
        }

        public virtual IMapper<T> GetMapper<T>()
        {
            var key = typeof(T).FullName;
            if (_mappers.ContainsKey(key)) return _mappers[key] as IMapper<T>;
            else throw new NotSupportedException($"No mapper was registered for {typeof(T).Name}");
        }
    }
}
