using OKHOSTING.Sql.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OKHOSTING.Sql.ORM.Operations
{
	/// <summary>
	/// Represents a select join
	/// </summary>
	public class SelectJoin
	{
		public DataType Type { get; set; }
		public SelectJoinType JoinType { get; set; }
		public IEnumerable<Filters.FilterBase> On { get; set; }
	}
}