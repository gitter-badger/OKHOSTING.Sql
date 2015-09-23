using OKHOSTING.Sql.Schema;
using System;

namespace OKHOSTING.Sql.Operations
{
	/// <summary>
	/// A tuple of a column and a value that will be written to that column on insert or update operations
	/// </summary>
	public class ColumnValue
	{
		public int Id { get; set; }
		public readonly Column Column;
		public readonly object Value;

		public ColumnValue(Column column, object value)
		{
			if (column == null)
			{
				throw new ArgumentNullException("column");
			}

			Column = column;
			Value = value;
		}
	}
}