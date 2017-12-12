using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace C3R.MiniAdo.SqlServer
{
    /// <summary>
    /// Implementation of DataContext for SqlServer
    /// </summary>
    public class MsSqlDataContext : DataContext
    {
        private IComponentFactory _factory = new MsSqlComponentFactory();

        /// <summary>
        /// SqlServer ComponentFactory
        /// </summary>
        public override IComponentFactory Factory
        {
            get
            {
                return _factory;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="connectionString">Connection string to database server</param>
        public MsSqlDataContext(string connectionString) 
            : base(connectionString)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="connectionString">Connection string database server</param>
        /// <param name="readOnlyConnectionString">Connection string to read-only database server</param>
        /// <remarks>
        /// In case of read/write splitting, when executing RunXYZReadOnly methods, read-only connection string will be used
        /// </remarks>
        public MsSqlDataContext(string connectionString, string readOnlyConnectionString) 
            : base(connectionString, readOnlyConnectionString)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="settings">Settings for current DataContext</param>
        public MsSqlDataContext(ContextSettings settings) 
            : base(settings)
        {
        }        
    }
}
