using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Xml;
using OKHOSTING.Sql.Schema;
using OKHOSTING.Core.Data;

namespace OKHOSTING.Sql.Filters
{
	/// <summary>
	/// Compares two Member's on the same object
	/// </summary>
	public class ColumnCompareFilter : CompareFilter
	{
		/// <summary>
		/// Member to compare with the Member defined in the 
		/// Field with the same name
		/// </summary>
		public Column ColumnToCompare;

		/// <summary>
		/// Optional alias used for the table
		/// </summary>
		public string ColumnToCompareTableAlias;
	}
}