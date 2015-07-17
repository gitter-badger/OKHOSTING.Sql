using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OKHOSTING.Sql.ORM.Tests
{
	public class Address
	{
		public int Id;
		
		[System.ComponentModel.DataAnnotations.StringLength(100)]
		public string Street;

		[System.ComponentModel.DataAnnotations.StringLength(100)]
		public string Number;
		
		public Country Country { get; set; }
	}
}