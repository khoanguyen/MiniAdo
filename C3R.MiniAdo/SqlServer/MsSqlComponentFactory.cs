using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace C3R.MiniAdo.SqlServer
{
    class MsSqlComponentFactory : IComponentFactory
    {
        public IDbCommand CreateCommand(string cmdText, CommandType cmdType)
        {
            return new SqlCommand(cmdText)
            {
                CommandType = cmdType
            };
        }

        public IDbConnection CreateConnection(string connectionString)
        {
            return new SqlConnection(connectionString);
        }

        public IDataParameter CreateParameter(string name, object value)
        {
            return new SqlParameter(name, value);
        }

        public IDataParameter CreateParameter(string name, object value, DbType dbType)
        {
            return new SqlParameter(name, MapDbType(dbType))
            {
                SqlValue = value
            };
        }

        public IDataParameter CreateParameter(string name, object value, DbType dbType, int size)
        {
            return new SqlParameter(name, MapDbType(dbType), size)
            {
                SqlValue = value
            };
        }

        public IDataParameter CreateParameter(string name, object value, SqlDbType dbType)
        {
            return new SqlParameter(name, dbType)
            {
                SqlValue = value
            };
        }

        public IDataParameter CreateParameter(string name, object value, SqlDbType dbType, int size)
        {
            return new SqlParameter(name, dbType, size)
            {
                SqlValue = value
            };
        }

        public IQuery CreateQuery(DataContext context, string query, CommandType cmdType)
        {
            return new Query(context, query, cmdType);
        }

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
