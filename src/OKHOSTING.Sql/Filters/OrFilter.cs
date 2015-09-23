using OKHOSTING.Data;
using System.Collections.Generic;

namespace OKHOSTING.Sql.Filters
{
	/// <summary>
	/// Filter defined with several conditions merged between them,
	/// with a logical Or operator
	/// </summary>
	public class OrFilter : LogicalOperatorFilter
	{
		/// <summary>
		/// Constructs the filter
		/// </summary>
		/// <param name="innerFilters">
		/// Collection of conditions or filters that will be merged 
		/// with the Or operator
		/// </param>
		public OrFilter(): this(null)
		{
		}

		/// <summary>
		/// Constructs the filter
		/// </summary>
		/// <param name="innerFilters">
		/// Collection of conditions or filters that will be merged 
		/// with the Or operator
		/// </param>
		public OrFilter(List<Filters.FilterBase> innerFilters) : base(innerFilters, LogicalOperator.Or) 
		{ 
		}
	}
}