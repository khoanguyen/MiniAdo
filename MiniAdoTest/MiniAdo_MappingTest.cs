using C3R.MiniAdo.Mapping;
using MiniAdoTest.Data;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace MiniAdoTest
{
    [TestFixture]
    public class MiniAdo_MappingTest
    {
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            TestDataManager.MountTestData();

            var provider = Mapper.MapperProvider as DefaultMapperProvider;
            provider.RegisterMapper<Student>(new StudentMapper());
            provider.RegisterMapper<Program>(new ProgramMapper());
            provider.RegisterMapper<Enrollment>(new EnrollmentMapper());
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
        public void TestQueryMapping()
        {
            var query = "SELECT * FROM Students WHERE Status=@status";
            using (var ctx = Helper.CreateMsSql())
            {
                var students = ctx.Query(query)
                               .Param("@status", 1)
                               .RunQuery<Student>()
                               .OrderBy(s => s.StudentId)
                               .ToArray();

                var expected = QueryRunner.Select("SELECT * FROM Students WHERE Status = 1 ORDER bY StudentId");

                Assert.AreEqual(expected.Rows.Count, students.Length);

                for (var i = 0; i < students.Length; i++)
                {
                    var s = students[i];
                    var row = expected.Rows[i];
                    Assert.AreEqual(s.StudentId, row.Field<int>("StudentId"));
                    Assert.AreEqual(s.FirstName, row.Field<string>("FirstName"));
                    Assert.AreEqual(s.LastName, row.Field<string>("LastName"));
                    Assert.AreEqual(s.Email, row.Field<string>("Email"));
                    Assert.AreEqual(s.Status, row.Field<byte>("Status"));
                }
            }
        }

        [Test]
        public void Test_ReadOnly_QueryMapping()
        {
            var query = "SELECT * FROM Students WHERE Status=@status";
            using (var ctx = Helper.CreateMsSqlReadOnly())
            {
                var students = ctx.Query(query)
                               .Param("@status", 1)
                               .RunQueryReadOnly<Student>()
                               .OrderBy(s => s.StudentId)
                               .ToArray();

                var expected = QueryRunner.Select("SELECT * FROM Students WHERE Status = 1 ORDER bY StudentId");

                Assert.AreEqual(expected.Rows.Count, students.Length);

                for (var i = 0; i < students.Length; i++)
                {
                    var s = students[i];
                    var row = expected.Rows[i];
                    Assert.AreEqual(s.StudentId, row.Field<int>("StudentId"));
                    Assert.AreEqual(s.FirstName, row.Field<string>("FirstName"));
                    Assert.AreEqual(s.LastName, row.Field<string>("LastName"));
                    Assert.AreEqual(s.Email, row.Field<string>("Email"));
                    Assert.AreEqual(s.Status, row.Field<byte>("Status"));
                }
            }
        }

        [Test]
        public void Test_ManualMapping()
        {
            var query1 = "SELECT * FROM Students";
            var query2 = "SELECT * FROM Programs";
            var query = $"{query1};{query2}";
            using (var ctx = Helper.CreateMsSql())
            {
                var tables = ctx.Query(query).RunQuery().ToArray();

                var students = Mapper.Map<Student>(tables[0]).OrderBy(s => s.StudentId).ToArray();
                var programs = Mapper.Map<Program>(tables[1]).OrderBy(p => p.ProgramId).ToArray();

                var studentsTable = QueryRunner.Select(query1);
                var programsTable = QueryRunner.Select(query2);

                Assert.AreEqual(studentsTable.Rows.Count, students.Length);

                for (var i = 0; i < students.Length; i++)
                {
                    var s = students[i];
                    var row = studentsTable.Rows[i];
                    Assert.AreEqual(s.StudentId, row.Field<int>("StudentId"));
                    Assert.AreEqual(s.FirstName, row.Field<string>("FirstName"));
                    Assert.AreEqual(s.LastName, row.Field<string>("LastName"));
                    Assert.AreEqual(s.Email, row.Field<string>("Email"));
                    Assert.AreEqual(s.Status, row.Field<byte>("Status"));
                }

                Assert.AreEqual(programsTable.Rows.Count, programs.Length);

                for (var i = 0; i < programs.Length; i++)
                {
                    var p = programs[i];
                    var row = programsTable.Rows[i];
                    Assert.AreEqual(p.ProgramId, row.Field<int>("ProgramId"));
                    Assert.AreEqual(p.ProgramName, row.Field<string>("ProgramName"));
                    Assert.AreEqual(p.ProgramCode, row.Field<string>("ProgramCode"));
                    Assert.AreEqual(p.Year, row.Field<string>("Year"));
                    Assert.AreEqual(p.Status, row.Field<byte>("Status"));
                }
            }
        }
    }
}
