using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OKHOSTING.Sql.ORM.Operations
{
	public static class OperationConverter
	{
		public static Sql.Operations.Insert Parse(Insert insert)
		{
			var native = new OKHOSTING.Sql.Operations.Insert();
			native.Into = insert.Into.Table;

			foreach (MemberValue mvalue in insert.Values)
			{
				native.Values.Add(new Sql.Operations.ColumnValue(mvalue.MemberMap.Column, mvalue.Value));
			}

			return native;
		}

		public static Sql.Operations.Update Parse(Update update)
		{
			var native = new OKHOSTING.Sql.Operations.Update();
			native.From = update.From.Table;

			foreach (MemberValue mvalue in update.Set)
			{
				native.Set.Add(new Sql.Operations.ColumnValue(mvalue.MemberMap.Column, mvalue.Value));
			}

			foreach (Filters.FilterBase filter in update.Where)
			{
				native.Where.Add(Filters.FilterConverter.Parse(filter));
			}

			return native;
		}

		public static Sql.Operations.Delete Parse(Delete delete)
		{
			var native = new OKHOSTING.Sql.Operations.Delete();
			native.From = delete.From.Table;

			foreach (Filters.FilterBase filter in delete.Where)
			{
				native.Where.Add(Filters.FilterConverter.Parse(filter));
			}

			return native;
		}

		public static Sql.Operations.Select Parse(Select select)
		{
			var native = new OKHOSTING.Sql.Operations.Select();
			native.From = select.From.Table;
			native.Limit = Parse(select.Limit);

			foreach (DataMember member in select.Members)
			{
				native.Columns.Add(member.Column);
			}

			foreach (SelectJoin join in select.Joins)
			{
				native.Joins.Add(Parse(join));
			}
			
			foreach (Filters.FilterBase filter in select.Where)
			{
				native.Where.Add(Filters.FilterConverter.Parse(filter));
			}

			foreach (OrderBy orderBy in select.OrderBy)
			{
				native.OrderBy.Add(Parse(orderBy));
			}

			return native;
		}

		public static Sql.Operations.OrderBy Parse(OrderBy orderBy)
		{
			var native = new OKHOSTING.Sql.Operations.OrderBy();
			native.Column = orderBy.Member.Column;
			native.Direction = orderBy.Direction;

			return native;
		}

		public static Sql.Operations.SelectLimit Parse(SelectLimit orderBy)
		{
			var native = new OKHOSTING.Sql.Operations.SelectLimit();
			native.From = orderBy.From;
			native.To = orderBy.To;

			return native;
		}

		public static Sql.Operations.SelectJoin Parse(SelectJoin join)
		{
			var native = new OKHOSTING.Sql.Operations.SelectJoin();
			native.Table = join.Type.Table;
			native.JoinType = join.JoinType;

			foreach (Filters.FilterBase filter in join.On)
			{
				native.On.Add(Filters.FilterConverter.Parse(filter));
			}

			return native;
		}

	}
}