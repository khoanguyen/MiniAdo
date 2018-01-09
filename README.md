# MiniAdo

A .NET lightweight library for accessing database

## DataContext

DataContext abstract class is the primary class for providing components and configurations

- Creating query
- Provide Database Settings
- Start/End transaction

DataContext is **NOT** a UnitOfWork pattern

This library does not focusing on implementing UnitOfWork.

## SQL Server
The implementation for Sql Server is MsSqlDataContext.

````
using (var context = new MsSqlDataContext(connStr)) 
{
    ...
}
````

## Read/Write Splitting 

For providing support of Read/Write splitting, DataContext class allowa setting a Read-Only connection string so that
when running RunQueryReadOnly or RunScalarReadOnly, Read-Only connection string will be used for accessing Read-Only server

Simpler code of accessing Read/Write splitting databases 

```
using (var context = new MsSqlDataContext(writeConnString, readOnlyConnStr)) 
{    
    /// Access write server
    var tables = ctx.Query("SELECT * FROM Students; SELECT * FROM Programs")
                    .RunQuery().ToArray();
    
    /// Access read server    
    tables = ctx.Query("SELECT * FROM Students; SELECT * FROM Programs")
                    .RunQueryReadOnly().ToArray();    
}
```

## Usage

There are 3 types of call:
- Query: returns a DataTable which contains one or more rows and columns. E.g : SELECT * FROM Students
- Scalar: returns a single value. E.g : SELECT COUNT(*) FROM Students
- Non-query: this one return nothing, some database servers may return a status code indicating result of the the operation. These are usually INSERT/UPDATE/DELETE query.

### Query

```
using (var context = new MsSqlDataContext(connStr)) 
{    
    var table1 = ctx.Query("SELECT * FROM Students").RunQuery().First();



    /// With parameters
    var table2 = ctx.Query("SELECT * FROM Students WHERE Status=@status AND Email<>@email")
                    .Param("@status", 1)
                    .Param("@email", null)
                    .RunQuery().First();
}
```

Read-Only access

```
using (var context = new MsSqlDataContext(connStr, readOnlyConnStr)) 
{
    var table1 = ctx.Query("SELECT * FROM Students").RunQueryReadOnly().First();



    /// With parameters
    var table2 = ctx.Query("SELECT * FROM Students WHERE Status=@status AND Email<>@email")
                    .Param("@status", 1)
                    .Param("@email", null)
                    .RunQueryReadOnly().First();    
}
```

<i style="color:blue">
Non-Generic RunQuery and RunQueryReadOnly return a collection of DataTable objects 
for supporting multiple result sets returned from query
</i>

### Multiple Queries
There are many way to perform multiple queries with single call to database

**Multiple statements in querytext**

```
using (var context = new MsSqlDataContext(connStr)) 
{    
    var tables = ctx.Query("SELECT * FROM Students; SELECT * FROM Programs")
                    .RunQuery().ToArray();
    var students = tables[0]; // First query result table
    var programs = tables[1]; // Second query result table



    /// With parameters
    tables = ctx.Query(@"SELECT * FROM Students WHERE Status=@status; 
                         SELECT * FROM Programs WHERE code=@progCode")
                    .Param("@status", 1)
                    .Param("@progCode", 'BCOSC')
                    .RunQuery();
    students = tables[0]; // First query result table
    programs = tables[1]; // Second query result table
}
```
Read-Only access

```
using (var context = new MsSqlDataContext(connStr, readOnlyConnStr)) 
{    
    var tables = ctx.Query("SELECT * FROM Students; SELECT * FROM Programs")
                    .RunQueryReadOnly().ToArray();
    var students = tables[0]; // First query result table
    var programs = tables[1]; // Second query result table



    /// With parameters
    tables = ctx.Query(@"SELECT * FROM Students WHERE Status=@status; 
                             SELECT * FROM Programs WHERE code=@progCode")
                    .Param("@status", 1)
                    .Param("@progCode", 'BCOSC')
                    .RunQueryReadOnly();
    students = tables[0]; // First query result table
    programs = tables[1]; // Second query result table
}
```

