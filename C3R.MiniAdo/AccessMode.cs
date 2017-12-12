using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace C3R.MiniAdo
{
    /// <summary>
    /// Indicates access mode when excuting the query. 
    /// This is used internally.
    /// </summary>
    public enum AccessMode
    {
        /// <summary>
        /// Read and Write mode (default)
        /// </summary>
        ReadWrite = 0,

        /// <summary>
        /// Read-only mode
        /// </summary>
        ReadOnly = 1
    }
}
