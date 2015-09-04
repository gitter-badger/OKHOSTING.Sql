using System;
using System.Collections.Generic;

namespace OKHOSTING.Sql.ORM.Tests
{
	public static class CodeGeneration
	{
		public static void GenerateWebForms()
		{
			Type[] types = new Type[] { typeof(Person), typeof(Employee), typeof(Customer), typeof(CustomerContact), typeof(Address), typeof(Country) };

			DataType.DefaultMap(types);
		}
	}
}
