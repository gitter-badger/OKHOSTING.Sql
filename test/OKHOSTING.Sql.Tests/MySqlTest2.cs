using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OKHOSTING.Sql.Operations;
using OKHOSTING.Sql.Schema;
using OKHOSTING.Sql.Filters;

namespace OKHOSTING.Sql.Tests
{
    [TestClass]
    class MySqlTest2
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


        [TestMethod]
        public void selectLike()
        {
            DataBase db = Connect();
            var generator = new OKHOSTING.Sql.MySql.SqlGenerator();

            //define table schema
            Table table = new Table("Address");

            table.Columns.Add(new Column() { Name = "Id", DbType = DbType.Int32, IsPrimaryKey = true, IsAutoNumber = true, Table = table });
            table.Columns.Add(new Column() { Name = "Street", DbType = DbType.AnsiString, Length = 100, IsNullable = false, Table = table });
            table.Columns.Add(new Column() { Name = "Number", DbType = DbType.AnsiString, Length = 100, IsNullable = false, Table = table });
            table.Columns.Add(new Column() { Name = "Suburb", DbType = DbType.AnsiString, Length = 100, IsNullable = false, Table = table });
            table.Columns.Add(new Column() { Name = "State", DbType = DbType.AnsiString, Length = 100, IsNullable = false, Table = table });
            table.Columns.Add(new Column() { Name = "Country", DbType = DbType.AnsiString, Length = 100, IsNullable = false, Table = table });

            //create
            var sql = generator.Create(table);
            db.Execute(sql);
            Assert.IsTrue(db.ExistsTable(table.Name));

            //insert
            Insert insert = new Insert();
            insert.Table = table;
            insert.Values.Add(new ColumnValue(table["Id"], 1));
            insert.Values.Add(new ColumnValue(table["Street"], "First Avenue"));
            insert.Values.Add(new ColumnValue(table["Number"], "12-B"));
            insert.Values.Add(new ColumnValue(table["Suburb"], "The Roses"));
            insert.Values.Add(new ColumnValue(table["State"], "California"));
            insert.Values.Add(new ColumnValue(table["Country"], "United States"));

            sql = generator.Insert(insert);
            int affectedRows = db.Execute(sql);
            Assert.AreEqual(affectedRows, 1);

            insert = new Insert();
            insert.Table = table;
            insert.Values.Add(new ColumnValue(table["Id"], 2));
            insert.Values.Add(new ColumnValue(table["Street"], "Second Life"));
            insert.Values.Add(new ColumnValue(table["Number"], "1120"));
            insert.Values.Add(new ColumnValue(table["Suburb"], "Park Avenue"));
            insert.Values.Add(new ColumnValue(table["State"], "New York"));
            insert.Values.Add(new ColumnValue(table["Country"], "United States"));

            sql = generator.Insert(insert);
            affectedRows = db.Execute(sql);
            Assert.AreEqual(affectedRows, 1);

            //select
            Select select = new Select();
            select.Table = table;
            select.Columns.Add(table["Id"]);
            select.Columns.Add(table["Street"]);
            select.Columns.Add(table["Suburb"]);
            select.Where.Add(new ValueCompareFilter() { Column = table["Street"], ValueToCompare = "Second", Operator = Data.CompareOperator.Like });

            sql = generator.Select(select);
            var result = db.GetDataTable(sql);

            foreach (IDataRow row in result)
            {
                foreach (object obj in row)
                {
                    Console.Write(obj);
                }
            }

        }

