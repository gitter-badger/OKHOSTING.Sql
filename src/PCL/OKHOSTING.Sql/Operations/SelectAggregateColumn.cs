using OKHOSTING.Sql.Schema;

namespace OKHOSTING.Sql.Operations
{
	/// <summary>
	/// Defines a field on a select query that will be processed 
	/// with an aggregate function on a ResultSet grouped
	/// </summary>
	public class SelectAggregateColumn: SelectColumn
	{
		/// <summary>
		/// Aggregate function to use for calculate the column
		/// </summary>
		public SelectAggregateFunction AggregateFunction;

		/// <summary>
		/// Speficy if the DISTINCT modifier must be applied
		/// </summary>
		public bool Distinct;

		/// <summary>
		/// Constructs the AggegateSelectField
		/// </summary>
		/// <param name="dataValue">
		/// DataMember for build the field definition
		/// </param>
		public SelectAggregateColumn(Column column): this(column, SelectAggregateFunction.None) 
		{ 
		}

		/// <summary>
		/// Constructs the AggegateSelectField
		/// </summary>
		/// <param name="dataValue">
		/// DataMember for build the field definition
		/// </param>
		/// <param name="aggregateFunction">
		/// Aggregate function to use for calculate the column
		/// </param>
		public SelectAggregateColumn(Column column, SelectAggregateFunction aggregateFunction) : this(column, aggregateFunction, string.Empty) { }

		/// <summary>
		/// Constructs the AggegateSelectField
		/// </summary>
		/// <param name="dataValue">
		/// DataMember for build the field definition
		/// </param>
		/// <param name="aggregateFunction">
		/// Aggregate function to use for calculate the column
		/// </param>
		/// <param name="alias">
		/// Alias name of the resulting field
		/// </param>
		public SelectAggregateColumn(Column column, SelectAggregateFunction aggregateFunction, string alias) : this(column, aggregateFunction, alias, false) { }

		/// <summary>
		/// Constructs the AggegateSelectField
		/// </summary>
		/// <param name="dataValue">
		/// DataMember for build the field definition
		/// </param>
		/// <param name="aggregateFunction">
		/// Aggregate function to use for calculate the column
		/// </param>
		/// <param name="distinct">
		/// Speficy if the DISTINCT modifier must be applied
		/// </param>
		/// <param name="alias">
		/// Alias name of the resulting field
		/// </param>
		public SelectAggregateColumn(Column column, SelectAggregateFunction aggregateFunction, string alias, bool distinct): base(column, alias)
		{
			AggregateFunction = aggregateFunction;
			Distinct = distinct;
		}
	}
}