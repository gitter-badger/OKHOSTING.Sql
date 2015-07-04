using System;
using NUnit.Framework;
using OKHOSTING.Sql.Operations;
using System.Collections.Generic;
using OKHOSTING.Sql.Schema;
using OKHOSTING.Sql.Filters;

namespace OKHOSTING.Sql.Tests
{
	[TestFixture]
	public class MySqlTests
	{
		public DataBase Connect()
		{
			return new MySql.DataBase(System.Configuration.ConfigurationManager.ConnectionStrings["mysql"].ConnectionString);
		}

		[Test]
		public void LoadSchema()
		{
			DataBase db = Connect();
			var schema = db.Schema;

			Assert.IsNotNull(schema);
		}

		[Test]
		public void CreateTable()
		{
			DataBase db = Connect();
			MySql.SqlGenerator generator = new MySql.SqlGenerator();

			//define table schema
			Table table = new Table("test1");
			table.Columns.Add(new Column() { Name = "Id", DbType = System.Data.DbType.Int32, IsPrimaryKey = true, IsAutoNumber = true, Table = table });
			table.Columns.Add(new Column() { Name = "TextField", DbType = System.Data.DbType.AnsiString, Length = 100, IsNullable = false, Table = table });
			table.Columns.Add(new Column() { Name = "NumberField", DbType = System.Data.DbType.Int32, IsNullable = false, Table = table });
			table.Indexes.Add(new Index() { Name = "IX_TextField", Unique = true, Table = table });
			table.Indexes[0].Columns.Add(table["TextField"]);

			//create
			var sql = generator.Create(table);
			db.Execute(sql);
			Assert.True(db.ExistsTable(table.Name));

			//add index
			sql = generator.Create(table.Indexes[0]);
			db.Execute(sql);

			//insert
			Insert insert = new Insert();
			insert.Into = table;
			insert.Values.Add(new ColumnValue(table["Id"], 1));
			insert.Values.Add(new ColumnValue(table["TextField"], "test11"));
			insert.Values.Add(new ColumnValue(table["NumberField"], 100));

			sql = generator.Insert(insert);
			int affectedRows = db.Execute(sql);
			Assert.AreEqual(affectedRows, 1);

			//select
			Select select = new Select();
			select.From = table;
			select.Columns.Add(table["id"]);
			select.Columns.Add(table["TextField"]);
			select.Where.Add(new ValueCompareFilter(table["TextField"], "test11"));

			sql = generator.Select(select);
			var result = db.GetDataTable(sql);
			Assert.AreEqual(result.Rows.Count, 1);

			//delete
			Delete delete = new Delete();
			delete.From = table;
			delete.Where.Add(new ValueCompareFilter(table["TextField"], "test11"));

			sql = generator.Delete(delete);
			affectedRows = db.Execute(sql);
			Assert.AreEqual(affectedRows, 1);

			//drop
			sql = generator.Drop(table);
			db.Execute(sql);
			Assert.False(db.ExistsTable(table.Name));
		}
	}
}