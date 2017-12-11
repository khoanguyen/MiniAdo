using C3R.MiniAdo.Mapping;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C3R.MiniAdo
{
    public class Query : BaseExecutionItem, IQuery
    {
        public Query(DataContext context, string cmd = null, CommandType cmdType = CommandType.Text) : base(context)
        {
            Command = Context.Factory.CreateCommand(cmd, cmdType);            
        }

        public virtual DataTable RunQuery()
        {
            var conn = Context.GetConnection();
            var shouldDisposeConn = conn.State != ConnectionState.Open;
            try
            {
                if (shouldDisposeConn) conn.Open();
                return RunQueryInternal(conn);
            }
            finally
            {
                if (shouldDisposeConn)
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
        }

        public virtual IEnumerable<T> RunQuery<T>()
        {
            var table = RunQuery();
            for (var i = 0; i < table.Rows.Count; i++)
            {
                yield return Mapper.Map<T>(table.Rows[i]);
            }
        }

        public DataTable RunQueryReadOnly()
        {
            var conn = Context.GetConnection(AccessMode.ReadOnly);
            var shouldDisposeConn = conn.State != ConnectionState.Open;
            try
            {
                if (shouldDisposeConn) conn.Open();
                return RunQueryInternal(conn);
            }
            finally
            {
                if (shouldDisposeConn)
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
        }

        public IEnumerable<T> RunQueryReadOnly<T>()
        {
            var table = RunQueryReadOnly();
            for (var i = 0; i < table.Rows.Count; i++)
            {
                yield return Mapper.Map<T>(table.Rows[i]);
            }
        }

        public object RunScalar()
        {
            var conn = Context.GetConnection();
            var shouldDisposeConn = conn.State != ConnectionState.Open;
            try
            {
                if (shouldDisposeConn) conn.Open();
                return RunScalarInternal(conn);
            }
            finally
            {
                if (shouldDisposeConn)
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
        }

        public object RunScalarReadOnly()
        {
            var conn = Context.GetConnection(AccessMode.ReadOnly);
            var shouldDisposeConn = conn.State != ConnectionState.Open;
            try
            {
                if (shouldDisposeConn) conn.Open();
                return RunScalarInternal(conn);
            }
            finally
            {
                if (shouldDisposeConn)
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
        }

        public T RunScalar<T>()
        {
            return (T)RunScalar();
        }

        public T RunScalarReadOnly<T>()
        {
            return (T)RunScalarReadOnly();
        }

        public int Run()
        {
            var conn = Context.GetConnection();
            var shouldDisposeConn = conn.State != ConnectionState.Open;
            try
            {
                if (shouldDisposeConn) conn.Open();
                return RunInternal(conn);
            }
            finally
            {
                if (shouldDisposeConn)
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
        }

        public int RunReaOnly()
        {
            var conn = Context.GetConnection(AccessMode.ReadOnly);
            var shouldDisposeConn = conn.State != ConnectionState.Open;
            try
            {
                if (shouldDisposeConn) conn.Open();
                return RunInternal(conn);
            }
            finally
            {
                if (shouldDisposeConn)
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
        }

        public override void Dispose()
        {
            this.Command.Dispose();
        }

        protected virtual DataTable RunQueryInternal(IDbConnection conn)
        {
            var result = new DataTable();

            Command.Connection = conn;
            Command.Transaction = Context.CurrentTransaction;
            using (var reader = Command.ExecuteReader())
            {
                for (var i = 0; i < reader.FieldCount; i++)
                {
                    result.Columns.Add(new DataColumn(reader.GetName(i), reader.GetFieldType(i)));
                }

                while (reader.Read())
                {
                    var values = new object[reader.FieldCount];
                    reader.GetValues(values);
                    result.Rows.Add(values);
                }
            }

            return result;
        }

        protected virtual object RunScalarInternal(IDbConnection conn)
        {
            Command.Connection = conn;
            Command.Transaction = Context.CurrentTransaction;
            return Command.ExecuteScalar();
        }

        protected virtual int RunInternal(IDbConnection conn)
        {
            Command.Connection = conn;
            Command.Transaction = Context.CurrentTransaction;
            return Command.ExecuteNonQuery();
        }

        IQuery IQuery.Param(IDataParameter parameter)
        {
            return (IQuery)base.Param(parameter);
        }

        public IQuery Param(string name, object value)
        {
            return (IQuery)base.Param(name, value);
        }

        IQuery IQuery.Param(string name, object value, ParameterDirection direction)
        {
            return (IQuery)base.Param(name, value, direction);
        }

        public IQuery Param(string name, object value, DbType dbType)
        {
            return (IQuery)base.Param(name, value, dbType);
        }

        IQuery IQuery.Param(string name, object value, DbType dbType, ParameterDirection direction)
        {
            return (IQuery)base.Param(name, value, dbType, direction);
        }

        public IQuery Param(string name, object value, DbType dbType, int size)
        {
            return (IQuery)base.Param(name, value, dbType, size);
        }

        IQuery IQuery.Param(string name, object value, DbType dbType, int size, ParameterDirection direction)
        {
            return (IQuery)base.Param(name, value, dbType, size, direction);
        }
        
    }
}
