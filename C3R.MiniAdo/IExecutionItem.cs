using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C3R.MiniAdo
{
    public interface IExecutionItem : IDisposable
    {
        IExecutionItem Param(IDataParameter parameter);
        IExecutionItem Param(string name, object value);
        IExecutionItem Param(string name, object value, DbType dbType);
        IExecutionItem Param(string name, object value, DbType dbType, int size);
        IDataParameter GetParam(string name);
    }
}
