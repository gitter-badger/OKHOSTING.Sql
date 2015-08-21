using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OKHOSTING.Sql.ORM.Converters
{
	/// <summary>
	/// Trims a string value, removing white spaces from begining and end
	/// </summary>
	public class Trim: ConverterBase<string, string>
	{
		public override string MemberToColumn(string memberValue)
		{
			return memberValue.Trim();
		}

		public override string ColumnToMember(string columnValue)
		{
			return columnValue.Trim();
		}


		public override object MemberToColumn(object memberValue)
		{
			return MemberToColumn((string) memberValue);
		}

		public override object ColumnToMember(object columnValue)
		{
			return ColumnToMember((string) columnValue);
		}
	}
}