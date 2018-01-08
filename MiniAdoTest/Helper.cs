using C3R.MiniAdo;
using C3R.MiniAdo.SqlServer;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiniAdoTest
{
    internal static class Helper
    {
        public static DataContext CreateMsSql()
        {
            return new MsSqlDataContext(TestDataManager.TestDataConnStr);
        }
        
        public static DataContext CreateMsSqlReadOnly()
        {
            return new MsSqlDataContext("Not a ConnectionString", TestDataManager.TestDataConnStr);
        }              
        
        public static void AssertTestDbServer()
        {
            if (!TestDataManager.HasLocalSqlServer)
                Assert.Inconclusive("No appropriate SqlServer instance. Check App.config for setting up one.");
        }  
    }
}
