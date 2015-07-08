using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OKHOSTING.Sql.ORM.Converters
{
	/// <summary>
	/// A generic converter usefull as a base class for your own converters
	/// </summary>
	/// <typeparam name="TType">Return type of the DataMember</typeparam>
	/// <typeparam name="TColumn">Type of the column</typeparam>
	public abstract class ConverterBase<TType, TColumn> : IConverter
	{
		public abstract TColumn MemberToColumn(TType memberValue);
		public abstract TType ColumnToMember(TColumn columnValue);

		object IConverter.MemberToColumn(object memberValue)
		{
			return MemberToColumn((TType) memberValue);
		}

		object IConverter.ColumnToMember(object columnValue)
		{
			return ColumnToMember((TColumn) columnValue);
		}
	}
}