using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OKHOSTING.Sql.Schema
{
	/// <summary>
	/// A view in a DataBase
	/// </summary>
	[System.ComponentModel.DefaultProperty("Name")]
	public class View
	{
		public int Id { get; set; }
		/// <summary>
		/// The name of the table
		/// </summary>
		public string Name { get; set; }
		public string Description { get; set; }
		public string Command { get; set; }
		public DataBaseSchema DataBase { get; set; }
	}
}