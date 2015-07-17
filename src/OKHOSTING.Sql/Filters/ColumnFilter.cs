using System;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Xml;
using OKHOSTING.Sql.Schema;

namespace OKHOSTING.Sql.Filters
{
	/// <summary>
	/// Represents a filter based on a value of a object (Field or Property)
	/// </summary>
	public abstract class ColumnFilter : FilterBase
	{
		/// <summary>
		/// DataMember used to filter
		/// </summary>
		public Column Column;
		
		/// <summary>
		/// Optional alias used for the table
		/// </summary>
		public string TableAlias;
	}
}