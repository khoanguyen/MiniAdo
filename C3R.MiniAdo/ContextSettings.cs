using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace C3R.MiniAdo
{
    /// <summary>
    /// Settings of DataContext
    /// </summary>
    public class ContextSettings
    {
        /// <summary>
        /// Default connection string
        /// </summary>
        public virtual string ConnectionString { get; set; }

        /// <summary>
        /// Read-only connection string
        /// </summary>
        public virtual string ReadOnlyConnectionString { get; set; } = null;
    }
}
