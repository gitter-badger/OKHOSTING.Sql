using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OKHOSTING.Sql.ORM.Tests
{
	public class Customer
	{
		public int Id;

		[System.ComponentModel.DataAnnotations.StringLength(100)]
		public string LegalName { get; set; }

		[System.ComponentModel.DataAnnotations.StringLength(100)]
		public string Phone { get; set; }
		
		public string Email { get; set; }
	}
}