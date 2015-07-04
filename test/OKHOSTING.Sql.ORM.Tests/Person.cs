using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OKHOSTING.Sql.ORM.Tests
{
	public class Person
	{
		public int Id;
		public string Firstname;
		public string LastName;
		public DateTime BirthDate { get; set; }

		public string FullName
		{
			get
			{
				return Firstname + " " + LastName;
			}
		}
	}
}