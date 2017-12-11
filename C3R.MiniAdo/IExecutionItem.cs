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
        IExecutionItem Param(string name, object value, ParameterDirection direction = ParameterDirection.Input);
        IExecutionItem Param(string name, object value, DbType dbType, ParameterDirection direction = ParameterDirection.Input);
        IExecutionItem Param(string name, object value, DbType dbType, int size, ParameterDirection direction = ParameterDirection.Input);
        IDataParameter GetParam(string name);
    }
}
