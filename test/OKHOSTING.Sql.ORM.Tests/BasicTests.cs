﻿using System;
using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;
using OKHOSTING.Sql.Operations;
using OKHOSTING.Sql.Schema;
using OKHOSTING.Sql.Filters;
using OKHOSTING.Sql.ORM.Operations;

namespace OKHOSTING.Sql.ORM.Tests
{
	[TestFixture]
	public class BasicTests
	{
		DataBase DataBase;

		public BasicTests()
		{
			DataBase = new DataBase(new MySql.DataBase() { ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["mysql"].ConnectionString }, new MySql.SqlGenerator());
		}

		public void MapTypes()
		{
			var types = new Type[] 
				{ 
					typeof(Person), 
					typeof(Employee), 
					typeof(Customer), 
					typeof(CustomerContact), 
					typeof(Address), 
					typeof(Country) 
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
			DataBase.Create<Country>();
			DataBase.Create<Address>();
			DataBase.Create<Person>();
			DataBase.Create<Employee>();
			DataBase.Create<Customer>();
			DataBase.Create<CustomerContact>();

			Assert.IsTrue(DataBase.NativeDataBase.ExistsTable("test_Person"));
			Assert.IsTrue(DataBase.NativeDataBase.ExistsTable("test_Employee"));
			Assert.IsTrue(DataBase.NativeDataBase.ExistsTable("test_Customer"));
			Assert.IsTrue(DataBase.NativeDataBase.ExistsTable("test_CustomerContact"));
			Assert.IsTrue(DataBase.NativeDataBase.ExistsTable("test_Address"));
			Assert.IsTrue(DataBase.NativeDataBase.ExistsTable("test_Country"));
		}

		public void Drop()
		{
			DataBase.Drop<CustomerContact>();
			DataBase.Drop<Customer>();
			DataBase.Drop<Employee>();
			DataBase.Drop<Person>();
			DataBase.Drop<Address>();
			DataBase.Drop<Country>();

			Assert.IsFalse(DataBase.NativeDataBase.ExistsTable("test_Person"));
			Assert.IsFalse(DataBase.NativeDataBase.ExistsTable("test_Employee"));
			Assert.IsFalse(DataBase.NativeDataBase.ExistsTable("test_Customer"));
			Assert.IsFalse(DataBase.NativeDataBase.ExistsTable("test_CustomerContact"));
			Assert.IsFalse(DataBase.NativeDataBase.ExistsTable("test_Address"));
			Assert.IsFalse(DataBase.NativeDataBase.ExistsTable("test_Country"));
		}

		public void CreateAndDrop()
		{
			MapTypes();
			Create();
			Drop();
		}

		[Test]
		public void InsertTest()
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
		public void Update()
		{
			MapTypes();
			Create();

			Employee employee = new Employee();
			employee.Firstname = "Susana";
			employee.LastName = "Mendoza";
			employee.BirthDate = new DateTime(1980, 1, 1);
			employee.Salary = 1000;
			DataBase.Table<int, Employee>().Add(new KeyValuePair<int, Employee>(0, employee));

			employee.Salary = 3000;
			employee.BirthDate = new DateTime(2000, 1, 1);
			DataBase.Table<int, Employee>()[employee.Id] = employee;

			Drop();
		}

		[Test]
		public void SelectTest()
		{
			MapTypes();
			Create();

			Employee employee = new Employee();
			employee.Firstname = "Susana";
			employee.LastName = "Mendoza";
			employee.BirthDate = new DateTime(1980, 1, 1);
			employee.Salary = 1000;
			DataBase.Table<int, Employee>().Add(new KeyValuePair<int, Employee>(0, employee));

			var e = DataBase.Table<int, Employee>()[employee.Id];

			Assert.AreEqual(e.LastName, employee.LastName);

			Drop();
		}

		[Test]
		public void BigSelect()
		{
			MapTypes();
			Create();

			for (int i = 0; i < 5000; i++)
			{
				CustomerContact contact = new CustomerContact();
				
				contact.Customer = new Customer();
				contact.Customer.LegalName = "Empresa " + i;
				contact.Customer.Phone = i.ToString();
				contact.Customer.Email= "empres@a.com" + i;
				DataBase.Table<int, Customer>().Add(new KeyValuePair<int, Customer>(0, contact.Customer));
				
				contact.Firstname = "Argentina " + i;
				contact.LastName = "Chichona";
				contact.BirthDate = new DateTime(1980, 1, 1).AddDays(i);
				DataBase.Table<int, CustomerContact>().Add(new KeyValuePair<int, CustomerContact>(0, contact));
			}

			foreach (var e in DataBase.Table<int, CustomerContact>())
			{
				Console.WriteLine(e.Key + " " + e.Value.Firstname);
			}

			Drop();
		}

		[Test]
		public void ComplexSelect()
		{
			MapTypes();
			Create();

			for (int i = 0; i < 20; i++)
			{
				CustomerContact contact = new CustomerContact();

				contact.Customer = new Customer();
				contact.Customer.LegalName = "Empresa " + i;
				contact.Customer.Phone = i.ToString();
				contact.Customer.Email = "empres@a.com" + i;
				DataBase.Table<int, Customer>().Add(new KeyValuePair<int, Customer>(0, contact.Customer));

				contact.Firstname = "Argentina " + i;
				contact.LastName = "Chichona";
				contact.BirthDate = new DateTime(1980, 1, 1).AddDays(i);
				DataBase.Table<int, CustomerContact>().Add(new KeyValuePair<int, CustomerContact>(0, contact));
			}

			Operations.Select<CustomerContact> select = new Operations.Select<CustomerContact>();
			
			foreach (var member in select.From.Members)
			{
				select.Members.Add(new Operations.SelectMember(member));
			}

			Operations.SelectJoin join = new Operations.SelectJoin();
			join.JoinType = SelectJoinType.Inner;
			join.Type = typeof(Customer);
			join.On.Add(new Filters.MemberCompareFilter(){ Member = select.From["Customer.Id"], MemberToCompare = join.Type["id"], Operator = Core.Data.CompareOperator.Equal });
			select.Joins.Add(join);

			foreach (var member in join.Type.Members.Where(m=> !m.Column.IsPrimaryKey))
			{
				select.Members.Add(new Operations.SelectMember(member));
			}

			foreach (CustomerContact e in DataBase.Select(select))
			{
				Console.WriteLine(e.Firstname + " " + e.Customer.LegalName);
			}

			Drop();
		}

		[Test]
		public void ManualMap()
		{
			DataType<Person> dtype = DataType<Person>.Map();
			dtype.Map(m => m.Id);
			dtype.Map(m => m.Firstname);

			DataType<CustomerContact> dtype2 = DataType<CustomerContact>.Map();
			dtype2.Map(m => m.Id);
			dtype2.Map(m => m.Customer.Id);
			dtype2.Map(m => m.Customer.LegalName);
			dtype2.Map(m => m.Customer.Phone);
			DataBase.Create<CustomerContact>();

			for(int i = 0; i < 30; i++)
			{
				CustomerContact cc = new CustomerContact();
				cc.Firstname = "Maria " + i;
				cc.Customer = new Customer();
				cc.Customer.LegalName = "Empresa " + i;
				cc.Customer.Phone= "Telefono " + i;

				DataBase.Table<int, CustomerContact>().Add(0, cc);
			}

			foreach (var cc in DataBase.Table<int, CustomerContact>())
			{
				Console.WriteLine(cc.Value.Firstname + " " + cc.Value.Customer.LegalName);
			}
			
			DataBase.Drop<CustomerContact>();
		}

		[Test]
		public void GenericOperations()
		{
			MapTypes();
			//Drop();
			//Create();

			for (int i = 0; i < 0; i++)
			{
				CustomerContact cc = new CustomerContact();
				cc.Firstname = "Fulanito " + i;
				cc.LastName = "De tal " + i;
				cc.BirthDate = DateTime.Now;
				
				cc.Customer = new Customer();
				cc.Customer.LegalName = "Cliente " + i;
				cc.Customer.Email = "Email " + i;
				
				cc.Address1 = new Address();
				cc.Address1.Street = "Calle " + i;
				cc.Address1.Number = "# " + i;
				cc.Address1.Country = new Country();
				cc.Address1.Country.Name = "Pais " + i;

				cc.Address2 = new Address();
				cc.Address2.Street = "2 Calle " + i;
				cc.Address2.Number = "2 # " + i;
				cc.Address2.Country = cc.Address1.Country;

				DataBase.Table<int, Country>().Add(0, cc.Address1.Country);
				DataBase.Table<int, Address>().Add(0, cc.Address1);
				DataBase.Table<int, Address>().Add(0, cc.Address2);
				DataBase.Table<int, Customer>().Add(0, cc.Customer);
				DataBase.Table<int, CustomerContact>().Add(0, cc);
			}

			Select<CustomerContact> select = new Select<CustomerContact>();
			select.AddMembers
			(
				c => c.Id, 
				c => c.BirthDate, 
				c => c.Customer.LegalName, 
				c=> c.Address1.Street, 
				c=> c.Address1.Country.Name,
				c=> c.Address2.Street,
				c=> c.Address2.Country.Name
			);

			var dtype = DataType<CustomerContact>.GetMap();
			select.Where.Add(new Filters.ValueCompareFilter()
			{
				Member = dtype[x => x.Id],
				ValueToCompare = 3,
				Operator = Core.Data.CompareOperator.LessThanEqual,
			});
			
			select.OrderBy.Add(new Operations.OrderBy() { Member = dtype[x => x.Id], Direction = Core.Data.SortDirection.Descending });

			var result = DataBase.Select<CustomerContact>(select).ToList();

			//Drop();
		}
	}
}