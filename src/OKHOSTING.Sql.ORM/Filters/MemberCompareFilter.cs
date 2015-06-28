using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Xml;
using OKHOSTING.Core.Data;

namespace OKHOSTING.Sql.ORM.Filters
{
	/// <summary>
	/// Compares two Member's on the same object
	/// </summary>
	public class MemberCompareFilter : CompareFilter
	{
		/// <summary>
		/// Member to compare with the Member defined in the 
		/// Field with the same name
		/// </summary>
		public readonly DataMember MemberToCompare;

		/// <summary>
		/// Constructs the filter
		/// </summary>
		/// <param name="dataMember">
		/// First Member used to comparison
		/// </param>
		/// <param name="dataValueToCompare">
		/// Second Member used to comparison
		/// </param>
		public MemberCompareFilter(DataMember dmember, DataMember dmemberToCompare) : this(dmember, dmemberToCompare, CompareOperator.Equal) { }

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
		public MemberCompareFilter(DataMember member, DataMember memberToCompare, CompareOperator op): base(member, op)
		{
			this.MemberToCompare = memberToCompare;
		}
	}
}