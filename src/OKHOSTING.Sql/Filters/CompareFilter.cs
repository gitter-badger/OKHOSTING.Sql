using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Xml;
using System.Reflection;
using OKHOSTING.Sql.Schema;
using OKHOSTING.Core.Data;

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
		public readonly CompareOperator Operator;

		/// <summary>
		/// Construct the filter
		/// </summary>
		/// <param name="dataMember">
		/// DataMember to compare
		/// </param>
		protected CompareFilter(Column column) : this(column, CompareOperator.Equal) { }

		/// <summary>
		/// Construct the filter
		/// </summary>
		/// <param name="dataMember">
		/// DataMember to compare
		/// </param>
		/// <param name="op">
		/// Operator to use in the comparison
		/// </param>
		protected CompareFilter(Column column, CompareOperator op): base(column)
		{
			this.Operator = op;
		}
	}
}