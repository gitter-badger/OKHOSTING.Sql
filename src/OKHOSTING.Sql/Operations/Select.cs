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
		public IEnumerable<Column> Columns { get; set; }
		public Table From { get; set; }
		public IEnumerable<SelectJoin> Joins { get; set; }
		public IEnumerable<Filters.FilterBase> Where { get; set; }
		public IEnumerable<OrderBy> OrderBy { get; set; }
		public SelectLimit Limit { get; set; }
	}
}