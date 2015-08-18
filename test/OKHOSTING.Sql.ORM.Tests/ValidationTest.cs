using System;
using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;
using OKHOSTING.Core.Data.Validation;

namespace OKHOSTING.Test111
{
	[TestFixture]
	public class ValidationTest
	{
		[Test]
		public void DataTypeTest()
		{
			var dtype = new DataType<OKHOSTING.Sql.ORM.Tests.Person>();
			//dtype.Validators.Add(new StringLengthValidator(){ Member = dtype[m=> m.Firstname], Operator
		}
	}
}