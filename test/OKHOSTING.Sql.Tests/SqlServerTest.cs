using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OKHOSTING.Sql.Operations;
using OKHOSTING.Sql.Schema;
using OKHOSTING.Sql.Filters;

namespace OKHOSTING.Sql.Tests
{
    [TestClass]
    public class SqlServerTest
    {
        public DataBase Connect()
        {
            return new Net4.SqlServer.DataBase() { ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["sqlserver"].ConnectionString };
        }


        [TestMethod]
        public void InnerJoinTest()
        {
            DataBase db = Connect();
            var generator = new OKHOSTING.Sql.Net4.SqlServer.SqlGenerator();

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
            var generator = new OKHOSTING.Sql.Net4.SqlServer.SqlGenerator();

            //define table schema
            Table table = new Table("test1");

            table.Columns.Add(new Column() { Name = "Id", DbType = DbType.Int32, IsPrimaryKey = true, Table = table, IsAutoNumber = true });
            table.Columns.Add(new Column() { Name = "TextField", DbType = DbType.AnsiString, Length = 100, IsNullable = false, Table = table });
            table.Columns.Add(new Column() { Name = "NumberField", DbType = DbType.Int32, IsNullable = false, Table = table });
            table.Indexes.Add(new Index() { Name = "IX_TextField", Unique = false, Table = table });
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
            //insert.Values.Add(new ColumnValue(table["Id"], 1));
            insert.Values.Add(new ColumnValue(table["TextField"], "test11"));
            insert.Values.Add(new ColumnValue(table["NumberField"], 100));

            sql = generator.Insert(insert);
            int affectedRows = db.Execute(sql);
            Assert.AreEqual(affectedRows, 1);

            //insert
            insert = new Insert();
            insert.Table = table;
            //insert.Values.Add(new ColumnValue(table["Id"], 2));
            insert.Values.Add(new ColumnValue(table["TextField"], "test15"));
            insert.Values.Add(new ColumnValue(table["NumberField"], 110));

            sql = generator.Insert(insert);
            affectedRows = db.Execute(sql);
            Assert.AreEqual(affectedRows, 1);

            //select
            Select select = new Select();
            select.Table = table;
            select.Columns.Add(table["id"]);
            select.Columns.Add(table["TextField"]);
            select.Where.Add(new ValueCompareFilter() { Column = table["TextField"], ValueToCompare = "test11", Operator = Data.CompareOperator.Equal });

            sql = generator.Select(select);
            var result = db.GetDataTable(sql);
            Assert.AreEqual(result.Count, 1);

            //update
            Update update = new Update();
            update.Table = table;
            update.Where.Add(new ValueCompareFilter() { Column = table["TextField"], ValueToCompare = "test11" });


            //delete
            Delete delete = new Delete();
            delete.Table = table;
            delete.Where.Add(new ValueCompareFilter() { Column = table["TextField"], ValueToCompare = "test11" });

            sql = generator.Delete(delete);
            affectedRows = db.Execute(sql);
            Assert.AreEqual(affectedRows, 1);

            //drop
            sql = generator.Drop(table);
            db.Execute(sql);
            Assert.IsFalse(db.ExistsTable(table.Name));
        }

