using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Xml;
using OKHOSTING.Core.Data;

namespace OKHOSTING.Sql.ORM.Filters
{
	/// <summary>
	/// Compare a DataValue with a value
	/// </summary>
	public class ValueCompareFilter<T> : CompareFilter<T>
	{
		/// <summary>
		/// Value for comparison
		/// </summary>
		public readonly IComparable ValueToCompare;

		/// <summary>
		/// Constructs the filter
		/// </summary>
		/// <param name="dataMember">
		///  DataValue for the comparison
		/// </param>
		/// <param name="valueToCompare">
		/// Value for comparison
		/// </param>
		public ValueCompareFilter(System.Linq.Expressions.Expression<Func<T, object>> member, IComparable valueToCompare) : this(member, valueToCompare, CompareOperator.Equal) { }

		/// <summary>
		/// Constructs the filter
		/// </summary>
		/// <param name="dataMember">
		///  DataValue for the comparison
		/// </param>
		/// <param name="valueToCompare">
		/// Value for comparison
		/// </param>
		/// <param name="op">
		/// Operator for comparison
		/// </param>
		public ValueCompareFilter(System.Linq.Expressions.Expression<Func<T, object>> member, IComparable valueToCompare, CompareOperator op) : base(member, op)
		{
			this.ValueToCompare = valueToCompare;
		}
	}
}