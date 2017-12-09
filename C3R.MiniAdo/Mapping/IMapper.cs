using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace C3R.MiniAdo.Mapping
{
    public interface IMapper<T>
    {
        T Map(DataRow row);
        void PopulateRow(T entity, DataRow target);
        void Populate(DataRow row, T target);
    }
}
