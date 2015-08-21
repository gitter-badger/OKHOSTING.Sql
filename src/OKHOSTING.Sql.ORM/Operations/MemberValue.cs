using OKHOSTING.Sql.Schema;
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
		public readonly DataMember DataMember;
		public readonly object Instance;

		public object Value
		{
			get
			{
				return DataMember.Member.GetValue(Instance);
			}
		}

		public object ValueForColumn
		{
			get
			{
				return DataMember.GetValueForColumn(Instance);
			}
		}

		public MemberValue(DataMember member, object instance)
		{
			if (member == null)
			{
				throw new ArgumentNullException("member");
			}

			if (instance == null)
			{
				throw new ArgumentNullException("instance");
			}

			DataMember = member;
			Instance = instance;
		}
	}
}