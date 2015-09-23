using System;

namespace OKHOSTING.Sql.Filters
{
	/// <summary>
	/// Implements a filter criterion based on a value range
	/// </summary>
	public class RangeFilter: ColumnFilter
	{
		/// <summary>
		/// Minimum value of the allowed range
		/// </summary>
		public IComparable MinValue;
		
		/// <summary>
		/// Maximum value of the allowed range
		/// </summary>
		public IComparable MaxValue;
	}
}