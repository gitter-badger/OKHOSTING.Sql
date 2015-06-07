﻿using DatabaseSchemaReader.DataSchema;
using DatabaseSchemaReader.SqlGen;
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

namespace DatabaseSchemaReaderTest.SqlGen.Migrations
{
    [TestClass]
    public class MigrationPostgreSqlTest
    {
        private const string ProviderName = "Npgsql";
        //private const string ProviderName = "Devart.Data.PostgreSql";

        /*
 
CREATE FUNCTION X_Insert(IN INT,IN VARCHAR(20),IN VARCHAR(30))
RETURNS VOID AS $$ 
INSERT INTO X(XId, XName, XAddress) VALUES($1, $2, $3); 
$$ LANGUAGE SQL;
 
ALTER TABLE products ALTER COLUMN price SET DEFAULT 7.77;
 */

        [TestMethod, TestCategory("Postgresql")]
        public void TestMigration()
        {
            //arrange
            var connectionString = ConnectionStrings.PostgreSql;
            var tableName = MigrationCommon.FindFreeTableName(ProviderName, connectionString);
            var migration = new DdlGeneratorFactory(SqlType.PostgreSql).MigrationGenerator();

            MigrationCommon.ExecuteScripts(ProviderName, connectionString, tableName, migration);
        }
    }
}
