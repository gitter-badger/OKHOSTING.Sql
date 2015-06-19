using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OKHOSTING.Sql.ORM.Operations
{
	public class Insert
	{
		public TypeMap Into { get; set; }
		public IEnumerable<MemberValue> Values { get; set; }
	}
}