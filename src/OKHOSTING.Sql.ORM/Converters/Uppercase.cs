﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OKHOSTING.Sql.ORM.Converters
{
	/// <summary>
	/// Trims a string value, removing white spaces from begining and end
	/// </summary>
	public class Uppercase : ConverterBase<string, string>
	{
		public override string MemberToColumn(string memberValue)
		{
			return memberValue.ToUpper();
		}

		public override string ColumnToMember(string columnValue)
		{
			return columnValue.ToUpper();
		}
	}
}