        /// <summary>
        /// Create 3 tables a user whit 2 joins
        /// </summary>
        [TestMethod]
        public void TablesTest()
        {
            DataBase db = Connect();
            var generator = new OKHOSTING.Sql.Net4.SqlServer.SqlGenerator();

            //Create table team            
            Table team = new Table("team");
            team.Columns.Add(new Column() { Name = "Id", DbType = DbType.Int32, IsPrimaryKey = true, IsAutoNumber = true, Table = team });
            team.Columns.Add(new Column() { Name = "Name", DbType = DbType.AnsiString, Length = 50, IsNullable = false, Table = team });
            team.Columns.Add(new Column() { Name = "Leage", DbType = DbType.Int32, IsNullable = false, Table = team });
            team.Columns.Add(new Column() { Name = "Country", DbType = DbType.Int32, IsNullable = false, Table = team });

            //Create table leage
            Table leage = new Table("leage");
            leage.Columns.Add(new Column() { Name = "Id", DbType = DbType.Int32, IsPrimaryKey = true, IsAutoNumber = true, Table = leage });
            leage.Columns.Add(new Column() { Name = "Name", DbType = DbType.AnsiString, IsNullable = false, Table = leage });

            //Create table country
            Table country = new Table("country");
            country.Columns.Add(new Column() { Name = "Id", DbType = DbType.Int32, IsPrimaryKey = true, IsAutoNumber = true, Table = country });
            country.Columns.Add(new Column() { Name = "Name", DbType = DbType.AnsiString, IsNullable = false, Table = country });

            //Create ForeignKey FK_team_country
            ForeignKey countryFK = new ForeignKey();
            countryFK.Table = team;
            countryFK.RemoteTable = country;
            countryFK.Name = "FK_team_country";
            countryFK.Columns.Add(new Tuple<Column, Column>(team["Country"], country["Id"]));
            countryFK.DeleteAction = countryFK.UpdateAction = ConstraintAction.Restrict;

            //Create ForeignKey FK_team_leage
            ForeignKey leageFK = new ForeignKey();
            leageFK.Table = team;
            leageFK.RemoteTable = leage;
            leageFK.Name = "FK_team_leage";
            leageFK.Columns.Add(new Tuple<Column, Column>(team["Leage"], leage["Id"]));
            leageFK.DeleteAction = countryFK.UpdateAction = ConstraintAction.Restrict;

            Command sql = generator.Create(team);
            db.Execute(sql);

            sql = generator.Create(leage);
            db.Execute(sql);

            sql = generator.Create(country);
            db.Execute(sql);

            //insert Country
            Insert insert = new Insert();
            insert.Table = country;
            insert.Values.Add(new ColumnValue(country["Id"], 15));
            insert.Values.Add(new ColumnValue(country["Name"], "Argentina"));

            sql = generator.Insert(insert);
            int affectedRows = db.Execute(sql);
            Assert.AreEqual(affectedRows, 1);

            insert = new Insert();
            insert.Table = country;
            insert.Values.Add(new ColumnValue(country["Id"], 10));
            insert.Values.Add(new ColumnValue(country["Name"], "Brasil"));

            sql = generator.Insert(insert);
            affectedRows = db.Execute(sql);
            Assert.AreEqual(affectedRows, 1);

            //insert leage
            insert = new Insert();
            insert.Table = leage;
            insert.Values.Add(new ColumnValue(leage["Id"], 100));
            insert.Values.Add(new ColumnValue(leage["Name"], "Champions"));

            sql = generator.Insert(insert);
            affectedRows = db.Execute(sql);
            Assert.AreEqual(affectedRows, 1);

            insert = new Insert();
            insert.Table = leage;
            insert.Values.Add(new ColumnValue(leage["Id"], 110));
            insert.Values.Add(new ColumnValue(leage["Name"], "Concacaff"));

            sql = generator.Insert(insert);
            affectedRows = db.Execute(sql);
            Assert.AreEqual(affectedRows, 1);

            //insert team
            insert = new Insert();
            insert.Table = team;
            insert.Values.Add(new ColumnValue(team["Id"], 1));
            insert.Values.Add(new ColumnValue(team["Name"], "Barza"));
            insert.Values.Add(new ColumnValue(team["Leage"], 100));
            insert.Values.Add(new ColumnValue(team["Country"], 10));

            sql = generator.Insert(insert);
            affectedRows = db.Execute(sql);
            Assert.AreEqual(affectedRows, 1);

            insert = new Insert();
            insert.Table = team;
            insert.Values.Add(new ColumnValue(team["Id"], 2));
            insert.Values.Add(new ColumnValue(team["Name"], "Pumas"));
            insert.Values.Add(new ColumnValue(team["Leage"], 110));
            insert.Values.Add(new ColumnValue(team["Country"], 15));

            sql = generator.Insert(insert);
            affectedRows = db.Execute(sql);
            Assert.AreEqual(affectedRows, 1);

            //select
            Select select = new Select();
            select.Table = team;
            select.Columns.Add(new SelectColumn(team["id"]));
            select.Columns.Add(new SelectColumn(team["Name"]));

            //Create inner join to country
            SelectJoin join = new SelectJoin();
            join.Table = country;
            join.On.Add(new ColumnCompareFilter() { Column = team["country"], ColumnToCompare = country["id"], Operator = Data.CompareOperator.Equal });
            join.Columns.Add(new SelectColumn(country["name"], "countryName"));
            join.JoinType = SelectJoinType.Inner;

            select.Joins.Add(join);

            //Create inner join to leage
            SelectJoin join2 = new SelectJoin();
            join2.Table = leage;
            join2.On.Add(new ColumnCompareFilter() { Column = team["leage"], ColumnToCompare = leage["id"], Operator = Data.CompareOperator.Equal });
            join2.Columns.Add(new SelectColumn(leage["name"], "leageName"));
            join2.JoinType = SelectJoinType.Inner;

            select.Joins.Add(join2);

            select.Where.Add(new ValueCompareFilter() { Column = team["Name"], ValueToCompare = "Pumas", Operator = Data.CompareOperator.Equal });
            sql = generator.Select(select);
            var result = db.GetDataTable(sql);

            //delete column id in table leage where id = 110
            Delete delete = new Delete();
            delete.Table = leage;
            delete.Where.Add(new ValueCompareFilter() { Column = leage["id"], ValueToCompare = 110, Operator = Data.CompareOperator.Equal });

            sql = generator.Delete(delete);
            affectedRows = db.Execute(sql);
            Assert.AreEqual(affectedRows, 1);

            //drop table leage
            sql = generator.Drop(leage);
            db.Execute(sql);
            Assert.IsFalse(db.ExistsTable(leage.Name));

            //drop table country
            sql = generator.Drop(country);
            db.Execute(sql);
            Assert.IsFalse(db.ExistsTable(country.Name));

            //drop table team
            sql = generator.Drop(team);
            db.Execute(sql);
            Assert.IsFalse(db.ExistsTable(team.Name));
        }

