using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace C3R.MiniAdo.Mapping
{
    public interface IMapperProvider
    {
        IMapper<T> GetMapper<T>();
    }
}