        [TestMethod]
        public void MultiJoinTest()
        {
            DataBase db = Connect();
            var generator = new OKHOSTING.Sql.MySql.SqlGenerator();

            // define table schema
            Table customer = new Table("customer");
            customer.Columns.Add(new Column() { Name = "Id", DbType = DbType.Int32, IsPrimaryKey = true, IsAutoNumber = true, Table = customer });
            customer.Columns.Add(new Column() { Name = "Name", DbType = DbType.AnsiString, Length = 100, IsNullable = false, Table = customer });


            Table product = new Table("product");
            product.Columns.Add(new Column() { Name = "Id", DbType = DbType.Int32, IsPrimaryKey = true, IsAutoNumber = true, Table = product });
            product.Columns.Add(new Column() { Name = "Name", DbType = DbType.AnsiString, Length = 100, IsNullable = false, Table = product });
            product.Columns.Add(new Column() { Name = "Price", DbType = DbType.Decimal, IsNullable = false, Table = product });

            // define table schema
            Table tax = new Table("tax");
            tax.Columns.Add(new Column() { Name = "Id", DbType = DbType.Int32, IsPrimaryKey = true, IsAutoNumber = true, Table = tax });
            tax.Columns.Add(new Column() { Name = "Name", DbType = DbType.AnsiString, Length = 100, IsNullable = false, Table = tax });
            tax.Columns.Add(new Column() { Name = "Rate", DbType = DbType.Decimal, IsNullable = false, Table = tax });

            Table sale = new Table("sale");
            sale.Columns.Add(new Column() { Name = "Id", DbType = DbType.Int32, IsPrimaryKey = true, IsAutoNumber = true, Table = sale });
            sale.Columns.Add(new Column() { Name = "Customer", DbType = DbType.Int32, IsNullable = false, Table = sale });
            sale.Columns.Add(new Column() { Name = "Product", DbType = DbType.Int32, IsNullable = false, Table = sale });
            sale.Columns.Add(new Column() { Name = "Tax", DbType = DbType.Int32, IsNullable = false, Table = sale });

            ForeignKey customerFK = new ForeignKey();
            customerFK.Table = sale;
            customerFK.RemoteTable = customer;
            customerFK.Name = "FK_sale_customer";
            customerFK.Columns.Add(new Tuple<Column, Column>(sale["Customer"], customer["Id"]));
            customerFK.DeleteAction = customerFK.UpdateAction = ConstraintAction.Restrict;

            ForeignKey productFK = new ForeignKey();
            productFK.Table = sale;
            productFK.RemoteTable = product;
            productFK.Name = "FK_sale_product";
            productFK.Columns.Add(new Tuple<Column, Column>(sale["Product"], product["Id"]));
            productFK.DeleteAction = productFK.UpdateAction = ConstraintAction.Restrict;

            ForeignKey taxFK = new ForeignKey();
            taxFK.Table = sale;
            taxFK.RemoteTable = tax;
            taxFK.Name = "FK_sale_tax";
            taxFK.Columns.Add(new Tuple<Column, Column>(sale["Tax"], tax["Id"]));
            taxFK.DeleteAction = taxFK.UpdateAction = ConstraintAction.Restrict;

            //Create
            var sql = generator.Create(customer);
            db.Execute(sql);

            sql = generator.Create(product);
            db.Execute(sql);

            sql = generator.Create(tax);
            db.Execute(sql);

            sql = generator.Create(sale);
            db.Execute(sql);

            sql = generator.Create(customerFK);
            db.Execute(sql);

            sql = generator.Create(productFK);
            db.Execute(sql);

            sql = generator.Create(taxFK);
            db.Execute(sql);


            //insert into customer
            Insert insert = new Insert();
            insert.Table = customer;
            insert.Values.Add(new ColumnValue(customer["Id"], 1));
            insert.Values.Add(new ColumnValue(customer["Name"], "Joyas Loyane SA de CV"));

            sql = generator.Insert(insert);
            int affectedRows = db.Execute(sql);
            Assert.AreEqual(affectedRows, 1);

            insert = new Insert();
            insert.Table = customer;
            insert.Values.Add(new ColumnValue(customer["Id"], 2));
            insert.Values.Add(new ColumnValue(customer["Name"], "Cartie Joyerias SC de CV"));

            sql = generator.Insert(insert);
            affectedRows = db.Execute(sql);
            Assert.AreEqual(affectedRows, 1);

            insert = new Insert();
            insert.Table = product;
            insert.Values.Add(new ColumnValue(product["Id"], 1));
            insert.Values.Add(new ColumnValue(product["Name"], "Hosting"));
            insert.Values.Add(new ColumnValue(product["Price"], 125.50));

            sql = generator.Insert(insert);
            affectedRows = db.Execute(sql);
            Assert.AreEqual(affectedRows, 1);

            insert = new Insert();
            insert.Table = product;
            insert.Values.Add(new ColumnValue(product["Id"], 2));
            insert.Values.Add(new ColumnValue(product["Name"], "Web Page"));
            insert.Values.Add(new ColumnValue(product["Price"], 1300.75));

            sql = generator.Insert(insert);
            affectedRows = db.Execute(sql);
            Assert.AreEqual(affectedRows, 1);

            insert = new Insert();
            insert.Table = tax;
            insert.Values.Add(new ColumnValue(tax["Id"], 1));
            insert.Values.Add(new ColumnValue(tax["Name"], "IVA"));
            insert.Values.Add(new ColumnValue(tax["Rate"], 16.00));

            sql = generator.Insert(insert);
            affectedRows = db.Execute(sql);
            Assert.AreEqual(affectedRows, 1);

            insert = new Insert();
            insert.Table = sale;
            insert.Values.Add(new ColumnValue(sale["Id"], 1));
            insert.Values.Add(new ColumnValue(sale["Customer"], 1));
            insert.Values.Add(new ColumnValue(sale["Product"], 1));
            insert.Values.Add(new ColumnValue(sale["Tax"], 1));

            sql = generator.Insert(insert);
            affectedRows = db.Execute(sql);
            Assert.AreEqual(affectedRows, 1);

            insert = new Insert();
            insert.Table = sale;
            insert.Values.Add(new ColumnValue(sale["Id"], 2));
            insert.Values.Add(new ColumnValue(sale["Customer"], 2));
            insert.Values.Add(new ColumnValue(sale["Product"], 2));
            insert.Values.Add(new ColumnValue(sale["Tax"], 1));

            sql = generator.Insert(insert);
            affectedRows = db.Execute(sql);
            Assert.AreEqual(affectedRows, 1);

            /*
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
            */
        }
    }
}
