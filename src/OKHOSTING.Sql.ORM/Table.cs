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
			Select select = new Select();
			select.From = typeof(TType);
			select.Members.Add(select.From.PrimaryKey.First());
			select.Where.Add(new Filters.ValueCompareFilter(select.From.PrimaryKey.First(), key));

			string sql = DataBase.SqlGenerator.Select(OperationConverter.Parse(select));
			return DataBase.NativeDataBase.ExistsData(sql);
		}

		public override IEnumerator<KeyValuePair<TKey, TType>> GetEnumerator()
		{
			Select select = new Select();
			select.From = typeof(TType);

			foreach (DataMember member in select.From.Members)
			{
				select.Members.Add(new SelectMember(member, member.Member.Replace('.', '_')));
			}

			string sql = DataBase.SqlGenerator.Select(OperationConverter.Parse(select));
			var dataReader = DataBase.NativeDataBase.GetDataReader(sql);
			DataMember pkMember = select.From.PrimaryKey.First();

			while (dataReader.Read())
			{
				TType instance = Activator.CreateInstance<TType>();
				
				foreach (SelectMember member in select.Members)
				{
					object value = Convert.ChangeType(string.IsNullOrWhiteSpace(member.Alias)? dataReader[member.Member.Column.Name] : dataReader[member.Alias], member.Member.ReturnType);
					member.Member.SetValue(instance, value);
				}

				TKey key = (TKey) Convert.ChangeType(pkMember.GetValue(instance), pkMember.ReturnType);

				yield return new KeyValuePair<TKey, TType>(key, instance);
			}
		}

		public override bool IsReadOnly
		{
			get 
			{
				return false; ; 
			}
		}

		public override ICollection<TKey> Keys
		{
			get 
			{
				Select select = new Select();
				select.From = typeof(TType);
				select.Members.Add(select.From.PrimaryKey.First());

				string sql = DataBase.SqlGenerator.Select(OperationConverter.Parse(select));
				var dataReader = DataBase.NativeDataBase.GetDataReader(sql);
				List<TKey> keys = new List<TKey>();

				while (dataReader.Read())
				{
					keys.Add((TKey) dataReader[0]);
				}

				return keys;
			}
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
