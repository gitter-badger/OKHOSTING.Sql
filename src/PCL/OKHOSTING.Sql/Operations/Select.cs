﻿using OKHOSTING.Sql.Schema;
using System.Collections.Generic;

namespace OKHOSTING.Sql.Operations
{
	/// <summary>
	/// A select SQL query
	/// </summary>
	public class Select: Operation
	{
		public SelectLimit Limit { get; set; }
		public readonly List<SelectColumn> Columns = new List<SelectColumn>();
		public readonly List<SelectJoin> Joins = new List<SelectJoin>();
		public readonly List<Filters.FilterBase> Where = new List<Filters.FilterBase>();
		public readonly List<OrderBy> OrderBy = new List<OrderBy>();
	}
}