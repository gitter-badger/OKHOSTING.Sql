using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Xml;
using System.Reflection;
using OKHOSTING.Core.Data;

namespace OKHOSTING.Sql.ORM.Filters
{
	/// <summary>
	/// Base class for value-to-compare filters
	/// </summary>
	public abstract class CompareFilter : MemberFilter
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
		protected CompareFilter(DataMember dmember) : this(dmember, CompareOperator.Equal) { }

		/// <summary>
		/// Construct the filter
		/// </summary>
		/// <param name="dataMember">
		/// DataMember to compare
		/// </param>
		/// <param name="op">
		/// Operator to use in the comparison
		/// </param>
		protected CompareFilter(DataMember dmember, CompareOperator op): base(dmember)
		{
			this.Operator = op;
		}
	}
}