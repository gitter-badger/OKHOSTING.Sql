using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OKHOSTING.Sql.ORM.Filters
{
	public static class FilterConverter
	{
		public Sql.Filters.ValueCompareFilter Parse(ValueCompareFilter filter)
		{
			return new Sql.Filters.ValueCompareFilter(filter.Member.Column, filter.ValueToCompare, filter.Operator);
		}

		public Sql.Filters.ColumnCompareFilter Parse(MemberCompareFilter filter)
		{
			return new Sql.Filters.ColumnCompareFilter(filter.Member.Column, filter.MemberToCompare.Column, filter.Operator);
		}

		public Sql.Filters.AndFilter Parse(AndFilter filter)
		{
			var native = new Sql.Filters.AndFilter();

			foreach (FilterBase f in filter.InnerFilters)
			{
				native.InnerFilters.Add(Parse(f));
			}

			return native;
		}

		public Sql.Filters.OrFilter Parse(OrFilter filter)
		{
			var native = new Sql.Filters.OrFilter();

			foreach (FilterBase f in filter.InnerFilters)
			{
				native.InnerFilters.Add(Parse(f));
			}

			return native;
		}


		public Sql.Filters.FilterBase Parse(FilterBase filter)
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
	}
}