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
		public ForeignKeyFilter(MemberMap<T> member, object foreignKey): base(member)
		{
			ForeignKey = foreignKey;
		}

		/// <summary>
		/// Compares the DataMember on the specified object with the 
		/// ValueToCompare field
		/// </summary>
		/// <param name="obj"></param>
		/// <returns>
		/// True if the comparison is fulfilled, otherwise false
		/// </returns>
		public override bool Match(T obj)
		{
			if (obj == null)
			{
				throw new ArgumentNullException("obj");
			}

			return new PrimaryKeyFilter<T>(ForeignKey).Match(obj);
		}
	}
}