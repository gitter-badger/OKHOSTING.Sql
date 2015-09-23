using OKHOSTING.Data;
using System.Collections.Generic;

namespace OKHOSTING.Sql.Filters
{
	
	/// <summary>
	/// Base class for filters that contains a collection of filters and
	/// that compares them with a logical operator
	/// </summary>
	public class LogicalOperatorFilter : FilterBase
	{ 
		/// <summary>
		/// Collection of conditions or filters that will be merged 
		/// with the and operator
		/// </summary>
		public readonly List<Filters.FilterBase> InnerFilters = new List<FilterBase>();

		/// <summary>
		/// Logical operator used in the filter
		/// </summary>
		public LogicalOperator LogicalOperator { get; set; }

		/// <summary>
		/// Constructs the filter
		/// </summary>
		public LogicalOperatorFilter(List<Filters.FilterBase> innerFilters) : this(innerFilters, LogicalOperator.And) { }

		/// <summary>
		/// Constructs the filter
		/// </summary>
		/// <param name="logicalOperator">
		/// Logical operator used in the filter
		/// </param>
		/// <param name="innerFilters">
		/// Collection of conditions or filters that will be merged 
		/// with the and operator
		/// </param>
		public LogicalOperatorFilter(List<Filters.FilterBase> innerFilters, LogicalOperator logicalOperator)
		{
			InnerFilters = innerFilters;
			LogicalOperator = logicalOperator;
		}
	}
}