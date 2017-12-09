using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace C3R.MiniAdo
{
    public abstract class DataContext : IDisposable
    {
        private object _syncLock = new object();
        
        public abstract IComponentFactory Factory { get; }
        protected IDbConnection Connection { get; set; }
     
        public ContextSettings Settings { get; private set; }

        protected internal IDbTransaction CurrentTransaction {
            get;
            protected set;
        }
        private volatile bool _closeConnOnEndTrans = true;

        protected DataContext(string connectionString)
            : this(new ContextSettings
            {
                ConnectionString = connectionString,
            })
        {
        }

        protected DataContext(string connectionString, string readonlyConnectionString)
            : this(new ContextSettings
            {
                ConnectionString = connectionString,
                ReadOnlyConnectionString = readonlyConnectionString,
            })
        {
        }

        protected DataContext(ContextSettings settings)
        {
            Settings = settings;
        }

        public virtual IQuery Query(string query)
        {
            return Factory.CreateQuery(this, query, CommandType.Text);
        }

        public virtual IDbTransaction StartTransaction(IsolationLevel? isolationLevel = null)
        {
            var conn = GetConnection();
            Connection = conn;

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

        public virtual void Commit()
        {
            if (CurrentTransaction != null)
            {
                CurrentTransaction.Commit();
                CurrentTransaction = null;
                if (_closeConnOnEndTrans) CloseConnection();
            }
        }
        
        public virtual void Rollback()
        {
            if (CurrentTransaction != null)
            {
                CurrentTransaction.Rollback();
                CurrentTransaction = null;
                if (_closeConnOnEndTrans) CloseConnection();
            }
        }
        
        public virtual void Dispose()
        {
            Rollback();
            CloseConnection();
        }

        internal IDbConnection GetConnection(AccessMode mode = AccessMode.ReadWrite)
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
