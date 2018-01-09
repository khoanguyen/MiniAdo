using C3R.MiniAdo;
using MiniAdoTest.Data;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;

namespace MiniAdoTest
{
    [TestFixture]
    public class MiniAdo_TransactionScopeTest
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
        public void Test_TransactionScope_Commit_SimpleQuery()
        {
            using (var ts = new TransactionScope())
            {
                using (var ctx = Helper.CreateMsSql())
                {

                    var query = ctx.Query("INSERT INTO Students VALUES(@id, @firstName, @lastName, @email, @status)")
                    .Param("@id", 99)
                    .Param("@firstName", "Chewbacca")
                    .Param("@lastName", "Wookiee")
                    .Param("@email", "c.wookiee@millenium.com")
                    .Param("@status", 0)
                    .Run();

                    ts.Complete();
                }
            }

            var table = QueryRunner.Select("SELECT * FROM Students WHERE StudentId=99");
            Assert.AreEqual(1, table.Rows.Count);
        }

        [Test]
        public void Test_TransactionScope_Rollback_SimpleQuery()
        {
            using (var ts = new TransactionScope())
            {
                using (var ctx = Helper.CreateMsSql())
                {

                    var query = ctx.Query("INSERT INTO Students VALUES(@id, @firstName, @lastName, @email, @status)")
                    .Param("@id", 98)
                    .Param("@firstName", "Chewbacca")
                    .Param("@lastName", "Wookiee")
                    .Param("@email", "c.wookiee@millenium.com")
                    .Param("@status", 0)
                    .Run();
                }
            }

            var table = QueryRunner.Select("SELECT * FROM Students WHERE StudentId=98");
            Assert.AreEqual(0, table.Rows.Count);
        }

        [Test]
        public void Test_TransactionScope_Commit_MultiInsert()
        {
            var insertedData = new object[]
            {
                new { StudentId=91, FirstName="Jaina", LastName="Solo", Email = "jaina.solo@hans.com", Status = 0},
                new { StudentId=92, FirstName="Jace", LastName="Solo", Email = "jace@sith.com", Status = 1},
                new { StudentId=93, FirstName="Ben", LastName="Skywalker", Email = "", Status = 2},
            };

            var queryText = "INSERT INTO Students VALUES(@id, @firstName, @lastName, @email, @status)";
            using (var ts = new TransactionScope())
            {
                using (var ctx = Helper.CreateMsSql())
                {

                    IQuery query = ctx.Query("");

                    foreach (dynamic data in insertedData)
                    {
                        ctx.Query(queryText)
                            .Param("@id", data.StudentId)
                            .Param("@firstName", data.FirstName)
                            .Param("@lastName", data.LastName)
                            .Param("@email", data.Email)
                            .Param("@status", data.Status)
                            .Run();
                    }

                    ts.Complete();

                }
            }

            var table = QueryRunner.Select("SELECT * FROM Students WHERE StudentId>=91 AND StudentId<=93");
            Assert.AreEqual(3, table.Rows.Count);
        }

        [Test]
        public void Test_TransactionScope_Rollback_MultiInsert()
        {
            var insertedData = new object[]
            {
                new { StudentId=61, FirstName="Jaina", LastName="Solo", Email = "jaina.solo@hans.com", Status = 0},
                new { StudentId=62, FirstName="Jace", LastName="Solo", Email = "jace@sith.com", Status = 1},
                new { StudentId=63, FirstName="Ben", LastName="Skywalker", Email = "", Status = 2},
            };

            var queryText = "INSERT INTO Students VALUES(@id, @firstName, @lastName, @email, @status)";

            using (var ts = new TransactionScope())
            {
                using (var ctx = Helper.CreateMsSql())
                {

                    IQuery query = ctx.Query("");

                    foreach (dynamic data in insertedData)
                    {
                        ctx.Query(queryText)
                           .Param("@id", data.StudentId)
                           .Param("@firstName", data.FirstName)
                           .Param("@lastName", data.LastName)
                           .Param("@email", data.Email)
                           .Param("@status", data.Status)
                           .Run();
                    }                    
                }
            }

            var table = QueryRunner.Select("SELECT * FROM Students WHERE StudentId>=61 AND StudentId<=63");
            Assert.AreEqual(0, table.Rows.Count);
        }

        [Test]
        public void Test_TransactionScope_Commit_MultiInsert_WithMerge()
        {
            var insertedData = new object[]
            {
                new { StudentId=81, FirstName="Jaina", LastName="Solo", Email = "jaina.solo@hans.com", Status = 0},
                new { StudentId=82, FirstName="Jace", LastName="Solo", Email = "jace@sith.com", Status = 1},
                new { StudentId=83, FirstName="Ben", LastName="Skywalker", Email = "", Status = 2},
            };

            var queryText = "INSERT INTO Students VALUES(@id, @firstName, @lastName, @email, @status)";

            using (var ts = new TransactionScope())
            {
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
                    ts.Complete();

                }
            }

            var table = QueryRunner.Select("SELECT * FROM Students WHERE StudentId>=81 AND StudentId<=83");
            Assert.AreEqual(3, table.Rows.Count);
        }

        [Test]
        public void Test_TransactionScope_Rollback_MultiInsert_WithMerge()
        {
            var insertedData = new object[]
            {
                new { StudentId=71, FirstName="Jaina", LastName="Solo", Email = "jaina.solo@hans.com", Status = 0},
                new { StudentId=72, FirstName="Jace", LastName="Solo", Email = "jace@sith.com", Status = 1},
                new { StudentId=73, FirstName="Ben", LastName="Skywalker", Email = "", Status = 2},
            };

            var queryText = "INSERT INTO Students VALUES(@id, @firstName, @lastName, @email, @status)";

            using (var ts = new TransactionScope())
            {
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
                }
            }

            var table = QueryRunner.Select("SELECT * FROM Students WHERE StudentId>=71 AND StudentId<=73");
            Assert.AreEqual(0, table.Rows.Count);
        }
    }
}