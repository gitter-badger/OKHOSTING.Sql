using OKHOSTING.Sql.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OKHOSTING.Sql.Operations
{
	/// <summary>
	/// Represents a select join
	/// </summary>
	public class SelectJoin
	{
		public Table Table { get; set; }
		public SelectJoinType Type { get; set; }
		public readonly List<Filters.FilterBase> On = new List<Filters.FilterBase>();
	}
}