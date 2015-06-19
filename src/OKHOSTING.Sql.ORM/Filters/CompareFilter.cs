using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Xml;
using System.Reflection;

namespace OKHOSTING.Sql.ORM.Filters
{
	/// <summary>
	/// Base class for value-to-compare filters
	/// </summary>
	public abstract class CompareFilter<T> : MemberFilter<T>
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
		protected CompareFilter(System.Linq.Expressions.Expression<Func<T, object>> member) : this(member, CompareOperator.Equal) { }

		/// <summary>
		/// Construct the filter
		/// </summary>
		/// <param name="dataMember">
		/// DataMember to compare
		/// </param>
		/// <param name="op">
		/// Operator to use in the comparison
		/// </param>
		protected CompareFilter(System.Linq.Expressions.Expression<Func<T, object>> member, CompareOperator op): base(member)
		{
			this.Operator = op;
		}
	}
}