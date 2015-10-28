using System;
using System.Collections.Generic;
using OKHOSTING.Core;
using System.Data.Common;
using System.Data;

namespace OKHOSTING.Sql.Net4.SqlServer
{
	/// <summary>
	/// Implements methods for executing Sql scripts in a MySql Server DataBase
	/// </summary>
	public class DataBase : OKHOSTING.Sql.Net4.DataBase
	{
		/// <summary>
		/// Initializes a new instance of the executer
		/// </summary>
		/// <param name="connectionString"> 
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
				reader = this.GetDataReader("SELECT SYSDATE() AS CurrentDate");
				reader.Read();
				currentDate = reader.GetFieldValue<DateTime>(0);
			}
			catch
			{
				throw;
			}
			finally
			{
				//Closing the reader if apply
				if (reader != null && !reader.IsClosed)
				{
					reader.Close();
					reader.Dispose();
				}
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
			IDataTable tblData = this.GetDataTable("select UUID()");
			//Reading the ID
			uniqueIdentifier = tblData[0][0].ToString();

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
			bool exists = false;
			string table, constraint;
			IDataReader reader = null;

			//Validating if the name is correctly specified
			if (name.IndexOf(".") == -1)
			{
				throw new ArgumentException("For MySql constraints, the constraint name must be completly qualified with the syntax Table.Constraint", "Name");
			}

			//Desglosando el nombre del índice en tabla e indice
			table = name.Substring(0, name.IndexOf("."));
			constraint = name.Substring(name.IndexOf(".") + 1);

			try
			{
				//Creating the reader and searching for the index
				reader = this.GetDataReader(string.Format("SELECT `constraint_name` FROM `information_schema`.`key_column_usage` WHERE `referenced_table_name` IS NOT NULL AND `table_name`='{0}' AND `constraint_name` = '{1}'", table, constraint));
				exists = reader.Read();
			}
			catch
			{
				exists = false;
			}
			//Closing the reader if apply
			if (reader != null && !reader.IsClosed)
			{
				reader.Close();
				reader.Dispose();
			}

			//Setting the return value
			return exists;
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
		public override bool ExistsIndex(string name)
		{
			//Local vars
			bool exists = false;
			string table, index;
			IDataReader reader = null;

			//Validating if the name is correctly specified
			if (name.IndexOf(".") == -1)
			{
				throw new ArgumentException("For MySql indexes, the index name must be completly qualified with the syntax Table.Index","Name");
			}

			//Desglosando el nombre del índice en tabla e indice
			table = name.Substring(0, name.IndexOf("."));
			index = name.Substring(name.IndexOf(".") + 1);

			try
			{
				//Creating the reader and searching for the index
				reader = this.GetDataReader(string.Format("SHOW KEYS FROM `{0}` WHERE key_Name = '{1}'", table, index));
				exists = reader.Read();
			}
			catch
			{
				exists = false;
				//throw;
			}
			//Closing the reader if apply
			if (reader != null && !reader.IsClosed)
			{
				reader.Close();
				reader.Dispose();
			}

			//Setting the return value
			return exists;
		}

		private static Dictionary<DbType, SqlDbType> DbTypeMap;

		static DataBase()
		{
			DbTypeMap = new Dictionary<DbType, SqlDbType>();

			DbTypeMap.Add(DbType.AnsiString, SqlDbType.Text);
			DbTypeMap.Add(DbType.AnsiStringFixedLength, SqlDbType.Text);
			DbTypeMap.Add(DbType.Binary, SqlDbType.Binary);
			DbTypeMap.Add(DbType.Boolean, SqlDbType.Bit);
			DbTypeMap.Add(DbType.Byte, SqlDbType.TinyInt);
			DbTypeMap.Add(DbType.Currency, SqlDbType.Money);
			DbTypeMap.Add(DbType.Date, SqlDbType.Date);
			DbTypeMap.Add(DbType.DateTime, SqlDbType.DateTime);
			DbTypeMap.Add(DbType.DateTime2, SqlDbType.DateTime);
			DbTypeMap.Add(DbType.DateTimeOffset, SqlDbType.DateTime);
			DbTypeMap.Add(DbType.Decimal, SqlDbType.Decimal);
			DbTypeMap.Add(DbType.Double, SqlDbType.Float);
			DbTypeMap.Add(DbType.Int16, SqlDbType.SmallInt);
			DbTypeMap.Add(DbType.Int32, SqlDbType.Int);
			DbTypeMap.Add(DbType.Int64, SqlDbType.BigInt);
			DbTypeMap.Add(DbType.Guid, SqlDbType.VarChar);
			DbTypeMap.Add(DbType.Object, SqlDbType.Binary);
			DbTypeMap.Add(DbType.SByte, SqlDbType.TinyInt);
			DbTypeMap.Add(DbType.Single, SqlDbType.Decimal);
			DbTypeMap.Add(DbType.String, SqlDbType.Text);
			DbTypeMap.Add(DbType.StringFixedLength, SqlDbType.Text);
			DbTypeMap.Add(DbType.Time, SqlDbType.Time);
			DbTypeMap.Add(DbType.UInt16, SqlDbType.Int);
            DbTypeMap.Add(DbType.UInt32, SqlDbType.BigInt);
            DbTypeMap.Add(DbType.UInt64, SqlDbType.BigInt);
			DbTypeMap.Add(DbType.VarNumeric, SqlDbType.Int);
			DbTypeMap.Add(DbType.Xml, SqlDbType.Text);

			//DbTypeMap.Add(DbType.AnsiString, SqlDbType.VarChar);
			//DbTypeMap.Add(DbType.Binary, SqlDbType.Timestamp);
			//DbTypeMap.Add(DbType.Binary, SqlDbType.VarBinary);
			//DbTypeMap.Add(DbType.String, SqlDbType.VarString);
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
				return "MySql.Data.MySqlClient"; 
			}
		}
	}
}