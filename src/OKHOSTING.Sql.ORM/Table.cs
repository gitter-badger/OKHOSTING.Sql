using System;
using System.Linq;
using System.Collections.Generic;
using OKHOSTING.Sql.ORM.Operations;

namespace OKHOSTING.Sql.ORM
{
	public class Table<TKey, TType>: Core.Data.DictionaryBase<TKey, TType> where TKey : IComparable
	{
		public readonly DataBase DataBase;

		public override void Add(KeyValuePair<TKey, TType> item)
		{
			Insert insert = new Insert();
			insert.Into = typeof(TType);

			foreach (DataMember dmember in insert.Into.Members)
			{
				insert.Values.Add(new MemberValue(dmember, dmember.GetValue(item)));
			}

			string sql = DataBase.SqlGenerator.Insert(OperationConverter.Parse(insert));
			DataBase.NativeDataBase.Execute(sql);
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
			Delete delete = new Delete();
			delete.From = typeof(TType);
			delete.Where.Add(new Filters.ValueCompareFilter(delete.From.PrimaryKey.First(), key));

			string sql = DataBase.SqlGenerator.Delete(OperationConverter.Parse(delete));
			var result = DataBase.NativeDataBase.Execute(sql);
			
			return result > 0;
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

		internal Table(DataBase database)
		{
		}
	}
}
