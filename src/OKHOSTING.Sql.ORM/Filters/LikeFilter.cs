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
	public class LikeFilter : MemberFilter
	{
		/// <summary>
		/// Defines the pattern for the filter
		/// </summary>
		public string Pattern;

		/// <summary>
		/// Indicates if the filter comparison will be case sensitive
		/// </summary>
		public bool CaseSensitive;
	}
}