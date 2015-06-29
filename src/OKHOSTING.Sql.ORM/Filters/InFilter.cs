using System;
using System.Linq;
using System.Linq.Expressions;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Xml;

namespace OKHOSTING.Sql.ORM.Filters
{
	/// <summary>
	/// Defines a filter in which the DataMember must be part
	/// of a values set
	/// </summary>
	/// <typeparam name="T">
	/// Type of the items in the values set
	/// </typeparam>
	public class InFilter: MemberFilter
	{
		#region Fields

		/// <summary>
		/// List of values of the filter
		/// </summary>
		public readonly List<IComparable> Values = new List<IComparable>();

		/// <summary>
		/// Indicates if the filter comparison will be case sensitive
		/// when listItemsType = System.String
		/// </summary>
		public bool CaseSensitive;

		#endregion

		#region Constructors

		/// <summary>
		/// Constructs the In Filter
		/// </summary>
		public InFilter() : this(null, null, false) { }

		/// <summary>
		/// Constructs the In Filter
		/// </summary>
		/// <param name="dataMember">
		/// DataMember used in the filter
		/// </param>
		public InFilter(DataMember member) : this(member, null, false) { }

		/// <summary>
		/// Constructs the In Filter
		/// </summary>
		/// <param name="dataMember">
		/// DataMember used in the filter
		/// </param>
		/// <param name="values">
		/// List of possible values of the filter
		/// </param>
		public InFilter(DataMember member, List<IComparable> values): this(member, values, false)
		{
		}

		/// <summary>
		/// Constructs the In Filter
		/// </summary>
		/// <param name="dataMember">
		/// DataMember used in the filter
		/// </param>
		/// <param name="values">
		/// List of possible values of the filter
		/// </param>
		/// <param name="caseSensitive">
		/// Indicates if the filter comparison will be case sensitive
		/// when listItemsType = System.String
		/// </param>
		public InFilter(DataMember member, List<IComparable> values, bool caseSensitive): base(member)
		{
			this.Values = values;
			this.CaseSensitive = caseSensitive;
		}

		#endregion
	}
}