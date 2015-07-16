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
		public DataType From { get; set; }
		public SelectLimit Limit { get; set; }
		public readonly List<SelectMember> Members = new List<SelectMember>();
		public readonly List<SelectJoin> Joins = new List<SelectJoin>();
		public readonly List<Filters.FilterBase> Where = new List<Filters.FilterBase>();
		public readonly List<OrderBy> OrderBy = new List<OrderBy>();
	}
}