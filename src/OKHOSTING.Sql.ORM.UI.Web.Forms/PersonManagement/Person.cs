using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OKHOSTING.Sql.ORM.UI.Web.PersonManagement
{
	public class Person
	{
		public int Id;
		
		[System.ComponentModel.DataAnnotations.StringLength(100)]
		public string FirstName;

		[System.ComponentModel.DataAnnotations.StringLength(100)]
		public string LastName;

		public DateTime BirthDate { get; set; }

		public bool IsAlive { get; set; }
		
		public string FullName
		{
			get
			{
				return FirstName + " " + LastName;
			}
		}
	}
}