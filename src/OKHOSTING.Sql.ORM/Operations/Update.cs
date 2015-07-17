using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OKHOSTING.Sql.ORM.Operations
{
	public class Update
	{
		public Update()
		{
		}

		public DataType From { get; set; }
		public readonly List<MemberValue> Set = new List<MemberValue>();
		public readonly List<Filters.FilterBase> Where = new List<Filters.FilterBase>();
	}

	public class Update<T> : Update
	{
		public Update()
		{
			From = typeof(T);
		}
	}
}