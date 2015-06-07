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
		public Table Into { get; set; }
		public IEnumerable<ColumnValue> Values { get; set; }
	}
}