using OKHOSTING.Data;
using ServiceStack;
using ServiceStack.OrmLite;
using System;
using System.Collections.Generic;

namespace OKHOSTING.Sql.Net4.OrmLite
{
	public class Table<TKey, TValue> : DictionaryBase<TKey, TValue>, IDisposable where TValue : class
	{
		public System.Data.IDbConnection Connection { get; set; }

		public override void Add(KeyValuePair<TKey, TValue> item)
		{
			Connect();
			Connection.Insert<TValue>(item.Value);
		}

		public override bool ContainsKey(TKey key)
		{
			Connect();
			return Connection.SingleById<TValue>(key) != null;
		}

		public override ICollection<TKey> Keys
		{
			get 
			{
				Connect();
				return Connection.Column<TKey>(Connection.From<TValue>().Select(typeof(TValue).GetIdProperty().Name));
			}
		}

		public override bool Remove(TKey key)
		{
			Connect();
			return Connection.DeleteById<TValue>(key) > 0;
		}

		public override TValue this[TKey key]
		{
			get
			{
				Connect();
				return Connection.SingleById<TValue>(key);
			}
			set
			{
				Connect();

				if (ContainsKey(key))
				{
					Connection.Update<TValue>(value);
				}
				else
				{
					Connection.Insert<TValue>(value);
				}
			}
		}

		public override bool IsReadOnly
		{
			get 
			{ 
				return false; 
			}
		}

		public override IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
		{
			Connect();
			
			foreach (var item in Connection.SelectLazy<TValue>())
			{
				yield return Parse(item);
			}
		}

		public override System.Collections.IEnumerable EnumerateKeys()
		{
			Connect();
			return Connection.ColumnLazy<TKey>(Connection.From<TValue>().Select(typeof(TValue).GetIdProperty().Name));
		}

		public void Connect()
		{
			if (Connection.State == System.Data.ConnectionState.Closed || Connection.State == System.Data.ConnectionState.Broken)
			{
				Connection.Open();
			}
		}

		protected KeyValuePair<TKey, TValue> Parse(TValue obj)
		{
			return new KeyValuePair<TKey, TValue>((TKey) obj.GetId(), obj);
		}

		public void Dispose()
		{
			Connection.Close();
			Connection.Dispose();
		}
	}
}