**Using AppendQuery**

```
using (var ctx = new MsSqlDataContext(connStr)
{
    var query1 = "SELECT * FROM Students";
    var query2 = "SELECT * FROM Programs";

    var tables = ctx.Query(query1)
                    .AppendQuery(query2)
                    .RunQuery().ToArray();



    /// With parameters
    query1 = "SELECT * FROM Students WHERE Status=@status";
    query2 = "SELECT * FROM Programs WHERE ProgramName=@progName";
    
    var tables = ctx.Query(query1)
                    .AppendQuery(query2)
                    .Param("@status", 1)
                    .Param("@progName", "Bachelor of Computer Science")
                    .RunQuery().ToArray();
}
```
Read-Only access

```
using (var context = new MsSqlDataContext(connStr, readOnlyConnStr)) 
{    
    var query1 = "SELECT * FROM Students";
    var query2 = "SELECT * FROM Programs";

    var tables = ctx.Query(query1)
                    .AppendQuery(query2)
                    .RunQueryReadOnly().ToArray();



    /// With parameters
    query1 = "SELECT * FROM Students WHERE Status=@status";
    query2 = "SELECT * FROM Programs WHERE ProgramName=@progName";
    
    var tables = ctx.Query(query1)
                    .AppendQuery(query2)
                    .Param("@status", 1)
                    .Param("@progName", "Bachelor of Computer Science")
                    .RunQueryReadOnly().ToArray();
}
```

**Using Merge**

```
using (var ctx = new MsSqlDataContext(connStr)
{
    var query1 = "SELECT * FROM Students";
    var query2 = "SELECT * FROM Programs";

    var query1 = ctx.Query(queryText1);
    var query2 = ctx.Query(queryText2);

    var query = query1.Merge(query2);

    var tables = query.RunQuery().ToArray();



    /// With parameters
    queryText1 = "SELECT * FROM Students WHERE Status=@status AND Email=@email";
    queryText2 = "SELECT * FROM Programs WHERE ProgramName=@progName";

    var query1 = ctx.Query(queryText1)
                    .Param("@status", 1)
                    .Param("@email", null);
    var query2 = ctx.Query(queryText2)
                    .Param(new SqlParameter("@progName", SqlDbType.NVarChar, 1000)
                    {
                        SqlValue = "Bachelor of Computer Science"
                    });


    var query = query1.Merge(query2);

    var tables = query.RunQuery().ToArray();
}
```
Read-Only access

```
using (var context = new MsSqlDataContext(connStr, readOnlyConnStr)) 
{    
    var query1 = "SELECT * FROM Students";
    var query2 = "SELECT * FROM Programs";

    var query1 = ctx.Query(queryText1);
    var query2 = ctx.Query(queryText2);

    var query = query1.Merge(query2);

    var tables = query.RunQueryReadOnly().ToArray();

    

    /// With parameters
    queryText1 = "SELECT * FROM Students WHERE Status=@status AND Email=@email";
    queryText2 = "SELECT * FROM Programs WHERE ProgramName=@progName";

    var query1 = ctx.Query(queryText1)
                    .Param("@status", 1)
                    .Param("@email", null);
    var query2 = ctx.Query(queryText2)
                    .Param(new SqlParameter("@progName", SqlDbType.NVarChar, 1000)
                    {
                        SqlValue = "Bachelor of Computer Science"
                    });


    var query = query1.Merge(query2);

    var tables = query.RunQueryReadOnly().ToArray();
}
```

### Scalar

MiniAdo does not support multiple queries for Scalar, neither AppendQuery nor Merge.

In case multiple queries, RunScalar/RunScalarReadOnly only returns 1 value, it is up to Database server.

