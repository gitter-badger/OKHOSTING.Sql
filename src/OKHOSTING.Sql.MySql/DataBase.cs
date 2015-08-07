using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Data.Common;
using System.Data;
using System.Text;
using OKHOSTING.Core.Extensions;

namespace OKHOSTING.Sql.MySql
{
	/// <summary>
	/// Implements methods for executing Sql scripts in a MySql Server DataBase
	/// </summary>
	public class DataBase : OKHOSTING.Sql.DataBase
	{
		/// <summary>
		/// Initializes a new instance of the executer
		/// </summary>
		/// <param name="connectionString"> 
		/// The connection string to use to connect to the DataBase
		/// </param>
		public DataBase(): base(MySqlClientFactory.Instance)
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
			DbDataReader reader = null;

			try
			{
				//Loading DataReader and getting the date and time
				reader = this.GetDataReader("select SYSDATE() AS CurrentDate");
				reader.Read();
				currentDate = reader.GetDateTime(0);
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
			DataTable tblData = this.GetDataTable("select UUID()");
			//Reading the ID
			uniqueIdentifier = tblData.Rows[0][0].ToString();

			//Returning the GUID
			return uniqueIdentifier;
		}

		/// <summary>
		/// Returns a value that indicates if the Constrainst 
		/// exists in the Underlying Database 
		/// </summary>
		/// <param name="name">
		/// Name of the constraint to validate
		/// </param>
		/// <returns>
		/// Value that indicates if the Constrainst 
		/// exists in the Underlying Database 
		/// </returns>
		public override bool ExistsConstraint(string name)
		{
			//Local vars
			bool existsIndex = false;
			string Table, Constraint;
			DbDataReader reader = null;

			//Validating if the name is correctly specified
			if (name.IndexOf(".") == -1)
			{
				throw new ArgumentException(
					"For MySql constraints, the constraint name must be completly qualified with the syntax Table.Constraint",
					"Name");
			}

			//Desglosando el nombre del índice en tabla e indice
			Table = name.Substring(0, name.IndexOf("."));
			Constraint = name.Substring(name.IndexOf(".") + 1);

			try
			{
				//Creating the reader and searching for the index
				reader = this.GetDataReader("show keys from `" + Table + "` where key_Name = '" + Constraint + "'");

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
			DbDataReader reader = null;

			try
			{
				//Loading data reader
				reader = this.GetDataReader("select * from `" + name + "` limit 1");

				//If the previous query executes it successfully, 
				//then the table exists
				existsTable = true;
			}
			catch (System.Exception)
			{
				//The query fails; the table dont exists
				existsTable = false;
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
			throw new NotSupportedException("MySqlExecuter is not compatible with the method BaseExecuter.IndexExists()");
		}

		private static Dictionary<DbType, MySqlDbType> DbTypeMap;

		static DataBase()
		{
			DbTypeMap = new Dictionary<DbType, MySqlDbType>();

			DbTypeMap.Add(DbType.AnsiString, MySqlDbType.Text);
			DbTypeMap.Add(DbType.AnsiStringFixedLength, MySqlDbType.String);
			DbTypeMap.Add(DbType.Binary, MySqlDbType.Binary);
			DbTypeMap.Add(DbType.Boolean, MySqlDbType.Bit);
			DbTypeMap.Add(DbType.Byte, MySqlDbType.Byte);
			DbTypeMap.Add(DbType.Currency, MySqlDbType.Decimal);
			DbTypeMap.Add(DbType.Date, MySqlDbType.Date);
			DbTypeMap.Add(DbType.DateTime, MySqlDbType.DateTime);
			DbTypeMap.Add(DbType.DateTime2, MySqlDbType.DateTime);
			DbTypeMap.Add(DbType.DateTimeOffset, MySqlDbType.DateTime);
			DbTypeMap.Add(DbType.Decimal, MySqlDbType.Decimal);
			DbTypeMap.Add(DbType.Double, MySqlDbType.Double);
			DbTypeMap.Add(DbType.Int16, MySqlDbType.Int16);
			DbTypeMap.Add(DbType.Int32, MySqlDbType.Int32);
			DbTypeMap.Add(DbType.Int64, MySqlDbType.Int16);
			DbTypeMap.Add(DbType.Guid, MySqlDbType.Guid);
			DbTypeMap.Add(DbType.Object, MySqlDbType.Binary);
			DbTypeMap.Add(DbType.SByte, MySqlDbType.Byte);
			DbTypeMap.Add(DbType.Single, MySqlDbType.Float);
			DbTypeMap.Add(DbType.String, MySqlDbType.Text);
			DbTypeMap.Add(DbType.StringFixedLength, MySqlDbType.String);
			DbTypeMap.Add(DbType.Time, MySqlDbType.Time);
			DbTypeMap.Add(DbType.UInt16, MySqlDbType.UInt16);
			DbTypeMap.Add(DbType.UInt32, MySqlDbType.UInt32);
			DbTypeMap.Add(DbType.UInt64, MySqlDbType.UInt16);
			DbTypeMap.Add(DbType.VarNumeric, MySqlDbType.Int64);
			DbTypeMap.Add(DbType.Xml, MySqlDbType.Text);

			//DbTypeMap.Add(DbType.AnsiString, MySqlDbType.VarChar);
			//DbTypeMap.Add(DbType.Binary, MySqlDbType.Timestamp);
			//DbTypeMap.Add(DbType.Binary, MySqlDbType.VarBinary);
			//DbTypeMap.Add(DbType.String, MySqlDbType.VarString);
		}

		public static new MySqlDbType Parse(DbType dbType)
		{
			return DbTypeMap[dbType];
		}

		public static DbType Parse(MySqlDbType dbType)
		{
			return DbTypeMap.Reverse(dbType);
		}

		protected override string SchemaProvider
		{
			get 
			{
				return "MySql.Data.MySqlClient"; 
			}
		}
	}
}