﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OKHOSTING.Sql.ORM.Converters
{
	public class Json<TType> : ConverterBase<TType, string>
	{
		public override string MemberToColumn(TType memberValue)
		{
			return Newtonsoft.Json.JsonConvert.SerializeObject(memberValue);
		}

		public override TType ColumnToMember(string columnValue)
		{
			return Newtonsoft.Json.JsonConvert.DeserializeObject<TType>(columnValue);
		}

		public override object MemberToColumn(object memberValue)
		{
			return MemberToColumn((TType) memberValue);
		}

		public override object ColumnToMember(object columnValue)
		{
			return ColumnToMember((string) columnValue);
		}
	}
}