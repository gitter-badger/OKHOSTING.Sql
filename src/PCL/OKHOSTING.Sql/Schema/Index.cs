using OKHOSTING.Data;
using OKHOSTING.Data.Validation;
using System.Collections.Generic;

namespace OKHOSTING.Sql.Schema
{
	public class Index
	{
		public int Id { get; set; }

		[RequiredValidator]
		[StringLengthValidator(100)]
		public string Name { get; set; }

		[RequiredValidator]
		public Table Table { get; set; }

		[RequiredValidator]
		public SortDirection Direction { get; set; }

		[RequiredValidator]
		public bool Unique { get; set; }

		[RequiredValidator]
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