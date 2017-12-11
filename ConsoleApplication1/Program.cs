using C3R.MiniAdo.Mapping;
using C3R.MiniAdo.SqlServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            Mapper.RegisterMapper(new TestMapper());

            using(var ctx = new MsSqlDataContext("Server=.;Database=Khoa_test;Integrated Security=SSPI"))
            {
                var tests = ctx.Query("SELECT * FROM Test")
                               .RunQuery<Test>();

                foreach (var test in tests) Console.WriteLine(test);               
            }

            Console.WriteLine("Done !");
            Console.Read();
        }
    }
}
