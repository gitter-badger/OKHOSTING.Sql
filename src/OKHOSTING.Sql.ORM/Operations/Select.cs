using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OKHOSTING.Sql.ORM.Operations
{
	/// <summary>
	/// A select SQL query
	/// </summary>
	public class Select
	{
		public IEnumerable<DataMember> Columns { get; set; }
		public DataType From { get; set; }
		public IEnumerable<SelectJoin> Joins { get; set; }
		public IEnumerable<Filters.FilterBase> Where { get; set; }
		public IEnumerable<OrderBy> OrderBy { get; set; }
		public SelectLimit Limit { get; set; }
	}
}