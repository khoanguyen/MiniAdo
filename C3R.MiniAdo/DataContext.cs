using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace C3R.MiniAdo
{
    /// <summary>
    /// <para>
    /// DataContext class provides infrastructure for accessing database, including:
    /// </para>
    /// <para>- Connection management (internally, user cannot access this feature)</para>
    /// <para>- Transaction management, user can start and commit/rollback transaction through DataContext</para>
    /// <para>- Creating query</para>    
    /// </summary>    
    public abstract class DataContext : IDisposable
    {
        /// <summary>
        /// Synchronization lock for implementing Thread-safe access. 
        /// Accessing DataContext from multi-thread is not recommended
        /// </summary>
        private object _syncLock = new object();

        /// <summary>
        /// Flag object indicating closing connection on ending transaction
        /// </summary>
        private volatile bool _closeConnOnEndTrans = true;

        /// <summary>
        /// Provides Data Access Component regarding to underline Database server
        /// </summary>
        public abstract IComponentFactory Factory { get; }

        /// <summary>
        /// Connection to database server
        /// </summary>
        protected IDbConnection Connection { get; set; }

        /// <summary>
        /// Settings of current DataContext
        /// </summary>
        public virtual ContextSettings Settings { get; private set; }

        /// <summary>
        /// Current pending Transaction
        /// </summary>
        protected internal IDbTransaction CurrentTransaction
        {
            get;
            protected set;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="connectionString">Connection string to database server</param>
        protected DataContext(string connectionString)
            : this(new ContextSettings
            {
                ConnectionString = connectionString,
            })
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="connectionString">Connection string to database server</param>
        /// <param name="readonlyConnectionString">Connection string to read-only database server</param>
        protected DataContext(string connectionString, string readonlyConnectionString)
            : this(new ContextSettings
            {
                ConnectionString = connectionString,
                ReadOnlyConnectionString = readonlyConnectionString,
            })
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="settings">DataContext's settings</param>
        protected DataContext(ContextSettings settings)
        {
            Settings = settings;
        }

        /// <summary>
        /// Create a query object for preparing query statement to Database server
        /// </summary>
        /// <param name="query">Query statement</param>
        /// <returns>IQeury object</returns>
        public virtual IQuery Query(string query)
        {
            return Factory.CreateQuery(this, query, CommandType.Text);
        }

        /// <summary>
        /// Create a query object for preparing stored procedure call
        /// </summary>
        /// <param name="procName">Name of stored procedure to be called</param>
        /// <returns>IQuery object</returns>
        public virtual IQuery Proc(string procName)
        {
            return Factory.CreateQuery(this, procName, CommandType.StoredProcedure);
        }

        /// <summary>
        /// Start a Transaction
        /// </summary>
        /// <param name="isolationLevel">Isolation level of the starting Transaction (Optional)</param>
        /// <returns>IDbTransaction object</returns>
        /// <remarks>
        /// If no isolationLevel indicated, it will be decided by Database server
        /// </remarks>
        public virtual IDbTransaction StartTransaction(IsolationLevel? isolationLevel = null)
        {
            var conn = GetConnection();
            Connection = conn;

            /// Open connection.
            /// Mark _closeConnOnEndTrans as true if the connection is opened by this method call
            /// If _closeConnOnEndTrans is true, the connection will be closed when user call Commmit or Rollback 
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
                _closeConnOnEndTrans = true;
            }
            else _closeConnOnEndTrans = false;

            var trans = isolationLevel.HasValue ?
                conn.BeginTransaction(isolationLevel.Value) :
                conn.BeginTransaction();

            CurrentTransaction = trans;

            return trans;
        }

        /// <summary>
        /// Commit current pending Transaction
        /// </summary>
        public virtual void Commit()
        {
            if (CurrentTransaction != null)
            {
                try
                {
                    CurrentTransaction.Commit();
                    CurrentTransaction.Dispose();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
                finally
                {
                    CurrentTransaction = null;
                    if (_closeConnOnEndTrans) CloseConnection();
                }
            }
        }

        /// <summary>
        /// Rollback current pending Transaction
        /// </summary>
        public virtual void Rollback()
        {
            if (CurrentTransaction != null)
            {
                try
                {
                    CurrentTransaction.Rollback();
                    CurrentTransaction.Dispose();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
                finally
                {
                    CurrentTransaction = null;
                    if (_closeConnOnEndTrans) CloseConnection();
                }
            }
        }

        /// <summary>
        /// Dispose this DataContext        
        /// </summary>
        /// <remarks>
        /// During disposing DataContext, pending transaction will be rolled back and disposed,
        /// being opened connection will be closed and disposed
        /// </remarks>
        public virtual void Dispose()
        {
            Rollback();
            CloseConnection();
        }

        /// <summary>
        /// Create a DbConneciton object with given access mode
        /// </summary>
        /// <param name="mode">Access mode</param>
        /// <returns>DbConnection object</returns>
        /// <remarks>
        /// Access mode indicates which connection string whould be used. 
        /// If Read-only connection string is not set when executing ReadOnly query, default connection string will be used
        /// </remarks>
        internal protected virtual IDbConnection GetConnection(AccessMode mode = AccessMode.ReadWrite)
        {
            lock (_syncLock)
            {
                if (Connection == null)
                {
                    switch (mode)
                    {
                        case AccessMode.ReadOnly:
                            return (Settings.ReadOnlyConnectionString != null ?
                                Factory.CreateConnection(Settings.ReadOnlyConnectionString) :
                                Factory.CreateConnection(Settings.ConnectionString));

                        default:
                            return Factory.CreateConnection(Settings.ConnectionString);
                    }
                }
                else return Connection;
            }
        }

        /// <summary>
        /// Close and dispose internal connection object
        /// </summary>
        private void CloseConnection()
        {
            if (Connection != null)
            {
                if (Connection.State == ConnectionState.Open) Connection.Close();
                Connection.Dispose();
                Connection = null;
                _closeConnOnEndTrans = true;
            }
        }
    }
}
