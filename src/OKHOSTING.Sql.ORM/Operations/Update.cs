using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OKHOSTING.Sql.ORM.Operations
{
	public class Update
	{
		public Update()
		{
		}

		public Update(object instance)
		{
			From = instance.GetType();

			foreach (DataMember dmember in From.Members)
			{
				Set.Add(new MemberValue(dmember, instance));
			}

			foreach(var pk in From.PrimaryKey)
			{
				var filter = new Filters.ValueCompareFilter(pk, (IComparable) pk.GetValue(instance));
				Where.Add(filter);
			}
		}

		public DataType From { get; set; }
		public readonly List<MemberValue> Set = new List<MemberValue>();
		public readonly List<Filters.FilterBase> Where = new List<Filters.FilterBase>();
	}
}