        /// <summary>
        /// Create 1 table and users select whit AndFilter
        /// </summary>
        [TestMethod]
        public void InventoryTest()
        {
            DataBase db = Connect();
            var generator = new OKHOSTING.Sql.Net4.SqlServer.SqlGenerator();

            //Create table store
            Table store = new Table("store");
            store.Columns.Add(new Column() { Name = "Id", DbType = DbType.Int32, IsPrimaryKey = true, IsAutoNumber = true, Table = store });
            store.Columns.Add(new Column() { Name = "Name", DbType = DbType.AnsiString, Length = 50, IsNullable = false, Table = store });
            store.Columns.Add(new Column() { Name = "Inventory", DbType = DbType.AnsiString, Length = 50, IsNullable = false, Table = store });
            store.Columns.Add(new Column() { Name = "Employee", DbType = DbType.AnsiString, Length = 100, IsNullable = false, Table = store });

            Command sql = generator.Create(store);
            db.Execute(sql);

            //First inserts store 
            Insert insert = new Insert();
            insert.Table = store;
            insert.Values.Add(new ColumnValue(store["Id"], 1));
            insert.Values.Add(new ColumnValue(store["Name"], "Abarrotes Torrez"));
            insert.Values.Add(new ColumnValue(store["Inventory"], "Torreon"));
            insert.Values.Add(new ColumnValue(store["Employee"], "Juan Rocha Gomez"));

            sql = generator.Insert(insert);
            int affectedRows = db.Execute(sql);
            Assert.AreEqual(affectedRows, 1);

            //Second inserts store
            insert = new Insert();
            insert.Table = store;
            insert.Values.Add(new ColumnValue(store["Id"], 2));
            insert.Values.Add(new ColumnValue(store["Name"], "La Furiosa"));
            insert.Values.Add(new ColumnValue(store["Inventory"], "Zacatecas"));
            insert.Values.Add(new ColumnValue(store["Employee"], "Martin Torrez"));

            sql = generator.Insert(insert);
            affectedRows = db.Execute(sql);
            Assert.AreEqual(affectedRows, 1);

            //third inserts store
            insert = new Insert();
            insert.Table = store;
            insert.Values.Add(new ColumnValue(store["Id"], 3));
            insert.Values.Add(new ColumnValue(store["Name"], "Los dos amigos"));
            insert.Values.Add(new ColumnValue(store["Inventory"], "Durango"));
            insert.Values.Add(new ColumnValue(store["Employee"], "Luis Martinez"));

            sql = generator.Insert(insert);
            affectedRows = db.Execute(sql);
            Assert.AreEqual(affectedRows, 1);

            //Fourth inserts store
            insert = new Insert();
            insert.Table = store;
            insert.Values.Add(new ColumnValue(store["Id"], 4));
            insert.Values.Add(new ColumnValue(store["Name"], "La Pasada"));
            insert.Values.Add(new ColumnValue(store["Inventory"], "Nayarit"));
            insert.Values.Add(new ColumnValue(store["Employee"], "Raul Gomez"));

            sql = generator.Insert(insert);
            affectedRows = db.Execute(sql);
            Assert.AreEqual(affectedRows, 1);

            //select whit OrFilter
            Select select = new Select();
            select.Table = store;
            select.Columns.Add(store["id"]);
            select.Columns.Add(store["Name"]);
            select.Columns.Add(store["Employee"]);

            OrFilter filter = new OrFilter();
            filter.InnerFilters.Add(new ValueCompareFilter() { Column = store["id"], ValueToCompare = 1, Operator = Data.CompareOperator.Equal });
            filter.InnerFilters.Add(new ValueCompareFilter() { Column = store["id"], ValueToCompare = 2, Operator = Data.CompareOperator.Equal });
            select.Where.Add(filter);


            sql = generator.Select(select);
            var result = db.GetDataTable(sql);

            Assert.AreEqual(result.Count, 2);

            //select whit like
            select = new Select();
            select.Table = store;
            select.Columns.Add(store["Name"]);
            select.Columns.Add(store["Employee"]);
            select.Where.Add(new ValueCompareFilter() { Column = store["Name"], ValueToCompare = '%' + "la" + '%', Operator = Data.CompareOperator.Like });

            sql = generator.Select(select);
            result = db.GetDataTable(sql);
            Assert.AreEqual(result.Count, 2);


            Assert.AreEqual(result.Count, 0);


            //select whit AndFilter
            select = new Select();
            select.Table = store;
            select.Columns.Add(store["id"]);
            select.Columns.Add(store["Name"]);
            select.Columns.Add(store["Inventory"]);

            AndFilter and = new AndFilter();
            and.InnerFilters.Add(new ValueCompareFilter() { Column = store["id"], ValueToCompare = 1, Operator = Data.CompareOperator.Equal });
            and.InnerFilters.Add(new ValueCompareFilter() { Column = store["Name"], ValueToCompare = "Abarrotes Torrez", Operator = Data.CompareOperator.Equal });
            select.Where.Add(and);

            sql = generator.Select(select);
            result = db.GetDataTable(sql);
            Assert.AreEqual(result.Count, 0);

            //Drop table
            sql = generator.Drop(store);
            db.Execute(sql);
            Assert.IsFalse(db.ExistsTable(store.Name));
        }


