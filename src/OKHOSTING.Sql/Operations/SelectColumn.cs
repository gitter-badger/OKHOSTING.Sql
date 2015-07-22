using OKHOSTING.Sql.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace OKHOSTING.Sql.Operations
{
	/// <summary>
	/// Defines a field on a select query that can contain an alias
	/// </summary>
	public class SelectColumn
	{
		public int Id { get; set; }

		/// <summary>
		/// Alias name of the resulting field
		/// </summary>
		public string Alias;

		/// <summary>
		/// DataMember for build the field definition
		/// </summary>
		public Column Column;

		/// <summary>
		/// Constructs the AggegateSelectField
		/// </summary>
		/// <param name="dataValue">
		/// DataMember for build the field definition
		/// </param>
		public SelectColumn(Column column): this(column, null)
		{ 
		}

		/// <summary>
		/// Constructs the AggegateSelectField
		/// </summary>
		/// <param name="dataValue">
		/// DataMember for build the field definition
		/// </param>
		/// <param name="alias">
		/// Alias name of the resulting field
		/// </param>
		public SelectColumn(Column column, string alias)
		{
			if (column == null)
			{
				throw new ArgumentNullException("column");
			}

			Column = column;
			Alias = alias;
		}

		public static implicit operator SelectColumn(Column column)
		{
			return new SelectColumn(column);
		}
	}
}