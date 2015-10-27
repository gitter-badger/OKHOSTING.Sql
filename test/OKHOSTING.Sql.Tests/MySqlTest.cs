using System;
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
        public void InnerJoinTest()
        {
            DataBase db = Connect();
            var generator = new OKHOSTING.Sql.MySql.SqlGenerator();

            // define table schema
            Table customer = new Table("customer");
            customer.Columns.Add(new Column() { Name = "Id", DbType = DbType.Int32, IsPrimaryKey = true, IsAutoNumber = true, Table = customer });
            customer.Columns.Add(new Column() { Name = "Name", DbType = DbType.AnsiString, Length = 100, IsNullable = false, Table = customer });
            customer.Columns.Add(new Column() { Name = "Country", DbType = DbType.Int32, IsNullable = false, Table = customer });
            
            Table country = new Table("country");
            country.Columns.Add(new Column() { Name = "Id", DbType = DbType.Int32, IsPrimaryKey = true, IsAutoNumber = true, Table = country });
            country.Columns.Add(new Column() { Name = "Name", DbType = DbType.AnsiString, Length = 100, IsNullable = false, Table = country });

            ForeignKey countryFK = new ForeignKey();
            countryFK.Table = customer;
            countryFK.RemoteTable = country;
            countryFK.Name = "FK_customer_country";
            countryFK.Columns.Add(new Tuple<Column, Column>(customer["Country"], country["id"]));
            countryFK.DeleteAction = countryFK.UpdateAction = ConstraintAction.Restrict;

            var sql = generator.Create(customer);
            db.Execute(sql);

            sql = generator.Create(country);
            db.Execute(sql);

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

            SelectJoin join = new SelectJoin();
            join.Table = country;
            join.On.Add(new ColumnCompareFilter() { Column = customer["country"], ColumnToCompare = country["id"], Operator = Data.CompareOperator.Equal });
            join.Columns.Add(new SelectColumn(country["name"], "countryName"));
            join.JoinType = SelectJoinType.Inner;

            select.Joins.Add(join);

            sql = generator.Select(select);
            var result = db.GetDataTable(sql);

            foreach (IDataRow row in result)
            {
                foreach (object obj in row)
                {
                    Console.Write(obj);
                }
            }

            Assert.AreEqual(result.Count, 1);
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

            foreach (IDataRow row in result)
            {
                foreach (object obj in row)
                {
                    Console.Write(obj);
                }
            }

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

        [TestMethod]
        public void InserInTable()
        {
            DataBase db = Connect();
            var generator = new OKHOSTING.Sql.MySql.SqlGenerator();

            //define table schema
            Table table = new Table("Person");

            table.Columns.Add(new Column() { Name = "Id", DbType = DbType.Int32, IsPrimaryKey = true, IsAutoNumber = true, Table = table });
            table.Columns.Add(new Column() { Name = "Name", DbType = DbType.AnsiString, Length = 100, IsNullable = false, Table = table });
            table.Columns.Add(new Column() { Name = "Age", DbType = DbType.Int32, IsNullable = false, Table = table });
            

            //create
            var sql = generator.Create(table);
            db.Execute(sql);
            Assert.IsTrue(db.ExistsTable(table.Name));

            //insert
            Insert insert = new Insert();
            insert.Table = table;
            insert.Values.Add(new ColumnValue(table["Id"], 1));
            insert.Values.Add(new ColumnValue(table["Name"], "Angel"));
            insert.Values.Add(new ColumnValue(table["Age"], 25));

            sql = generator.Insert(insert);
            int affectedRows = db.Execute(sql);
            Assert.AreEqual(affectedRows, 1);
        }

        [TestMethod]
        public void dropRow()
        {
            DataBase db = Connect();
            var generator = new OKHOSTING.Sql.MySql.SqlGenerator();

            //define table schema
            Table table = new Table("Customer");

            table.Columns.Add(new Column() { Name = "Id", DbType = DbType.Int32, IsPrimaryKey = true, IsAutoNumber = true, Table = table });
            table.Columns.Add(new Column() { Name = "Company", DbType = DbType.AnsiString, Length = 100, IsNullable = false, Table = table });
            table.Columns.Add(new Column() { Name = "Address", DbType = DbType.AnsiString, Length = 500, IsNullable = false, Table = table });
            table.Columns.Add(new Column() { Name = "Email", DbType = DbType.AnsiString, Length = 50, IsNullable = false, Table = table });
            table.Columns.Add(new Column() { Name = "Telephone", DbType = DbType.AnsiString, Length = 12, IsNullable = false, Table = table });

            //create
            var sql = generator.Create(table);
            db.Execute(sql);
            Assert.IsTrue(db.ExistsTable(table.Name));

            //insert
            Insert insert = new Insert();
            insert.Table = table;
            insert.Values.Add(new ColumnValue(table["Id"], 1));
            insert.Values.Add(new ColumnValue(table["Company"], "Software Create Inc."));
            insert.Values.Add(new ColumnValue(table["Address"], "San Angel #123-A Col. Metropolis Mexico. D.F."));
            insert.Values.Add(new ColumnValue(table["Email"], "softcreate@admind.mx"));
            insert.Values.Add(new ColumnValue(table["Telephone"], "013318592634"));

            sql = generator.Insert(insert);
            int affectedRows = db.Execute(sql);
            Assert.AreEqual(affectedRows, 1);

            insert = new Insert();
            insert.Table = table;
            insert.Values.Add(new ColumnValue(table["Id"], 2));
            insert.Values.Add(new ColumnValue(table["Company"], "Monsters Inc. Corporate"));
            insert.Values.Add(new ColumnValue(table["Address"], "First Street #12 Blv. Flowers San Diego. C.A."));
            insert.Values.Add(new ColumnValue(table["Email"], "mic_admind@info.com"));
            insert.Values.Add(new ColumnValue(table["Telephone"], "0122389456278"));

            sql = generator.Insert(insert);
            affectedRows = db.Execute(sql);
            Assert.AreEqual(affectedRows, 1);

            //delete
            Delete delete = new Delete();
            delete.Table = table;
            delete.Where.Add(new ValueCompareFilter() { Column = table["Company"], ValueToCompare = "Monsters Inc. Corporate", Operator = Data.CompareOperator.Equal });

            sql = generator.Delete(delete);
            affectedRows = db.Execute(sql);
            Assert.AreEqual(affectedRows, 1);
        }

        [TestMethod]
        public void selectPerson()
        {
            DataBase db = Connect();
            var generator = new OKHOSTING.Sql.MySql.SqlGenerator();

            //define table schema
            Table table = new Table("Person");

            table.Columns.Add(new Column() { Name = "Id", DbType = DbType.Int32, IsPrimaryKey = true, IsAutoNumber = true, Table = table });
            table.Columns.Add(new Column() { Name = "Name", DbType = DbType.AnsiString, Length = 100, IsNullable = false, Table = table });
            table.Columns.Add(new Column() { Name = "Age", DbType = DbType.Int32, IsNullable = false, Table = table });


            //create
            var sql = generator.Create(table);
            db.Execute(sql);
            Assert.IsTrue(db.ExistsTable(table.Name));

            //insert
            Insert insert = new Insert();
            insert.Table = table;
            insert.Values.Add(new ColumnValue(table["Id"], 1));
            insert.Values.Add(new ColumnValue(table["Name"], "Angel"));
            insert.Values.Add(new ColumnValue(table["Age"], 25));

            sql = generator.Insert(insert);
            int affectedRows = db.Execute(sql);
            Assert.AreEqual(affectedRows, 1);

            //select
            Select select = new Select();
            select.Table = table;
            select.Columns.Add(table["Id"]);
            select.Columns.Add(table["Name"]);
            select.Columns.Add(table["Age"]);
            select.Where.Add(new ValueCompareFilter() { Column = table["Name"], ValueToCompare = "Angel", Operator = Data.CompareOperator.Equal });

            sql = generator.Select(select);
            var result = db.GetDataTable(sql);

            foreach (IDataRow row in result)
            {
                foreach (object obj in row)
                {
                    Console.Write(obj);
                }
            }

            Assert.AreEqual(result.Count, 1);
        }

        [TestMethod]
        public void DropTable()
        {
            DataBase db = Connect();
            var generator = new OKHOSTING.Sql.MySql.SqlGenerator();

            //define table schema
            Table table = new Table("Song");

            table.Columns.Add(new Column() { Name = "Id", DbType = DbType.Int32, IsPrimaryKey = true, IsAutoNumber = true, Table = table });
            table.Columns.Add(new Column() { Name = "Name", DbType = DbType.AnsiString, Length = 100, IsNullable = false, Table = table });
            table.Columns.Add(new Column() { Name = "Sing", DbType = DbType.AnsiString, Length = 120, IsNullable = false, Table = table });


            //create
            var sql = generator.Create(table);
            db.Execute(sql);
            Assert.IsTrue(db.ExistsTable(table.Name));

            //insert
            Insert insert = new Insert();
            insert.Table = table;
            insert.Values.Add(new ColumnValue(table["Id"], 1));
            insert.Values.Add(new ColumnValue(table["Name"], "More than words"));
            insert.Values.Add(new ColumnValue(table["Sing"], "Extreme"));

            sql = generator.Insert(insert);
            int affectedRows = db.Execute(sql);
            Assert.AreEqual(affectedRows, 1);

            //drop
            sql = generator.Drop(table);
            db.Execute(sql);
            Assert.IsFalse(db.ExistsTable(table.Name));
        }
    }


}