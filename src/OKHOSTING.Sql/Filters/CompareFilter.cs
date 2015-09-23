using OKHOSTING.Data;

namespace OKHOSTING.Sql.Filters
{
	/// <summary>
	/// Base class for value-to-compare filters
	/// </summary>
	public abstract class CompareFilter : ColumnFilter
	{
		/// <summary>
		/// Operator of the comparison
		/// </summary>
		public CompareOperator Operator;
	}
}