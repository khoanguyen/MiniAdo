using C3R.MiniAdo;
using C3R.MiniAdo.SqlServer;
using MiniAdoTest.Data;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace MiniAdoTest
{
    [TestFixture]
    public class MiniAdo_QueryTest
    {
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            TestDataManager.MountTestData();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            TestDataManager.UnmountTestData();
        }

        [SetUp]
        public void TestSetUp()
        {
            Helper.AssertTestDbServer();
        }

        [Test]
        public void TestSimpleSelectQuery_WithParams()
        {
            var query = "SELECT * FROM Students WHERE Status=@status";
            using (var ctx = Helper.CreateMsSql())
            {
                var table = ctx.Query(query)
                               .Param("@status", 1)
                               .RunQuery().First();

                var expected = QueryRunner.Select("SELECT * FROM Students WHERE Status = 1");

                AssertEx.AreEqual(expected, table);
            }
        }

        [Test]
        public void Test_ReadOnly_SimpleSelectQuery_WithParams()
        {
            var query = "SELECT * FROM Students WHERE Status=@status";
            using (var ctx = Helper.CreateMsSqlReadOnly())
            {
                var table = ctx.Query(query)
                               .Param("@status", 1)
                               .RunQueryReadOnly().First();

                var expected = QueryRunner.Select("SELECT * FROM Students WHERE Status = 1");

                AssertEx.AreEqual(expected, table);
            }
        }

        [Test]
        public void TestSimpleSelectQuery_WithParams_2()
        {
            var query = "SELECT * FROM Programs WHERE ProgramCode=@programCode";
            using (var ctx = Helper.CreateMsSql())
            {
                var table = ctx.Query(query)
                               .Param("@programCode", "BCOSC")
                               .RunQuery().First();

                var expected = QueryRunner.Select("SELECT * FROM Programs WHERE ProgramCode='BCOSC'");

                AssertEx.AreEqual(expected, table);
            }
        }

        [Test]
        public void Test_ReadOnly_SimpleSelectQuery_WithParams_2()
        {
            var query = "SELECT * FROM Programs WHERE ProgramCode=@programCode";
            using (var ctx = Helper.CreateMsSqlReadOnly())
            {
                var table = ctx.Query(query)
                               .Param("@programCode", "BCOSC")
                               .RunQueryReadOnly().First();

                var expected = QueryRunner.Select("SELECT * FROM Programs WHERE ProgramCode='BCOSC'");

                AssertEx.AreEqual(expected, table);
            }
        }

        [Test]
        public void TestSimpleSelectQuery_WithMultipleParams()
        {
            var query = "SELECT * FROM Students WHERE Status=@status AND Email<>@email";
            using (var ctx = Helper.CreateMsSql())
            {
                var table = ctx.Query(query)
                               .Param("@status", 1)
                               .Param("@email", null)
                               .RunQuery().First();

                var expected = QueryRunner.Select("SELECT * FROM Students WHERE Status=1 AND Email<>NULL");

                AssertEx.AreEqual(expected, table);
            }
        }

        [Test]
        public void Test_ReadOnly_SimpleSelectQuery_WithMultipleParams()
        {
            var query = "SELECT * FROM Students WHERE Status=@status AND Email<>@email";
            using (var ctx = Helper.CreateMsSqlReadOnly())
            {
                var table = ctx.Query(query)
                               .Param("@status", 1)
                               .Param("@email", null)
                               .RunQueryReadOnly().First();

                var expected = QueryRunner.Select("SELECT * FROM Students WHERE Status=1 AND Email<>NULL");

                AssertEx.AreEqual(expected, table);
            }
        }

        [Test]
        public void TestSimpleSelectQuery()
        {
            var query = "SELECT * FROM Students";
            using (var ctx = Helper.CreateMsSql())
            {
                var table = ctx.Query(query).RunQuery().First();
                var expected = QueryRunner.Select(query);

                AssertEx.AreEqual(expected, table);
            }
        }

        [Test]
        public void Test_ReadOnly_SimpleSelectQuery()
        {
            var query = "SELECT * FROM Students";
            using (var ctx = Helper.CreateMsSqlReadOnly())
            {
                var table = ctx.Query(query).RunQueryReadOnly().First();
                var expected = QueryRunner.Select(query);

                AssertEx.AreEqual(expected, table);
            }
        }

        [Test]
        public void Test_ReadOnly_MultipleSelectQuery_OneQuery_WithParams()
        {
            var query1 = "SELECT * FROM Students WHERE Status=@status";
            var query2 = "SELECT * FROM Programs WHERE ProgramName=@progName";
            var query3 = "SELECT * FROM Programs WHERE Status=@progStatus";
            var query = $"{query1};{query2};{query3}";
            using (var ctx = Helper.CreateMsSqlReadOnly())
            {
                var table = ctx.Query(query)
                               .Param(ctx.Factory.CreateParameter("@status", 1))
                               .Param("@progStatus", 1, DbType.Byte)
                               .Param("@progName", "Bachelor of Computer Science")
                               .RunQueryReadOnly().ToArray();

                var students = QueryRunner.Select("SELECT * FROM Students WHERE Status=1");
                var programs2 = QueryRunner.Select("SELECT * FROM Programs WHERE ProgramName='Bachelor of Computer Science'");
                var programs3 = QueryRunner.Select("SELECT * FROM Programs WHERE Status=1");

                Assert.AreEqual(3, table.Length);

                AssertEx.AreEqual(students, table[0]);
                AssertEx.AreEqual(programs2, table[1]);
                AssertEx.AreEqual(programs3, table[2]);
            }
        }

        [Test]
        public void TestMultipleSelectQuery_OneQuery()
        {
            var query1 = "SELECT * FROM Students";
            var query2 = "SELECT * FROM Programs";
            var query = $"{query1};{query2}";
            using (var ctx = Helper.CreateMsSql())
            {
                var table = ctx.Query(query).RunQuery().ToArray();

                var students = QueryRunner.Select(query1);
                var programs = QueryRunner.Select(query2);

                Assert.AreEqual(2, table.Length);

                AssertEx.AreEqual(students, table[0]);
                AssertEx.AreEqual(programs, table[1]);
            }
        }

        [Test]
        public void Test_ReadOnly_MultipleSelectQuery_OneQuery()
        {
            var query1 = "SELECT * FROM Students";
            var query2 = "SELECT * FROM Programs";
            var query = $"{query1};{query2}";
            using (var ctx = Helper.CreateMsSqlReadOnly())
            {
                var table = ctx.Query(query).RunQueryReadOnly().ToArray();

                var students = QueryRunner.Select(query1);
                var programs = QueryRunner.Select(query2);

                Assert.AreEqual(2, table.Length);

                AssertEx.AreEqual(students, table[0]);
                AssertEx.AreEqual(programs, table[1]);
            }
        }

        [Test]
        public void TestMultipleSelectQuery_AppendQuery_WithParams()
        {
            var query1 = "SELECT * FROM Students WHERE Status=@status";
            var query2 = "SELECT * FROM Programs WHERE ProgramName=@progName";

            using (var ctx = Helper.CreateMsSql())
            {
                var tables = ctx.Query(query1)
                                .AppendQuery(query2)
                                .Param(ctx.Factory.CreateParameter("@status", 1))
                                .Param("@progName", "Bachelor of Computer Science")
                                .RunQuery().ToArray();

                var students = QueryRunner.Select("SELECT * FROM Students WHERE Status=1");
                var programs = QueryRunner.Select("SELECT * FROM Programs WHERE ProgramName='Bachelor of Computer Science'");

                Assert.AreEqual(2, tables.Length);

                AssertEx.AreEqual(students, tables[0]);
                AssertEx.AreEqual(programs, tables[1]);
            }
        }


        [Test]
        public void Test_ReadOnly_MultipleSelectQuery_AppendQuery_WithParams()
        {
            var query1 = "SELECT * FROM Students WHERE Status=@status";
            var query2 = "SELECT * FROM Programs WHERE ProgramName=@progName";

            using (var ctx = Helper.CreateMsSqlReadOnly())
            {
                var tables = ctx.Query(query1)
                                .AppendQuery(query2)
                                .Param(ctx.Factory.CreateParameter("@status", 1))
                                .Param("@progName", "Bachelor of Computer Science")
                                .RunQueryReadOnly().ToArray();

                var students = QueryRunner.Select("SELECT * FROM Students WHERE Status=1");
                var programs = QueryRunner.Select("SELECT * FROM Programs WHERE ProgramName='Bachelor of Computer Science'");

                Assert.AreEqual(2, tables.Length);

                AssertEx.AreEqual(students, tables[0]);
                AssertEx.AreEqual(programs, tables[1]);
            }
        }

        [Test]
        public void TestMultipleSelectQuery_AppendQuery()
        {
            var query1 = "SELECT * FROM Students";
            var query2 = "SELECT * FROM Programs";

            using (var ctx = Helper.CreateMsSql())
            {
                var tables = ctx.Query(query1)
                               .AppendQuery(query2)
                               .RunQuery().ToArray();

                var students = QueryRunner.Select(query1);
                var programs = QueryRunner.Select(query2);

                Assert.AreEqual(2, tables.Length);

                AssertEx.AreEqual(students, tables[0]);
                AssertEx.AreEqual(programs, tables[1]);
            }
        }

        public void Test_ReadOnly_MultipleSelectQuery_AppendQuery()
        {
            var query1 = "SELECT * FROM Students";
            var query2 = "SELECT * FROM Programs";

            using (var ctx = Helper.CreateMsSqlReadOnly())
            {
                var tables = ctx.Query(query1)
                               .AppendQuery(query2)
                               .RunQueryReadOnly().ToArray();

                var students = QueryRunner.Select(query1);
                var programs = QueryRunner.Select(query2);

                Assert.AreEqual(2, tables.Length);

                AssertEx.AreEqual(students, tables[0]);
                AssertEx.AreEqual(programs, tables[1]);
            }
        }

        [Test]
        public void TestMultipleSelectQuery_MergeQuery_WithParams()
        {
            var queryText1 = "SELECT * FROM Students WHERE Status=@status AND Email=@email";
            var queryText2 = "SELECT * FROM Programs WHERE ProgramName=@progName";

            using (var ctx = Helper.CreateMsSql())
            {
                var query1 = ctx.Query(queryText1)
                                .Param("@status", 1)
                                .Param("@email", null);
                var query2 = ctx.Query(queryText2)
                                .Param(new SqlParameter("@progName", SqlDbType.NVarChar, 1000)
                                {
                                    SqlValue = "Bachelor of Computer Science"
                                });


                var query = query1.Merge(query2);

                var tables = query.RunQuery().ToArray();

                var students = QueryRunner.Select("SELECT * FROM Students WHERE Status=1 AND Email=NULL");
                var programs = QueryRunner.Select("SELECT * FROM Programs WHERE ProgramName='Bachelor of Computer Science'");

                Assert.AreEqual(2, tables.Length);

                AssertEx.AreEqual(students, tables[0]);
                AssertEx.AreEqual(programs, tables[1]);
            }
        }

        [Test]
        public void Test_ReadOnly_MultipleSelectQuery_MergeQuery_WithParams()
        {
            var queryText1 = "SELECT * FROM Students WHERE Status=@status AND Email=@email";
            var queryText2 = "SELECT * FROM Programs WHERE ProgramName=@progName";

            using (var ctx = Helper.CreateMsSqlReadOnly())
            {
                var query1 = ctx.Query(queryText1)
                                .Param("@status", 1)
                                .Param("@email", null);
                var query2 = ctx.Query(queryText2)
                                .Param(new SqlParameter("@progName", SqlDbType.NVarChar, 1000)
                                {
                                    SqlValue = "Bachelor of Computer Science"
                                });


                var query = query1.Merge(query2);

                var tables = query.RunQueryReadOnly().ToArray();

                var students = QueryRunner.Select("SELECT * FROM Students WHERE Status=1 AND Email=NULL");
                var programs = QueryRunner.Select("SELECT * FROM Programs WHERE ProgramName='Bachelor of Computer Science'");

                Assert.AreEqual(2, tables.Length);

                AssertEx.AreEqual(students, tables[0]);
                AssertEx.AreEqual(programs, tables[1]);
            }
        }

        [Test]
        public void TestMultipleSelectQuery_MergeQuery()
        {
            var queryText1 = "SELECT * FROM Students";
            var queryText2 = "SELECT * FROM Programs";

            using (var ctx = Helper.CreateMsSql())
            {
                var query1 = ctx.Query(queryText1);
                var query2 = ctx.Query(queryText2);

                var query = query1.Merge(query2);

                var tables = query.RunQuery().ToArray();

                var students = QueryRunner.Select(queryText1);
                var programs = QueryRunner.Select(queryText2);

                Assert.AreEqual(2, tables.Length);

                AssertEx.AreEqual(students, tables[0]);
                AssertEx.AreEqual(programs, tables[1]);
            }
        }

        [Test]
        public void Test_ReadOnly_MultipleSelectQuery_MergeQuery()
        {
            var queryText1 = "SELECT * FROM Students";
            var queryText2 = "SELECT * FROM Programs";

            using (var ctx = Helper.CreateMsSqlReadOnly())
            {
                var query1 = ctx.Query(queryText1);
                var query2 = ctx.Query(queryText2);

                var query = query1.Merge(query2);

                var tables = query.RunQueryReadOnly().ToArray();

                var students = QueryRunner.Select(queryText1);
                var programs = QueryRunner.Select(queryText2);

                Assert.AreEqual(2, tables.Length);

                AssertEx.AreEqual(students, tables[0]);
                AssertEx.AreEqual(programs, tables[1]);
            }
        }

        [Test]
        public void TestSimpleScalarQuery()
        {
            var query = "SELECT COUNT(*) FROM Students";

            using (var ctx = Helper.CreateMsSql())
            {
                var objCount = ctx.Query(query).RunScalar();
                var intCount = ctx.Query(query).RunScalar<int>();

                var expected = QueryRunner.Scalar(query);

                Assert.AreEqual(expected, objCount);
                Assert.AreEqual(expected, intCount);
            }
        }

        [Test]
        public void Test_ReadOnly_SimpleScalarQuery()
        {
            var query = "SELECT COUNT(*) FROM Students";

            using (var ctx = Helper.CreateMsSqlReadOnly())
            {
                var objCount = ctx.Query(query).RunScalarReadOnly();
                var intCount = ctx.Query(query).RunScalarReadOnly<int>();

                var expected = QueryRunner.Scalar(query);

                Assert.AreEqual(expected, objCount);
                Assert.AreEqual(expected, intCount);
            }
        }

        [Test]
        public void TestSimpleScalarQuery_WithParams()
        {
            var query = "SELECT COUNT(*) FROM Students WHERE [Status]=@status";

            using (var ctx = Helper.CreateMsSql())
            {
                var objCount = ctx.Query(query).Param("@status", 1).RunScalar();
                var intCount = ctx.Query(query)
                    .Param(ctx.Factory.CreateParameter("@status", 1)).RunScalar<int>();

                var expected = QueryRunner.Scalar("SELECT COUNT(*) FROM Students WHERE[Status]=1");

                Assert.AreEqual(expected, objCount);
                Assert.AreEqual(expected, intCount);
            }
        }

        [Test]
        public void Test_ReadOnly_SimpleScalarQuery_WithParams()
        {
            var query = "SELECT COUNT(*) FROM Students WHERE [Status]=@status";

            using (var ctx = Helper.CreateMsSqlReadOnly())
            {
                var objCount = ctx.Query(query).Param("@status", 1).RunScalarReadOnly();
                var intCount = ctx.Query(query)
                    .Param(ctx.Factory.CreateParameter("@status", 1)).RunScalarReadOnly<int>();

                var expected = QueryRunner.Scalar("SELECT COUNT(*) FROM Students WHERE[Status]=1");

                Assert.AreEqual(expected, objCount);
                Assert.AreEqual(expected, intCount);
            }
        }

        [Test]
        public void Test_Insert()
        {
            var queryText = "INSERT INTO Students VALUES(@id, @firstName, @lastName, @email, @status)";

            using (var ctx = Helper.CreateMsSql())
            {
                var query = ctx.Query(queryText)
                    .Param("@id", 9)
                    .Param("@firstName", "Chewbacca")
                    .Param("@lastName", "Wookiee")
                    .Param("@email", "c.wookiee@millenium.com")
                    .Param("@status", 0);

                query.Run();

                var table = QueryRunner.Select("SELECT * FROM Students WHERE StudentId=9");

                Assert.AreEqual(1, table.Rows.Count);

                var row = table.Rows[0];

                Assert.AreEqual(9, row["StudentId"]);
                Assert.AreEqual("Chewbacca", row["FirstName"]);
                Assert.AreEqual("Wookiee", row["LastName"]);
                Assert.AreEqual("c.wookiee@millenium.com", row["Email"]);
                Assert.AreEqual(0, row["Status"]);
            }
        }

        [Test, Order(1)]
        public void Test_MultiInsert()
        {
            var insertedData = new object[]
            {
                new { StudentId=11, FirstName="Jaina", LastName="Solo", Email = "jaina.solo@hans.com", Status = 0},
                new { StudentId=12, FirstName="Jace", LastName="Solo", Email = "jace@sith.com", Status = 1},
                new { StudentId=13, FirstName="Ben", LastName="Skywalker", Email = "", Status = 2},
            };
            
            var queryText = "INSERT INTO Students VALUES(@id, @firstName, @lastName, @email, @status)";

            using (var ctx = Helper.CreateMsSql())
            {
                IQuery query = ctx.Query("");
                
                foreach(dynamic data in insertedData)
                {
                    var tmp = ctx.Query(queryText)
                        .Param("@id", data.StudentId)
                        .Param("@firstName", data.FirstName)
                        .Param("@lastName", data.LastName)
                        .Param("@email", data.Email)
                        .Param("@status", data.Status);

                    query = query.Merge(tmp);
                }

                query.Run();

                var table = QueryRunner.Select("SELECT * FROM Students WHERE StudentId IN (11, 12, 13)");

                Assert.AreEqual(3, table.Rows.Count);

                for(var i = 11; i < 14; i++)
                {
                    var row = table.Rows.OfType<DataRow>().First(r => r.Field<int>("StudentId") == i);
                    dynamic data = insertedData[i - 11];

                    Assert.AreEqual(data.StudentId, row["StudentId"]);
                    Assert.AreEqual(data.FirstName, row["FirstName"]);
                    Assert.AreEqual(data.LastName, row["LastName"]);
                    Assert.AreEqual(data.Email, row["Email"]);
                    Assert.AreEqual(data.Status, row["Status"]);
                }                
            }
        }

        [Test]
        public void Test_Update()
        {
            var queryText = "UPDATE Students SET Email=@email, Status=@status WHERE StudentId=@studentId";

            using (var ctx = Helper.CreateMsSql())
            {
                var query = ctx.Query(queryText)
                    .Param("@email", "random.guy@yahoo.com")
                    .Param("@status", 3)
                    .Param("@studentId", 4);

                query.Run();

                var table = QueryRunner.Select("SELECT * FROM Students WHERE StudentId=4");

                Assert.AreEqual(1, table.Rows.Count);

                var row = table.Rows[0];

                Assert.AreEqual("random.guy@yahoo.com", row["Email"]);
                Assert.AreEqual(3, row["Status"]);
            }
        }

        [Test]
        public void Test_MultiUpdate()
        {
            var insertedData = new object[]
            {
                new { StudentId=3, FirstName="Jaina", LastName="Solo", Email = "jaina.solo@hans.com", Status = 0},
                new { StudentId=4, FirstName="Jace", LastName="Solo", Email = "jace@sith.com", Status = 1},
                new { StudentId=5, FirstName="Ben", LastName="Skywalker", Email = "", Status = 2},
            };

            var queryText = @"UPDATE Students SET FirstName=@firstName, 
                                                  LastName=@lastName, 
                                                  Email=@email, 
                                                  Status=@status
                              WHERE StudentId=@id";

            using (var ctx = Helper.CreateMsSql())
            {
                IQuery query = ctx.Query("");

                foreach (dynamic data in insertedData)
                {
                    var tmp = ctx.Query(queryText)
                        .Param("@id", data.StudentId)
                        .Param("@firstName", data.FirstName)
                        .Param("@lastName", data.LastName)
                        .Param("@email", data.Email)
                        .Param("@status", data.Status);

                    query = query.Merge(tmp);
                }

                query.Run();

                var table = QueryRunner.Select("SELECT * FROM Students WHERE StudentId IN (3, 4, 5)");

                Assert.AreEqual(3, table.Rows.Count);

                for (var i = 3; i < 6; i++)
                {
                    var row = table.Rows.OfType<DataRow>().First(r => r.Field<int>("StudentId") == i);
                    dynamic data = insertedData[i - 3];

                    Assert.AreEqual(data.StudentId, row["StudentId"]);
                    Assert.AreEqual(data.FirstName, row["FirstName"]);
                    Assert.AreEqual(data.LastName, row["LastName"]);
                    Assert.AreEqual(data.Email, row["Email"]);
                    Assert.AreEqual(data.Status, row["Status"]);
                }
            }
        }

        [Test, Order(2)]
        public void Test_Delete()
        {
            var queryText = "DELETE Students WHERE StudentId=@id";

            using (var ctx = Helper.CreateMsSql())
            {
                var query = ctx.Query(queryText)
                    .Param("@id", 2);

                query.Run();

                var table = QueryRunner.Select("SELECT * FROM Students WHERE StudentId=2");

                Assert.AreEqual(0, table.Rows.Count);
            }
        }

        [Test, Order(3)]
        public void Test_MultiDelete()
        {
            var queryText = "DELETE Students WHERE StudentId=@id";

            using (var ctx = Helper.CreateMsSql())
            {
                IQuery query = ctx.Query("");

                for(var i = 6; i < 9; i++)
                {
                    query = query.Merge(ctx.Query(queryText)
                                   .Param("@id", i));
                }

                query.Run();

                var table = QueryRunner.Select("SELECT * FROM Students WHERE StudentId IN (5,6,7,8)");

                Assert.AreEqual(1, table.Rows.Count);
                Assert.AreEqual(5, table.Rows[0]["StudentId"]);
            }
        }
    }
}
