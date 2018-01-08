using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MiniAdoTest
{
    internal static class TestDataManager
    {
        public static bool HasLocalSqlServer { get; private set; } = false;

        private static string _masterConnStr = "";
        public static string TestDataConnStr { get; private set; } = "";

        private static string _originFile = "MiniAdoTestData.mdf";
        private static string _originLogFile = "MiniAdoTestData_log.ldf";
        private static string _cloneFile = "MiniAdoTestData_Clone.mdf";
        private static string _cloneLogFile = "MiniAdoTestData_Clone_log.ldf";
        private static string _cloneDBName = "MiniAdoTestData_Clone";
        private static string _folder = "";

        private static object _gate = new object();

        static TestDataManager()
        {
            _folder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            _originFile = Path.Combine(_folder, "Data", _originFile);
            _originLogFile = Path.Combine(_folder, "Data", _originLogFile);
            _cloneFile = Path.Combine(_folder, "Data", _cloneFile);
            _cloneLogFile = Path.Combine(_folder, "Data", _cloneLogFile);

            _masterConnStr = ConfigurationManager.ConnectionStrings["SqlServerMaster"].ConnectionString ?? "";
            TestDataConnStr = ConfigurationManager.ConnectionStrings["TestData"].ConnectionString ?? "";

            using (var conn = new SqlConnection(_masterConnStr))
            {
                try
                {
                    conn.Open();
                    HasLocalSqlServer = true;
                }
                catch (Exception)
                {
                    // Do nothing    
                }
                finally
                {
                    if (conn.State == System.Data.ConnectionState.Open) conn.Close();
                }
            }
        }

        public static void MountTestData()
        {
            lock (_gate)
            {
                try
                {
                    DropClone();
                }
                catch (Exception)
                {
                    // Bypass error, do nothing
                }

                if (File.Exists(_cloneFile)) File.Delete(_cloneFile);
                if (File.Exists(_cloneLogFile)) File.Delete(_cloneLogFile);

                File.Copy(_originFile, _cloneFile);
                File.Copy(_originLogFile, _cloneLogFile);

                AttachClone();
            }
        }

        public static void UnmountTestData()
        {
            lock (_gate)
            {
                DropClone();
            }
        }

        private static void AttachClone()
        {
            using (var conn = new SqlConnection(_masterConnStr))
            {
                var serverConnection = new ServerConnection(conn);
                var server = new Server(serverConnection);

                server.AttachDatabase(_cloneDBName, new System.Collections.Specialized.StringCollection
                {
                    _cloneFile,
                    _cloneLogFile
                });
            }
        }

        private static void DropClone()
        {
            using (var conn = new SqlConnection(_masterConnStr))
            {
                var serverConnection = new ServerConnection(conn);
                var server = new Server(serverConnection);

                var database = server.Databases[_cloneDBName];

                if (database != null)
                {
                    server.KillDatabase(_cloneDBName);
                                        
                    if (File.Exists(_cloneFile)) File.Delete(_cloneFile);
                    if (File.Exists(_cloneLogFile)) File.Delete(_cloneLogFile);
                }
            }
        }
    }
}
