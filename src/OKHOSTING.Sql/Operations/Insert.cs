using OKHOSTING.Sql.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OKHOSTING.Sql.Operations
{
	public class Insert
	{
		public int Id { get; set; }
		public Table Into { get; set; }
		public readonly List<ColumnValue> Values = new List<ColumnValue>();
	}
}