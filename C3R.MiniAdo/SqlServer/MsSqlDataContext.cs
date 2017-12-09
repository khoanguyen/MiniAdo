using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace C3R.MiniAdo.SqlServer
{
    public class MsSqlDataContext : DataContext
    {
        private IComponentFactory _factory = new MsSqlComponentFactory();        

        public override IComponentFactory Factory
        {
            get
            {
                return _factory;
            }
        }

        public MsSqlDataContext(string connectionString) 
            : base(connectionString)
        {
        }

        public MsSqlDataContext(string connectionString, string readOnlyConnectionString) 
            : base(connectionString, readOnlyConnectionString)
        {
        }

        public MsSqlDataContext(ContextSettings settings) 
            : base(settings)
        {
        }
    }
}
