using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Data.Common;
using System.Data;
using System.Text;
using OKHOSTING.Core.Extensions;

namespace OKHOSTING.Sql.Odbc
{
	/// <summary>
	/// Implements methods for executing Sql scripts in a Odbc DataBase
	/// </summary>
	public class DataBase : OKHOSTING.Sql.DataBase
	{
		/// <summary>
		/// Initializes a new instance of the executer
		/// </summary>
		/// <param name="ConnectionString"> 
		/// The connection string to use to connect to the DataBase
		/// </param>
		public DataBase(): base(System.Data.Odbc.OdbcFactory.Instance){}

		/// <summary>
		/// Returns the Date and Hour from the database Server
		/// </summary>
		/// <returns>
		/// Date and Hour from the database Server
		/// </returns>
		public override DateTime DateTimeOnDBServer()
		{
			throw new NotSupportedException("OdbcExecuter is not compatible with the method BaseExecuter.DateTimeOnDBServer()");
		}

		/// <summary>
		/// Returns a Global Unique Identifier from the Database
		/// </summary>
		/// <returns>
		/// Global Unique Identifier 
		/// </returns>
		public override string GetUniqueIdentifier()
		{
			throw new NotSupportedException("OdbcExecuter is not compatible with the method BaseExecuter.GetUniqueIdentifier()");
		}

		/// <summary>
		/// Returns a value that indicates if the Constrainst 
		/// exists in the Underlying Database 
		/// </summary>
		/// <param name="Name">
		/// Name of the constraint to validate
		/// </param>
		/// <returns>
		/// Value that indicates if the Constrainst 
		/// exists in the Underlying Database 
		/// </returns>
		public override bool ExistsConstraint(string Name)
		{
			throw new NotSupportedException("OdbcExecuter is not compatible with the method BaseExecuter.ConstraintExists()");
		}

		/// <summary>
		/// Verify if exists the specified index on the Database
		/// </summary>
		/// <param name="Name">
		/// Name of the index
		/// </param>
		/// <returns>
		/// Boolean value that indicates if exists 
		/// the specified index on the Database
		/// </returns>
		public override bool ExistsIndex(string Name)
		{
			throw new NotSupportedException("OdbcExecuter is not compatible with the method BaseExecuter.IndexExists()");
		}

		private static Dictionary<DbType, OdbcType> DbTypeMap;

		static DataBase()
		{
			DbTypeMap = new Dictionary<DbType, OdbcType>();

			DbTypeMap.Add(DbType.AnsiString, OdbcType.VarChar);
			DbTypeMap.Add(DbType.AnsiStringFixedLength, OdbcType.Char);
			DbTypeMap.Add(DbType.Binary, OdbcType.Binary);
			DbTypeMap.Add(DbType.Boolean, OdbcType.Bit);
			DbTypeMap.Add(DbType.Byte, OdbcType.TinyInt);
			DbTypeMap.Add(DbType.Currency, OdbcType.Decimal);
			DbTypeMap.Add(DbType.Date, OdbcType.Date);
			DbTypeMap.Add(DbType.DateTime, OdbcType.DateTime);
			DbTypeMap.Add(DbType.DateTime2, OdbcType.DateTime);
			DbTypeMap.Add(DbType.DateTimeOffset, OdbcType.DateTime);
			DbTypeMap.Add(DbType.Decimal, OdbcType.Decimal);
			DbTypeMap.Add(DbType.Double, OdbcType.Double);
			DbTypeMap.Add(DbType.Int16, OdbcType.SmallInt);
			DbTypeMap.Add(DbType.Int32, OdbcType.Int);
			DbTypeMap.Add(DbType.Int64, OdbcType.BigInt);
			DbTypeMap.Add(DbType.Guid, OdbcType.VarChar);
			DbTypeMap.Add(DbType.Object, OdbcType.Binary);
			DbTypeMap.Add(DbType.SByte, OdbcType.TinyInt);
			DbTypeMap.Add(DbType.Single, OdbcType.Real);
			DbTypeMap.Add(DbType.String, OdbcType.VarChar);
			DbTypeMap.Add(DbType.StringFixedLength, OdbcType.Char);
			DbTypeMap.Add(DbType.Time, OdbcType.Time);
			DbTypeMap.Add(DbType.UInt16, OdbcType.SmallInt);
			DbTypeMap.Add(DbType.UInt32, OdbcType.Int);
			DbTypeMap.Add(DbType.UInt64, OdbcType.BigInt);
			DbTypeMap.Add(DbType.VarNumeric, OdbcType.Numeric);
			DbTypeMap.Add(DbType.Xml, OdbcType.VarChar);

			//DbTypeMap.Add(DbType.AnsiString, OdbcType.VarChar);
			//DbTypeMap.Add(DbType.Binary, OdbcType.Timestamp);
			//DbTypeMap.Add(DbType.Binary, OdbcType.VarBinary);
			//DbTypeMap.Add(DbType.String, OdbcType.VarString);
		}

		public static new OdbcType Parse(DbType dbType)
		{
			return DbTypeMap[dbType];
		}

		public static DbType Parse(OdbcType dbType)
		{
			return DbTypeMap.Reverse(dbType);
		}

		protected override string SchemaProvider
		{
			get
			{
				throw new NotImplementedException();
			}
		}
	}
}