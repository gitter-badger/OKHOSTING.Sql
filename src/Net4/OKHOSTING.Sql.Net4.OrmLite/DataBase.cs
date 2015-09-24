using OKHOSTING.Data;
using System;
using System.Collections.Generic;

namespace OKHOSTING.Sql.Net4.OrmLite
{
	public class DataBase : IOrmDataBase, IDisposable
	{
		public int Id { get; set; }
		
		public System.Data.IDbConnection Connection { get; set; }
		
		protected readonly Dictionary<Type, object> Tables = new Dictionary<Type,object>();

		public DataBase()
		{
		}

		public Table<TKey, TValue> Table<TKey, TValue>() where TValue : class
		{
			Table<TKey, TValue> table = null;

			if(Tables.ContainsKey(typeof(TValue)))
			{
				table = (Table<TKey, TValue>) Tables[typeof(TValue)];
			}
			else
			{
				table = new Table<TKey, TValue>();
				table.Connection = Connection;
				Tables.Add(typeof(TValue), table);
			}

			return table;
		}

		IDictionary<TKey, TValue> IOrmDataBase.Table<TKey, TValue>()
		{
			return this.Table<TKey, TValue>();
		}

		public void Dispose()
		{
			Connection.Dispose();
		}
	}
}