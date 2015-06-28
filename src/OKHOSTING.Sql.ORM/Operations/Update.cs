using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OKHOSTING.Sql.ORM.Operations
{
	public class Update
	{
		public DataType From { get; set; }
		public readonly List<MemberValue> Set = new List<MemberValue>();
		public readonly List<Filters.FilterBase> Where = new List<Filters.FilterBase>();

		public OKHOSTING.Sql.Operations.Update Parse()
		{
			var update = new OKHOSTING.Sql.Operations.Update();
			update.From = From.Table;

			foreach (MemberValue mvalue in Set)
			{
				update.Set.Add(new Sql.Operations.ColumnValue(mvalue.MemberMap.Column, mvalue.Value));
			}

			foreach (Filters.FilterBase filter in Where)
			{
				//update.Where.Add(new Sql.Operations.ColumnValue(mvalue.MemberMap.Column, mvalue.Value));
			}
			
			return update;
		}
	}
}