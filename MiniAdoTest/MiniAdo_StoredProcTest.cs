using MiniAdoTest.Data;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace MiniAdoTest
{
    [TestFixture]
    public class MiniAdo_StoredProcTest
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


        [Test, Order(1)]
        public void Test_Query_SimpleProc()
        {
            var procName = "GetAllStudents";
            using (var ctx = Helper.CreateMsSql())
            {
                var table = ctx.Proc(procName).RunQuery().First();
                var expected = QueryRunner.Select($"exec {procName}");

                AssertEx.AreEqual(expected, table);
            }
        }

        [Test, Order(1)]
        public void Test_ReadOnly_Query_SimpleProc()
        {
            var procName = "GetAllStudents";
            using (var ctx = Helper.CreateMsSqlReadOnly())
            {
                var table = ctx.Proc(procName).RunQueryReadOnly().First();
                var expected = QueryRunner.Select($"exec {procName}");

                AssertEx.AreEqual(expected, table);
            }
        }

        [Test, Order(1)]
        public void Test_Query_SimpleProc_WithParams()
        {
            var procName = "GetStudentsByStatus";
            using (var ctx = Helper.CreateMsSql())
            {
                var table = ctx.Proc(procName)
                               .Param("@status", 1)
                               .RunQuery().First();
                var expected = QueryRunner.Select($"exec {procName} 1");

                AssertEx.AreEqual(expected, table);
            }
        }

        [Test, Order(1)]
        public void Test__ReadOnly_Query_SimpleProc_WithParams()
        {
            var procName = "GetStudentsByStatus";
            using (var ctx = Helper.CreateMsSqlReadOnly())
            {
                var table = ctx.Proc(procName)
                               .Param("@status", 1)
                               .RunQueryReadOnly().First();
                var expected = QueryRunner.Select($"exec {procName} 1");

                AssertEx.AreEqual(expected, table);
            }
        }


        [Test, Order(1)]
        public void Test_Query_SimpleProc_WithParams_2()
        {
            var procName = "GetProgramsByCode";
            using (var ctx = Helper.CreateMsSql())
            {
                var table = ctx.Proc(procName)
                               .Param("@programCode", "BCOSC")
                               .RunQuery().First();

                var expected = QueryRunner.Select($"exec GetProgramsByCode 'BCOSC'");

                AssertEx.AreEqual(expected, table);
            }
        }

        [Test, Order(1)]
        public void Test_ReadOnly_Query_SimpleProc_WithParams_2()
        {
            var procName = "GetProgramsByCode";
            using (var ctx = Helper.CreateMsSqlReadOnly())
            {
                var table = ctx.Proc(procName)
                               .Param("@programCode", "BCOSC")
                               .RunQueryReadOnly().First();

                var expected = QueryRunner.Select($"exec GetProgramsByCode 'BCOSC'");

                AssertEx.AreEqual(expected, table);
            }
        }

        [Test, Order(1)]
        public void Test_Query_MultipleProc()
        {
            var procName1 = "GetAllStudents";
            var procName2 = "GetAllPrograms";
            using (var ctx = Helper.CreateMsSql())
            {
                var proc1 = ctx.Proc(procName1);
                var proc2 = ctx.Proc(procName2);
                var tables = proc1.Merge(proc2).RunQuery().ToArray();

                var expected1 = QueryRunner.Select($"exec {procName1}");
                var expected2 = QueryRunner.Select($"exec {procName2}");

                Assert.AreEqual(2, tables.Length);
                AssertEx.AreEqual(expected1, tables[0]);
                AssertEx.AreEqual(expected2, tables[1]);
            }
        }

        [Test, Order(1)]
        public void Test_ReadOnly_Query_MultipleProc()
        {
            var procName1 = "GetAllStudents";
            var procName2 = "GetAllPrograms";
            using (var ctx = Helper.CreateMsSqlReadOnly())
            {
                var proc1 = ctx.Proc(procName1);
                var proc2 = ctx.Proc(procName2);
                var tables = proc1.Merge(proc2).RunQueryReadOnly().ToArray();

                var expected1 = QueryRunner.Select($"exec {procName1}");
                var expected2 = QueryRunner.Select($"exec {procName2}");

                Assert.AreEqual(2, tables.Length);
                AssertEx.AreEqual(expected1, tables[0]);
                AssertEx.AreEqual(expected2, tables[1]);
            }
        }


        [Test, Order(1)]
        public void Test_Query_MultipleProc_WithParams()
        {
            var procName1 = "GetProgramByPK";
            var procName2 = "GetStudentsByProgram";
            using (var ctx = Helper.CreateMsSql())
            {
                var proc1 = ctx.Proc(procName1).Param("@programId", 1);
                var proc2 = ctx.Proc(procName2).Param(ctx.Factory.CreateParameter("@programId", 1, DbType.Int32));
                var tables = proc1.Merge(proc2).RunQuery().ToArray();

                var expected1 = QueryRunner.Select($"exec {procName1} 1");
                var expected2 = QueryRunner.Select($"exec {procName2} 1");

                Assert.AreEqual(2, tables.Length);
                AssertEx.AreEqual(expected1, tables[0]);
                AssertEx.AreEqual(expected2, tables[1]);
            }
        }

        [Test, Order(1)]
        public void Test_ReadOnly_Query_MultipleProc_WithParams()
        {
            var procName1 = "GetProgramByPK";
            var procName2 = "GetStudentsByProgram";
            using (var ctx = Helper.CreateMsSqlReadOnly())
            {
                var proc1 = ctx.Proc(procName1).Param("@programId", 1);
                var proc2 = ctx.Proc(procName2).Param(ctx.Factory.CreateParameter("@programId", 1, DbType.Int32));
                var tables = proc1.Merge(proc2).RunQueryReadOnly().ToArray();

                var expected1 = QueryRunner.Select($"exec {procName1} 1");
                var expected2 = QueryRunner.Select($"exec {procName2} 1");

                Assert.AreEqual(2, tables.Length);
                AssertEx.AreEqual(expected1, tables[0]);
                AssertEx.AreEqual(expected2, tables[1]);
            }
        }

        [Test, Order(1)]
        public void Test_Query_MultipleProc_WithParams_4Procs()
        {
            var procName1 = "GetProgramByPK";
            var procName2 = "GetStudentsByProgram";
            var procName3 = "GetAllStudents";
            var procName4 = "GetProgramsByCode";
            using (var ctx = Helper.CreateMsSql())
            {
                var proc1 = ctx.Proc(procName1).Param("@programId", 1);
                var proc2 = ctx.Proc(procName2).Param(ctx.Factory.CreateParameter("@programId", 1, DbType.Int32));
                var proc3 = ctx.Proc(procName3);
                var proc4 = ctx.Query("exec " + procName4 + " @programCode=@programCode")
                               .Param("@programCode", "BCOSC", ParameterDirection.Input);

                var query = proc1
                    .Merge(proc2)
                    .Merge(proc3)
                    .Merge(proc4);

                var tables = query.RunQuery().ToArray();

                var expected1 = QueryRunner.Select($"exec {procName1} 1");
                var expected2 = QueryRunner.Select($"exec {procName2} 1");
                var expected3 = QueryRunner.Select($"exec {procName3}");
                var expected4 = QueryRunner.Select($"exec {procName4} 'BCOSC'");

                Assert.AreEqual(4, tables.Length);
                AssertEx.AreEqual(expected1, tables[0]);
                AssertEx.AreEqual(expected2, tables[1]);
                AssertEx.AreEqual(expected3, tables[2]);
                AssertEx.AreEqual(expected4, tables[3]);
            }
        }

        [Test, Order(1)]
        public void Test_ReadOnly_Query_MultipleProc_WithParams_4Procs()
        {
            var procName1 = "GetProgramByPK";
            var procName2 = "GetStudentsByProgram";
            var procName3 = "GetAllStudents";
            var procName4 = "GetProgramsByCode";
            using (var ctx = Helper.CreateMsSqlReadOnly())
            {
                var proc1 = ctx.Proc(procName1).Param("@programId", 1);
                var proc2 = ctx.Proc(procName2).Param(ctx.Factory.CreateParameter("@programId", 1, DbType.Int32));
                var proc3 = ctx.Proc(procName3);
                var proc4 = ctx.Proc(procName4).Param("@programCode", "BCOSC", ParameterDirection.Input);

                var query = proc1
                    .Merge(proc2)
                    .Merge(proc3)
                    .Merge(proc4);

                var tables = query.RunQueryReadOnly().ToArray();

                var expected1 = QueryRunner.Select($"exec {procName1} 1");
                var expected2 = QueryRunner.Select($"exec {procName2} 1");
                var expected3 = QueryRunner.Select($"exec {procName3}");
                var expected4 = QueryRunner.Select($"exec {procName4} 'BCOSC'");

                Assert.AreEqual(4, tables.Length);
                AssertEx.AreEqual(expected1, tables[0]);
                AssertEx.AreEqual(expected2, tables[1]);
                AssertEx.AreEqual(expected3, tables[2]);
                AssertEx.AreEqual(expected4, tables[3]);
            }
        }

        [Test, Order(1)]
        public void Test_Scalar_Query_Proc()
        {
            var procName = "[CountAllStudents]";
            using (var ctx = Helper.CreateMsSql())
            {
                var objCount = ctx.Proc(procName).RunScalar();
                var intCount = ctx.Proc(procName).RunScalar<int>();

                var expected = QueryRunner.Scalar("exec CountAllStudents");

                Assert.AreEqual(expected, objCount);
                Assert.AreEqual(expected, intCount);
            }
        }

        [Test, Order(1)]
        public void Test_ReadOnly_Scalar_Query_Proc()
        {
            var procName = "[CountAllStudents]";
            using (var ctx = Helper.CreateMsSqlReadOnly())
            {
                var objCount = ctx.Proc(procName).RunScalarReadOnly();
                var intCount = ctx.Proc(procName).RunScalarReadOnly<int>();

                var expected = QueryRunner.Scalar("exec CountAllStudents");

                Assert.AreEqual(expected, objCount);
                Assert.AreEqual(expected, intCount);
            }
        }

        [Test, Order(1)]
        public void Test_Scalar_Query_Proc_WithParams()
        {
            var procName = "[CountStudentsByProgram]";
            using (var ctx = Helper.CreateMsSql())
            {
                var objCount = ctx.Proc(procName).Param("@programId", 2).RunScalar();
                var intCount = ctx.Proc(procName).Param("@programId", 2).RunScalar<int>();

                var expected = QueryRunner.Scalar("exec CountStudentsByProgram 2");

                Assert.AreEqual(expected, objCount);
                Assert.AreEqual(expected, intCount);
            }
        }

        [Test, Order(1)]
        public void Test_ReadOnly_Scalar_Query_Proc_WithParams()
        {
            var procName = "[CountStudentsByProgram]";
            using (var ctx = Helper.CreateMsSqlReadOnly())
            {
                var objCount = ctx.Proc(procName).Param("@programId", 2).RunScalarReadOnly();
                var intCount = ctx.Proc(procName).Param("@programId", 2).RunScalarReadOnly<int>();

                var expected = QueryRunner.Scalar("exec CountStudentsByProgram 2");

                Assert.AreEqual(expected, objCount);
                Assert.AreEqual(expected, intCount);
            }
        }

        [Test, Order(2)]
        public void Test_Insert_Proc_RunQuery()
        {
            using(var ctx = Helper.CreateMsSql())
            {
                var table = ctx.Proc("Enroll")
                               .Param("@studentId", 1)
                               .Param("@programId", 2)
                               .RunQuery().First();

                var expected = QueryRunner.Select("SELECT * FROM Enrollments WHERE StudentId=1 AND ProgramId=2");

                Assert.AreEqual(1, expected.Rows.Count);
                AssertEx.AreEqual(table, expected);
            }
        }

        [Test, Order(2)]
        public void Test_Insert_Proc_Run()
        {
            using (var ctx = Helper.CreateMsSql())
            {
                ctx.Proc("Enroll")
                   .Param("@studentId", 1)
                   .Param("@programId", 3)
                   .Run();

                var inserted = QueryRunner.Select("SELECT * FROM Enrollments WHERE StudentId=1 AND ProgramId=3");

                Assert.AreEqual(1, inserted.Rows.Count);
                Assert.AreEqual(1, inserted.Rows[0]["StudentId"]);
                Assert.AreEqual(3, inserted.Rows[0]["ProgramId"]);
            }
        }
        
        [Test, Order(2)]
        public void Test_MultiInsert_Proc_Run()
        {
            using (var ctx = Helper.CreateMsSql())
            {
                var query = ctx.Proc("Enroll")
                               .Param("@studentId", 3)
                               .Param("@programId", 5);

                query = query.Merge(ctx.Proc("Enroll")
                             .Param("@studentId", 8)
                             .Param("@programId", 1));

                query.Run();

                var inserted = QueryRunner.Select(@"SELECT * FROM Enrollments WHERE (StudentId=3 AND ProgramId=5) OR
                                                                                    (StudentId=8 AND ProgramId=1)");

                Assert.AreEqual(2, inserted.Rows.Count);                
            }
        }

        [Test, Order(3)]
        public void Test_Update_Proc_RunQuery()
        {
            using (var ctx = Helper.CreateMsSql())
            {
                var table = ctx.Proc("ChangeProgram")
                               .Param("@studentId", 7)
                               .Param("@oldProgramId", 2)
                               .Param("@newProgramId", 4)
                               .RunQuery().First();

                var expected = QueryRunner.Select("SELECT * FROM Enrollments WHERE StudentId=7 AND ProgramId=2");
                Assert.AreEqual(0, expected.Rows.Count);

                expected = QueryRunner.Select("SELECT * FROM Enrollments WHERE StudentId=7 AND ProgramId=4");
                Assert.AreEqual(1, expected.Rows.Count);
                AssertEx.AreEqual(table, expected);
            }
        }

        [Test, Order(3)]
        public void Test_Update_Proc_Run()
        {
            using (var ctx = Helper.CreateMsSql())
            {
                ctx.Proc("ChangeProgram")
                    .Param("@studentId", 6)
                    .Param("@oldProgramId", 1)
                    .Param("@newProgramId", 2)                   
                    .Run();

                var inserted = QueryRunner.Select("SELECT * FROM Enrollments WHERE StudentId=6 AND ProgramId=1");
                Assert.AreEqual(0, inserted.Rows.Count);

                inserted = QueryRunner.Select("SELECT * FROM Enrollments WHERE StudentId=6 AND ProgramId=2");
                Assert.AreEqual(1, inserted.Rows.Count);                
            }
        }

        [Test, Order(3)]
        public void Test_MultiUpdate_Proc_Run()
        {
            using (var ctx = Helper.CreateMsSql())
            {
                var query = ctx.Proc("ChangeProgram")
                               .Param("@studentId", 1)
                               .Param("@oldProgramId", 4)
                               .Param("@newProgramId", 5);

                query = query.Merge(ctx.Proc("ChangeProgram")
                                   .Param("@studentId", 5)
                                   .Param("@oldProgramId", 5)
                                   .Param("@newProgramId", 4));

                query.Run();

                var updated = QueryRunner.Select(@"SELECT * FROM Enrollments WHERE (StudentId=1 AND ProgramId=5) OR
                                                                                   (StudentId=5 AND ProgramId=4)");
                Assert.AreEqual(2, updated.Rows.Count);

                updated = QueryRunner.Select(@"SELECT * FROM Enrollments WHERE (StudentId=1 AND ProgramId=4) OR
                                                                               (StudentId=5 AND ProgramId=5)");
                Assert.AreEqual(0, updated.Rows.Count);
            }
        }

        [Test, Order(4)]
        public void Test_Delete_Proc_Run()
        {
            using (var ctx = Helper.CreateMsSql())
            {
                ctx.Proc("UnEnroll")
                    .Param("@studentId", 8)
                    .Param("@programId", 3)
                    .Run();

                var deleted = QueryRunner.Select("SELECT * FROM Enrollments WHERE StudentId=8 AND ProgramId=3");
                Assert.AreEqual(0, deleted.Rows.Count);                
            }
        }

        [Test, Order(4)]
        public void Test_MultiDelete_Proc_Run()
        {
            using (var ctx = Helper.CreateMsSql())
            {
                var query = ctx.Proc("UnEnroll")
                               .Param("@studentId", 6)
                               .Param("@programId", 5);

                query = query.Merge(ctx.Proc("UnEnroll")
                                       .Param("@studentId", 7)
                                       .Param("@programId", 5));

                query.Run();

                var deleted = QueryRunner.Select(@"SELECT * FROM Enrollments WHERE (StudentId=6 AND ProgramId=5) OR
                                                                                   (StudentId=7 AND ProgramId=5)");
                Assert.AreEqual(0, deleted.Rows.Count);                
            }
        }
    }
}
