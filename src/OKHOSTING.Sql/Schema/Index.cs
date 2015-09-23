using OKHOSTING.Data;
using System.Collections.Generic;

namespace OKHOSTING.Sql.Schema
{
	public class Index
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public Table Table { get; set; }
		public SortDirection Direction { get; set; }
		public bool Unique { get; set; }
		public readonly List<Column> Columns = new List<Column>();

		public string FullName
		{
			get
			{
				if (Table != null)
				{
					return Table.Name + "." + Name;
				}
				else
				{
					return Name;
				}
			}
		}

		public override string ToString()
		{
			return Name;
		}
	}
}