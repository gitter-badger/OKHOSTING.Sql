using System;
using System.Linq;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Xml;

namespace OKHOSTING.Sql.ORM.Filters
{
	/// <summary>
	/// Implements a filter based on the comparison
	/// of a foreign key
	/// </summary>
	public class ForeignKeyFilter<T> : MemberFilter<T>
	{
		/// <summary>
		/// The actual value that a foreign key should have in order to match this filter
		/// </summary>
		public readonly object ForeignKey;

		/// <summary>
		/// Constructs the filter
		/// </summary>
		/// <param name="dataMember">
		/// DataMember used to link the local with the foreign object
		/// </param>
		/// <param name="foreignKey">
		/// Foreign object
		/// </param>
		public ForeignKeyFilter(DataMember<T> member, object foreignKey): base(member)
		{
			ForeignKey = foreignKey;
		}
	}
}