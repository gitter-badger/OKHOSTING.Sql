using System;
using System.Linq;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Xml;
using System.IO;

namespace OKHOSTING.Sql.ORM.Filters
{
	/// <summary>
	/// Represents a Filter by Primary Key
	/// </summary>
	public class PrimaryKeyFilter<T>: FilterBase<T>
	{
		/// <summary>
		/// Inner object that will be compared in the Match method
		/// </summary>
		/// <remarks>
		/// If the Type has a multi-member primary key, you should assign here an actual object with all PK members assigned.
		/// Otherwise you can use an atomic value like an int or a string
		/// </remarks>
		public readonly object PrimaryKey;

		/// <summary>
		/// Constructs the filter
		/// </summary>
		/// <param name="obj">
		/// Inner object that will be compared in the Match method
		/// </param>
		public PrimaryKeyFilter(object primaryKey)
		{
			if (primaryKey == null)
			{
				throw new ArgumentNullException("primaryKey");
			}

			PrimaryKey = primaryKey;
		}
	}
}