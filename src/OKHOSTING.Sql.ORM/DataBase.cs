using OKHOSTING.Sql.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace OKHOSTING.Sql.ORM
{
	public class DataBase : IOrmDataBase
	{
		protected readonly Dictionary<Type, object> Tables;
		public readonly Sql.DataBase NativeDataBase;
		public readonly SqlGeneratorBase SqlGenerator;

		public DataBase(Sql.DataBase nativeDataBase, SqlGeneratorBase sqlGenerator)
		{
			NativeDataBase = nativeDataBase;
			SqlGenerator = sqlGenerator;
		}

		public Table<TKey, TValue> Table<TKey, TValue>() where TKey: IComparable
		{
			Table<TKey, TValue> table = null;

			if (Tables.ContainsKey(typeof(TValue)))
			{
				table = (Table<TKey, TValue>) Tables[typeof(TValue)];
			}
			else
			{
				table = new Table<TKey, TValue>(this);
				Tables.Add(typeof(TValue), table);
			}

			return table;
		}

		IDictionary<TKey, TValue> IOrmDataBase.Table<TKey, TValue>() 
		{
			return this.Table<TKey, TValue>();
		}
	}
}