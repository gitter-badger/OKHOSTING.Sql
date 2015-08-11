using OKHOSTING.Sql.ORM.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OKHOSTING.Sql.ORM.UI
{
	public class List<T>
	{
		public Select<T> Select { get; set; }
	}
}