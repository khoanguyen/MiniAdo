using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace C3R.MiniAdo.SqlServer
{
    class MsSqlQuery : Query
    {
        public MsSqlQuery(DataContext context, string cmd = null, CommandType cmdType = CommandType.Text) : base(context, cmd, cmdType)
        {
        }

        public override IQuery Merge(IQuery query)
        {
            return new MsSqlMergedQuery(Context, this, (MsSqlQuery)query);
        }
    }
}
