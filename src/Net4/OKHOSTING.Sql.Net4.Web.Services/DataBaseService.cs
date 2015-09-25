using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace OKHOSTING.Sql.Net4.Web.Services
{
	public class DataBaseService : ApiController
	{
		private Net4.DataBase DataBase = null;

		// GET api/<controller>
		public int Execute(Command command)
		{
			return DataBase.Execute(command);
		}

		public int Execute(IEnumerable<Command> commands)
		{
			return DataBase.Execute(commands);
		}

		public DataReader GetDataReader(Command command)
		{
			return (DataReader) DataBase.GetDataReader(command);
		}

		public DataTable GetDataTable(Command command)
		{
			return (DataTable) DataBase.GetDataTable(command);
		}
	}
}