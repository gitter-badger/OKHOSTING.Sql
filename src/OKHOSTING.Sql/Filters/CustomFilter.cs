using System;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Xml;

namespace OKHOSTING.Sql.Filters
{

	/// <summary>
	/// Represents a custom filter
	/// </summary>
	public class CustomFilter : FilterBase
	{
		/// <summary>
		/// Sql filter expression
		/// </summary>
		public string Filter { get; set; }
	}
}