```
using (var context = new MsSqlDataContext(connStr)) 
{    
    var objCount = ctx.Query("SELECT COUNT(*) FROM Students").RunScalar();
    var intCount = ctx.Query("SELECT COUNT(*) FROM Students").RunScalar<int>();

    // With parameters
    objCount = ctx.Query("SELECT * FROM Students WHERE Status=@status")
                  .Param("@status", 1)
                  .RunScalar();
    intCount = ctx.Query("SELECT * FROM Students WHERE Status=@status")
                  .Param("@status", 1)
                  .RunScalar<int>();
}
```

Read-Only access

```
using (var context = new MsSqlDataContext(connStr, readOnlyConnStr)) 
{    
    var objCount = ctx.Query("SELECT COUNT(*) FROM Students").RunScalarReadOnly();
    var intCount = ctx.Query("SELECT COUNT(*) FROM Students").RunScalarReadOnly<int>();

    // With parameters
    objCount = ctx.Query("SELECT * FROM Students WHERE Status=@status")
                  .Param("@status", 1)
                  .RunScalarReadOnly();
    intCount = ctx.Query("SELECT * FROM Students WHERE Status=@status")
                  .Param("@status", 1)
                  .RunScalarReadOnly<int>();
}
```

### Non-Query

```
using (var ctx = new MsSqlDataContext(connStr))
{
    var queryText = "UPDATE Students SET Email=@email, Status=@status WHERE StudentId=@studentId";
    
    var query = ctx.Query(queryText)
        .Param("@email", "random.guy@yahoo.com")
        .Param("@status", 3)
        .Param("@studentId", 4);

    query.Run();
}
```


Multiple-Queries feature (Append, Merge) works with Non-Query 

```
var insertedData = new object[]
{
    new { StudentId=11, FirstName="Jaina", LastName="Solo", Email = "jaina.solo@hans.com", Status = 0},
    new { StudentId=12, FirstName="Jace", LastName="Solo", Email = "jace@sith.com", Status = 1},
    new { StudentId=13, FirstName="Ben", LastName="Skywalker", Email = "", Status = 2},
};
            
var queryText = "INSERT INTO Students VALUES(@id, @firstName, @lastName, @email, @status)";

using (var ctx = new MsSqlDataContext(connStr))
{
    IQuery query = ctx.Query("");
                
    foreach(dynamic data in insertedData)
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
```

### Stored Procedure

Use **DataContext.Proc** method to create a StoredProcedure call

RunQueryReadOnly and RunScalarReadOnly is also supported with Stored Procedure call

```
/// Query
var procName = "GetAllStudents";
using (var ctx = new MsSqlDataContext(connStr))
{
    var table = ctx.Proc(procName).RunQuery().First();    
}
```

```
/// Scalar
var procName = "[CountStudentsByProgram]";
using (var ctx = new MsSqlDataContext(connStr))
{
    var objCount = ctx.Proc(procName).Param("@programId", 2).RunScalar();
    var intCount = ctx.Proc(procName).Param("@programId", 2).RunScalar<int>();
}
```

```
/// Non-Query
using (var ctx = new MsSqlDataContext(connStr))
{
    ctx.Proc("ChangeProgram")
        .Param("@studentId", 6)
        .Param("@oldProgramId", 1)
        .Param("@newProgramId", 2)                   
        .Run();        
}
```

Multiple Procedure call with Merge.

AppendQuery is **not recommended**

```
using (var ctx = new MsSqlDataContext(connStr))
{
    var query = ctx.Proc("Enroll")
                    .Param("@studentId", 3)
                    .Param("@programId", 5);

    query = query.Merge(ctx.Proc("Enroll")
                    .Param("@studentId", 8)
                    .Param("@programId", 1));

    query.Run();             
}
```

## Transaction

User can choose using DataContext's methods or TransactionScope (distributed transaction)

DataContext provides 3 methods for manipulating the Transaction
 - StartTransaction
 - Commit
 - Rollback

Using MiniAdo's Transaction methods

