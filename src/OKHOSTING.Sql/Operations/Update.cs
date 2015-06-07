using OKHOSTING.Sql.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OKHOSTING.Sql.Operations
{
	public class Update
	{
		public Table From { get; set; }
		public IEnumerable<ColumnValue> Set { get; set; }
		public IEnumerable<Filters.FilterBase> Where { get; set; }
	}
}