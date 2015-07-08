using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OKHOSTING.Sql.ORM.Converters
{
	public class Json : ConverterBase<object, string>
	{
		public readonly Type MemberType;

		public Json(Type memberType)
		{
			MemberType = memberType;
		}

		public override string MemberToColumn(object memberValue)
		{
			return Newtonsoft.Json.JsonConvert.SerializeObject(memberValue);
		}

		public override object ColumnToMember(string columnValue)
		{
			return Newtonsoft.Json.JsonConvert.DeserializeObject(columnValue, MemberType);
		}
	}
	
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
	}
}