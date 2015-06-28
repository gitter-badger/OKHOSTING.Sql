using System;
using System.Collections.Generic;
using OKHOSTING.Sql.ORM.Operations;

namespace OKHOSTING.Sql.ORM
{
	public class Table<TKey, TType>: Core.Data.DictionaryBase<TKey, TType>
	{
		public override void Add(KeyValuePair<TKey, TType> item)
		{
			Insert insert = new Insert();
			insert.Into = typeof(TType);
			List<MemberValue> values = new List<MemberValue>();

			foreach (DataMember dmember in insert.Into.Members)
			{
				values.Add(new MemberValue(dmember, dmember.GetValue(item)));
			}

			insert.Values = values;
		}

		public override bool ContainsKey(TKey key)
		{
			throw new NotImplementedException();
		}

		public override IEnumerator<KeyValuePair<TKey, TType>> GetEnumerator()
		{
			throw new NotImplementedException();
		}

		public override bool IsReadOnly
		{
			get { throw new NotImplementedException(); }
		}

		public override ICollection<TKey> Keys
		{
			get { throw new NotImplementedException(); }
		}

		public override bool Remove(TKey key)
		{
			throw new NotImplementedException();
		}

		public override TType this[TKey key]
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}
	}
}
