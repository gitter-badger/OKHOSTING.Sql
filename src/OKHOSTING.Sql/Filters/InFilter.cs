using System;
using System.Linq;
using System.Linq.Expressions;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Xml;
using OKHOSTING.Sql.Schema;

namespace OKHOSTING.Sql.Filters
{
	/// <summary>
	/// Defines a filter in which the DataMember must be part
	/// of a values set
	/// </summary>
	/// <typeparam name="T">
	/// Type of the items in the values set
	/// </typeparam>
	public class InFilter: ColumnFilter
	{
		#region Fields

		/// <summary>
		/// List of values of the filter
		/// </summary>
		public IEnumerable<IComparable> Values;

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
		public InFilter(Column column) : this(column, null, false) { }

		/// <summary>
		/// Constructs the In Filter
		/// </summary>
		/// <param name="dataMember">
		/// DataMember used in the filter
		/// </param>
		/// <param name="values">
		/// List of possible values of the filter
		/// </param>
		public InFilter(Column column, IEnumerable<IComparable> values): this(column, values, false)
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
		public InFilter(Column column, IEnumerable<IComparable> values, bool caseSensitive): base(column)
		{
			this.Values = values;
			this.CaseSensitive = caseSensitive;
		}

		#endregion
	}
}