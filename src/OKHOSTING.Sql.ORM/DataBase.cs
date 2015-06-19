using OKHOSTING.Sql.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace OKHOSTING.Sql.ORM
{
	public class DataBase
	{
		public DataBase(Sql.DataBase nativeDataBase, SqlGeneratorBase sqlGenerator)
		{
		}

		public void Map(Type type, Table table)
		{
		}

		public void Map(PropertyInfo type, Column table)
		{
		}
	}
}