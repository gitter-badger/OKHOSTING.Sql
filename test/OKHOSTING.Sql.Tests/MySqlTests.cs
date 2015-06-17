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
			return new MySql.DataBase();
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
			Table table = new Table("prueba");
			table.Columns.Add(new Column() { Name = "Id", DbType = System.Data.DbType.Int32, IsPrimaryKey = true, IsAutoNumber = true });
			table.Columns.Add(new Column() { Name = "Texto", DbType = System.Data.DbType.AnsiString, Length = 100, IsNullable = false });
			table.Columns.Add(new Column() { Name = "Numero", DbType = System.Data.DbType.Int32, IsNullable = false });
			table.Indexes.Add(new Index() { Name = "IX_Texto", Unique = true, Table = table });
			table.Indexes[0].Columns.Add(table["Texto"]);

			//create
			var sql = generator.Create(table);
			db.Execute(sql);
			Assert.True(db.TableExists(table.Name));

			//add index
			sql = generator.Create(table.Indexes[0]);
			db.Execute(sql);

			//insert
			Insert insert = new Insert();
			insert.Into = table;
			insert.Values.Add(new ColumnValue(table["id"], 1));
			insert.Values.Add(new ColumnValue(table["texto"], "prueba1"));
			insert.Values.Add(new ColumnValue(table["numero"], 100));

			sql = generator.Insert(insert);
			int affectedRows = db.Execute(sql);
			Assert.AreEqual(affectedRows, 1);

			//select
			Select select = new Select();
			select.From = table;
			select.Columns.Add(table["id"]);
			select.Columns.Add(table["texto"]);
			select.Where.Add(new ValueCompareFilter(table["texto"], "prueba1"));

			sql = generator.Select(select);
			var result = db.GetDataTable(sql);
			Assert.AreEqual(result.Rows.Count, 1);

			//delete
			Delete delete = new Delete();
			delete.From = table;
			delete.Where.Add(new ValueCompareFilter(table["texto"], "prueba1"));

			sql = generator.Delete(delete);
			affectedRows = db.Execute(sql);
			Assert.AreEqual(affectedRows, 1);

			//drop
			sql = generator.Drop(table);
			db.Execute(sql);
			Assert.False(db.TableExists(table.Name));
		}
	}
}