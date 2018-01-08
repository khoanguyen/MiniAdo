using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace MiniAdoTest.Data
{
    public static class QueryRunner
    {
        public static DataTable Select(string query)
        {
            using (var conn = new SqlConnection(TestDataManager.TestDataConnStr))
            {
                var result = new DataTable();

                var cmd = conn.CreateCommand();
                cmd.CommandText = query;

                var adapter = new SqlDataAdapter(cmd);
                adapter.Fill(result);

                return result;
            }
        }

        public static object Scalar(string query)
        {
            using (var conn = new SqlConnection(TestDataManager.TestDataConnStr))
            {
                try
                {
                    conn.Open();
                                        
                    var cmd = conn.CreateCommand();
                    cmd.CommandText = query;

                    return cmd.ExecuteScalar();
                }
                finally
                {
                    if (conn.State != ConnectionState.Closed) conn.Close();
                }
                
            }
        }
    }
}
