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
        /// Create Table test2 and use insert
        /// </summary>
        [TestMethod]
        public void InserInTable()
        {
            //open connect to database
            DataBase db = Connect();
            var generator = new OKHOSTING.Sql.Net4.SqlServer.SqlGenerator();

            //define table test2
            Table table = new Table("test2");
            table.Columns.Add(new Column() { Name = "Id", DbType = DbType.Int32, IsPrimaryKey = true, IsAutoNumber = true, Table = table });
            table.Columns.Add(new Column() { Name = "Name", DbType = DbType.AnsiString, Length = 100, IsNullable = false, Table = table });
            table.Columns.Add(new Column() { Name = "Age", DbType = DbType.Int32, IsNullable = false, Table = table });

            //create table test2
            var sql = generator.Create(table);
            db.Execute(sql);
            Assert.IsTrue(db.ExistsTable(table.Name));

            //insert values into test2
            Insert insert = new Insert();
            insert.Table = table;
            insert.Values.Add(new ColumnValue(table["Name"], "Angel"));
            insert.Values.Add(new ColumnValue(table["Age"], 25));

            sql = generator.Insert(insert);
            int affectedRows = db.Execute(sql);
            Assert.AreEqual(affectedRows, 1);
        }

        /// <summary>
        /// Create table test4, insert 2 rows, and delete 1 row from test4.Company = Monsters Inc. Corporate
        /// </summary>
        [TestMethod]
        public void dropRow()
        {
            //Open connect to database;
            DataBase db = Connect();
            var generator = new OKHOSTING.Sql.Net4.SqlServer.SqlGenerator();

            //define table test4
            Table table = new Table("test4");

            table.Columns.Add(new Column() { Name = "Id", DbType = DbType.Int32, IsPrimaryKey = true, IsAutoNumber = true, Table = table });
            table.Columns.Add(new Column() { Name = "Company", DbType = DbType.AnsiString, Length = 200, IsNullable = false, Table = table });
            table.Columns.Add(new Column() { Name = "Address", DbType = DbType.AnsiString, Length = 500, IsNullable = false, Table = table });
            table.Columns.Add(new Column() { Name = "Email", DbType = DbType.AnsiString, Length = 100, IsNullable = false, Table = table });
            table.Columns.Add(new Column() { Name = "Telephone", DbType = DbType.AnsiString, Length = 20, IsNullable = false, Table = table });
            table.Indexes.Add(new Index() { Name = "IX_Company", Unique = true, Table = table });
            table.Indexes[0].Columns.Add(table["Company"]);

            //create table test4
            var sql = generator.Create(table);
            db.Execute(sql);
            Assert.IsTrue(db.ExistsTable(table.Name));

            //add index
            sql = generator.Create(table.Indexes[0]);
            db.Execute(sql);

            //insert values into test4
            Insert insert = new Insert();
            insert.Table = table;
            insert.Values.Add(new ColumnValue(table["Company"], "Monsters Inc. Corporate"));
            insert.Values.Add(new ColumnValue(table["Address"], "First Street #12 Blv. Flowers San Diego. C.A."));
            insert.Values.Add(new ColumnValue(table["Email"], "mic_admind@info.com"));
            insert.Values.Add(new ColumnValue(table["Telephone"], "0122389456278"));

            sql = generator.Insert(insert);
            int affectedRows = db.Execute(sql);
            Assert.AreEqual(affectedRows, 1);

            //insert values into test4
            insert = new Insert();
            insert.Table = table;
            insert.Values.Add(new ColumnValue(table["Company"], "Baby tunes SA de CV"));
            insert.Values.Add(new ColumnValue(table["Address"], "Paid Call #202 Blv. Saint, James Tucson. Arizona"));
            insert.Values.Add(new ColumnValue(table["Email"], "info@babyyunes.com"));
            insert.Values.Add(new ColumnValue(table["Telephone"], "012235656178"));

            sql = generator.Insert(insert);
            affectedRows = db.Execute(sql);
            Assert.AreEqual(affectedRows, 1);

            //delete row from test4.company = Monsters Inc. Corporate
            Delete delete = new Delete();
            delete.Table = table;
            delete.Where.Add(new ValueCompareFilter() { Column = table["Company"], ValueToCompare = "Monsters Inc. Corporate", Operator = Data.CompareOperator.Equal });

            sql = generator.Delete(delete);
            affectedRows = db.Execute(sql);
            Assert.AreEqual(affectedRows, 1);
        }

        /// <summary>
        /// Crate Table Address4, insert 2 rows, use selecte with filter Lke
        /// </summary>
        [TestMethod]
        public void selectLike()
        {
            //Open Connect to DataBase
            DataBase db = Connect();
            var generator = new OKHOSTING.Sql.Net4.SqlServer.SqlGenerator();

            //define table Address4
            Table table = new Table("Address4");

            table.Columns.Add(new Column() { Name = "Id", DbType = DbType.Int32, IsPrimaryKey = true, IsAutoNumber = true, Table = table });
            table.Columns.Add(new Column() { Name = "Street", DbType = DbType.AnsiString, Length = 100, IsNullable = false, Table = table });
            table.Columns.Add(new Column() { Name = "Number", DbType = DbType.AnsiString, Length = 100, IsNullable = false, Table = table });
            table.Columns.Add(new Column() { Name = "Suburb", DbType = DbType.AnsiString, Length = 100, IsNullable = false, Table = table });
            table.Columns.Add(new Column() { Name = "State", DbType = DbType.AnsiString, Length = 100, IsNullable = false, Table = table });
            table.Columns.Add(new Column() { Name = "Country", DbType = DbType.AnsiString, Length = 100, IsNullable = false, Table = table });

            //create tanle Address4
            var sql = generator.Create(table);
            db.Execute(sql);
            Assert.IsTrue(db.ExistsTable(table.Name));

            //insert values into Address4
            Insert insert = new Insert();
            insert.Table = table;
            insert.Values.Add(new ColumnValue(table["Street"], "First Avenue"));
            insert.Values.Add(new ColumnValue(table["Number"], "12-B"));
            insert.Values.Add(new ColumnValue(table["Suburb"], "The Roses"));
            insert.Values.Add(new ColumnValue(table["State"], "California"));
            insert.Values.Add(new ColumnValue(table["Country"], "United States"));

            sql = generator.Insert(insert);
            int affectedRows = db.Execute(sql);
            Assert.AreEqual(affectedRows, 1);

            //insert values into Address4
            insert = new Insert();
            insert.Table = table;
            insert.Values.Add(new ColumnValue(table["Street"], "Second Life"));
            insert.Values.Add(new ColumnValue(table["Number"], "1120"));
            insert.Values.Add(new ColumnValue(table["Suburb"], "Park Avenue"));
            insert.Values.Add(new ColumnValue(table["State"], "New York"));
            insert.Values.Add(new ColumnValue(table["Country"], "United States"));

            sql = generator.Insert(insert);
            affectedRows = db.Execute(sql);
            Assert.AreEqual(affectedRows, 1);

            //SELECT street and suburb FROM address4 WHERE street LIKE 'Second'
            Select select = new Select();
            select.Table = table;
            select.Columns.Add(table["Id"]);
            select.Columns.Add(table["Street"]);
            select.Columns.Add(table["Suburb"]);
            select.Where.Add(new ValueCompareFilter() { Column = table["Street"], ValueToCompare = "Second", Operator = Data.CompareOperator.Like });

            sql = generator.Select(select);
            var result = db.GetDataTable(sql);

            //Show result from query
            foreach (IDataRow row in result)
            {
                foreach (object obj in row)
                {
                    Console.Write(obj);
                }
            }

            //Drop Table Address4
            sql = generator.Drop(table);
            db.Execute(sql);
            Assert.IsFalse(db.ExistsTable(table.Name));
        }

        /// <summary>
        /// Create 4 tables(Customer4, Product4, Tax4, Sale4), with Foreign Keys, Uses select with 3 Inner Joins 
        /// </summary>
        [TestMethod]
        public void MultiJoinTest()
        {
            //Open Connect to DataBase
            DataBase db = Connect();
            var generator = new OKHOSTING.Sql.Net4.SqlServer.SqlGenerator();

            // define table Customer4
            Table customer = new Table("customer4");
            customer.Columns.Add(new Column() { Name = "Id", DbType = DbType.Int32, IsPrimaryKey = true, Table = customer });
            customer.Columns.Add(new Column() { Name = "Name", DbType = DbType.AnsiString, Length = 100, IsNullable = false, Table = customer });
            
            // define table Product4
            Table product = new Table("product4");
            product.Columns.Add(new Column() { Name = "Id", DbType = DbType.Int32, IsPrimaryKey = true, Table = product });
            product.Columns.Add(new Column() { Name = "Name", DbType = DbType.AnsiString, Length = 100, IsNullable = false, Table = product });
            product.Columns.Add(new Column() { Name = "Price", DbType = DbType.Decimal, IsNullable = false, Table = product });

            // define table Tax4
            Table tax = new Table("tax4");
            tax.Columns.Add(new Column() { Name = "Id", DbType = DbType.Int32, IsPrimaryKey = true, Table = tax });
            tax.Columns.Add(new Column() { Name = "Name", DbType = DbType.AnsiString, Length = 100, IsNullable = false, Table = tax });
            tax.Columns.Add(new Column() { Name = "Rate", DbType = DbType.Decimal, IsNullable = false, Table = tax });

            // define table Sale4
            Table sale = new Table("sale4");
            sale.Columns.Add(new Column() { Name = "Id", DbType = DbType.Int32, IsPrimaryKey = true, Table = sale });
            sale.Columns.Add(new Column() { Name = "Date", DbType = DbType.DateTime, Table = sale });
            sale.Columns.Add(new Column() { Name = "Customer", DbType = DbType.Int32, IsNullable = false, Table = sale });
            sale.Columns.Add(new Column() { Name = "Product", DbType = DbType.Int32, IsNullable = false, Table = sale });
            sale.Columns.Add(new Column() { Name = "Tax", DbType = DbType.Int32, IsNullable = false, Table = sale });

            //define Foreign Key Sale to Customer4
            ForeignKey customerFK = new ForeignKey();
            customerFK.Table = sale;
            customerFK.RemoteTable = customer;
            customerFK.Name = "FK_sale4_customer4";
            customerFK.Columns.Add(new Tuple<Column, Column>(sale["Customer"], customer["Id"]));
            customerFK.DeleteAction = customerFK.UpdateAction = ConstraintAction.Restrict;

            //define Foreign Key Sale to Product4
            ForeignKey productFK = new ForeignKey();
            productFK.Table = sale;
            productFK.RemoteTable = product;
            productFK.Name = "FK_sale4_product4";
            productFK.Columns.Add(new Tuple<Column, Column>(sale["Product"], product["Id"]));
            productFK.DeleteAction = productFK.UpdateAction = ConstraintAction.Restrict;

            //define Foreign Key Sale to Tax4
            ForeignKey taxFK = new ForeignKey();
            taxFK.Table = sale;
            taxFK.RemoteTable = tax;
            taxFK.Name = "FK_sale4_tax4";
            taxFK.Columns.Add(new Tuple<Column, Column>(sale["Tax"], tax["Id"]));
            taxFK.DeleteAction = taxFK.UpdateAction = ConstraintAction.Restrict;

            //Create table Customer4
            var sql = generator.Create(customer);
            db.Execute(sql);

            //Create table Product4
            sql = generator.Create(product);
            db.Execute(sql);

            //Create table Tax4
            sql = generator.Create(tax);
            db.Execute(sql);

            //Create table Sale4
            sql = generator.Create(sale);
            db.Execute(sql);

            //Create foreign Key Sale to Customer4
            sql = generator.Create(customerFK);
            db.Execute(sql);

            //Create foreign Key Sale to Product4
            sql = generator.Create(productFK);
            db.Execute(sql);

            //Create foreign Key Sale to Tax4
            sql = generator.Create(taxFK);
            db.Execute(sql);


            //insert values into customer4
            Insert insert = new Insert();
            insert.Table = customer;
            insert.Values.Add(new ColumnValue(customer["Id"], 1));
            insert.Values.Add(new ColumnValue(customer["Name"], "Joyas Loyane SA de CV"));

            sql = generator.Insert(insert);
            int affectedRows = db.Execute(sql);
            Assert.AreEqual(affectedRows, 1);

            //insert values into customer4
            insert = new Insert();
            insert.Table = customer;
            insert.Values.Add(new ColumnValue(customer["Id"], 2));
            insert.Values.Add(new ColumnValue(customer["Name"], "Cartie Joyerias SC de CV"));

            sql = generator.Insert(insert);
            affectedRows = db.Execute(sql);
            Assert.AreEqual(affectedRows, 1);

            //insert values into product4
            insert = new Insert();
            insert.Table = product;
            insert.Values.Add(new ColumnValue(product["Id"], 1));
            insert.Values.Add(new ColumnValue(product["Name"], "Hosting"));
            insert.Values.Add(new ColumnValue(product["Price"], 125.50));

            sql = generator.Insert(insert);
            affectedRows = db.Execute(sql);
            Assert.AreEqual(affectedRows, 1);

            //insert values into product4
            insert = new Insert();
            insert.Table = product;
            insert.Values.Add(new ColumnValue(product["Id"], 2));
            insert.Values.Add(new ColumnValue(product["Name"], "Web Page"));
            insert.Values.Add(new ColumnValue(product["Price"], 1300.75));

            sql = generator.Insert(insert);
            affectedRows = db.Execute(sql);
            Assert.AreEqual(affectedRows, 1);

            //insert values into tax4
            insert = new Insert();
            insert.Table = tax;
            insert.Values.Add(new ColumnValue(tax["Id"], 1));
            insert.Values.Add(new ColumnValue(tax["Name"], "IVA"));
            insert.Values.Add(new ColumnValue(tax["Rate"], 16.00));

            sql = generator.Insert(insert);
            affectedRows = db.Execute(sql);
            Assert.AreEqual(affectedRows, 1);

            //insert values into sale4
            insert = new Insert();
            insert.Table = sale;
            insert.Values.Add(new ColumnValue(sale["Id"], 1));
            insert.Values.Add(new ColumnValue(sale["Date"], DateTime.Today));
            insert.Values.Add(new ColumnValue(sale["Customer"], 1));
            insert.Values.Add(new ColumnValue(sale["Product"], 1));
            insert.Values.Add(new ColumnValue(sale["Tax"], 1));

            sql = generator.Insert(insert);
            affectedRows = db.Execute(sql);
            Assert.AreEqual(affectedRows, 1);

            //insert values into sale4
            insert = new Insert();
            insert.Table = sale;
            insert.Values.Add(new ColumnValue(sale["Id"], 2));
            insert.Values.Add(new ColumnValue(sale["Date"], DateTime.Today));
            insert.Values.Add(new ColumnValue(sale["Customer"], 2));
            insert.Values.Add(new ColumnValue(sale["Product"], 2));
            insert.Values.Add(new ColumnValue(sale["Tax"], 1));

            sql = generator.Insert(insert);
            affectedRows = db.Execute(sql);
            Assert.AreEqual(affectedRows, 1);

            //select sale for customer4 'Joyas Loyane SA de CV'
            Select select = new Select();
            select.Table = sale;
            select.Columns.Add(new SelectColumn(sale["id"]));
            select.Columns.Add(new SelectColumn(sale["Date"]));

            //join for get name Customer4
            SelectJoin join = new SelectJoin();
            join.Table = customer;
            join.On.Add(new ColumnCompareFilter() { Column = sale["Customer"], ColumnToCompare = customer["Id"], Operator = Data.CompareOperator.Equal });
            join.Columns.Add(new SelectColumn(customer["Name"], "customerName"));
            join.JoinType = SelectJoinType.Inner;
            //Add Join in select
            select.Joins.Add(join);

            //join for get name Product4
            SelectJoin join2 = new SelectJoin();
            join2.Table = product;
            join2.On.Add(new ColumnCompareFilter() { Column = sale["Product"], ColumnToCompare = product["Id"], Operator = Data.CompareOperator.Equal });
            join2.Columns.Add(new SelectColumn(product["Name"], "productName"));
            join2.JoinType = SelectJoinType.Inner;
            //Add Join in select
            select.Joins.Add(join2);

            //join for get name Tax4
            SelectJoin join3 = new SelectJoin();
            join3.Table = tax;
            join3.On.Add(new ColumnCompareFilter() { Column = sale["Tax"], ColumnToCompare = tax["Id"], Operator = Data.CompareOperator.Equal });
            join3.Columns.Add(new SelectColumn(tax["Name"], "taxName"));
            join3.JoinType = SelectJoinType.Inner;
            //Add Join in select
            select.Joins.Add(join3);

            //Where Customer4 = Joyas Loyane SA de CV.
            select.Where.Add(new ValueCompareFilter() { Column = customer["Name"], ValueToCompare = "Joyas Loyane SA de CV", Operator = Data.CompareOperator.Equal });

            //Execute Select
            sql = generator.Select(select);
            var result = db.GetDataTable(sql);

            //Show result in Command Line
            foreach (IDataRow row in result)
            {
                foreach (object obj in row)
                {
                    Console.Write(obj);
                }
            }

            Assert.AreEqual(result.Count, 1);

            //Drop Table Sale4 (Firts drop the table contains foreign keys)
            sql = generator.Drop(sale);
            db.Execute(sql);
            Assert.IsFalse(db.ExistsTable(sale.Name));

            //Drop Table Customer4
            sql = generator.Drop(customer);
            db.Execute(sql);
            Assert.IsFalse(db.ExistsTable(sale.Name));

            //Drop Table Product4
            sql = generator.Drop(product);
            db.Execute(sql);
            Assert.IsFalse(db.ExistsTable(sale.Name));

            //Drop Table Tax4
            sql = generator.Drop(tax);
            db.Execute(sql);
            Assert.IsFalse(db.ExistsTable(sale.Name));
        }

        /// <summary>
        /// Crate Table Address5, insert 2 rows, and use update
        /// </summary>
        [TestMethod]
        public void useUpdate()
        {
            //Open Connect to DataBase
            DataBase db = Connect();
            var generator = new OKHOSTING.Sql.Net4.SqlServer.SqlGenerator();

            //define table Address5
            Table table = new Table("Address5");

            table.Columns.Add(new Column() { Name = "Id", DbType = DbType.Int32, IsPrimaryKey = true, IsAutoNumber = true, Table = table });
            table.Columns.Add(new Column() { Name = "Street", DbType = DbType.AnsiString, Length = 100, IsNullable = false, Table = table });
            table.Columns.Add(new Column() { Name = "Number", DbType = DbType.AnsiString, Length = 100, IsNullable = false, Table = table });
            table.Columns.Add(new Column() { Name = "Suburb", DbType = DbType.AnsiString, Length = 100, IsNullable = false, Table = table });
            table.Columns.Add(new Column() { Name = "State", DbType = DbType.AnsiString, Length = 100, IsNullable = false, Table = table });
            table.Columns.Add(new Column() { Name = "Country", DbType = DbType.AnsiString, Length = 100, IsNullable = false, Table = table });

            //create tanle Address5
            var sql = generator.Create(table);
            db.Execute(sql);
            Assert.IsTrue(db.ExistsTable(table.Name));

            //insert values into Address5
            Insert insert = new Insert();
            insert.Table = table;
            insert.Values.Add(new ColumnValue(table["Street"], "First Avenue"));
            insert.Values.Add(new ColumnValue(table["Number"], "12-B"));
            insert.Values.Add(new ColumnValue(table["Suburb"], "The Roses"));
            insert.Values.Add(new ColumnValue(table["State"], "California"));
            insert.Values.Add(new ColumnValue(table["Country"], "United States"));

            sql = generator.Insert(insert);
            int affectedRows = db.Execute(sql);
            Assert.AreEqual(affectedRows, 1);

            //insert values into Address5
            insert = new Insert();
            insert.Table = table;
            insert.Values.Add(new ColumnValue(table["Street"], "Second Life"));
            insert.Values.Add(new ColumnValue(table["Number"], "1120"));
            insert.Values.Add(new ColumnValue(table["Suburb"], "Park Avenue"));
            insert.Values.Add(new ColumnValue(table["State"], "New York"));
            insert.Values.Add(new ColumnValue(table["Country"], "United States"));

            sql = generator.Insert(insert);
            affectedRows = db.Execute(sql);
            Assert.AreEqual(affectedRows, 1);

            //Uptade row where street = "Second Life"
            Update update = new Update();
            update.Table = table;
            update.Where.Add(new ValueCompareFilter() { Column = table["Street"], ValueToCompare = "Second Life", Operator = Data.CompareOperator.Equal });
            update.Set.Add(new ColumnValue(table["Street"], "Third Country"));

            sql = generator.Update(update);
            affectedRows = db.Execute(sql);
            Assert.AreEqual(affectedRows, 1);

            //Drop Table Address5
            sql = generator.Drop(table);
            db.Execute(sql);
            Assert.IsFalse(db.ExistsTable(table.Name));
        }

    }

}