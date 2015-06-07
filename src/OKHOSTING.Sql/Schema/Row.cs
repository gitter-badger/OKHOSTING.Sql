using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OKHOSTING.Sql.Schema
{
	public class Row
	{
		public readonly Table Table;
		public readonly Dictionary<Column, object> Cells;

		public Row(Table table)
		{
			Table = table;
			Cells = new Dictionary<Column, object>();
		}

		public void InitValues()
		{
			foreach (Column c in Table.Columns)
			{
				Cells.Add(c, null);
			}
		}
	}
}