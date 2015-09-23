using OKHOSTING.Sql.Schema;
using System.Collections.Generic;

namespace OKHOSTING.Sql.Operations
{
	public class Delete
	{
		public int Id { get; set; }
		public Table From { get; set; }
		public readonly List<Filters.FilterBase> Where = new List<Filters.FilterBase>();
	}
}