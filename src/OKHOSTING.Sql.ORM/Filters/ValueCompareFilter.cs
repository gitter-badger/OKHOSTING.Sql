using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Xml;
using OKHOSTING.Core.Data;

namespace OKHOSTING.Sql.ORM.Filters
{
	/// <summary>
	/// Compare a DataValue with a value
	/// </summary>
	public class ValueCompareFilter : CompareFilter
	{
		/// <summary>
		/// Value for comparison
		/// </summary>
		public IComparable ValueToCompare;
	}
}