        /// <summary>
        /// Create 3 tables a user whit 1 joins
        /// </summary>
        [TestMethod]
        public void PriceadjustmentTest()
        {

            /*
            [TestMethod]
            public void CreateTable()

            {
                DataBase db = Connect();
                var generator = new OKHOSTING.Sql.Net4.SqlServer.SqlGenerator();

                //Declare table application            
                Table application = new Table("application");
                application.Columns.Add(new Column() { Name = "Id", DbType = DbType.Int32, IsPrimaryKey = true, IsAutoNumber = true, Table = application });
                application.Columns.Add(new Column() { Name = "Element", DbType = DbType.Int32, IsNullable = false, Table = application });
                application.Columns.Add(new Column() { Name = "Priceadjustment", DbType = DbType.Int32, IsNullable = false, Table = application });

                //Declare table priceadjustment
                Table priceadjustment = new Table("priceadjustment");
                priceadjustment.Columns.Add(new Column() { Name = "Id", DbType = DbType.Int32, IsPrimaryKey = true, IsAutoNumber = true, Table = priceadjustment });
                priceadjustment.Columns.Add(new Column() { Name = "Name", DbType = DbType.AnsiString, Length = 50, IsNullable = false, Table = priceadjustment });
                priceadjustment.Columns.Add(new Column() { Name = "Customer", DbType = DbType.AnsiString, Length = 50, IsNullable = false, Table = priceadjustment });
                priceadjustment.Columns.Add(new Column() { Name = "Amount", DbType = DbType.AnsiString, Length = 20, IsNullable = false, Table = priceadjustment });

                //Declare table element
                Table element = new Table("element");
                element.Columns.Add(new Column() { Name = "Id", DbType = DbType.Int32, IsPrimaryKey = true, IsAutoNumber = true, Table = element });
                element.Columns.Add(new Column() { Name = "Name", DbType = DbType.AnsiString, Length = 50, IsNullable = false, Table = element });

                //Declare ForeignKey FK_application_priceadjustment
                ForeignKey priceadjustmentFK = new ForeignKey();
                priceadjustmentFK.Table = application;
                priceadjustmentFK.RemoteTable = priceadjustment;
                priceadjustmentFK.Name = "FK_application_priceadjustment";
                priceadjustmentFK.Columns.Add(new Tuple<Column, Column>(application["priceadjustment"], priceadjustment["Id"]));
                priceadjustmentFK.DeleteAction = priceadjustmentFK.UpdateAction = ConstraintAction.Restrict;

                //Declare ForeignKey FK_application_element
                ForeignKey elementFK = new ForeignKey();
                elementFK.Table = application;
                elementFK.RemoteTable = element;
                elementFK.Name = "FK_application_element";
                elementFK.Columns.Add(new Tuple<Column, Column>(application["element"], element["Id"]));
                elementFK.DeleteAction = elementFK.UpdateAction = ConstraintAction.Restrict;

                //Create table application
                Command sql = generator.Create(application);
                db.Execute(sql);

                //Create table priceadjustment
                sql = generator.Create(priceadjustment);
                db.Execute(sql);

                //Create table element
                sql = generator.Create(element);
                db.Execute(sql);

                //Create Foreign Key priceadjustmentFK
                sql = generator.Create(priceadjustmentFK);
                db.Execute(sql);

                //Create Foreign Key elementFK
                sql = generator.Create(elementFK);
                db.Execute(sql);

                //inserts priceadjustment
                Insert insert = new Insert();
                insert.Table = priceadjustment;
                insert.Values.Add(new ColumnValue(priceadjustment["Name"], "Afiliados"));
                insert.Values.Add(new ColumnValue(priceadjustment["Customer"], "Raul Gomez"));
                insert.Values.Add(new ColumnValue(priceadjustment["Amount"], "$550"));

                //Create table store
                Table store = new Table("store");
                store.Columns.Add(new Column() { Name = "Id", DbType = DbType.Int32, IsPrimaryKey = true, IsAutoNumber = true, Table = store });
                store.Columns.Add(new Column() { Name = "Name", DbType = DbType.AnsiString, Length = 50, IsNullable = false, Table = store });
                store.Columns.Add(new Column() { Name = "Inventory", DbType = DbType.AnsiString, Length = 50, IsNullable = false, Table = store });
                store.Columns.Add(new Column() { Name = "Employee", DbType = DbType.AnsiString, Length = 100, IsNullable = false, Table = store });

                Command sql = generator.Create(store);
                db.Execute(sql);

                //First inserts store 
                Insert insert = new Insert();
                insert.Table = store;
                insert.Values.Add(new ColumnValue(store["Id"], 1));
                insert.Values.Add(new ColumnValue(store["Name"], "Abarrotes Torrez"));
                insert.Values.Add(new ColumnValue(store["Inventory"], "Torreon"));
                insert.Values.Add(new ColumnValue(store["Employee"], "Juan Rocha Gomez"));

                sql = generator.Insert(insert);
                int affectedRows = db.Execute(sql);
                Assert.AreEqual(affectedRows, 1);

                insert = new Insert();
                insert.Table = priceadjustment;
                insert.Values.Add(new ColumnValue(priceadjustment["Name"], "Clientes Frecuentes"));
                insert.Values.Add(new ColumnValue(priceadjustment["Customer"], "Juan Ramírez"));
                insert.Values.Add(new ColumnValue(priceadjustment["Amount"], "$780"));

                sql = generator.Insert(insert);
                affectedRows = db.Execute(sql);
                Assert.AreEqual(affectedRows, 1);

                //inserts element
                insert = new Insert();
                insert.Table = element;
                insert.Values.Add(new ColumnValue(element["Name"], "Llanta"));

                sql = generator.Insert(insert);
                affectedRows = db.Execute(sql);
                Assert.AreEqual(affectedRows, 1);

                insert = new Insert();
                insert.Table = element;
                insert.Values.Add(new ColumnValue(element["Name"], "Rin"));

                sql = generator.Insert(insert);
                affectedRows = db.Execute(sql);
                Assert.AreEqual(affectedRows, 1);

                //inserts application
                insert = new Insert();
                insert.Table = application;
                insert.Values.Add(new ColumnValue(application["Element"], 1));
                insert.Values.Add(new ColumnValue(application["Priceadjustment"], 1));

                sql = generator.Insert(insert);
                affectedRows = db.Execute(sql);
                Assert.AreEqual(affectedRows, 1);

                insert = new Insert();
                insert.Table = application;
                insert.Values.Add(new ColumnValue(application["Element"], 2));
                insert.Values.Add(new ColumnValue(application["Priceadjustment"], 2));

                sql = generator.Insert(insert);
                affectedRows = db.Execute(sql);
                Assert.AreEqual(affectedRows, 1);

                //select whit inner join
                Select select = new Select();
                select.Table = priceadjustment;
                select.Columns.Add(new SelectColumn(priceadjustment["Name"]));
                select.Columns.Add(new SelectColumn(priceadjustment["Customer"]));
                select.Columns.Add(new SelectColumn(priceadjustment["Amount"]));

                //Create inner join to priceadjustment
                SelectJoin join2 = new SelectJoin();
                join2.Table = application;
                join2.On.Add(new ColumnCompareFilter() { Column = application["Priceadjustment"], ColumnToCompare = priceadjustment["id"], Operator = Data.CompareOperator.Equal });            
                join2.JoinType = SelectJoinType.Inner;

                select.Joins.Add(join2);

                select.Where.Add(new ValueCompareFilter() { Column = application["Element"], ValueToCompare = 1, Operator = Data.CompareOperator.Equal });
                sql = generator.Select(select);
                var result = db.GetDataTable(sql);

                //drop table application
                sql = generator.Drop(application);
                db.Execute(sql);
                Assert.IsFalse(db.ExistsTable(application.Name));

                //drop table element
                sql = generator.Drop(element);
                db.Execute(sql);
                Assert.IsFalse(db.ExistsTable(element.Name));

                //drop table priceadjustment
                sql = generator.Drop(priceadjustment);
                db.Execute(sql);
                Assert.IsFalse(db.ExistsTable(priceadjustment.Name));            
            }

                //select
                Select select = new Select();
                select.Table = ;
                select.Columns.Add(table["id"]);
                select.Columns.Add(table["TextField"]);
                select.Where.Add(new ValueCompareFilter() { Column = table["TextField"], ValueToCompare = "test11", Operator = Data.CompareOperator.Equal });

                Command sql = generator.Select(select);
                var result = db.GetDataTable(sql);            

                Assert.AreEqual(result.Count, 1);*/
            //}
        }
    }
}
