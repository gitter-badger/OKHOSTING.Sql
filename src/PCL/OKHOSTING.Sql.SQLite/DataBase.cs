using System;
using System.Collections.Generic;
using SQLitePCL.pretty;
using OKHOSTING.Core;

namespace OKHOSTING.Sql.SQLite
{
	public class DataBase: Sql.DataBase
	{
		public DataBase ()
		{
		}

		#region Fields and Properties

		/// <summary>
		/// Object used for lock operations
		/// </summary>
		private readonly object Locker = new object();

		/// <summary>
		/// Connection in which all the operations are performed
		/// </summary>
		protected readonly SQLiteDatabaseConnection Connection;

		#endregion

		#region Operations

		/// <summary>
		/// Executes a SQL Script that doesn't retun rows
		/// </summary>
		/// <param name="command">
		/// A list of SQL scripts to be executed
		/// </param>
		/// <returns>
		/// An int indicating the number of affected rows
		/// </returns>
		public override IEnumerable<int> Execute(IEnumerable<Command> commands)
		{
			//Local Vars
			Command current = new Command();

			//Validating that exists at least one script
			if (commands == null) throw new ArgumentNullException("commands");

			lock (Locker)
			{
				try
				{
					//Initializing command and connection
					OpenConnection();

					//Crossing the array of scripts
					foreach (Command command in commands)
					{
						//raising events
						CommandEventArgs e = new CommandEventArgs(command);
						OnBeforeExecute(e);
						current = command;

						//Executing the code only if have authorization 
						if (!e.Cancel)
						{
							object[] parameters = new object[command.Parameters.Count];

							for (int i = 0; i < command.Parameters.Count; i++)
							{
								parameters[i] = command.Parameters[i].Value;
							}

							//Executing command and getting the number of affected rows
							Connection.Execute(command.Script, parameters);

						}

						//calling events
						OnAfterExecute(e);
					}
				}
				catch (System.Exception ex)
				{
					//Re throwing exception to the caller
					throw new SqlException(current, "Error on executing command against the database (" + ex.Message + ")", ex);
				}
				finally
				{
					//Closing the connection
					CloseConnection();
				}

				yield return -1;
			}
		}

		/// <summary>
		/// Executes a SQL Script and retrieves all data obteined
		/// </summary>
		/// <remarks>
		/// You must explicity close the returned DataReader once it's not used, 
		/// otherwise the assigned connection would stay opened and cause
		/// a bad use of resources
		/// </remarks>
		/// <param name="command">
		/// Sql Sentence to exectute for retrieve data
		/// </param>
		/// <returns>
		/// A System.Data.DbDataReader with all data obtained
		/// </returns>
		public override IDataReader GetDataReader(Command command)
		{
			//Local Vars
			IEnumerable<IReadOnlyList<IResultSetValue>> reader = null;

			lock (Locker)
			{
				try
				{
					//raising events
					CommandEventArgs e = new CommandEventArgs(command);
					OnBeforeGetDataReader(e);

					//running the script only if e.Cancel is false
					if (!e.Cancel)
					{
						object[] parameters = new object[command.Parameters.Count];

						for (int i = 0; i < command.Parameters.Count; i++)
						{
							parameters[i] = command.Parameters[i].Value;
						}

						OpenConnection();

						//Validating if there is a current transaction
						//Loading the data reader
						reader = Connection.Query(command.Script, parameters);
					}

					//raising events
					e.Result = reader;
					OnAfterGetDataReader(e);
				}
				catch (System.Exception ex)
				{
					//Closing the reader if apply
					CloseConnection();

					//Re - throw the excepción to the caller
					throw new SqlException(command, "Error on creating DataReader (" + ex.Message + ")", ex);
				}
			}

			//Returning the DataReader
			return new DataReader(this, reader);
		}

		/// <summary>
		/// Executes a scalar function SQL Script and retrieves a single value
		/// </summary>
		/// <remarks>
		/// Usefull to execute aggregated functions that returns only one row and one value
		/// </remarks>
		/// <param name="command">
		/// Sql Sentence to exectute for retrieve data
		/// </param>
		/// <returns>
		/// Value returned in the first row and firstn collumn
		/// </returns>
		public override object GetScalar(Command command)
		{
			using (var reader = GetDataReader(command))
			{
				return reader[0];
			}
		}

