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
	public class ValueCompareFilter : CompareFilter
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
		public ValueCompareFilter(DataMember dmember, IComparable valueToCompare) : this(dmember, valueToCompare, CompareOperator.Equal) { }

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
		public ValueCompareFilter(DataMember dmember, IComparable valueToCompare, CompareOperator op) : base(dmember, op)
		{
			this.ValueToCompare = valueToCompare;
		}
	}
}