﻿using OKHOSTING.Sql.ORM.Filters;
using OKHOSTING.Sql.ORM.Operations;
using OKHOSTING.Sql.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace OKHOSTING.Sql.ORM
{
	public class DataBase : IOrmDataBase
	{
		protected readonly Dictionary<Type, object> Tables = new Dictionary<Type,object>();

		public readonly Sql.DataBase NativeDataBase;
		public readonly SqlGeneratorBase SqlGenerator;

		public DataBase(Sql.DataBase nativeDataBase, SqlGeneratorBase sqlGenerator)
		{
			NativeDataBase = nativeDataBase;
			SqlGenerator = sqlGenerator;
		}

		public Table<TKey, TType> Table<TKey, TType>()
		{
			Table<TKey, TType> table = null;

			if (Tables.ContainsKey(typeof(TType)))
			{
				table = (Table<TKey, TType>) Tables[typeof(TType)];
			}
			else
			{
				table = new Table<TKey, TType>(this);
				Tables.Add(typeof(TType), table);
			}

			return table;
		}

		public MultipleKeyTable<TType> MultipleKeyTable<TType>()
		{
			MultipleKeyTable<TType> table = null;

			if (Tables.ContainsKey(typeof(TType)))
			{
				table = (MultipleKeyTable<TType>) Tables[typeof(TType)];
			}
			else
			{
				table = new MultipleKeyTable<TType>(this);
				Tables.Add(typeof(TType), table);
			}

			return table;
		}

		IDictionary<TKey, TType> IOrmDataBase.Table<TKey, TType>() 
		{
			return this.Table<TKey, TType>();
		}

		public int Insert(Insert insert)
		{
			Command sql = SqlGenerator.Insert(Parse(insert));
			int result = NativeDataBase.Execute(sql);

			//detect auto generated ID, when applicable
			var autoNumberPK = insert.Into.PrimaryKey.Where(pk => pk.Column.IsAutoNumber).SingleOrDefault();

			if (autoNumberPK != null)
			{
				var generatedId = NativeDataBase.GetScalar(SqlGenerator.LastAutogeneratedId(insert.Into.Table));
				autoNumberPK.SetValueFromColumn(insert.Values.First().Instance, generatedId);
			}

			return result;
		}

		public int Update(Update update)
		{
			Command sql = SqlGenerator.Update(Parse(update));
			return NativeDataBase.Execute(sql);
		}

		public int Delete(Delete delete)
		{
			Command sql = SqlGenerator.Delete(Parse(delete));
			return NativeDataBase.Execute(sql);
		}

		public IEnumerable<TType> Select<TType>(Select select)
		{
			Command sql = SqlGenerator.Select(Parse(select));
			
			using (var dataReader = NativeDataBase.GetDataReader(sql))
			{
				while (dataReader.Read())
				{
					TType instance = Activator.CreateInstance<TType>();

					foreach (SelectMember member in select.Members)
					{
						object value = Convert.ChangeType(string.IsNullOrWhiteSpace(member.Alias) ? dataReader[member.Member.Column.Name] : dataReader[member.Alias], member.Member.ReturnType);
						member.Member.SetValueFromColumn(instance, value);
					}

					yield return instance;
				}
			}
		}

		public void Create<TType>()
		{
			DataType dtype = typeof(TType);
			Command sql;

			foreach (DataType parent in dtype.GetBaseDataTypes().Reverse())
			{
				if (NativeDataBase.ExistsTable(parent.Table.Name))
				{
					continue;
				}

				sql = SqlGenerator.Create(parent.Table);
				NativeDataBase.Execute(sql);

				foreach (Sql.Schema.Index index in parent.Table.Indexes)
				{
					sql = SqlGenerator.Create(index);
					NativeDataBase.Execute(sql);
				}

				foreach (Sql.Schema.ForeignKey fk in parent.Table.ForeignKeys)
				{
					sql = SqlGenerator.Create(fk);
					NativeDataBase.Execute(sql);
				}
			}
		}

		public void Drop<TType>()
		{
			DataType dtype = typeof(TType);
			Command sql;

			if (!NativeDataBase.ExistsTable(dtype.Table.Name))
			{
				return;
			}

			foreach (Sql.Schema.Index index in dtype.Table.Indexes)
			{
				sql = SqlGenerator.Drop(index);
				NativeDataBase.Execute(sql);
			}

			foreach (Sql.Schema.ForeignKey fk in dtype.Table.ForeignKeys)
			{
				sql = SqlGenerator.Drop(fk);
				NativeDataBase.Execute(sql);
			}

			sql = SqlGenerator.Drop(dtype.Table);
			NativeDataBase.Execute(sql);
		}

		#region Filter parsing

		protected Sql.Filters.FilterBase Parse(FilterBase filter)
		{
			//Validating if there are filters defined
			if (filter == null) return null;

			if (filter is CustomFilter) return Parse((CustomFilter)filter);
			if (filter is InFilter) return Parse((InFilter)filter);
			if (filter is LikeFilter) return Parse((LikeFilter)filter);
			if (filter is RangeFilter) return Parse((RangeFilter)filter);
			if (filter is MemberCompareFilter) return Parse((MemberCompareFilter)filter);
			if (filter is ValueCompareFilter) return Parse((ValueCompareFilter)filter);
			if (filter is AndFilter) return Parse((AndFilter)filter);
			if (filter is OrFilter) return Parse((OrFilter)filter);

			throw new ArgumentOutOfRangeException("filter");
		}

		protected Sql.Filters.CustomFilter Parse(CustomFilter filter)
		{
			return new Sql.Filters.CustomFilter()
			{
				Filter = filter.Filter,
			};
		}

		protected Sql.Filters.InFilter Parse(InFilter filter)
		{
			var native = new Sql.Filters.InFilter();
			native.Column = filter.Member.Column;
			native.CaseSensitive = filter.CaseSensitive;
			native.TableAlias = filter.TypeAlias;

			foreach (IComparable v in filter.Values)
			{
				IComparable converted;

				if (filter.Member.Converter != null)
				{
					converted = (IComparable) filter.Member.Converter.MemberToColumn(v);
				}
				else
				{
					converted = v;
				}

				native.Values.Add(converted);
			}

			return native;
		}

		protected Sql.Filters.LikeFilter Parse(LikeFilter filter)
		{
			string pattern;

			if (filter.Member.Converter != null)
			{
				pattern = (string) filter.Member.Converter.MemberToColumn(filter.Pattern);
			}
			else
			{
				pattern = filter.Pattern;
			}

			return new Sql.Filters.LikeFilter()
			{
				Column = filter.Member.Column,
				Pattern = filter.Pattern,
				CaseSensitive = filter.CaseSensitive,
				TableAlias = filter.TypeAlias
			};
		}

		protected Sql.Filters.RangeFilter Parse(RangeFilter filter)
		{
			IComparable minValue, maxValue;

			if (filter.Member.Converter != null)
			{
				minValue = (IComparable)filter.Member.Converter.MemberToColumn(filter.MinValue);
				maxValue = (IComparable)filter.Member.Converter.MemberToColumn(filter.MaxValue);
			}
			else
			{
				minValue = filter.MinValue;
				maxValue = filter.MaxValue;
			}

			return new Sql.Filters.RangeFilter()
			{
				Column = filter.Member.Column,
				MinValue = minValue,
				MaxValue = maxValue,
				TableAlias = filter.TypeAlias,
			};
		}

		protected Sql.Filters.ColumnCompareFilter Parse(MemberCompareFilter filter)
		{
			return new Sql.Filters.ColumnCompareFilter()
			{
				Column = filter.Member.Column, 
				ColumnToCompare = filter.MemberToCompare.Column,
				Operator = filter.Operator,
				TableAlias = filter.TypeAlias,
				ColumnToCompareTableAlias = filter.MemberToCompareTypeAlias
			};
		}

		protected Sql.Filters.ValueCompareFilter Parse(ValueCompareFilter filter)
		{
			IComparable value;

			if (filter.Member.Converter != null)
			{
				value = (IComparable)filter.Member.Converter.MemberToColumn(filter.ValueToCompare);
			}
			else
			{
				value = filter.ValueToCompare;
			}

			return new Sql.Filters.ValueCompareFilter()
			{
				Column = filter.Member.Column,
				ValueToCompare = value,
				Operator = filter.Operator,
				TableAlias = filter.TypeAlias
			};
		}

		protected Sql.Filters.AndFilter Parse(AndFilter filter)
		{
			var native = new Sql.Filters.AndFilter();

			foreach (FilterBase f in filter.InnerFilters)
			{
				native.InnerFilters.Add(Parse(f));
			}

			return native;
		}

		protected Sql.Filters.OrFilter Parse(OrFilter filter)
		{
			var native = new Sql.Filters.OrFilter();

			foreach (FilterBase f in filter.InnerFilters)
			{
				native.InnerFilters.Add(Parse(f));
			}

			return native;
		}

		#endregion

		#region Operation parsing

		protected Sql.Operations.Insert Parse(Insert insert)
		{
			if (insert == null)
			{
				return null;
			}

			var native = new OKHOSTING.Sql.Operations.Insert();
			native.Into = insert.Into.Table;

			foreach (MemberValue mvalue in insert.Values)
			{
				native.Values.Add(new Sql.Operations.ColumnValue(mvalue.DataMember.Column, mvalue.ValueForColumn));
			}

			return native;
		}

		protected Sql.Operations.Update Parse(Update update)
		{
			if (update == null)
			{
				return null;
			}
			
			var native = new OKHOSTING.Sql.Operations.Update();
			native.From = update.From.Table;

			foreach (MemberValue mvalue in update.Set)
			{
				native.Set.Add(new Sql.Operations.ColumnValue(mvalue.DataMember.Column, mvalue.ValueForColumn));
			}

			foreach (Filters.FilterBase filter in update.Where)
			{
				native.Where.Add(Parse(filter));
			}

			return native;
		}

		protected Sql.Operations.Delete Parse(Delete delete)
		{
			if (delete == null)
			{
				return null;
			}
			
			var native = new OKHOSTING.Sql.Operations.Delete();
			native.From = delete.From.Table;

			foreach (Filters.FilterBase filter in delete.Where)
			{
				native.Where.Add(Parse(filter));
			}

			return native;
		}

		protected Sql.Operations.Select Parse(Select select)
		{
			if (select == null)
			{
				return null;
			}
			
			if (select is SelectAggregate)
			{
				return Parse((SelectAggregate)select);
			}

			return Parse(select, new OKHOSTING.Sql.Operations.Select());
		}

		protected Sql.Operations.SelectAggregate Parse(SelectAggregate select)
		{
			if (select == null)
			{
				return null;
			}
			
			var native = (OKHOSTING.Sql.Operations.SelectAggregate)Parse(select, new OKHOSTING.Sql.Operations.SelectAggregate());

			foreach (SelectAggregateMember agregateMember in select.AggregateMembers)
			{
				native.AggregateColumns.Add(Parse(agregateMember));
			}

			foreach (DataMember groupBy in select.GroupBy)
			{
				native.GroupBy.Add(groupBy.Column);
			}

			return native;
		}

		protected Sql.Operations.SelectAggregateColumn Parse(SelectAggregateMember aggregateMember)
		{
			return new Sql.Operations.SelectAggregateColumn(aggregateMember.Member.Column, aggregateMember.AggregateFunction, aggregateMember.Alias, aggregateMember.Distinct);
		}

		protected Sql.Operations.OrderBy Parse(OrderBy orderBy)
		{
			if (orderBy == null)
			{
				return null;
			}
			
			var native = new OKHOSTING.Sql.Operations.OrderBy();
			native.Column = orderBy.Member.Column;
			native.Direction = orderBy.Direction;

			return native;
		}

		protected Sql.Operations.SelectLimit Parse(SelectLimit limit)
		{
			if (limit == null)
			{
				return null;
			}

			var native = new OKHOSTING.Sql.Operations.SelectLimit();
			native.From = limit.From;
			native.To = limit.To;

			return native;
		}

		protected Sql.Operations.SelectJoin Parse(SelectJoin join)
		{
			if (join == null)
			{
				return null;
			}
			
			var native = new OKHOSTING.Sql.Operations.SelectJoin();
			native.Table = join.Type.Table;
			native.JoinType = join.JoinType;
			native.Alias = join.Alias;

			foreach (SelectMember selectMember in join.Members)
			{
				native.Columns.Add(new Sql.Operations.SelectColumn(selectMember.Member.Column, selectMember.Alias));
			}

			foreach (Filters.FilterBase filter in join.On)
			{
				native.On.Add(Parse(filter));
			}

			return native;
		}

		protected Sql.Operations.Select Parse(Select select, Sql.Operations.Select native)
		{
			if (select == null)
			{
				return null;
			}

			if (native == null)
			{
				throw new ArgumentNullException("native");
			}
			
			native.From = select.From.Table;
			native.Limit = Parse(select.Limit);

			foreach (SelectMember selectMember in select.Members)
			{
				native.Columns.Add(new Sql.Operations.SelectColumn(selectMember.Member.Column, selectMember.Alias));
			}

			foreach (SelectJoin join in select.Joins)
			{
				native.Joins.Add(Parse(join));
			}

			foreach (Filters.FilterBase filter in select.Where)
			{
				native.Where.Add(Parse(filter));
			}

			foreach (OrderBy orderBy in select.OrderBy)
			{
				native.OrderBy.Add(Parse(orderBy));
			}

			return native;
		}

		#endregion
	}
}