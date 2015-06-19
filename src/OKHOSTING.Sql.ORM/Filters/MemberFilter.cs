using System;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Xml;

namespace OKHOSTING.Sql.ORM.Filters
{
	/// <summary>
	/// Represents a filter based on a value of a object (Field or Property)
	/// </summary>
	public abstract class MemberFilter : FilterBase
	{
		/// <summary>
		/// DataMember used to filter
		/// </summary>
		public readonly MemberMap Member;

		/// <summary>
		/// Creates the filter
		/// </summary>
		/// <param name="dataMember">
		/// DataMember used to filter
		/// </param>
		protected MemberFilter(MemberMap member)
		{
			//Establishing the DataMember to filter
			this.Member = member;
		}
	}

	public abstract class MemberFilter<T> : MemberFilter
	{
		/// <summary>
		/// Creates the filter
		/// </summary>
		/// <param name="dataMember">
		/// DataMember used to filter
		/// </param>
		protected MemberFilter(System.Linq.Expressions.Expression<Func<T, object>> member): base((MemberMap<T>) member)
		{
		}
	}
}