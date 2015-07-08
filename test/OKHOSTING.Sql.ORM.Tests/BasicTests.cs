using System;
using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;
using OKHOSTING.Sql.Operations;
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

		public void MapTypes()
		{
			var types = new Type[] 
				{ 
					typeof(Person), 
					typeof(Employee), 
					typeof(Customer), 
					typeof(CustomerContact) 
				};

			var dtypes = DataType.DefaultMap(types).ToList();

			//add prefixes to tables
			foreach (var dtype in dtypes)
			{
				dtype.Table.Name = "test_" + dtype.Table.Name;
			}
		}

		public void Create()
		{
			DataBase.Create<Person>();
			DataBase.Create<Employee>();
			DataBase.Create<Customer>();
			DataBase.Create<CustomerContact>();

			Assert.IsTrue(DataBase.NativeDataBase.ExistsTable("test_Person"));
			Assert.IsTrue(DataBase.NativeDataBase.ExistsTable("test_Employee"));
			Assert.IsTrue(DataBase.NativeDataBase.ExistsTable("test_Customer"));
			Assert.IsTrue(DataBase.NativeDataBase.ExistsTable("test_CustomerContact"));
		}

		public void Drop()
		{
			DataBase.Drop<CustomerContact>();
			DataBase.Drop<Customer>();
			DataBase.Drop<Employee>();
			DataBase.Drop<Person>();

			Assert.IsFalse(DataBase.NativeDataBase.ExistsTable("test_Person"));
			Assert.IsFalse(DataBase.NativeDataBase.ExistsTable("test_Employee"));
			Assert.IsFalse(DataBase.NativeDataBase.ExistsTable("test_Customer"));
			Assert.IsFalse(DataBase.NativeDataBase.ExistsTable("test_CustomerContact"));
		}

		public void CreateAndDrop()
		{
			MapTypes();
			Create();
			Drop();
		}

		[Test]
		public void Insert()
		{
			MapTypes();
			Create();

			Employee employee = new Employee();
			employee.Firstname = "Susana";
			employee.LastName = "Mendoza";
			employee.BirthDate = new DateTime(1980, 1, 1);
			employee.Salary = 1000;
			DataBase.Table<int, Employee>().Add(new KeyValuePair<int, Employee>(0, employee));

			Drop();
		}


		[Test]
		public void manualMap()
		{
			Schema.Table person = new Table();
		}
	}
}