```
using (var ctx = Helper.CreateMsSql())
{
    using (var ts = ctx.StartTransaction(System.Data.IsolationLevel.ReadUncommitted))
    {
        try
        {
            var query = ctx.Query("INSERT INTO Students VALUES(@id, @firstName, @lastName, @email, @status)")
            .Param("@id", 99)
            .Param("@firstName", "Chewbacca")
            .Param("@lastName", "Wookiee")
            .Param("@email", "c.wookiee@millenium.com")
            .Param("@status", 0)
            .Run();

            ctx.Commit();

            var table = QueryRunner.Select("SELECT * FROM Students WHERE StudentId=99");
            Assert.AreEqual(1, table.Rows.Count);

        }
        catch (Exception ex)
        {
            ctx.Rollback();
            Assert.Fail(ex.ToString());
        }
    }
}
```

Using TransactionScope if there is possibility of distributed transaction

```
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
```

## Mapping

MiniAdo provides mapping ability through **MiniAdo.Mapping** namespace.

- **Mapper**: static util class for accessing Mapping infrastructure
- **IMapper**: interface for implementing a mapper for specific data type
- **IMapperProvider** : interface for implementing mapper provider which resolves data mapper for data type

Query.RunQuery&lt;T&gt; and Query.RunQueryReadOnly&lt;T&gt; get data from database, 
then they use Mapper class for mapping DataTable into collection instance of type T

**DefaultMapperProvider** class is default implementation comes along with the library

With DefaultMapperProvider, in order to enable mapping for specific data type, we follow these steps

- Create a data mapper for given type
- Register it with DefaultMapperProvider (usually at the startup of the application)

```
public class Student
{
    public int StudentId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public byte Status { get; set; }
}

/// Create data mapper for Student class
public class StudentMapper : IMapper<Student>
{
    public Student Map(DataRow row)
    {
        return new Student
        {
            StudentId = row.Field<int>("StudentId"),
            FirstName = row.Field<string>("FirstName"),
            LastName = row.Field<string>("LastName"),
            Email = row.Field<string>("Email"),
            Status = row.Field<byte>("Status")
        };
    }

    public void Populate(DataRow row, Student target)
    {
        target.StudentId = row.Field<int>("StudentId");
        target.FirstName = row.Field<string>("FirstName");
        target.LastName = row.Field<string>("LastName");
        target.Email = row.Field<string>("Email");
        target.Status = row.Field<byte>("Status");
    }

    public void PopulateRow(Student entity, DataRow target)
    {
        target.SetField<int>("StudentId", entity.StudentId);
        target.SetField<string>("FirstName", entity.FirstName);
        target.SetField<string>("LastName", entity.LastName);
        target.SetField<string>("Email", entity.Email);
        target.SetField<byte>("Status", entity.Status);            
    }
}

...
    /// Register it with DefaultMapperProvider
    var provider = Mapper.MapperProvider as DefaultMapperProvider;
    provider.RegisterMapper<Student>(new StudentMapper());
...
```

<i style="color:red">
Generic methods RunQuery&lt;T&gt; and RunQuery&lt;T&gt; do not support multiple queries execution.
They only return mapping of the first table of the result.
</i>

However, Mapper class provides Mapper.Map&lt;T&gt;(DataTable table) method so that you can map the whole DataTable into collection of type T.
You can combine it with RunQuery as following


```
using (var context = new MsSqlDataContext(connStr)) 
{    
    var tables = ctx.Query("SELECT * FROM Students; SELECT * FROM Programs")
                    .RunQuery().ToArray();
    var students = Mapper.Map<Student>(tables[0]); // First query result table
    var programs = Mapper.Map<Program>(tables[1]); // Second query result table

}
```

### Using your own MapperProvider

You can replace DefaultMapperProvider with your own Mapper, by setting Mapper.MapperProvider static property

```
public class MyMapperProvider : IMapperProvider
{
    ...
}

Mapper.MapperProvider = new MyMapperProvider();
```
