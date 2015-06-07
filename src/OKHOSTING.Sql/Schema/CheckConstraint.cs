using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OKHOSTING.Sql.Schema
{
	/// <summary>
	/// A DataBase contrsint
	/// </summary>
	public class CheckConstraint
	{
		/// <summary>
		/// The table that contains this constraint
		/// </summary>
		public Table Table { get; set; }

		/// <summary>
		/// The expression of the check constraint
		/// </summary>
		public string Expression { get; set; }
	}
}