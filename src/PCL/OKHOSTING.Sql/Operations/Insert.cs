using OKHOSTING.Sql.Schema;
using System.Collections.Generic;

namespace OKHOSTING.Sql.Operations
{
	public class Insert
	{
		public int Id { get; set; }
		public Table Into { get; set; }
		public readonly List<ColumnValue> Values = new List<ColumnValue>();
	}
}