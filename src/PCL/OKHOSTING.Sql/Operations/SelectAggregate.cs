using OKHOSTING.Sql.Schema;
using System.Collections.Generic;

namespace OKHOSTING.Sql.Operations
{
	/// <summary>
	/// A select statement that also contains aggreate functions, which requier a groupby element as well
	/// </summary>
	public class SelectAggregate: Select
	{
		public readonly List<SelectAggregateColumn> AggregateColumns = new List<SelectAggregateColumn>();
		public readonly List<Column> GroupBy = new List<Column>();
	}
}