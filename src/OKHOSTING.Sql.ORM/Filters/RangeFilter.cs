using System;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Xml;
using System.IO;

namespace OKHOSTING.Sql.ORM.Filters
{
	/// <summary>
	/// Implements a filter criterion based on a value range
	/// </summary>
	public class RangeFilter<T>: MemberFilter<T>
	{
		/// <summary>
		/// Minimum value of the allowed range
		/// </summary>
		public readonly IComparable MinValue;
		
		/// <summary>
		/// Maximum value of the allowed range
		/// </summary>
		public readonly IComparable MaxValue;

		/// <summary>
		/// Constructs the filter 
		/// </summary>
		/// <param name="dataMember">
		/// Member that must be on the specified range
		/// </param>
		/// <param name="minValue">
		/// Minimum value of the allowed range
		/// </param>
		/// <param name="maxValue">
		/// Maximum value of the allowed range
		/// </param>
		public RangeFilter(System.Linq.Expressions.Expression<Func<T, object>> member, IComparable minValue, IComparable maxValue): base(member)
		{
			MinValue = minValue;
			MaxValue = maxValue;
		}
	}
}