using OKHOSTING.Sql.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OKHOSTING.Sql.Operations
{
	public class Delete
	{
		public Table From { get; set; }
		public IEnumerable<Filters.FilterBase> Where { get; set; }
	}
}