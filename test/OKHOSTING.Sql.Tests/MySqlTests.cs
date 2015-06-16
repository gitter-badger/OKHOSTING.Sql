using System;
using NUnit.Framework;
using OKHOSTING.Sql.Operations;
using System.Collections.Generic;
using OKHOSTING.Sql.Schema;

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
		public void RunInsert()
		{
			DataBase db = Connect();
			var schema = db.Schema;
			MySql.SqlGenerator generator = new MySql.SqlGenerator();
			Insert insert = new Insert();
			Table table = schema["centrocostos"];

			var values = new List<ColumnValue>();
			values.Add(new ColumnValue(table["oid"], Guid.NewGuid().ToString()));
			values.Add(new ColumnValue(table["nombre"], "prueba1"));

			insert.Into = table;
			insert.Values = values;

			var sql = generator.Insert(insert);

			db.Execute(sql);
		}
	}
}