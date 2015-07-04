using OKHOSTING.Sql.ORM.Filters;
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
		protected readonly Dictionary<Type, object> Tables;

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
			string sql = SqlGenerator.Insert(Parse(insert));
			return NativeDataBase.Execute(sql);
		}

		public int Update(Update update)
		{
			string sql = SqlGenerator.Update(Parse(update));
			return NativeDataBase.Execute(sql);
		}

		public int Delete(Delete delete)
		{
			string sql = SqlGenerator.Delete(Parse(delete));
			return NativeDataBase.Execute(sql);
		}

		public IEnumerable<TType> Select<TType>(Select select)
		{
			string sql = SqlGenerator.Select(Parse(select));
			var dataReader = NativeDataBase.GetDataReader(sql);

			while (dataReader.Read())
			{
				TType instance = Activator.CreateInstance<TType>();

				foreach (SelectMember member in select.Members)
				{
					object value = Convert.ChangeType(string.IsNullOrWhiteSpace(member.Alias) ? dataReader[member.Member.Column.Name] : dataReader[member.Alias], member.Member.ReturnType);
					member.Member.SetValue(instance, value);
				}

				yield return instance;
			}
		}

		public void Create<TType>()
		{
			DataType dtype = typeof(TType);
			string sql;

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
			string sql;

			foreach (DataType parent in dtype.GetBaseDataTypes().Reverse())
			{
				if (!NativeDataBase.ExistsTable(parent.Table.Name))
				{
					continue;
				}

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

				sql = SqlGenerator.Drop(parent.Table);
				NativeDataBase.Execute(sql);
			}
		}

		#region Filter parsing

		protected Sql.Filters.FilterBase Parse(FilterBase filter)
		{
			//Validating if there are filters defined
			if (filter == null) return null;

			if (filter is MemberCompareFilter) return Parse((MemberCompareFilter)filter);
			if (filter is CustomFilter) return Parse((CustomFilter)filter);
			if (filter is InFilter) return Parse((InFilter)filter);
			if (filter is LikeFilter) return Parse((LikeFilter)filter);
			if (filter is RangeFilter) return Parse((RangeFilter)filter);
			if (filter is ValueCompareFilter) return Parse((ValueCompareFilter)filter);
			if (filter is LogicalOperatorFilter) return Parse((LogicalOperatorFilter)filter);

			throw new ArgumentOutOfRangeException("filter");
		}

		protected Sql.Filters.CustomFilter Parse(CustomFilter filter)
		{
			return new Sql.Filters.CustomFilter(filter.Filter);
		}

		protected Sql.Filters.InFilter Parse(InFilter filter)
		{
			var native = new Sql.Filters.InFilter();
			native.Column = filter.Member.Column;
			native.CaseSensitive = filter.CaseSensitive;

			foreach (IComparable v in filter.Values)
			{
				native.Values.Add(v);
			}

			return native;
		}

		protected Sql.Filters.LikeFilter Parse(LikeFilter filter)
		{
			return new Sql.Filters.LikeFilter(filter.Member.Column, filter.Pattern, filter.CaseSensitive);
		}

		protected Sql.Filters.ColumnCompareFilter Parse(MemberCompareFilter filter)
		{
			return new Sql.Filters.ColumnCompareFilter(filter.Member.Column, filter.MemberToCompare.Column, filter.Operator);
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
			var native = new OKHOSTING.Sql.Operations.Insert();
			native.Into = insert.Into.Table;

			foreach (MemberValue mvalue in insert.Values)
			{
				native.Values.Add(new Sql.Operations.ColumnValue(mvalue.MemberMap.Column, mvalue.Value));
			}

			return native;
		}

		protected Sql.Operations.Update Parse(Update update)
		{
			var native = new OKHOSTING.Sql.Operations.Update();
			native.From = update.From.Table;

			foreach (MemberValue mvalue in update.Set)
			{
				native.Set.Add(new Sql.Operations.ColumnValue(mvalue.MemberMap.Column, mvalue.Value));
			}

			foreach (Filters.FilterBase filter in update.Where)
			{
				native.Where.Add(Parse(filter));
			}

			return native;
		}

		protected Sql.Operations.Delete Parse(Delete delete)
		{
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
			if (select is SelectAggregate)
			{
				return Parse((SelectAggregate)select);
			}

			return Parse(select, new OKHOSTING.Sql.Operations.Select());
		}

		protected Sql.Operations.SelectAggregate Parse(SelectAggregate select)
		{
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
			var native = new OKHOSTING.Sql.Operations.OrderBy();
			native.Column = orderBy.Member.Column;
			native.Direction = orderBy.Direction;

			return native;
		}

		protected Sql.Operations.SelectLimit Parse(SelectLimit orderBy)
		{
			var native = new OKHOSTING.Sql.Operations.SelectLimit();
			native.From = orderBy.From;
			native.To = orderBy.To;

			return native;
		}

		protected Sql.Operations.SelectJoin Parse(SelectJoin join)
		{
			var native = new OKHOSTING.Sql.Operations.SelectJoin();
			native.Table = join.Type.Table;
			native.JoinType = join.JoinType;

			foreach (Filters.FilterBase filter in join.On)
			{
				native.On.Add(Parse(filter));
			}

			return native;
		}

		protected Sql.Operations.Select Parse(Select select, Sql.Operations.Select native)
		{
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