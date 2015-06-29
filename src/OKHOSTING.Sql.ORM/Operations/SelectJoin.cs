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
		public OKHOSTING.Sql.Operations.SelectJoinType JoinType { get; set; }
		public readonly List<Filters.FilterBase> On = new List<Filters.FilterBase>();
	}
}