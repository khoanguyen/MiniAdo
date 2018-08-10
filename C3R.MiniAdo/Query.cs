using C3R.MiniAdo.Mapping;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C3R.MiniAdo
{
    /// <summary>
    /// Query object provides preparation and executes for data query to database server
    /// </summary>
    public abstract class Query : IQuery, IDisposable
    {
        /// <summary>
        /// Internal DbCommand object which will be executed against database server
        /// </summary>
        protected virtual IDbCommand Command { get; set; }

        /// <summary>
        /// DataContext associated with current Query object
        /// </summary>
        public virtual DataContext Context { get; }

        /// <summary>
        /// Command type of the query
        /// </summary>
        public virtual CommandType CommandType
        {
            get
            {
                return Command.CommandType;
            }
        }

        /// <summary>
        /// Query statement of current query
        /// </summary>
        public virtual string QueryText
        {
            get
            {
                return Command.CommandText;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context">Associated DataContext</param>
        /// <param name="cmd">Query statement</param>
        /// <param name="cmdType">Type of query call</param>
        public Query(DataContext context, string cmd = null, CommandType cmdType = CommandType.Text)
        {
            Context = context;
            Command = Context.Factory.CreateCommand(cmd ?? "", cmdType);
        }

        /// <summary>
        /// Execute query against Database server and retrieve back resultsets. 
        /// Each resultset will result into 1 DataTable.
        /// </summary>
        /// <returns>Collection of DataTable objects</returns>
        public virtual IEnumerable<DataTable> RunQuery()
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

        /// <summary>
        /// Execute query against Database server, retrieve back resultsets and map result into collection of given generic type. 
        /// Each resultset will result into 1 DataTable.
        /// </summary>
        /// <typeparam name="T">Type of result objects</typeparam>
        /// <returns>Collection of mapped objects</returns>
        public virtual IEnumerable<T> RunQuery<T>()
        {
            var table = RunQuery().FirstOrDefault();

            if (table == null) throw new InvalidOperationException("Query does not return any result");

            return Mapper.Map<T>(table);
        }

        /// <summary>
        /// Execute Read-only query against Database server and retrieve back resultsets. 
        /// Each resultset will result into 1 DataTable.
        /// </summary>
        /// <returns>Collection of DataTable objects</returns>
        public virtual IEnumerable<DataTable> RunQueryReadOnly()
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

        /// <summary>
        /// Execute Read-only query against Database server, retrieve back resultsets and map result into collection of given generic type. 
        /// Each resultset will result into 1 DataTable.
        /// </summary>
        /// <typeparam name="T">Type of result objects</typeparam>
        /// <returns>Collection of mapped objects</returns>
        public virtual IEnumerable<T> RunQueryReadOnly<T>()
        {
            var table = RunQueryReadOnly().FirstOrDefault();

            if (table == null) throw new InvalidOperationException("Query does not return any result");

            return Mapper.Map<T>(table);
        }

        /// <summary>
        /// Execute scalar query against database server
        /// </summary>
        /// <returns>Scalar result</returns>
        public virtual object RunScalar()
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

        /// <summary>
        /// Execute Read-only scalar query against database server
        /// </summary>
        /// <returns>Scalar result</returns>
        public virtual object RunScalarReadOnly()
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

        /// <summary>
        /// Execute scalar query against database server and convert result to given generic type
        /// </summary>
        /// <typeparam name="T">Type of result</typeparam>
        /// <returns>Scalar result of given generic type</returns>
        public virtual T RunScalar<T>()
        {
            return (T)RunScalar();
        }

        /// <summary>
        /// Execute Read-only scalar query against database server and convert result to given generic type
        /// </summary>
        /// <typeparam name="T">Type of result</typeparam>
        /// <returns>Scalar result of given generic type</returns>
        public virtual T RunScalarReadOnly<T>()
        {
            return (T)RunScalarReadOnly();
        }

        /// <summary>
        /// Execute non-query query against database server
        /// </summary>
        /// <returns>Integer value returned by server</returns>
        public virtual int Run()
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

        /// <summary>
        /// Execute Read-only non-query query against database server
        /// </summary>
        /// <returns>Integer value returned by server</returns>
        public virtual int RunReadOnly()
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


        /// <summary>
        /// Get DataParameter with given name
        /// </summary>
        /// <param name="name">Parameter's name</param>
        /// <returns>DataParameter object associated with current Query</returns>
        /// <exception cref="System.EntryPointNotFoundException">
        /// DataParameter with given name does not exist in the current Query object
        /// </exception>
        public virtual IDataParameter GetParam(string name)
        {
            if (!Command.Parameters.Contains(name))
                throw new EntryPointNotFoundException($"Parameter {name} not found");

            return Command.Parameters[name] as IDataParameter;
        }

        /// <summary>
        /// Add given DataParameter to query object
        /// </summary>
        /// <param name="parameter">DataParameter object</param> 
        public virtual IQuery Param(IDataParameter parameter)
        {
            Command.Parameters.Add(parameter);
            return this;
        }

        /// <summary>
        /// Add a DataParemeter to query object
        /// </summary>
        /// <param name="name">Parameter's name</param>
        /// <param name="value">Parameter's value</param>
        /// <param name="direction">Parameter's direction</param>
        /// <returns></returns>
        public virtual IQuery Param(string name, object value, ParameterDirection direction = ParameterDirection.Input)
        {
            var parameter = Context.Factory.CreateParameter(name, value, direction);
            Command.Parameters.Add(parameter);
            return this;
        }

        /// <summary>
        /// Add a DataParemeter to query object
        /// </summary>
        /// <param name="name">Parameter's name</param>
        /// <param name="value">Parameter's value</param>
        /// <param name="dbType">Parameter's DbType</param>
        /// <param name="direction">Parameter's direction</param>
        /// <returns></returns>
        public virtual IQuery Param(string name, object value, DbType dbType, ParameterDirection direction = ParameterDirection.Input)
        {
            var parameter = Context.Factory.CreateParameter(name, value, dbType, direction);
            Command.Parameters.Add(parameter);
            return this;
        }

        /// <summary>
        /// Add a DataParemeter to query object
        /// </summary>
        /// <param name="name">Parameter's name</param>
        /// <param name="value">Parameter's value</param>
        /// <param name="dbType">Parameter's DbType</param>
        /// <param name="size">Parameter's DbType size</param>
        /// <param name="direction">Parameter's direction</param>
        /// <returns></returns>
        public virtual IQuery Param(string name, object value, DbType dbType, int size, ParameterDirection direction = ParameterDirection.Input)
        {
            var parameter = Context.Factory.CreateParameter(name, value, dbType, size, direction);
            return this.Param(parameter);
        }

        /// <summary>
        /// Append given query into the end of current query
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public IQuery AppendQuery(string query)
        {
            lock (this.Command)
            {
                var currentQuery = Command.CommandText;

                Command.CommandText = string.Join(";", new[] {
                    currentQuery.TrimEnd(';'),
                    query.TrimStart(';')
                });
            }

            return this;
        }

        /// <summary>
        /// Get all Parameter added into query
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<IDataParameter> GetParams()
        {
            return Command.Parameters.Cast<IDataParameter>();
        }

        /// <summary>
        /// Merge with given query
        /// </summary>
        /// <param name="query">query to merge with</param>
        /// <returns>Merged query</returns>
        public abstract IQuery Merge(IQuery query);

        /// <summary>
        /// Dispose current query
        /// </summary>
        public virtual void Dispose()
        {
            this.Command.Dispose();
        }

        /// <summary>
        /// Run query and return a collection of DataTable
        /// </summary>
        /// <param name="conn"></param>
        /// <returns></returns>
        protected virtual IEnumerable<DataTable> RunQueryInternal(IDbConnection conn)
        {
            Command.Connection = conn;
            Command.Transaction = Context.CurrentTransaction;
            var tables = new List<DataTable>();
            using (var reader = Command.ExecuteReader())
            {
                do
                {
                    var table = new DataTable();
                    for (var i = 0; i < reader.FieldCount; i++)
                    {
                        var colName = reader.GetName(i);                        
                        var n = 1;
                        while (table.Columns.Contains(colName)) colName = reader.GetName(i) + n++;
                        table.Columns.Add(new DataColumn(colName, reader.GetFieldType(i)));
                    }

                    while (reader.Read())
                    {
                        var values = new object[reader.FieldCount];
                        reader.GetValues(values);
                        table.Rows.Add(values);
                    }

                    tables.Add(table);
                } while (reader.NextResult());
            }
            return tables.ToArray();
        }

        /// <summary>
        /// Run scalar query and return scalar value
        /// </summary>
        /// <param name="conn"></param>
        /// <returns></returns>
        protected virtual object RunScalarInternal(IDbConnection conn)
        {
            Command.Connection = conn;
            Command.Transaction = Context.CurrentTransaction;
            return Command.ExecuteScalar();
        }

        /// <summary>
        /// Run non-query query and return integer value returned by server
        /// </summary>
        /// <param name="conn"></param>
        /// <returns></returns>
        protected virtual int RunInternal(IDbConnection conn)
        {
            Command.Connection = conn;
            Command.Transaction = Context.CurrentTransaction;
            return Command.ExecuteNonQuery();
        }

    }
}
