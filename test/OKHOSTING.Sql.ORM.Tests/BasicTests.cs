using System;
using NUnit.Framework;
using OKHOSTING.Sql.Operations;
using System.Collections.Generic;
using OKHOSTING.Sql.Schema;
using OKHOSTING.Sql.Filters;

namespace OKHOSTING.Sql.ORM.Tests
{
	[TestFixture]
	public class BasicTests
	{
		DataBase DataBase;

		public BasicTests()
		{
			DataBase = new DataBase(new MySql.DataBase(System.Configuration.ConfigurationManager.ConnectionStrings["mysql"].ConnectionString), new MySql.SqlGenerator());
		}

		[Test]
		public void CreateAndDrop()
		{
			DataType<Person>.DefaultMap();
			DataType<Employee>.DefaultMap();
			DataType<Customer>.DefaultMap();
			DataType<CustomerContact>.DefaultMap();

			DataBase.Create<Person>();
			DataBase.Create<Employee>();
			DataBase.Create<Customer>();
			DataBase.Create<CustomerContact>();

			Assert.IsTrue(DataBase.NativeDataBase.ExistsTable("Person"));
			Assert.IsTrue(DataBase.NativeDataBase.ExistsTable("Employee"));
			Assert.IsTrue(DataBase.NativeDataBase.ExistsTable("Customer"));
			Assert.IsTrue(DataBase.NativeDataBase.ExistsTable("CustomerContact"));

			DataBase.Drop<Person>();
			DataBase.Drop<Employee>();
			DataBase.Drop<Customer>();
			DataBase.Drop<CustomerContact>();

			Assert.IsFalse(DataBase.NativeDataBase.ExistsTable("Person"));
			Assert.IsFalse(DataBase.NativeDataBase.ExistsTable("Employee"));
			Assert.IsFalse(DataBase.NativeDataBase.ExistsTable("Customer"));
			Assert.IsFalse(DataBase.NativeDataBase.ExistsTable("CustomerContact"));
		}

		[Test]
		public void manualMap()
		{
			Schema.Table person = new Table();
		}
	}
}