using OKHOSTING.Core.Data;

namespace OKHOSTING.Sql.ORM.Filters
{

	/// <summary>
	/// Filter defined with several conditions merged between them,
	/// with a logical And operator
	/// </summary>
	public class AndFilter : LogicalOperatorFilter
	{
		/// <summary>
		/// Constructs the filter
		/// </summary>
		/// <param name="innerFilters">
		/// Collection of conditions or filters that will be merged 
		/// with the And operator
		/// </param>
		public AndFilter(FilterCollection innerFilters) : base(innerFilters, LogicalOperator.And) { }

		/// <summary>
		/// Constructs the class
		/// </summary>
		public AndFilter() : this(new FilterCollection()) { }

		/// <summary>
		/// Constructs the class
		/// </summary>
		/// <param name="filter">
		/// Filter used on the evaluation
		/// </param>
		public AndFilter(AndFilter filter) : this(filter.InnerFilters) { }

	}
}