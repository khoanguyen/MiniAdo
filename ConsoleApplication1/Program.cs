using C3R.MiniAdo.Mapping;
using C3R.MiniAdo.SqlServer;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            DefaultMapperProvider.RegisterMapper(new TestMapper());

            var tests = GetData();

            foreach (var testObj in tests)
            {
                Console.WriteLine(testObj);                
            }

            Console.WriteLine("Done !");
            Console.Read();
        }

        public static IEnumerable<Test> GetData()
        {
            using (var ctx = new MsSqlDataContext("Server=.;Database=Khoa_test;Integrated Security=SSPI", "Server=.;Database=Khoa_test;Integrated Security=SSPI;ApplicationIntent=ReadOnly"))
            {
                return ctx.Proc("test_proc")
                          .Param("@tz", "Florida")
                          .RunQuery<Test>();                
            }
        }
    }

    
}
