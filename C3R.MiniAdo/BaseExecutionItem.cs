using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace C3R.MiniAdo
{
    public abstract class BaseExecutionItem : IExecutionItem
    {
        protected IDbCommand Command { get; set; }
       
        public DataContext Context { get; }

        protected BaseExecutionItem(DataContext context)
        {
            Context = context;
        }

        public virtual IDataParameter GetParam(string name)
        {
            return Command.Parameters[name] as IDataParameter;
        }

        public virtual IExecutionItem Param(IDataParameter parameter)
        {
            Command.Parameters.Add(parameter);
            return this;
        }

        public IExecutionItem Param(string name, object value)
        {
            var parameter = Context.Factory.CreateParameter(name, value);
            Command.Parameters.Add(parameter);
            return this;
        }

        public IExecutionItem Param(string name, object value, DbType dbType)
        {
            var parameter = Context.Factory.CreateParameter(name, value, dbType);
            Command.Parameters.Add(parameter);
            return this;
        }

        public virtual IExecutionItem Param(string name, object value, DbType dbType, int size)
        {
            var parameter = Context.Factory.CreateParameter(name, value, dbType, size);
            return this.Param(parameter);
        }

        public abstract void Dispose();
        
        
    }
}
