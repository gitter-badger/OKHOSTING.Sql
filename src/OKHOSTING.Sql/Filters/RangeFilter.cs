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
		public IComparable MinValue;
		
		/// <summary>
		/// Maximum value of the allowed range
		/// </summary>
		public IComparable MaxValue;
	}
}