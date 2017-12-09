using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace C3R.MiniAdo
{
    public class ContextSettings
    {
        public virtual string ConnectionString { get; set; }
        public virtual string ReadOnlyConnectionString { get; set; } = null;
    }
}
