using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace OKHOSTING.Sql.ORM.Operations
{
	/// <summary>
	/// Defines a field on a select query that will be processed 
	/// with an aggregate function on a ResultSet grouped
	/// </summary>
	public struct SelectAggregateColumn
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
		/// Alias name of the resulting field
		/// </summary>
		public string Alias;

		/// <summary>
		/// DataMember for build the field definition
		/// </summary>
		public DataMember Member;

		/// <summary>
		/// Constructs the AggegateSelectField
		/// </summary>
		/// <param name="dataValue">
		/// DataMember for build the field definition
		/// </param>
		public SelectAggregateColumn(DataMember member)
			: this(member, SelectAggregateFunction.None) 
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
		public SelectAggregateColumn(DataMember member, SelectAggregateFunction aggregateFunction) : this(member, aggregateFunction, string.Empty) { }

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
		public SelectAggregateColumn(DataMember member, SelectAggregateFunction aggregateFunction, string alias) : this(member, aggregateFunction, alias, false) { }

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
		public SelectAggregateColumn(DataMember member, SelectAggregateFunction aggregateFunction, string alias, bool distinct)
		{
			if (member == null)
			{
				throw new ArgumentNullException("member");
			}

			Member = member;
			AggregateFunction = aggregateFunction;
			Distinct = distinct;
			Alias = alias;
		}
	}
}