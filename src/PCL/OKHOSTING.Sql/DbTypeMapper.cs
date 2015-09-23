using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using OKHOSTING.Core;

namespace OKHOSTING.Sql
{
	public static class DbTypeMapper
	{
		public static Dictionary<DbType, Type> DbTypeMap;

		static DbTypeMapper()
		{
			DbTypeMap = new Dictionary<DbType, Type>();

			DbTypeMap.Add(DbType.String, typeof(string));
			DbTypeMap.Add(DbType.AnsiString, typeof(string));
			DbTypeMap.Add(DbType.AnsiStringFixedLength, typeof(string));
			DbTypeMap.Add(DbType.StringFixedLength, typeof(string));
			DbTypeMap.Add(DbType.Xml, typeof(string));

			DbTypeMap.Add(DbType.DateTime, typeof(DateTime));
			DbTypeMap.Add(DbType.DateTime2, typeof(DateTime));
			DbTypeMap.Add(DbType.DateTimeOffset, typeof(DateTime));
			DbTypeMap.Add(DbType.Date, typeof(DateTime));
			DbTypeMap.Add(DbType.Time, typeof(DateTime));

			DbTypeMap.Add(DbType.Currency, typeof(Decimal));
			DbTypeMap.Add(DbType.Decimal, typeof(Decimal));

			DbTypeMap.Add(DbType.Binary, typeof(byte[]));
			DbTypeMap.Add(DbType.Object, typeof(object));

			DbTypeMap.Add(DbType.Boolean, typeof(bool));
			DbTypeMap.Add(DbType.Double, typeof(Double));
			DbTypeMap.Add(DbType.Single, typeof(Single));
			DbTypeMap.Add(DbType.Guid, typeof(Guid));

			DbTypeMap.Add(DbType.SByte, typeof(SByte));
			DbTypeMap.Add(DbType.Int16, typeof(Int16));
			DbTypeMap.Add(DbType.Int32, typeof(Int32));
			DbTypeMap.Add(DbType.Int64, typeof(Int64));
			DbTypeMap.Add(DbType.VarNumeric, typeof(Int64));

			DbTypeMap.Add(DbType.Byte, typeof(Byte));
			DbTypeMap.Add(DbType.UInt16, typeof(UInt16));
			DbTypeMap.Add(DbType.UInt32, typeof(UInt32));
			DbTypeMap.Add(DbType.UInt64, typeof(UInt64));
		}

		public static Type Parse(DbType dbType)
		{
			return DbTypeMap[dbType];
		}

		public static DbType Parse(Type dbType)
		{
			if (dbType.GetTypeInfo().IsEnum)
			{
				return DbType.Int32;
			}

			return DbTypeMap.Reverse(dbType);
		}
	}
}
