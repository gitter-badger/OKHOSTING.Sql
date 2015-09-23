using System;
using System.Collections.Generic;

namespace OKHOSTING.Sql.Filters
{
	/// <summary>
	/// Defines a filter in which the DataMember must be part
	/// of a values set
	/// </summary>
	/// <typeparam name="T">
	/// Type of the items in the values set
	/// </typeparam>
	public class InFilter: ColumnFilter
	{
		/// <summary>
		/// List of values of the filter
		/// </summary>
		public readonly List<IComparable> Values = new List<IComparable>();

		/// <summary>
		/// Indicates if the filter comparison will be case sensitive
		/// when listItemsType = System.String
		/// </summary>
		public bool CaseSensitive { get; set; }
	}
}