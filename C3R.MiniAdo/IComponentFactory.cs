using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace C3R.MiniAdo
{
    /// <summary>
    /// Component factory provide data access object for being used database server
    /// </summary>
    public interface IComponentFactory
    {
        /// <summary>
        /// Create DbCommand object of given command text and command type
        /// </summary>
        /// <param name="cmdText">Command's text</param>
        /// <param name="cmdType">Commend's type</param>
        /// <returns>DbCommand object</returns>
        IDbCommand CreateCommand(string cmdText, CommandType cmdType);

        /// <summary>
        /// Create DataParameter object
        /// </summary>
        /// <param name="name">Parameter's name</param>
        /// <param name="value">Parameter's value</param>
        /// <param name="direction">Parameter's direction</param>
        /// <returns>DataParameter object</returns>
        IDataParameter CreateParameter(string name, object value, ParameterDirection direction = ParameterDirection.Input);

        /// <summary>
        /// Creat DataParameter object
        /// </summary>
        /// <param name="name">Parameter's name<</param>
        /// <param name="value">Parameter's value</param>
        /// <param name="dbType">Parameter's Database Type</param>
        /// <param name="direction">Parameter's direction</param>
        /// <returns>DataParameter object</returns>
        IDataParameter CreateParameter(string name, object value, DbType dbType, ParameterDirection direction = ParameterDirection.Input);

        /// <summary>
        /// Create DataParameter object
        /// </summary>
        /// <param name="name">Parameter's name</param>
        /// <param name="value">Parameter's value</param>
        /// <param name="dbType">Parameter's Database Type</param>
        /// <param name="direction">Parameter's direction</param>                
        /// <param name="size">Parameter's data size</param>
        /// <returns>DataParameter object</returns>
        IDataParameter CreateParameter(string name, object value, DbType dbType, int size, ParameterDirection direction = ParameterDirection.Input);

        /// <summary>
        /// Create DbConnection object with given connectionString
        /// </summary>
        /// <param name="connectionString">Database connection string</param>
        /// <returns>DbConnection object</returns>
        IDbConnection CreateConnection(string connectionString);

        /// <summary>
        /// Create Query object for preparing a query to database server
        /// </summary>
        /// <param name="context">DataContext associated with the query</param>
        /// <param name="query">Query statement</param>
        /// <param name="cmdType">Type of command</param>
        /// <returns>Query object</returns>        
        IQuery CreateQuery(DataContext context, string query, CommandType cmdType);
    }
}
