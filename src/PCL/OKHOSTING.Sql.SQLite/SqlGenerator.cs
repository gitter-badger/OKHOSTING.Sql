using OKHOSTING.Sql.Operations;
using OKHOSTING.Sql.Schema;
using System;

namespace OKHOSTING.Sql.SQLite
{
	/// <summary>
	/// Generator of Sql sequences for SQLite
	/// </summary>
	public class SqlGenerator: SqlGeneratorBase
	{
		/// <summary>
		/// Constructs the class
		/// </summary>
		public SqlGenerator() : base() { }

		protected override string AutoIncrementalSettingName
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		protected override Command AutoIncrementalFunction(Table table)
		{
			throw new NotImplementedException();
		}
	}
}