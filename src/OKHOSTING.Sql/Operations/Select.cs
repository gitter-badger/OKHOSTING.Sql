using OKHOSTING.Sql.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OKHOSTING.Sql.Operations
{
	/// <summary>
	/// A select SQL query
	/// </summary>
	public class Select
	{
		public Table From { get; set; }
		public SelectLimit Limit { get; set; }
		public readonly List<SelectColumn> Columns = new List<SelectColumn>();
		public readonly List<SelectJoin> Joins = new List<SelectJoin>();
		public readonly List<Filters.FilterBase> Where = new List<Filters.FilterBase>();
		public readonly List<OrderBy> OrderBy = new List<OrderBy>();


	}
}