using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace C3R.MiniAdo.Mapping
{
    public interface IMappingService
    {
        object Map(object source, Type targetType);
        void Populate(object source, object target);
    }
}
