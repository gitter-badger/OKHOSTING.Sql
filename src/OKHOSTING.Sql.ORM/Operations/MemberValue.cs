using OKHOSTING.Data.Sql.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OKHOSTING.Sql.ORM.Operations
{
	/// <summary>
	/// A tuple of a column and a value that will be written to that column on insert or update operations
	/// </summary>
	public class MemberValue
	{
		public readonly MemberMap MemberMap;
		public readonly object Value;

		public MemberValue(MemberMap member, object value)
		{
			if (member == null)
			{
				throw new ArgumentNullException("member");
			}

			MemberMap = member;
			Value = value;
		}
	}
}