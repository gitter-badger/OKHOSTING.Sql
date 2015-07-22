using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OKHOSTING.Sql.Schema
{
	/// <summary>
	/// A user that has access to a database
	/// </summary>
	[System.ComponentModel.DefaultProperty("Name")]
	public class User
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public DataBaseSchema DataBase { get; set; }
	}
}