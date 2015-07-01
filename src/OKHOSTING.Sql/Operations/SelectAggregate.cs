﻿using OKHOSTING.Sql.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OKHOSTING.Sql.Operations
{
	/// <summary>
	/// A select statement that also contains aggreate functions, which requier a groupby element as well
	/// </summary>
	public class SelectAggregate: Select
	{
		public readonly List<SelectAggregateColumn> AggregateColumns = new List<SelectAggregateColumn>();
		public readonly List<Column> GroupBy = new List<Column>();
	}
}