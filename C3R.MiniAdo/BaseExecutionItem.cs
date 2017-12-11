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

        public IExecutionItem Param(string name, object value, ParameterDirection direction = ParameterDirection.Input)
        {
            var parameter = Context.Factory.CreateParameter(name, value, direction);
            Command.Parameters.Add(parameter);
            return this;
        }

        public IExecutionItem Param(string name, object value, DbType dbType, ParameterDirection direction = ParameterDirection.Input)
        {
            var parameter = Context.Factory.CreateParameter(name, value, dbType, direction);
            Command.Parameters.Add(parameter);
            return this;
        }

        public virtual IExecutionItem Param(string name, object value, DbType dbType, int size, ParameterDirection direction = ParameterDirection.Input)
        {
            var parameter = Context.Factory.CreateParameter(name, value, dbType, size, direction);
            return this.Param(parameter);
        }

        public abstract void Dispose();
        
        
    }
}
