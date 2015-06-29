using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OKHOSTING.Sql.ORM.Operations
{
	public class Delete
	{
		public DataType From { get; set; }
		public readonly List<Filters.FilterBase> Where = new List<Filters.FilterBase>();
	}
}