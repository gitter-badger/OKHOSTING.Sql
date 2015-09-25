using System;

namespace OKHOSTING.Sql
{
	/// <summary>
	/// Represents a "simplified" column in a data result
	/// </summary>
	public class DataColumn
	{
		public DataColumn(string name, Type columnType)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}

			if (columnType == null)
			{
				throw new ArgumentNullException("columnType");
			}

			Name = name;
			ColumnType = columnType;
		}

		public readonly string Name;
		public readonly Type ColumnType;
	}
}