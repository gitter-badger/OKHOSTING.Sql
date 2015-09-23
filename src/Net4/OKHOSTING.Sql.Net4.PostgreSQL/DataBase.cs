using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OKHOSTING.Sql.Net4.PostgreSQL
{
	public class DataBase : OKHOSTING.Sql.Net4.DataBase
	{
		public DataBase(): base(NpgsqlFactory.Instance)
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
				return "Npgsql";
			}
		}
	}
}