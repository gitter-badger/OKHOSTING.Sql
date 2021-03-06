﻿using OKHOSTING.Data;
using OKHOSTING.Sql.Schema;

namespace OKHOSTING.Sql.Operations
{
	/// <summary>
	/// An item of an "Order By" clause for Select operations
	/// </summary>
	public class OrderBy
	{
		public int Id { get; set; }

		/// <summary>
		/// Column which determines the sorting
		/// </summary>
		public Column Column;

		/// <summary>
		/// The direction of the sorting
		/// </summary>
		public SortDirection Direction;

		/// <summary>
		/// Creates a new instance
		/// </summary>
		public OrderBy()
		{
		}

		/// <summary>
		/// Creates a new instance
		/// </summary>
		protected OrderBy(Column orderBy): this(orderBy, SortDirection.Ascending)
		{
		}

		/// <summary>
		/// Creates a new instance
		/// </summary>
		protected OrderBy(Column orderBy, SortDirection direction)
		{
			this.Column = orderBy;
			this.Direction = direction;
		}
	}
}