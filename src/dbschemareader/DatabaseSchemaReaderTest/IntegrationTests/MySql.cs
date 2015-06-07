﻿using DatabaseSchemaReader;
#if !NUNIT
using Microsoft.VisualStudio.TestTools.UnitTesting;
#else
using NUnit.Framework;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
using TestContext = System.Object;
using TestCategory = NUnit.Framework.CategoryAttribute;
#endif

namespace DatabaseSchemaReaderTest.IntegrationTests
{
    /// <summary>
    /// These are INTEGRATION tests using databases.
    /// The following databases should exist on localhost:
    ///     MySQL with sakila (user id root, passwod mysql)
    /// </summary>
    [TestClass]
    public class MySql
    {
        [TestMethod, TestCategory("MySql")]
        public void MySqlTest()
        {
            const string providername = "MySql.Data.MySqlClient";
            var connectionString = ConnectionStrings.MySql;
            ProviderChecker.Check(providername, connectionString);

            var dbReader = new DatabaseReader(connectionString, providername);
            var schema = dbReader.ReadAll();
            var country = schema.FindTableByName("country");
            Assert.AreEqual(3, country.Columns.Count);
            Assert.IsNotNull(country.PrimaryKeyColumn);

            var table = dbReader.Table("city");
            Assert.AreEqual(4, table.Columns.Count);
        }

        [TestMethod, TestCategory("MySql")]
        public void MySqlTableTest()
        {
            const string providername = "MySql.Data.MySqlClient";
            var connectionString = ConnectionStrings.MySql;
            ProviderChecker.Check(providername, connectionString);

            var dbReader = new DatabaseReader(connectionString, providername);
            var country = dbReader.Table("country");
            Assert.AreEqual(3, country.Columns.Count);
            Assert.IsNotNull(country.PrimaryKeyColumn);
            Assert.IsTrue(country.FindColumn("country_id").IsPrimaryKey);
        }

        [TestMethod, TestCategory("MySql")]
        public void MySqlUnsignedIntegersTest()
        {
            const string providername = "MySql.Data.MySqlClient";
            var connectionString = ConnectionStrings.MySql;
            ProviderChecker.Check(providername, connectionString);

            var dbReader = new DatabaseReader(connectionString, providername);
            var schema = dbReader.ReadAll();


            var country = schema.FindTableByName("country");

            var pk = country.PrimaryKeyColumn;
            Assert.IsNotNull(pk, "Primary key constraints should be loaded");
            Assert.AreEqual(pk.DbDataType, "smallint(5) unsigned");
            Assert.AreEqual(pk.DataType.TypeName, "SMALLINT");

        }

        [TestMethod, TestCategory("MySql.Devart")]
        public void MySqlViaDevartTest()
        {
            const string providername = "Devart.Data.MySql";
            var connectionString = ConnectionStrings.MySqlDevart;
            ProviderChecker.Check(providername, connectionString);

            DatabaseSchemaReader.Utilities.DiscoverProviderFactory.Discover(connectionString, providername);
            var dbReader = new DatabaseReader(connectionString, providername);
            dbReader.Owner = "sakila";
            var schema = dbReader.ReadAll();
            var country = schema.FindTableByName("country");
            Assert.AreEqual(3, country.Columns.Count);
            Assert.IsNotNull(country.PrimaryKeyColumn);

            var table = dbReader.Table("city");
            Assert.AreEqual(4, table.Columns.Count);
        }
    }
}
