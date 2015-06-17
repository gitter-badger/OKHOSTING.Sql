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
			return new MySql.DataBase("server=plesk11.okhosting.com;database=kaltire;user id=kaltire;password=Js%73H8)6Sd45JwE3hkL#9(5mGw!");
		}

		[Test]
		public void LoadSchema()
		{
			DataBase db = Connect();
			var schema = db.Schema;

			Assert.IsNotNull(schema);
		}

		[Test]
		public void InsertSelectUpdate()
		{
			DataBase db = Connect();
			var schema = db.Schema;
			MySql.SqlGenerator generator = new MySql.SqlGenerator();
			
			Insert insert = new Insert();
			Table table = schema["centrocostos"];

			insert.Into = table;
			insert.Values.Add(new ColumnValue(table["oid"], Guid.NewGuid().ToString()));
			insert.Values.Add(new ColumnValue(table["nombre"], "prueba1"));

			var sql = generator.Insert(insert);

			int affectedRows = db.Execute(sql);

			Assert.AreEqual(affectedRows, 1);

			Select select = new Select();
			select.From = table;
			select.Columns.Add(table["oid"]);
			select.Columns.Add(table["nombre"]);
			select.Where.Add(new ValueCompareFilter(table["nombre"], "prueba1"));

			sql = generator.Select(select);

			var result = db.GetDataTable(sql);

			Assert.AreEqual(result.Rows.Count, 1);

			Delete delete = new Delete();
			delete.From = table;
			delete.Where.Add(new ValueCompareFilter(table["nombre"], "prueba1"));

			sql = generator.Delete(delete);

			affectedRows = db.Execute(sql);

			Assert.AreEqual(affectedRows, 1);
		}

		public void CreateTable()
		{
			DataBase db = Connect();
			MySql.SqlGenerator generator = new MySql.SqlGenerator();

			Table table = new Table("pruebas");

			table.Columns.Add(new Column() { Name = "Id", DbType = System.Data.DbType.Int32, IsPrimaryKey = true, IsAutoNumber = true });
			table.Columns.Add(new Column() { Name = "Nombre", DbType = System.Data.DbType.AnsiString, Length = 100, IsNullable = false });
			table.Indexes.Add(new Index() { Name = "IX_Nombre", Unique = true, Table = table });
			table.Indexes[0].Columns.Add(table["Nombre"]);

			var sql = generator.Create(table);
			db.Execute(sql);

			sql = generator.Create(table.Indexes[0]);
			db.Execute(sql);

			sql = generator.Drop(table);
			db.Execute(sql);
		}

	}
}