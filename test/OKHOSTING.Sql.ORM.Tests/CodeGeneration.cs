using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace OKHOSTING.Sql.ORM.Tests
{
	public static class CodeGeneration
	{
		[Test]
		public static void GenerateWebForms()
		{
			Type[] types = new Type[] { typeof(Person), typeof(Employee), typeof(Customer), typeof(CustomerContact), typeof(Address), typeof(Country) };

			var dtypes = DataType.DefaultMap(types);
			OKHOSTING.Sql.ORM.UI.Web.Forms.CodeGenerator.Generate(dtypes, @"C:\Pruebas\Test1");
		}
	}
}
