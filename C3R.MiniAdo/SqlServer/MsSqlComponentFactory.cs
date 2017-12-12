using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace C3R.MiniAdo.SqlServer
{
    /// <summary>
    /// SqlServer implementation of IComponentFactory
    /// </summary>
    class MsSqlComponentFactory : IComponentFactory
    {
        /// <summary>
        /// Create SqlCommand object of given command text and command type
        /// </summary>
        /// <param name="cmdText">Command's text</param>
        /// <param name="cmdType">Commend's type</param>
        /// <returns>SqlCommand object</returns>
        public IDbCommand CreateCommand(string cmdText, CommandType cmdType)
        {
            return new SqlCommand(cmdText)
            {
                CommandType = cmdType
            };
        }

        /// <summary>
        /// Create SqlConnection object with given connectionString
        /// </summary>
        /// <param name="connectionString">Database connection string</param>
        /// <returns>SqlConnection object</returns>
        public IDbConnection CreateConnection(string connectionString)
        {
            return new SqlConnection(connectionString);
        }

        /// <summary>
        /// Create SqlParameter
        /// </summary>
        /// <param name="name">Parameter's name</param>
        /// <param name="value">Parameter's value</param>
        /// <param name="direction">Parameter's direction</param>
        /// <returns>SqlParameter object</returns>
        public IDataParameter CreateParameter(string name, object value, ParameterDirection direction = ParameterDirection.Input)
        {
            return new SqlParameter(name, value) { Direction = direction };
        }

        /// <summary>
        /// Creat SqlParameter
        /// </summary>
        /// <param name="name">Parameter's name<</param>
        /// <param name="value">Parameter's value</param>
        /// <param name="dbType">Parameter's Database Type</param>
        /// <param name="direction">Parameter's direction</param>
        /// <returns>SqlParameter object</returns>
        public IDataParameter CreateParameter(string name, object value, DbType dbType, ParameterDirection direction = ParameterDirection.Input)
        {
            return new SqlParameter(name, MapDbType(dbType))
            {
                SqlValue = value,
                Direction = direction,
            };
        }

        /// <summary>
        /// Create SqlParameter
        /// </summary>
        /// <param name="name">Parameter's name</param>
        /// <param name="value">Parameter's value</param>
        /// <param name="dbType">Parameter's Database Type</param>
        /// <param name="direction">Parameter's direction</param>                
        /// <param name="size">Parameter's data size</param>
        /// <returns>SqlParameter object</returns>
        public IDataParameter CreateParameter(string name, object value, DbType dbType, int size, ParameterDirection direction = ParameterDirection.Input)
        {
            return new SqlParameter(name, MapDbType(dbType), size)
            {
                SqlValue = value,
                Direction = direction
            };
        }

        /// <summary>
        /// Create SqlParameter
        /// </summary>
        /// <param name="name">Parameter's name</param>
        /// <param name="value">Parameter's value</param>
        /// <param name="dbType">Parameter's SQL Server Database Type</param>
        /// <returns>SqlParameter object</returns>
        public IDataParameter CreateParameter(string name, object value, SqlDbType dbType, ParameterDirection direction = ParameterDirection.Input)
        {
            return new SqlParameter(name, dbType)
            {
                SqlValue = value,
                Direction = direction
            };
        }

        /// <summary>
        /// Create SqlParameter
        /// </summary>
        /// <param name="name">Parameter's name</param>
        /// <param name="value">Parameter's value</param>
        /// <param name="dbType">Parameter's Database Type</param>
        /// <param name="direction">Parameter's SQL Server direction</param>                
        /// <param name="size">Parameter's data size</param>
        /// <returns>SqlParameter object</returns>
        public IDataParameter CreateParameter(string name, object value, SqlDbType dbType, int size, ParameterDirection direction = ParameterDirection.Input)
        {
            return new SqlParameter(name, dbType, size)
            {
                SqlValue = value,
                Direction = direction
            };
        }

        /// <summary>
        /// Create Query object for preparing a query to database server
        /// </summary>
        /// <param name="context">DataContext associated with the query</param>
        /// <param name="query">Query statement</param>
        /// <param name="cmdType">Type of command</param>
        /// <returns>Query object</returns>
        /// <remarks>
        /// Command Type should be Text or StoredProcedure. TableDirect is not supported
        /// </remarks>
        public IQuery CreateQuery(DataContext context, string query, CommandType cmdType)
        {
            if (cmdType == CommandType.TableDirect)
                throw new NotSupportedException("TableDirect is not supported at the moment");

            return new Query(context, query, cmdType);
        }

        /// <summary>
        /// Map abstract DbType into SqlServer DbType
        /// </summary>
        /// <param name="dbType">Abstract DbType</param>
        /// <returns>Sql DbType</returns>
        private SqlDbType MapDbType(DbType dbType)
        {
            switch(dbType)
            {
                case DbType.AnsiString: return SqlDbType.VarChar;
                case DbType.AnsiStringFixedLength: return SqlDbType.Char;
                case DbType.Binary: return SqlDbType.VarBinary;
                case DbType.Boolean: return SqlDbType.Bit;
                case DbType.Byte: return SqlDbType.TinyInt;
                case DbType.Currency: return SqlDbType.Decimal;
                case DbType.Date: return SqlDbType.Date;
                case DbType.DateTime: return SqlDbType.DateTime;
                case DbType.DateTime2: return SqlDbType.DateTime2;
                case DbType.DateTimeOffset: return SqlDbType.DateTimeOffset;
                case DbType.Decimal: return SqlDbType.Decimal;                
                case DbType.Double: return SqlDbType.Float;
                case DbType.Guid: return SqlDbType.UniqueIdentifier;
                case DbType.Int16: return SqlDbType.SmallInt;
                case DbType.Int32: return SqlDbType.Int;
                case DbType.Int64: return SqlDbType.BigInt;
                case DbType.Object: return SqlDbType.Variant;
                case DbType.SByte: return SqlDbType.TinyInt;
                case DbType.Single: return SqlDbType.Real;
                case DbType.String: return SqlDbType.NVarChar;
                case DbType.StringFixedLength: return SqlDbType.NChar;
                case DbType.Time: return SqlDbType.Time;
                case DbType.UInt16: return SqlDbType.SmallInt;
                case DbType.UInt32: return SqlDbType.Int;
                case DbType.UInt64: return SqlDbType.BigInt;
                case DbType.VarNumeric: return SqlDbType.Decimal;
                case DbType.Xml: return SqlDbType.Xml;                
                default:
                    throw new NotSupportedException($"{dbType.ToString()} is not supported");
            }
        }
    }
}
