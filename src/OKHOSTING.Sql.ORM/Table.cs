﻿using System;
using System.Linq;
using System.Collections.Generic;
using OKHOSTING.Sql.ORM.Operations;
using OKHOSTING.Sql.ORM.Filters;

namespace OKHOSTING.Sql.ORM
{
	public class Table<TKey, TType>: Core.Data.DictionaryBase<TKey, TType>
	{
		public readonly DataBase DataBase;
		public readonly DataType DataType = typeof(Type);

		public override void Add(KeyValuePair<TKey, TType> item)
		{
			foreach (DataType parent in DataType.GetBaseDataTypes().Reverse())
			{
				Insert insert = new Insert();
				insert.Into = parent;

				foreach (DataMember dmember in parent.Members)
				{
					insert.Values.Add(new MemberValue(dmember, dmember.GetValue(item)));
				}

				DataBase.Insert(insert);
			}
		}

		public override bool ContainsKey(TKey key)
		{
			Select select = new Select();
			select.From = DataType;
			select.Members.Add(select.From.PrimaryKey.First());
			select.Where.Add(GetPrimaryKeyFilter(key));

			return DataBase.Select<TType>(select).Count() > 1;
		}

		public override IEnumerator<KeyValuePair<TKey, TType>> GetEnumerator()
		{
			Select select = CreateSelect();
			DataMember pkMember = select.From.PrimaryKey.First();
			
			foreach (TType instance in DataBase.Select<TType>(select))
			{
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
				select.From = DataType;
				select.Members.Add(select.From.PrimaryKey.First());

				List<TKey> keys = new List<TKey>();
				DataMember pk = select.From.PrimaryKey.First();

				foreach(TType instance in DataBase.Select<TType>(select))
				{
					keys.Add((TKey) pk.GetValue(instance));
				}

				return keys;
			}
		}

		public override bool Remove(TKey key)
		{
			int result = 0;

			//go from child to base class inheritance
			foreach (DataType parent in DataType.GetBaseDataTypes())
			{
				Delete delete = new Delete();
				delete.From = parent;
				delete.Where.Add(GetPrimaryKeyFilter(key));

				result += DataBase.Delete(delete);
			}

			return result > 0;
		}

		public override TType this[TKey key]
		{
			get
			{
				Select select = CreateSelect();
				select.Where.Add(GetPrimaryKeyFilter(key));
				var result = DataBase.Select<TType>(select).ToList();

				if (result.Count > 0)
				{
					return result[0];
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
					foreach (DataType parent in DataType.GetBaseDataTypes().Reverse())
					{
						Update update = new Update();
						update.From = parent;

						foreach (DataMember dmember in parent.Members)
						{
							update.Set.Add(new MemberValue(dmember, dmember.GetValue(value)));
						}

						update.Where.Add(GetPrimaryKeyFilter(key));

						DataBase.Update(update);
					}
				}
				else
				{
					Add(new KeyValuePair<TKey, TType>(key, value));
				}
			}
		}

		#region Non public

		internal Table(DataBase database)
		{
			DataBase = database;
		}

		protected virtual Filters.FilterBase GetPrimaryKeyFilter(TKey key)
		{
			return GetPrimaryKeyFilter(DataType, key);
		}

		protected virtual Filters.FilterBase GetPrimaryKeyFilter(DataType dtype, TKey key)
		{
			return new Filters.ValueCompareFilter(dtype.PrimaryKey.First(), (IComparable) key);
		}

		protected virtual Filters.FilterBase GetSelectJoinFilter(DataType dtype)
		{
			return new Filters.MemberCompareFilter(dtype.PrimaryKey.First(), dtype.BaseDataType.PrimaryKey.First());
		}

		/// <summary>
		/// Creates a select operation with all DataType's members and inheritance inner joins
		/// </summary>
		/// <returns>Select operation</returns>
		protected Select CreateSelect()
		{
			DataType dtype = DataType;
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
				join.On.Add(GetSelectJoinFilter(dtype));

				foreach (DataMember member in dtype.BaseDataType.Members)
				{
					select.Members.Add(new SelectMember(member, member.Member.Replace('.', '_')));
				}

				select.Joins.Add(join);
				dtype = dtype.BaseDataType;
			}

			return select;
		}

		#endregion
	}
}