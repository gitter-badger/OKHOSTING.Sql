using OKHOSTING.Data.Sql.ORM.Generators;
using System;
using System.Xml.Serialization;

namespace OKHOSTING.Sql.ORM.Filters
{
	/// <summary>
	/// Represents a Filter to apply on a object
	/// </summary>
	public abstract class FilterBase
	{
		/// <summary>
		/// Returns the SQL string of the filter
		/// </summary>
		/// <returns>
		/// SQL string of the filter
		/// </returns>
		public abstract string GetSqlFilter();
	}

	public abstract class FilterBase<T> : FilterBase
	{
	}
}