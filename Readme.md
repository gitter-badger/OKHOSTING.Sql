Data acces library with a focus on portability, SQL generation and schema management.

# Features for Windows, Linux, Android and iOS via PCL

* Manage your database schema, create or drop tables, columns, indexes, and foreign keys
* Generate and execute Insert, Select, Delete, Update, Create & Drop operations, without writing SQL code yourself
* Select operation is capable of using joins, aggregation, group by, sort, limit, pretty much everything
* Work with MySql 
* Work with SQLite

# Features for Windows via Net4

* Work with SqlServer
* Work with Postgres
* Standarized data access for Entity Framework and OrmLite (for compatibility with projects that use them)

# Example

```csharp
var db = new Net4.MySql.DataBase() { ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["mysql"].ConnectionString };

var generator = new OKHOSTING.Sql.MySql.SqlGenerator();

// create customer table
Table customer = new Table("customer");
customer.Columns.Add(new Column() { Name = "Id", DbType = DbType.Int32, IsPrimaryKey = true, IsAutoNumber = true, Table = customer });
customer.Columns.Add(new Column() { Name = "Name", DbType = DbType.AnsiString, Length = 100, IsNullable = false, Table = customer });
customer.Columns.Add(new Column() { Name = "Country", DbType = DbType.Int32, IsNullable = false, Table = customer });
			
// create country table
Table country = new Table("country");
country.Columns.Add(new Column() { Name = "Id", DbType = DbType.Int32, IsPrimaryKey = true, IsAutoNumber = true, Table = country });
country.Columns.Add(new Column() { Name = "Name", DbType = DbType.AnsiString, Length = 100, IsNullable = false, Table = country });

// create a foreign key
ForeignKey countryFK = new ForeignKey();
countryFK.Table = customer;
countryFK.RemoteTable = country;
countryFK.Name = "FK_customer_country";
countryFK.Columns.Add(new Tuple<Column, Column>(customer["Country"], country["id"]));
countryFK.DeleteAction = countryFK.UpdateAction = ConstraintAction.Restrict;

// get SQL code for creating customer table
var sql = generator.Create(customer);

// execute that code and actually create table in DB
db.Execute(sql);

// same for country
sql = generator.Create(country);
db.Execute(sql);

// same for foreigk key
sql = generator.Create(countryFK);
db.Execute(sql);

//insert
Insert insert2 = new Insert();
insert2.Table = country;
insert2.Values.Add(new ColumnValue(country["Id"], 1));
insert2.Values.Add(new ColumnValue(country["Name"], "Mexico"));

sql = generator.Insert(insert2);
int affectedRows2 = db.Execute(sql);
Assert.AreEqual(affectedRows2, 1);

Insert insert = new Insert();
insert.Table = customer;
insert.Values.Add(new ColumnValue(customer["Id"], 1));
insert.Values.Add(new ColumnValue(customer["Name"], "Angel"));
insert.Values.Add(new ColumnValue(customer["Country"], 1));

sql = generator.Insert(insert);
int affectedRows = db.Execute(sql);
Assert.AreEqual(affectedRows, 1);

//select
Select select = new Select();
select.Table = customer;
select.Columns.Add(new SelectColumn(customer["id"]));
select.Columns.Add(new SelectColumn(customer["Name"]));

// create a join between customer and country
SelectJoin join = new SelectJoin();
join.Table = country;
join.On.Add(new ColumnCompareFilter() { Column = customer["country"], ColumnToCompare = country["id"], Operator = Data.CompareOperator.Equal });
join.Columns.Add(new SelectColumn(country["name"], "countryName"));
join.JoinType = SelectJoinType.Inner;

select.Joins.Add(join);

sql = generator.Select(select);
var result = db.GetDataTable(sql);

// get results
foreach (IDataRow row in result)
{
	foreach (object obj in row)
	{
		Console.Write(obj);
	}
}

Assert.AreEqual(result.Count, 1);
```

# Reading a DB schema

```csharp
//this way you can just read the existing tables from DB and then perform Insert, Select, Update or Delete operations
//on the tables without the need to manually creating schema like in the previous sample
var schema = db.Schema;
vat customerTable = schema["cuctomer"];
```