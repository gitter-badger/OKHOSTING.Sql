using System;
using System.Linq;
using System.Collections.Generic;
using System.Data.Entity;
using OKHOSTING.Data;

namespace OKHOSTING.Sql.Net4.EF
{
	public class Table<TKey, TValue> : DictionaryBase<TKey, TValue>, IDisposable where TValue : class
	{
		public DbContext Context { get; set; }
		public readonly System.Reflection.PropertyInfo PrimaryKey;

		public Table()
		{
			System.Reflection.PropertyInfo[] properties = typeof(TValue).GetProperties();

			foreach (System.Reflection.PropertyInfo property in properties)
			{
				System.Object[] attributes = property.GetCustomAttributes(true);

				foreach (object attribute in attributes)
				{
					if (attribute is System.Data.Entity.Core.Objects.DataClasses.EdmScalarPropertyAttribute)
					{
						if ((attribute as System.Data.Entity.Core.Objects.DataClasses.EdmScalarPropertyAttribute).EntityKeyProperty == true)
						{
							PrimaryKey = property;
							return;
						}
					}
					else if (attribute is System.ComponentModel.DataAnnotations.KeyAttribute)
					{
						PrimaryKey = property;
						return;
					}
				}
			}

			throw new ArgumentOutOfRangeException("TValue", typeof(TValue).FullName, "Cant find Primary Key for this type");
		}

		public override void Add(KeyValuePair<TKey, TValue> item)
		{
			Connect();
			Context.Set<TValue>().Add(item.Value);
		}

		public override bool ContainsKey(TKey key)
		{
			Connect();
			return Context.Set<TValue>().Find(key) != null;
		}

		public override ICollection<TKey> Keys
		{
			get 
			{
				Connect();
				return (from TValue obj in Context.Set<TValue>() select (TKey) PrimaryKey.GetValue(obj)).ToList();
			}
		}

		public override bool Remove(TKey key)
		{
			Connect();
			Context.Set<TValue>().Remove(this[key]);

			return true;
		}

		public override TValue this[TKey key]
		{
			get
			{
				Connect();
				return Context.Set<TValue>().Find(key);
			}
			set
			{
				Connect();
				Context.SaveChanges();
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

			foreach (TValue item in Context.Set<TValue>())
			{
				yield return Parse(item);
			}
		}

		public override System.Collections.IEnumerable EnumerateKeys()
		{
			Connect();
			
			foreach (TKey key in Keys)
			{
				yield return key;
			}
		}

		public void Connect()
		{
			if (Context.Database.Connection.State != System.Data.ConnectionState.Open)
			{
				Context.Database.Connection.Open();
			}
		}

		protected KeyValuePair<TKey, TValue> Parse(TValue obj)
		{
			return new KeyValuePair<TKey, TValue>((TKey) PrimaryKey.GetValue(obj), obj);
		}

		public void Dispose()
		{
			Context.Dispose();
		}
	}
}