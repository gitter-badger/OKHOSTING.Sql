﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OKHOSTING.Sql.Operations;
using OKHOSTING.Sql.Schema;
using OKHOSTING.Sql.Filters;

namespace OKHOSTING.Sql.Tests
{
	[TestClass]
	public class MySqlTest
	{
		public DataBase Connect()
		{
			return new Net4.MySql.DataBase() { ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["mysql"].ConnectionString };
		}

		[TestMethod]
		public void LoadSchema()
		{
			DataBase db = Connect();
			var schema = db.Schema;

			Assert.IsNotNull(schema);
		}

		[TestMethod]
		public void CreateTable()
		{
			DataBase db = Connect();
			var generator = new OKHOSTING.Sql.MySql.SqlGenerator();

			//define table schema
			Table table = new Table("test1");
			table.Columns.Add(new Column() { Name = "Id", DbType = DbType.Int32, IsPrimaryKey = true, IsAutoNumber = true, Table = table });
			table.Columns.Add(new Column() { Name = "TextField", DbType = DbType.AnsiString, Length = 100, IsNullable = false, Table = table });
			table.Columns.Add(new Column() { Name = "NumberField", DbType = DbType.Int32, IsNullable = false, Table = table });
			table.Indexes.Add(new Index() { Name = "IX_TextField", Unique = true, Table = table });
			table.Indexes[0].Columns.Add(table["TextField"]);

			//create
			var sql = generator.Create(table);
			db.Execute(sql);
			Assert.IsTrue(db.ExistsTable(table.Name));

			//add index
			sql = generator.Create(table.Indexes[0]);
			db.Execute(sql);

			//insert
			Insert insert = new Insert();
			insert.Table = table;
			insert.Values.Add(new ColumnValue(table["Id"], 1));
			insert.Values.Add(new ColumnValue(table["TextField"], "test11"));
			insert.Values.Add(new ColumnValue(table["NumberField"], 100));

			sql = generator.Insert(insert);
			int affectedRows = db.Execute(sql);
			Assert.AreEqual(affectedRows, 1);

			//select
			Select select = new Select();
			select.Table = table;
			select.Columns.Add(table["id"]);
			select.Columns.Add(table["TextField"]);
			select.Where.Add(new ValueCompareFilter(){ Column = table["TextField"], ValueToCompare = "test11", Operator = Data.CompareOperator.Equal });

			sql = generator.Select(select);
			var result = db.GetDataTable(sql);
			Assert.AreEqual(result.Count, 1);

			//delete
			Delete delete = new Delete();
			delete.Table = table;
			delete.Where.Add(new ValueCompareFilter() { Column = table["TextField"], ValueToCompare = "test11", Operator = Data.CompareOperator.Equal });

			sql = generator.Delete(delete);
			affectedRows = db.Execute(sql);
			Assert.AreEqual(affectedRows, 1);

			//drop
			sql = generator.Drop(table);
			db.Execute(sql);
			Assert.IsFalse(db.ExistsTable(table.Name));
		}
	}
}