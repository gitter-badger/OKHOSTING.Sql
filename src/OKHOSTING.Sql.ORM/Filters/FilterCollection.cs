using System;
using System.Collections.Generic;

namespace OKHOSTING.Sql.ORM.Filters
{
	/// <summary>
	/// Collection of filters
	/// </summary>
	public class FilterCollection: List<FilterBase>
	{
		/// <summary>
		/// Contruct the collection
		/// </summary>
		public FilterCollection()
		{
		}

		/// <summary>
		/// Constructs the collection
		/// </summary>
		/// <param name="collection">
		/// Array of filters to append to the collection
		/// </param>
		public FilterCollection(params FilterBase[] collection)
		{
			this.AddRange(collection);
		}
	}
}