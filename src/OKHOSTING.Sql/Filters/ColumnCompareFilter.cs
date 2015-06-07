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
		public readonly Column ColumnToCompare;

		/// <summary>
		/// Constructs the filter
		/// </summary>
		/// <param name="dataMember">
		/// First Member used to comparison
		/// </param>
		/// <param name="dataValueToCompare">
		/// Second Member used to comparison
		/// </param>
		public ColumnCompareFilter(Column column, Column columnToCompare) : this(column, columnToCompare, CompareOperator.Equal) { }

		/// <summary>
		/// Constructs the filter
		/// </summary>
		/// <param name="dataMember">
		/// First Member used to comparison
		/// </param>
		/// <param name="dataValueToCompare">
		/// Second Member used to comparison
		/// </param>
		/// <param name="op">
		/// Operator of comparison
		/// </param>
		public ColumnCompareFilter(Column column, Column columnToCompare, CompareOperator op): base(column, op)
		{
			this.ColumnToCompare = columnToCompare;
		}
	}
}