using OKHOSTING.Sql.Schema;
using System.Collections.Generic;

namespace OKHOSTING.Sql.Operations
{
	public class Delete: Operation
	{
		public readonly List<Filters.FilterBase> Where = new List<Filters.FilterBase>();
	}
}