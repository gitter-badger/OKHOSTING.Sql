using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using OKHOSTING.Core;

namespace OKHOSTING.Sql.Net4.SqlServer
{

	/// <summary>
	/// Implements methods for executing Sql scripts in a MS Sql Server DataBase
	/// </summary>
	public class DataBase : OKHOSTING.Sql.Net4.DataBase
	{
		/// <summary>
		/// Initializes a new instance of the executer
		/// </summary>
		/// <param name="ConnectionString"> 
		/// The connection string to use to connect to the DataBase
		/// </param>
		public DataBase(): base(System.Data.SqlClient.SqlClientFactory.Instance)
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
			//Local vars
			DateTime currentDate = DateTime.MinValue;
			IDataReader reader = null;

			try
			{
				//Loading DataReader and getting the date and time
				reader = this.GetDataReader("select CurrentDate = GETDATE()");

				reader.Read();
				currentDate =  reader.GetFieldValue<DateTime>(0);

				//Closing the DataReader and it underlying connection
				reader.Close();
			}
			catch
			{
				throw;
			}
			finally
			{
				//Closing the reader if apply
				if (reader != null && !reader.IsClosed) reader.Close();
			}

			//Returning the Date and time
			return currentDate;
		}

		/// <summary>
		/// Returns a Global Unique Identifier from the Database
		/// </summary>
		/// <returns>
		/// Global Unique Identifier 
		/// </returns>
		public override string GetUniqueIdentifier()
		{
			//Local Vars
			string uniqueIdentifier = string.Empty;

			//Creating DataTable
			IDataTable tblData = this.GetDataTable("SELECT NEWID()");

			//Reading the ID
			uniqueIdentifier = tblData[0][0].ToString();

			//Returning the GUID
			return uniqueIdentifier;
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
			//Local vars
			bool existsConstraint = false;
			IDataReader reader = null;

			try
			{
				//Loading DataReader...
				reader = 
					this.GetDataReader(
					"select " + 
					"	name " + 
					"from " + 
					"	sysobjects " +
					"where " + 
					"	name = '" + Name + "' and " +
					"	(xtype = 'UQ' or xtype = 'PK')");

				//Validating if exists the constraint
				existsConstraint = (reader.Read());
			}
			catch
			{
				throw;
			}
			finally
			{
				//Closing the reader if apply
				if (reader != null && !reader.IsClosed) reader.Close();
			}

			//Returning the value to the caller
			return existsConstraint;
		}

		/// <summary>
		/// Verify if exists the specified table on the database
		/// </summary>
		/// <param name="name">
		/// Table to search
		/// </param>
		/// <returns>
		/// Boolean value that indicates if the table exists
		/// </returns>
		public override bool ExistsTable(string name)
		{
			//Local vars
			bool existsTable = false;
			IDataReader reader = null;

			try
			{
				//Loading data reader
				reader = this.GetDataReader("select name from sysobjects where name = '" + name + "' and xtype = 'u'");

				//Validating if the table searched exists	
				existsTable = (reader.Read());

				//Closing the DataReader and it underlying connection
				reader.Close();
			}
			catch
			{
				throw;
			}
			finally
			{
				//Closing the reader if apply
				if (reader != null && !reader.IsClosed) reader.Close();
			}

			//Setting the return value
			return existsTable;

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
			//Local vars
			bool existsIndex = false;
			IDataReader reader = null;

			try
			{
				//Creating the reader and searching for the index
				reader = this.GetDataReader("SELECT * FROM sysindexes WHERE name = '" + Name + "'");
				existsIndex = (reader.Read());
			}
			catch
			{
				throw;
			}
			finally
			{
				//Closing the reader if apply
				if (reader != null && !reader.IsClosed) reader.Close();
			}

			//Setting the return value
			return existsIndex;
		}

		private new static Dictionary<DbType, SqlDbType> DbTypeMap;

		static DataBase()
		{
			DbTypeMap = new Dictionary<DbType, SqlDbType>();

			DbTypeMap.Add(DbType.AnsiString, SqlDbType.Text);
			DbTypeMap.Add(DbType.AnsiStringFixedLength, SqlDbType.Char);
			DbTypeMap.Add(DbType.Binary, SqlDbType.Binary);
			DbTypeMap.Add(DbType.Boolean, SqlDbType.Bit);
			DbTypeMap.Add(DbType.Byte, SqlDbType.TinyInt);
			DbTypeMap.Add(DbType.Currency, SqlDbType.Decimal);
			DbTypeMap.Add(DbType.Date, SqlDbType.Date);
			DbTypeMap.Add(DbType.DateTime, SqlDbType.DateTime);
			DbTypeMap.Add(DbType.DateTime2, SqlDbType.DateTime2);
			DbTypeMap.Add(DbType.DateTimeOffset, SqlDbType.DateTimeOffset);
			DbTypeMap.Add(DbType.Decimal, SqlDbType.Decimal);
			DbTypeMap.Add(DbType.Double, SqlDbType.Real);
			DbTypeMap.Add(DbType.Int16, SqlDbType.SmallInt);
			DbTypeMap.Add(DbType.Int32, SqlDbType.Int);
			DbTypeMap.Add(DbType.Int64, SqlDbType.BigInt);
			DbTypeMap.Add(DbType.Guid, SqlDbType.UniqueIdentifier);
			DbTypeMap.Add(DbType.Object, SqlDbType.Variant);
			DbTypeMap.Add(DbType.SByte, SqlDbType.TinyInt);
			DbTypeMap.Add(DbType.Single, SqlDbType.Float);
			DbTypeMap.Add(DbType.String, SqlDbType.NText);
			DbTypeMap.Add(DbType.StringFixedLength, SqlDbType.NChar);
			DbTypeMap.Add(DbType.Time, SqlDbType.Time);
			DbTypeMap.Add(DbType.UInt16, SqlDbType.SmallInt);
			DbTypeMap.Add(DbType.UInt32, SqlDbType.Int);
			DbTypeMap.Add(DbType.UInt64, SqlDbType.BigInt);
			DbTypeMap.Add(DbType.VarNumeric, SqlDbType.BigInt);
			DbTypeMap.Add(DbType.Xml, SqlDbType.Xml);

			//DbTypeMap.Add(DbType.AnsiString, SqlDbType.VarChar);
			//DbTypeMap.Add(DbType.Binary, SqlDbType.Image);
			//DbTypeMap.Add(DbType.Binary, SqlDbType.Timestamp);
			//DbTypeMap.Add(DbType.Binary, SqlDbType.VarBinary);
			//DbTypeMap.Add(DbType.DateTime, SqlDbType.SmallDateTime);
			//DbTypeMap.Add(DbType.Decimal, SqlDbType.SmallMoney);
			//DbTypeMap.Add(DbType.Decimal, SqlDbType.Money);
			//DbTypeMap.Add(DbType.String, SqlDbType.NVarChar);
			//DbTypeMap.Add(DbType.Object, SqlDbType.Xml);
			//DbTypeMap.Add(DbType.Object, SqlDbType.Udt);
			//DbTypeMap.Add(DbType.Object, SqlDbType.Structured);
		}

		public static new SqlDbType Parse(DbType dbType)
		{
			return DbTypeMap[dbType];
		}

		public static DbType Parse(SqlDbType dbType)
		{
			return DbTypeMap.Reverse(dbType);
		}

		protected override string SchemaProvider
		{
			get 
			{ 
				return "System.Data.SqlClient"; 
			}
		}
	}
}