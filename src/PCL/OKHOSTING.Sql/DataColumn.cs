using System;

namespace OKHOSTING.Sql
{
	/// <summary>
	/// Represents a "simplified" column in a data result
	/// </summary>
	public class DataColumn
	{
		public DataColumn(string name, DbType columnType)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}

			Name = name;
			ColumnType = columnType;
		}

		public readonly string Name;
		public readonly DbType ColumnType;
	}
}