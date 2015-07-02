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
			DataType dtype = typeof(TType);

			foreach (DataType parent in dtype.GetBaseDataTypes())
			{
				Insert insert = new Insert();
				insert.Into = parent;

				foreach (DataMember dmember in parent.Members)
				{
					insert.Values.Add(new MemberValue(dmember, dmember.GetValue(item)));
				}

				string sql = DataBase.SqlGenerator.Insert(OperationConverter.Parse(insert));
				DataBase.NativeDataBase.Execute(sql);
			}
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
			DataType dtype = typeof(TType);
			Select select = new Select();
			select.From = dtype;

			foreach (DataMember member in dtype.Members)
			{
				select.Members.Add(new SelectMember(member, member.Member.Replace('.', '_')));
			}

			//go from child type to base type adding joins
			while(dtype.BaseDataType != null)
			{
				SelectJoin join = new SelectJoin();
				join.JoinType = Sql.Operations.SelectJoinType.Inner; //could change?
				join.Type = dtype;
				join.On.Add(new Filters.MemberCompareFilter(dtype.PrimaryKey.First(), dtype.BaseDataType.PrimaryKey.First()));
				
				foreach (DataMember member in dtype.BaseDataType.Members)
				{
					select.Members.Add(new SelectMember(member, member.Member.Replace('.', '_')));
				}

				select.Joins.Add(join);
				dtype = dtype.BaseDataType;
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
			DataType dtype = typeof(TType);
			int result = 0;

			//go from child to base class inheritance
			foreach (DataType parent in dtype.GetBaseDataTypes().Reverse())
			{
				Delete delete = new Delete();
				delete.From = parent;
				delete.Where.Add(new Filters.ValueCompareFilter(parent.PrimaryKey.First(), key));

				string sql = DataBase.SqlGenerator.Delete(OperationConverter.Parse(delete));
				result += DataBase.NativeDataBase.Execute(sql);
			}

			return result > 0;
		}

		public override TType this[TKey key]
		{
			get
			{
				DataType dtype = typeof(TType);
				Select select = new Select();
				select.From = dtype;

				foreach (DataMember member in dtype.Members)
				{
					select.Members.Add(new SelectMember(member, member.Member.Replace('.', '_')));
				}

				//go from child type to base type adding joins
				while (dtype.BaseDataType != null)
				{
					SelectJoin join = new SelectJoin();
					join.JoinType = Sql.Operations.SelectJoinType.Inner; //could change?
					join.Type = dtype;
					join.On.Add(new Filters.MemberCompareFilter(dtype.PrimaryKey.First(), dtype.BaseDataType.PrimaryKey.First()));

					foreach (DataMember member in dtype.BaseDataType.Members)
					{
						select.Members.Add(new SelectMember(member, member.Member.Replace('.', '_')));
					}

					select.Joins.Add(join);
					dtype = dtype.BaseDataType;
				}

				dtype = typeof(TType);
				select.Where.Add(new Filters.ValueCompareFilter(dtype.PrimaryKey.First(), key));

				string sql = DataBase.SqlGenerator.Select(OperationConverter.Parse(select));
				var dataReader = DataBase.NativeDataBase.GetDataReader(sql);

				if (dataReader.Read())
				{
					TType instance = Activator.CreateInstance<TType>();

					foreach (DataType parent in dtype.GetBaseDataTypes())
					{
						foreach (SelectMember member in parent.Members)
						{
							object value = Convert.ChangeType(string.IsNullOrWhiteSpace(member.Alias) ? dataReader[member.Member.Column.Name] : dataReader[member.Alias], member.Member.ReturnType);
							member.Member.SetValue(instance, value);
						}
					}

					return instance;
				}
				else
				{
					return default(TType);
				}
			}
			set
			{
				if (ContainsKey(key))
				{
					Update update = new Update();
					update.From = typeof(TType);

					foreach (DataMember dmember in update.From.Members)
					{
						update.Set.Add(new MemberValue(dmember, dmember.GetValue(value)));
					}

					update.Where.Add(new Filters.ValueCompareFilter(update.From.PrimaryKey.First(), key));

					string sql = DataBase.SqlGenerator.Update(OperationConverter.Parse(update));
					var result = DataBase.NativeDataBase.Execute(sql);
			
				}
				else
				{
					Add(new KeyValuePair<TKey, TType>(key, value));
				}
			}
		}

		internal Table(DataBase database)
		{
		}
	}
}
