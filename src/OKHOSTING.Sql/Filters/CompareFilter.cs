using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Xml;
using System.Reflection;
using OKHOSTING.Sql.Schema;
using OKHOSTING.Core.Data;

namespace OKHOSTING.Sql.Filters
{
	/// <summary>
	/// Base class for value-to-compare filters
	/// </summary>
	public abstract class CompareFilter : ColumnFilter
	{
		/// <summary>
		/// Operator of the comparison
		/// </summary>
		public CompareOperator Operator;
	}
}