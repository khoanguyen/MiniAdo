using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C3R.MiniAdo
{
    public interface IQuery : IExecutionItem
    {
        new IQuery Param(IDataParameter parameter);
        new IQuery Param(string name, object value, ParameterDirection direction = ParameterDirection.Input);        
        new IQuery Param(string name, object value, DbType dbType, ParameterDirection direction = ParameterDirection.Input);
        new IQuery Param(string name, object value, DbType dbType, int size, ParameterDirection direction = ParameterDirection.Input);        

        DataTable RunQuery();
        DataTable RunQueryReadOnly();
        IEnumerable<T> RunQuery<T>();
        IEnumerable<T> RunQueryReadOnly<T>();

        object RunScalar();
        object RunScalarReadOnly();
        T RunScalar<T>();
        T RunScalarReadOnly<T>();

        int Run();
        int RunReaOnly();
    }

}
