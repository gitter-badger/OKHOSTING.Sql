using System;
using System.Data.SQLite;

namespace OKHOSTING.Sql.SQLite
{
	public class DataBase : OKHOSTING.Sql.DataBase
	{
		public DataBase(): base(SQLiteFactory.Instance)
		{
		}

		public override DateTime DateTimeOnDBServer()
		{
			throw new NotImplementedException();
		}

		public override string GetUniqueIdentifier()
		{
			throw new NotImplementedException();
		}

		public override bool ExistsConstraint(string Name)
		{
			throw new NotImplementedException();
		}

		public override bool ExistsIndex(string Name)
		{
			throw new NotImplementedException();
		}

		protected override string SchemaProvider
		{
			get
			{
				return "System.Data.SQLite";
			}
		}
	}
}