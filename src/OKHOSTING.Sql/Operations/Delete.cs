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
		public int Id { get; set; }
		public Table From { get; set; }
		public readonly List<Filters.FilterBase> Where = new List<Filters.FilterBase>();
	}
}