using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Xml;

namespace OKHOSTING.Sql.ORM.Filters
{
	/// <summary>
	/// Compares two Member's on the same object
	/// </summary>
	public class MemberCompareFilter<T> : CompareFilter<T>
	{
		/// <summary>
		/// Member to compare with the Member defined in the 
		/// Field with the same name
		/// </summary>
		public readonly MemberMap<T> MemberToCompare;

		/// <summary>
		/// Constructs the filter
		/// </summary>
		/// <param name="dataMember">
		/// First Member used to comparison
		/// </param>
		/// <param name="dataValueToCompare">
		/// Second Member used to comparison
		/// </param>
		public MemberCompareFilter(System.Linq.Expressions.Expression<Func<T, object>> dataMember, System.Linq.Expressions.Expression<Func<T, object>> dataValueToCompare): this(dataMember, dataValueToCompare, CompareOperator.Equal) { }

		/// <summary>
		/// Constructs the filter
		/// </summary>
		/// <param name="dataMember">
		/// First Member used to comparison
		/// </param>
		/// <param name="dataValueToCompare">
		/// Second Member used to comparison
		/// </param>
		/// <param name="op">
		/// Operator of comparison
		/// </param>
		public MemberCompareFilter(System.Linq.Expressions.Expression<Func<T, object>> dataMember, System.Linq.Expressions.Expression<Func<T, object>> dataValueToCompare, CompareOperator op): base(dataMember, op)
		{
			this.MemberToCompare = dataValueToCompare;
		}
	}
}