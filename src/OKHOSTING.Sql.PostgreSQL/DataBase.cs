using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OKHOSTING.Sql.PostgreSQL
{
	public class DataBase : OKHOSTING.Sql.DataBase
	{
		public DataBase(string connectionString): base(connectionString, NpgsqlFactory.Instance)
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

		public override bool ConstraintExists(string Name)
		{
			throw new NotImplementedException();
		}

		public override bool IndexExists(string Name)
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