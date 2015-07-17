using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Xml;
using OKHOSTING.Sql.Schema;

namespace OKHOSTING.Sql.Filters
{
	/// <summary>
	/// Represents a Filter with the Like operator
	/// </summary>
	public class LikeFilter : ColumnFilter
	{
		/// <summary>
		/// Defines the pattern for the filter
		/// </summary>
		public string Pattern { get; set; }

		/// <summary>
		/// Indicates if the filter comparison will be case sensitive
		/// </summary>
		public bool CaseSensitive { get; set; }
	}
}