		/// <summary>
		/// Executes a SQL Script and retrieves all data obteined
		/// </summary>
		/// <param name="command">
		/// Sql Sentence to execute for retrieve data
		/// </param>
		/// <returns>
		/// A System.Data.DataTable with all data obtained
		/// </returns>
		public override IDataTable GetDataTable(Command command)
		{
			return new DataTable(this, (DataReader) GetDataReader(command));
		}

		#endregion

		#region Transactional Management

		/// <summary>
		/// Starts a transaction for all the querys executed throught the Executer
		/// </summary>
		public override void BeginTransaction()
		{
			throw new NotSupportedException();
		}

		/// <summary> 
		/// Accepts the current transaction 
		/// </summary>
		public override void CommitTransaction()
		{
			throw new NotSupportedException();
		}

		/// <summary> 
		/// Cancels the current transaction
		/// </summary>		
		public override void RollBackTransaction()
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// Gets a value indicating if a transaction is currently active
		/// </summary>
		public override bool IsTransactionActive
		{
			get
			{
				return false;
			}
		}

		#endregion

		#region Utilities

		/// <summary>
		/// Opens the current connection, only if it is not already openned
		/// </summary>
		protected override void OpenConnection()
		{
		}

		/// <summary>
		/// Opens the current connection, only if it is not already openned
		/// </summary>
		protected override void CloseConnection()
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

		#endregion

		#region Static

		private static Dictionary<DbType, SQLiteType> DbTypeMap;

		static DataBase()
		{
			DbTypeMap = new Dictionary<DbType, SQLiteType>();

			DbTypeMap.Add(DbType.Binary, SQLiteType.Blob);
			DbTypeMap.Add(DbType.Object, SQLiteType.Blob);

			DbTypeMap.Add(DbType.Double, SQLiteType.Float);
			DbTypeMap.Add(DbType.Single, SQLiteType.Float);
			DbTypeMap.Add(DbType.Decimal, SQLiteType.Float);
			DbTypeMap.Add(DbType.VarNumeric, SQLiteType.Float);
			DbTypeMap.Add(DbType.Currency, SQLiteType.Float);

			DbTypeMap.Add(DbType.Int16, SQLiteType.Integer);
			DbTypeMap.Add(DbType.Int32, SQLiteType.Integer);
			DbTypeMap.Add(DbType.Int64, SQLiteType.Integer);
			DbTypeMap.Add(DbType.UInt16, SQLiteType.Integer);
			DbTypeMap.Add(DbType.UInt32, SQLiteType.Integer);
			DbTypeMap.Add(DbType.UInt64, SQLiteType.Integer);
			DbTypeMap.Add(DbType.Boolean, SQLiteType.Integer);
			DbTypeMap.Add(DbType.Byte, SQLiteType.Integer);
			DbTypeMap.Add(DbType.SByte, SQLiteType.Integer);

			DbTypeMap.Add(DbType.Date, SQLiteType.Integer);
			DbTypeMap.Add(DbType.DateTime, SQLiteType.Integer);
			DbTypeMap.Add(DbType.DateTimeOffset, SQLiteType.Integer);
			DbTypeMap.Add(DbType.Time, SQLiteType.Integer);

			DbTypeMap.Add(DbType.StringFixedLength, SQLiteType.Null);

			DbTypeMap.Add(DbType.String, SQLiteType.Text);
			DbTypeMap.Add(DbType.AnsiString, SQLiteType.Text);
			DbTypeMap.Add(DbType.AnsiStringFixedLength, SQLiteType.Text);
			DbTypeMap.Add(DbType.Guid, SQLiteType.Text);
			DbTypeMap.Add(DbType.Xml, SQLiteType.Text);
		}

		public static SQLiteType Parse(DbType dbType)
		{
			return DbTypeMap[dbType];
		}

		public static DbType Parse(SQLiteType dbType)
		{
			return DbTypeMap.Reverse(dbType);
		}

		#endregion
	}
}