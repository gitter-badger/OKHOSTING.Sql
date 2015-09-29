using OKHOSTING.Sql.Schema;
using System.Collections.Generic;

namespace OKHOSTING.Sql.Operations
{
	public class Insert: Operation
	{
		public readonly List<ColumnValue> Values = new List<ColumnValue>();
	}
}