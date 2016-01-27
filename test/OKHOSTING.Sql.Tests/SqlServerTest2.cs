using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OKHOSTING.Sql.Operations;
using OKHOSTING.Sql.Schema;
using OKHOSTING.Sql.Filters;

namespace OKHOSTING.Sql.Tests
{
	[TestClass]
	public class SqlServerTest2
	{
		public DataBase Connect()
		{
			return new Net4.SqlServer.DataBase() { ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["sqlserver"].ConnectionString };
		}

		/// <summary>
		/// Create Table Person and use insert
		/// </summary>
		[TestMethod]
		public void InserInTable()
		{
			//open connect to database
			DataBase db = Connect();
			var generator = new OKHOSTING.Sql.Net4.SqlServer.SqlGenerator();

			//define table person
			Table table = new Table("test2");
			table.Columns.Add(new Column() { Name = "Id", DbType = DbType.Int32, IsPrimaryKey = true, IsAutoNumber = true, Table = table });
			table.Columns.Add(new Column() { Name = "Name", DbType = DbType.AnsiString, Length = 100, IsNullable = false, Table = table });
			table.Columns.Add(new Column() { Name = "Age", DbType = DbType.Int32, IsNullable = false, Table = table });

			//create table person
			var sql = generator.Create(table);
			db.Execute(sql);
			Assert.IsTrue(db.ExistsTable(table.Name));

			//insert values into person
			Insert insert = new Insert();
			insert.Table = table;
			insert.Values.Add(new ColumnValue(table["Name"], "Angel"));
			insert.Values.Add(new ColumnValue(table["Age"], 25));

			sql = generator.Insert(insert);
			int affectedRows = db.Execute(sql);
			Assert.AreEqual(affectedRows, 1);
		}

		[TestMethod]
		public void dropRow()
		{
			//Open connect to database;
			DataBase db = Connect();
			var generator = new OKHOSTING.Sql.Net4.SqlServer.SqlGenerator();

			//define table customer
			Table table = new Table("test4");

			table.Columns.Add(new Column() { Name = "Id", DbType = DbType.Int32, IsPrimaryKey = true, IsAutoNumber = true, Table = table });
			table.Columns.Add(new Column() { Name = "Company", DbType = DbType.AnsiString, Length = 200, IsNullable = false, Table = table });
			table.Columns.Add(new Column() { Name = "Address", DbType = DbType.AnsiString, Length = 500, IsNullable = false, Table = table });
			table.Columns.Add(new Column() { Name = "Email", DbType = DbType.AnsiString, Length = 100, IsNullable = false, Table = table });
			table.Columns.Add(new Column() { Name = "Telephone", DbType = DbType.AnsiString, Length = 20, IsNullable = false, Table = table });
			table.Indexes.Add(new Index() { Name = "IX_Company", Unique = true, Table = table });
			table.Indexes[0].Columns.Add(table["Company"]);

			//create table customer
			var sql = generator.Create(table);
			db.Execute(sql);
			Assert.IsTrue(db.ExistsTable(table.Name));

			//add index
			sql = generator.Create(table.Indexes[0]);
			db.Execute(sql);

			//insert values into test3
			Insert insert = new Insert();
			insert.Table = table;
			insert.Values.Add(new ColumnValue(table["Company"], "Monsters Inc. Corporate"));
			insert.Values.Add(new ColumnValue(table["Address"], "First Street #12 Blv. Flowers San Diego. C.A."));
			insert.Values.Add(new ColumnValue(table["Email"], "mic_admind@info.com"));
			insert.Values.Add(new ColumnValue(table["Telephone"], "0122389456278"));

			sql = generator.Insert(insert);
			int affectedRows = db.Execute(sql);
			Assert.AreEqual(affectedRows, 1);

			//insert values into test3
			insert = new Insert();
			insert.Table = table;
			insert.Values.Add(new ColumnValue(table["Company"], "Baby tunes SA de CV"));
			insert.Values.Add(new ColumnValue(table["Address"], "Paid Call #202 Blv. Saint, James Tucson. Arizona"));
			insert.Values.Add(new ColumnValue(table["Email"], "info@babyyunes.com"));
			insert.Values.Add(new ColumnValue(table["Telephone"], "012235656178"));

			sql = generator.Insert(insert);
			affectedRows = db.Execute(sql);
			Assert.AreEqual(affectedRows, 1);

			//delete row from customer.company = Monsters Inc. Corporate
			Delete delete = new Delete();
			delete.Table = table;
			delete.Where.Add(new ValueCompareFilter() { Column = table["Company"], ValueToCompare = "Monsters Inc. Corporate", Operator = Data.CompareOperator.Equal });

			sql = generator.Delete(delete);
			affectedRows = db.Execute(sql);
			Assert.AreEqual(affectedRows, 1);
		}
	}

}