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

            using(var ctx = new MsSqlDataContext("Server=.;Database=Khoa_test;Integrated Security=SSPI"))
            {
                //var tests = ctx.Query("SELECT * FROM Test")
                //               .RunQuery<Test>();               

                //foreach (var test in tests) Console.WriteLine(test);          

                //var sw = Stopwatch.StartNew();     
                //var tables = ctx.Query("SELECT * FROM Test WHERE tzId=@tz1")
                //                .AppendQuery("SELECT * FROM Test WHERE tzId=@tz1")
                //                .AppendQuery("SELECT * FROM Test WHERE tzId=@tz2")
                //                .Param("@tz1", "Arizona")
                //                .Param("@tz2", "Florida")
                //                .RunQuery();
                //sw.Stop();
                //Console.WriteLine(sw.ElapsedMilliseconds);
                //foreach(var table in tables)
                //{
                //    var tests = Mapper.Map<Test>(table);
                //    foreach (var test in tests) Console.WriteLine(test);
                //}

                var query1 = ctx.Query("SELECT * FROM Test WHERE tzId=@tz1")
                                    .AppendQuery("SELECT * FROM Test WHERE tzId=@tz1")                                    
                                    .Param("@tz1", "Arizona");                                   

                var query2 = ctx.Proc("test_proc")
                                .Param("@tz", "Florida");

                var sw = Stopwatch.StartNew();     
                var merged = query1.Merge(query2);
                Console.WriteLine(merged.QueryText);
                var tables = merged.RunQuery();
                sw.Stop();
                Console.WriteLine(sw.ElapsedMilliseconds);
                foreach (var table in tables)
                {
                    var tests = Mapper.Map<Test>(table);
                    foreach (var test in tests) Console.WriteLine(test);

                    Console.WriteLine("---------");
                }

            }

            Console.WriteLine("Done !");
            Console.Read();
        }
    }
}
