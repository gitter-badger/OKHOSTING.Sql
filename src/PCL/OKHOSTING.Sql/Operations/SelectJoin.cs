using OKHOSTING.Sql.Schema;
using System.Collections.Generic;

namespace OKHOSTING.Sql.Operations
{
	/// <summary>
	/// Represents a select join
	/// </summary>
	public class SelectJoin
	{
		public int Id { get; set; }

		public Table Table { get; set; }
		
		/// <summary>
		/// Aditional columns that will be pat of the "Select" statement
		/// </summary>
		public readonly List<SelectColumn> Columns = new List<SelectColumn>();

		public SelectJoinType JoinType { get; set; }

		/// <summary>
		/// Alias name of the joined table
		/// </summary>
		/// <remarks>
		/// Usefull in case a DataType has 2 foreign keys to the same table, son you can query both tables in a select
		/// </remarks>
		public string Alias;
		
		public readonly List<Filters.FilterBase> On = new List<Filters.FilterBase>();
	}
}