using OKHOSTING.Data.Validation;
using System;

namespace OKHOSTING.Sql
{
	/// <summary>
	/// Represents a "simplified" column in a data result
	/// </summary>
	public class DataColumn
	{
		public DataColumn()
		{
		}

        public DataColumn(string name, DbType columnType)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}

			Name = name;
			ColumnType = columnType;
		}

		[RequiredValidator]
		[StringLengthValidator(50)]
		public string Name
		{
			get; set;
		}

		[RequiredValidator]
		public DbType ColumnType
		{
			get; set;
		}
	}
}