using System;
using System.Collections.Generic;
using System.Linq;

namespace OKHOSTING.Sql.EF
{
	public class DataBase : IOrmDataBase, IDisposable
	{
		public readonly System.Data.Entity.DbContext Context;
		protected readonly Dictionary<Type, object> Tables;

		public DataBase(System.Data.Entity.DbContext context)
		{
			Context = context;
		}

		public Table<TKey, TValue> Table<TKey, TValue>() where TValue : class
		{
			Table<TKey, TValue> table = null;

			if (Tables.ContainsKey(typeof(TValue)))
			{
				table = (Table<TKey, TValue>)Tables[typeof(TValue)];
			}
			else
			{
				table = new Table<TKey, TValue>(Context);
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
			Context.Dispose();
		}
	}
}