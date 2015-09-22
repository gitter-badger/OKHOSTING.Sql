using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Data.Common;
using System.Data;
using System.Text;
using OKHOSTING.Core;

namespace OKHOSTING.Sql.OleDb
{

	/// <summary>
	/// Implements methods for executing Sql scripts in a OleDb DataBase
	/// </summary>
	public class DataBase : OKHOSTING.Sql.DataBase
	{
		/// <summary>
		/// Initializes a new instance of the executer
		/// </summary>
		/// <param name="ConnectionString"> 
		/// The connection string to use to connect to the DataBase
		/// </param>
		public DataBase(): base(System.Data.OleDb.OleDbFactory.Instance)
		{
		}

		/// <summary>
		/// Returns the Date and Hour from the database Server
		/// </summary>
		/// <returns>
		/// Date and Hour from the database Server
		/// </returns>
		public override DateTime DateTimeOnDBServer()
		{
			throw new NotSupportedException("OleDbExecuter is not compatible with the method BaseExecuter.DateTimeOnDBServer()");
		}

		/// <summary>
		/// Returns a Global Unique Identifier from the Database
		/// </summary>
		/// <returns>
		/// Global Unique Identifier 
		/// </returns>
		public override string GetUniqueIdentifier()
		{
			throw new NotSupportedException("OleDbExecuter is not compatible with the method BaseExecuter.GetUniqueIdentifier()");
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
			throw new NotSupportedException("OleDbExecuter is not compatible with the method BaseExecuter.ConstraintExists()");
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
			throw new NotSupportedException("OleDbExecuter is not compatible with the method BaseExecuter.IndexExists()");
		}

		private static new Dictionary<DbType, OleDbType> DbTypeMap;

		static DataBase()
		{
			DbTypeMap = new Dictionary<DbType, OleDbType>();

			DbTypeMap.Add(DbType.AnsiString, OleDbType.VarChar);
			DbTypeMap.Add(DbType.AnsiStringFixedLength, OleDbType.Char);
			DbTypeMap.Add(DbType.Binary, OleDbType.Binary);
			DbTypeMap.Add(DbType.Boolean, OleDbType.Boolean);
			DbTypeMap.Add(DbType.Byte, OleDbType.UnsignedTinyInt);
			DbTypeMap.Add(DbType.Currency, OleDbType.Decimal);
			DbTypeMap.Add(DbType.Date, OleDbType.Date);
			DbTypeMap.Add(DbType.DateTime, OleDbType.DBTimeStamp);
			DbTypeMap.Add(DbType.DateTime2, OleDbType.DBTimeStamp);
			DbTypeMap.Add(DbType.DateTimeOffset, OleDbType.DBTimeStamp);
			DbTypeMap.Add(DbType.Decimal, OleDbType.Decimal);
			DbTypeMap.Add(DbType.Double, OleDbType.Double);
			DbTypeMap.Add(DbType.Int16, OleDbType.SmallInt);
			DbTypeMap.Add(DbType.Int32, OleDbType.Integer);
			DbTypeMap.Add(DbType.Int64, OleDbType.BigInt);
			DbTypeMap.Add(DbType.Guid, OleDbType.Guid);
			DbTypeMap.Add(DbType.Object, OleDbType.Binary);
			DbTypeMap.Add(DbType.SByte, OleDbType.TinyInt);
			DbTypeMap.Add(DbType.Single, OleDbType.Single);
			DbTypeMap.Add(DbType.String, OleDbType.VarChar);
			DbTypeMap.Add(DbType.StringFixedLength, OleDbType.Char);
			DbTypeMap.Add(DbType.Time, OleDbType.DBTime);
			DbTypeMap.Add(DbType.UInt16, OleDbType.UnsignedSmallInt);
			DbTypeMap.Add(DbType.UInt32, OleDbType.UnsignedInt);
			DbTypeMap.Add(DbType.UInt64, OleDbType.UnsignedBigInt);
			DbTypeMap.Add(DbType.VarNumeric, OleDbType.VarNumeric);
			DbTypeMap.Add(DbType.Xml, OleDbType.VarChar);

			//DbTypeMap.Add(DbType.AnsiString, OleDbType.VarChar);
			//DbTypeMap.Add(DbType.Binary, OleDbType.Timestamp);
			//DbTypeMap.Add(DbType.Binary, OleDbType.VarBinary);
			//DbTypeMap.Add(DbType.String, OleDbType.VarString);
		}

		public static new OleDbType Parse(DbType dbType)
		{
			return DbTypeMap[dbType];
		}

		public static DbType Parse(OleDbType dbType)
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
