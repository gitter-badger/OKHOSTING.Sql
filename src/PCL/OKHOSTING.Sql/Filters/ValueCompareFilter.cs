using System;

namespace OKHOSTING.Sql.Filters
{
	/// <summary>
	/// Compare a DataMember with a value
	/// </summary>
	public class ValueCompareFilter : CompareFilter
	{
		/// <summary>
		/// Value for comparison
		/// </summary>
		public IComparable ValueToCompare;
	}
}