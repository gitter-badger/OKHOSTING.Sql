using System;
using System.Linq;
using System.Collections.Generic;
using OKHOSTING.Sql.ORM.Operations;

namespace OKHOSTING.Sql.ORM
{
	public class MultipleKeyTable<TType>: Table<object[], TType>
	{
		public override ICollection<object[]> Keys
		{
			get 
			{
				Select select = new Select();
				select.From = DataType;
				
				foreach (DataMember pk in DataType.PrimaryKey)
				{
					select.Members.Add(pk);
				}

				var primaryKeys = DataType.PrimaryKey.ToList();
				var keys = new List<object[]>();

				foreach (TType instance in DataBase.Select<TType>(select))
				{
					object[] k = new object[primaryKeys.Count];
					
					for (int i = 0; i < primaryKeys.Count; i++)
					{
						k[i] = Convert.ChangeType(primaryKeys[i].GetValue(instance), primaryKeys[i].ReturnType);
					}
					
					keys.Add(k);
				}

				return keys;
			}
		}

		public override IEnumerator<KeyValuePair<object[], TType>> GetEnumerator()
		{
			Select select = CreateSelect();
			List<DataMember> primaryKeys = DataType.PrimaryKey.ToList();

			foreach (TType instance in DataBase.Select<TType>(select))
			{
				object[] key = new object[primaryKeys.Count];

				for (int i = 0; i < primaryKeys.Count; i++)
				{
					key[i] = Convert.ChangeType(primaryKeys[i].GetValue(instance), primaryKeys[i].ReturnType);
				}

				yield return new KeyValuePair<object[], TType>(key, instance);
			}
		}

		internal MultipleKeyTable(DataBase database): base(database)
		{
		}

		protected override Filters.FilterBase GetPrimaryKeyFilter(DataType dtype, object[] key)
		{
			Filters.AndFilter filter = new Filters.AndFilter();
			var primaryKeys = dtype.PrimaryKey.ToList();

			for (int i = 0; i < primaryKeys.Count; i++)
			{
				filter.InnerFilters.Add(new Filters.ValueCompareFilter()
				{
					Member = primaryKeys[i],
					ValueToCompare = (IComparable)key[i],
					Operator = Core.Data.CompareOperator.Equal,
				});
			}

			return filter;
		}

		protected override Filters.FilterBase GetSelectJoinFilter(DataType dtype)
		{
			Filters.AndFilter filter = new Filters.AndFilter();
			var primaryKeys = dtype.PrimaryKey.ToList();
			var basePrimaryKeys = dtype.BaseDataType.PrimaryKey.ToList();

			for (int i = 0; i < primaryKeys.Count; i++)
			{
				filter.InnerFilters.Add(new Filters.MemberCompareFilter()
				{
					Member = primaryKeys[i], 
					MemberToCompare = basePrimaryKeys[i],
					Operator = Core.Data.CompareOperator.Equal,
				});
			}

			return filter;
		}
	}
}