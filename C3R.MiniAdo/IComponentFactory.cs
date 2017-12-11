using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace C3R.MiniAdo
{
    public interface IComponentFactory
    {        
        IDbCommand CreateCommand(string cmdText, CommandType cmdType);
        IDataParameter CreateParameter(string name, object value, ParameterDirection direction = ParameterDirection.Input);
        IDataParameter CreateParameter(string name, object value, DbType dbType, ParameterDirection direction = ParameterDirection.Input);
        IDataParameter CreateParameter(string name, object value, DbType dbType, int size, ParameterDirection direction = ParameterDirection.Input);
        IDbConnection CreateConnection(string connectionString);
        IQuery CreateQuery(DataContext context, string query, CommandType cmdType);
    }
}
