using System;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Xml;

namespace OKHOSTING.Sql.ORM.Filters
{

	/// <summary>
	/// Represents a custom filter
	/// </summary>
	public class CustomFilter<T> : FilterBase<T>
	{
		/// <summary>
		/// Sql filter expression
		/// </summary>
		public string Filter;

		/// <summary>
		/// Constructs the filter
		/// </summary>
		/// <param name="filtering">
		/// Event handler of custom filter
		/// </param>
		public CustomFilter(string filter)
		{
			if (string.IsNullOrWhiteSpace(filter))
			{
				throw new ArgumentNullException("filter");
			}

			this.Filter = filter;
		}

		public override string GetSqlFilter()
		{
			return Filter;
		}
	}
}