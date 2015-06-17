using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OKHOSTING.Sql.Schema
{
	/// <summary>
	/// Represents a foreign key in a database
	/// </summary>
	[System.ComponentModel.DefaultProperty("Name")]
	public class ForeignKey
	{
		public ForeignKey()
		{
		}

		/// <summary>
		/// Name of the foreign key
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Table where this foreign key is defined
		/// </summary>
		public Table Table { get; set; }

		/// <summary>
		/// Table where this foreign key points to
		/// </summary>
		public Table RemoteTable { get; set; }

		/// <summary>
		/// Action on update
		/// </summary>
		public ConstraintAction UpdateAction { get; set; }

		/// <summary>
		/// Action on delete
		/// </summary>
		public ConstraintAction DeleteAction { get; set; }

		/// <summary>
		/// The list of local and remote columns, linked in a tuple
		/// </summary>
		public readonly List<Tuple<Column, Column>> Columns = new List<Tuple<Column, Column>>();

		public override string ToString()
		{
			return Name;
		}
	}
}