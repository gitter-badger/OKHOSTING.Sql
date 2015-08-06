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
		public IComparable ValueToCompare;

		public ValueCompareFilter()
		{
		}

		public ValueCompareFilter(DataMember member, IComparable valueToCompare): this(member, valueToCompare, CompareOperator.Equal)
		{
		}

		public ValueCompareFilter(DataMember member, IComparable valueToCompare, CompareOperator compareOperator)
		{
			Member = member;
			ValueToCompare = valueToCompare;
			Operator = compareOperator;
		}
	}
}