using System;
using System.Collections.Generic;

namespace OKHOSTING.Sql
{
	/// <summary>
	/// A DataBase that is accesed through a ORM like EntityFramework or OrmLite
	/// </summary>
	/// <typeparam name="TKey">Type of key that will be used among all the DataBase</typeparam>
	public interface IOrmDataBase
	{
		IDictionary<TKey, TValue> Table<TKey, TValue>() where TValue : class;
	}
}