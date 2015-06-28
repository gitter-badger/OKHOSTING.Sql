using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OKHOSTING.Sql.ORM.Operations
{
	public class Insert
	{
		public DataType Into { get; set; }
		public List<MemberValue> Values { get; set; }

		public OKHOSTING.Sql.Operations.Insert Parse()
		{
			var insert = new OKHOSTING.Sql.Operations.Insert();
			insert.Into = Into.Table;
			
			foreach (MemberValue mvalue in Values)
			{
				insert.Values.Add(new Sql.Operations.ColumnValue(mvalue.MemberMap.Column, mvalue.Value));
			}

			return insert;
		}
	}
}