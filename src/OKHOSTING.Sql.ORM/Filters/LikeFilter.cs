using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Xml;

namespace OKHOSTING.Sql.ORM.Filters
{
	/// <summary>
	/// Represents a Filter with the Like operator
	/// </summary>
	public class LikeFilter<T> : MemberFilter<T>
	{
		/// <summary>
		/// Defines the pattern for the filter
		/// </summary>
		public readonly string Pattern;

		/// <summary>
		/// Indicates if the filter comparison will be case sensitive
		/// </summary>
		public readonly bool CaseSensitive;

		/// <summary>
		/// Construct the filter
		/// </summary>
		/// <param name="dataMember">
		/// DataMember used to filter
		/// </param>
		/// <param name="pattern">
		/// Defines the pattern for the filter
		/// </param>
		public LikeFilter(System.Linq.Expressions.Expression<Func<T, object>> member, string pattern) : this(member, pattern, false) { }

		/// <summary>
		/// Construct the filter
		/// </summary>
		/// <param name="dataMember">
		/// DataMember used to filter
		/// </param>
		/// <param name="pattern">
		/// Defines the pattern for the filter
		/// </param>
		/// <param name="caseSensitive">
		/// Indicates if the filter comparison will be case sensitive
		/// </param>
		public LikeFilter(System.Linq.Expressions.Expression<Func<T, object>> member, string pattern, bool caseSensitive): base(member)
		{
			if (string.IsNullOrWhiteSpace(pattern))
			{
				throw new ArgumentNullException("pattern");
			}

			Pattern = pattern;
			CaseSensitive = caseSensitive;
		}
	}
}