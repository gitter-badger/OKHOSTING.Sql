using System;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Xml;
using System.IO;
using OKHOSTING.Sql.Schema;

namespace OKHOSTING.Sql.Filters
{
	/// <summary>
	/// Implements a filter criterion based on a value range
	/// </summary>
	public class RangeFilter: ColumnFilter
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
		public RangeFilter(Column column, IComparable minValue, IComparable maxValue): base(column)
		{
			MinValue = minValue;
			MaxValue = maxValue;
		}
	}
}