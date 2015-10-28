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
            var generator = new Net4.SqlServer.SqlGenerator();

            //define table person
            Table table = new Table("AnyTable");
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
            insert.Values.Add(new ColumnValue(table["Id"], 1));
            insert.Values.Add(new ColumnValue(table["Name"], "Angel"));
            insert.Values.Add(new ColumnValue(table["Age"], 25));

            sql = generator.Insert(insert);
            int affectedRows = db.Execute(sql);
            Assert.AreEqual(affectedRows, 1);
        }
    }

}