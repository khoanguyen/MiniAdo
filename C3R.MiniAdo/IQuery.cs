using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C3R.MiniAdo
{
    /// <summary>
    /// Query object provides preparation and executes for data query to database server
    /// </summary>
    public interface IQuery
    {
        /// <summary>
        /// Command type of the query
        /// </summary>
        CommandType CommandType { get; }

        /// <summary>
        /// Query statement of current query
        /// </summary>
        string QueryText { get; }

        /// <summary>
        /// Add given DataParameter to query object
        /// </summary>
        /// <param name="parameter">DataParameter object</param>        
        IQuery Param(IDataParameter parameter);
        
        /// <summary>
        /// Add a DataParemeter to query object
        /// </summary>
        /// <param name="name">Parameter's name</param>
        /// <param name="value">Parameter's value</param>
        /// <param name="direction">Parameter's direction</param>
        /// <returns></returns>
        IQuery Param(string name, object value, ParameterDirection direction = ParameterDirection.Input);

        /// <summary>
        /// Add a DataParemeter to query object
        /// </summary>
        /// <param name="name">Parameter's name</param>
        /// <param name="value">Parameter's value</param>
        /// <param name="dbType">Parameter's DbType</param>
        /// <param name="direction">Parameter's direction</param>
        /// <returns></returns>
        IQuery Param(string name, object value, DbType dbType, ParameterDirection direction = ParameterDirection.Input);

        /// <summary>
        /// Add a DataParemeter to query object
        /// </summary>
        /// <param name="name">Parameter's name</param>
        /// <param name="value">Parameter's value</param>
        /// <param name="dbType">Parameter's DbType</param>
        /// <param name="size">Parameter's DbType size</param>
        /// <param name="direction">Parameter's direction</param>
        /// <returns></returns>
        IQuery Param(string name, object value, DbType dbType, int size, ParameterDirection direction = ParameterDirection.Input);

        /// <summary>
        /// Get DataParameter with given name
        /// </summary>
        /// <param name="name">Parameter's name</param>
        /// <returns>DataParameter object associated with current Query</returns>
        /// <exception cref="System.EntryPointNotFoundException">
        /// DataParameter with given name does not exist in the current Query object
        /// </exception>
        IDataParameter GetParam(string name);

        /// <summary>
        /// Get all Parameter added into query
        /// </summary>
        /// <returns></returns>
        IEnumerable<IDataParameter> GetParams();

        /// <summary>
        /// Execute query against Database server and retrieve back resultsets. 
        /// Each resultset will result into 1 DataTable.
        /// </summary>
        /// <returns>Collection of DataTable objects</returns>
        IEnumerable<DataTable> RunQuery();

        /// <summary>
        /// Execute Read-only query against Database server and retrieve back resultsets. 
        /// Each resultset will result into 1 DataTable.
        /// </summary>
        /// <returns>Collection of DataTable objects</returns>
        IEnumerable<DataTable> RunQueryReadOnly();

        /// <summary>
        /// Execute query against Database server, retrieve back resultsets and map result into collection of given generic type. 
        /// Each resultset will result into 1 DataTable.
        /// </summary>
        /// <typeparam name="T">Type of result objects</typeparam>
        /// <returns>Collection of mapped objects</returns>
        IEnumerable<T> RunQuery<T>();

        /// <summary>
        /// Execute Read-only query against Database server, retrieve back resultsets and map result into collection of given generic type. 
        /// Each resultset will result into 1 DataTable.
        /// </summary>
        /// <typeparam name="T">Type of result objects</typeparam>
        /// <returns>Collection of mapped objects</returns>
        IEnumerable<T> RunQueryReadOnly<T>();

        /// <summary>
        /// Execute scalar query against database server
        /// </summary>
        /// <returns>Scalar result</returns>
        object RunScalar();

        /// <summary>
        /// Execute Read-only scalar query against database server
        /// </summary>
        /// <returns>Scalar result</returns>
        object RunScalarReadOnly();

        /// <summary>
        /// Execute scalar query against database server and convert result to given generic type
        /// </summary>
        /// <typeparam name="T">Type of result</typeparam>
        /// <returns>Scalar result of given generic type</returns>
        T RunScalar<T>();

        /// <summary>
        /// Execute Read-only scalar query against database server and convert result to given generic type
        /// </summary>
        /// <typeparam name="T">Type of result</typeparam>
        /// <returns>Scalar result of given generic type</returns>
        T RunScalarReadOnly<T>();

        /// <summary>
        /// Execute non-query query against database server
        /// </summary>
        /// <returns>Integer value returned by server</returns>
        int Run();

        /// <summary>
        /// Execute Read-only non-query query against database server
        /// </summary>
        /// <returns>Integer value returned by server</returns>
        int RunReadOnly();

        /// <summary>
        /// Append given query into the end of current query
        /// Stored Procedure calls are not supported. Use Merge method for Stored Procedures
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [Obsolete]
        IQuery AppendQuery(string query);

        /// <summary>
        /// Merge with given query
        /// </summary>
        /// <param name="query">query to merge with</param>
        /// <returns>Merged query</returns>
        IQuery Merge(IQuery query);
    }

}
