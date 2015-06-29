using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OKHOSTING.Sql.ORM.Filters
{
	/// <summary>
	/// Converts ORM filters into SQL Filters
	/// </summary>
	public static class FilterConverter
	{
		public static Sql.Filters.FilterBase Parse(FilterBase filter)
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

		public static Sql.Filters.CustomFilter Parse(CustomFilter filter)
		{
			return new Sql.Filters.CustomFilter(filter.Filter);
		}

		public static Sql.Filters.InFilter Parse(InFilter filter)
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

		public static Sql.Filters.LikeFilter Parse(LikeFilter filter)
		{
			return new Sql.Filters.LikeFilter(filter.Member.Column, filter.Pattern, filter.CaseSensitive);
		}

		public static Sql.Filters.ColumnCompareFilter Parse(MemberCompareFilter filter)
		{
			return new Sql.Filters.ColumnCompareFilter(filter.Member.Column, filter.MemberToCompare.Column, filter.Operator);
		}

		public static Sql.Filters.AndFilter Parse(AndFilter filter)
		{
			var native = new Sql.Filters.AndFilter();

			foreach (FilterBase f in filter.InnerFilters)
			{
				native.InnerFilters.Add(Parse(f));
			}

			return native;
		}

		public static Sql.Filters.OrFilter Parse(OrFilter filter)
		{
			var native = new Sql.Filters.OrFilter();

			foreach (FilterBase f in filter.InnerFilters)
			{
				native.InnerFilters.Add(Parse(f));
			}

			return native;
		}
	}
}