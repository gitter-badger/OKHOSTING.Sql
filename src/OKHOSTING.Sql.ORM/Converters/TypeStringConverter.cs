﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OKHOSTING.Sql.ORM.Converters
{
	public class TypeStringConverter: ConverterBase<Type, string>
	{
		public override string MemberToColumn(Type memberValue)
		{
			return string.Format("{0}, {1}", memberValue.FullName, memberValue.Assembly.FullName);
		}

		public override Type ColumnToMember(string columnValue)
		{
			return Type.GetType(columnValue);
		}

		public override object MemberToColumn(object memberValue)
		{
			return MemberToColumn((string)memberValue);
		}

		public override object ColumnToMember(object columnValue)
		{
			return ColumnToMember((string)columnValue);
		}
	}
}