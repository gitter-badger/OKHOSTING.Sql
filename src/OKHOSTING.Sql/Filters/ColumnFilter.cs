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
		public readonly Column Column;

		/// <summary>
		/// Creates the filter
		/// </summary>
		/// <param name="column">
		/// column used to filter
		/// </param>
		protected ColumnFilter(Column column)
		{
			//Establishing the DataMember to filter
			this.Column = column;
		}
	}
}