using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OKHOSTING.Sql.ORM.Operations
{
	public class Insert
	{
		public Insert()
		{
		}

		public Insert(object instance)
		{
			Into = instance.GetType();

			foreach (DataMember dmember in Into.Members)
			{
				Values.Add(new MemberValue(dmember, instance));
			}
		}

		public DataType Into { get; set; }
		public readonly List<MemberValue> Values = new List<MemberValue>();
	}
}