using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace MiniAdoTest
{
    public static class AssertEx
    {
        public static void AreEqual(DataTable expected, DataTable actual)
        {
            if (expected == null && actual == null)
            {
                Assert.Pass();
                return;
            }

            if (expected != actual && (expected == null || actual == null))
            {
                Assert.Fail("DataTables are not the same");
            }

            /// Assert columns
            Assert.AreEqual(expected.Columns.Count, actual.Columns.Count, "ColumnsCount not equal");
            for (var i = 0; i < expected.Columns.Count; i++)
            {
                Assert.AreEqual(expected.Columns[i].ColumnName, actual.Columns[i].ColumnName, "Column Name not equal");
                Assert.AreEqual(expected.Columns[i].DataType, actual.Columns[i].DataType, "Column datatype not equal");
            }

            /// Assert rows        
            Assert.AreEqual(expected.Rows.Count, actual.Rows.Count, "RowCount not equal");
            for (var i = 0; i < expected.Rows.Count; i++)
            {
                for (var j = 0; j < expected.Columns.Count; j++)
                {
                    var col = expected.Columns[j];
                    Assert.AreEqual(expected.Rows[i][col.ColumnName], actual.Rows[i][col.ColumnName]);
                }
            }
        }
    }
}
