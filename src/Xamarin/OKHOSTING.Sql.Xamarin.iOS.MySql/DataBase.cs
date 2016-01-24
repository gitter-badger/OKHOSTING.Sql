using System;
using System.Data;
using System.Collections.Generic;
using MySqlPCL = global::MySql.Data.MySqlClient;

namespace OKHOSTING.Sql.Xamarin.iOS.MySql
{
	public class DataBase: Sql.DataBase
	{
		public DataBase ()
		{
			Connection = new MySqlPCL.MySqlConnection();
		}

		#region Fields and Properties

		/// <summary>
		/// Object used for lock operations
		/// </summary>
		private readonly object Locker = new object();

		/// <summary>
		/// Connection in which all the operations are performed
		/// </summary>
		protected readonly MySqlPCL.MySqlConnection Connection;

		/// <summary>
		/// Transaction in which all the operations are performed, after BeginTrans() is called
		/// </summary>
		protected MySqlPCL.MySqlTransaction Transaction;

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
			int rowsAffected = 0;
			Command current = new Command();

			//Validating that exists at least one script
			if (commands == null) throw new ArgumentNullException("commands");

			lock (Locker)
			{
				CommandEventArgs e = null;

				try
				{
					//Initializing command and connection
					MySqlPCL.MySqlCommand dbCommand = new MySqlPCL.MySqlCommand();
					dbCommand.Connection = Connection;
					dbCommand.Transaction = Transaction;
					OpenConnection();

					//Crossing the array of scripts
					foreach (Command command in commands)
					{
						//raising events
						e = new CommandEventArgs(command);
						OnBeforeExecute(e);
						current = command;

						//Executing the code only if have authorization 
						if (!e.Cancel)
						{
							//Setting the command text
							dbCommand.CommandText = command.Script;

							foreach (CommandParameter param in command.Parameters)
							{
								dbCommand.Parameters.Add(Parse(param));
							}

							//Executing command and getting the number of affected rows
							rowsAffected = dbCommand.ExecuteNonQuery();
						}

						//clear parameters
						dbCommand.Parameters.Clear();

						//calling events
						e.Result = rowsAffected;
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

				//Returning value
				if (e != null)
				{
					yield return (int) e.Result;
				}
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
			MySqlPCL.MySqlDataReader reader = null;
			CommandEventArgs e = new CommandEventArgs(command);

			lock (Locker)
			{
				try
				{
					//raising events
					OnBeforeGetDataReader(e);

					//running the script only if e.Cancel is false
					if (!e.Cancel)
					{
						//Initializing command and connection
						MySqlPCL.MySqlCommand dbCommand = new MySqlPCL.MySqlCommand();
						dbCommand.Connection = Connection;
						dbCommand.CommandText = command.Script;
						dbCommand.Transaction = Transaction;

						foreach (CommandParameter param in command.Parameters)
						{
							dbCommand.Parameters.Add(Parse(param));
						}

						OpenConnection();

						//Validating if there is a current transaction
						if (Transaction == null)
						{
							//Loading the data reader indicating that the close of the reader
							//must close the connection too
							reader = dbCommand.ExecuteReader(CommandBehavior.CloseConnection);
						}
						else
						{
							//Loading the data reader
							reader = dbCommand.ExecuteReader();
						}
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
			return new DataReader(this, (MySqlPCL.MySqlDataReader) e.Result);
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
			//Local Vars
			object value = null;
			CommandEventArgs e = new CommandEventArgs(command);

			lock (Locker)
			{
				try
				{
					//raising events
					OnBeforeGetDataReader(e);

					//running the script only if e.Cancel is false
					if (!e.Cancel)
					{
						//Initializing command and connection
						MySqlPCL.MySqlCommand dbCommand = new MySqlPCL.MySqlCommand();

						dbCommand.Connection = Connection;
						dbCommand.CommandText = command.Script;
						dbCommand.Transaction = Transaction;

						foreach (CommandParameter param in command.Parameters)
						{
							dbCommand.Parameters.Add(Parse(param));
						}

						OpenConnection();

						//Loading the data reader 
						value = dbCommand.ExecuteScalar();
					}

					//raising events
					e.Result = value;
					OnAfterGetDataReader(e);
				}
				catch (System.Exception ex)
				{
					//Closing the reader if apply
					CloseConnection();

					//Re - throw the excepción to the caller
					throw new SqlException(command, "Error on creating DataReader (" + ex.Message + ")", ex);
				}
				finally
				{
					//Closing the connection
					CloseConnection();
				}
			}

			//Returning the DataReader
			return e.Result;
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
			//Local Vars
			System.Data.DataTable tableToReturn = null;
			MySqlPCL.MySqlDataAdapter adapter;

			lock (Locker)
			{
				try
				{
					//raising events
					CommandEventArgs e = new CommandEventArgs(command);
					OnBeforeGetDataTable(e);

					//running the script only if e.Cancel is false
					if (!e.Cancel)
					{
						//Initializing command and connection
						MySqlPCL.MySqlCommand dbCommand = new MySqlPCL.MySqlCommand();
						dbCommand.Connection = Connection;
						dbCommand.CommandText = command.Script;
						dbCommand.Transaction = Transaction;

						foreach (CommandParameter param in command.Parameters)
						{
							dbCommand.Parameters.Add(Parse(param));
						}

						OpenConnection();

						//Configuring the adapter
						adapter = new MySqlPCL.MySqlDataAdapter();
						adapter.SelectCommand = dbCommand;

						//Loading the data
						tableToReturn = new System.Data.DataTable();
						adapter.Fill(tableToReturn);
					}

					//raising events
					e.Result = tableToReturn;
					OnAfterGetDataTable(e);

					//Returning the data to the caller
					return new DataTable(this, tableToReturn);
				}
				catch (System.Exception ex)
				{
					//Re - throwing the exception to the caller
					throw new SqlException(command, "Error on creating DataTable (" + ex.Message + ")", ex);
				}
				finally
				{
					//Closing the connection
					CloseConnection();
				}
			}
		}

		#endregion

		#region Transactional Management

		/// <summary>
		/// Starts a transaction for all the querys executed throught the Executer
		/// </summary>
		public override void BeginTransaction()
		{
			//Validating if dont exists currently a transaction 
			if (this.Transaction != null)
				throw new InvalidOperationException("Can't start a transaction because a transaction already exists in this executer");

			//Getting the default connection
			try
			{
				//Starting the transaction 
				OpenConnection();
				Transaction = Connection.BeginTransaction();
			}
			catch
			{
				//Cleaning the memory
				Transaction = null;

				//Throwing the exception
				throw;
			}
		}

		/// <summary> 
		/// Accepts the current transaction 
		/// </summary>
		public override void CommitTransaction()
		{
			//Validating if exists currently a transaction 
			if (this.Transaction == null)
				throw new InvalidOperationException("Can't start a transaction because a transaction already exists in this executer");

			//Acceptig the transaction
			this.Transaction.Commit();

			//Closing the connection
			this.Connection.Close();

			//Cleaning the memory
			this.Transaction = null;
		}

		/// <summary> 
		/// Cancels the current transaction
		/// </summary>		
		public override void RollBackTransaction()
		{
			//Validating if exists currently a transaction 
			if (this.Transaction == null)
				throw new InvalidOperationException("A transaction has not being initialized and therefore can't be rolled back");

			//Canceling the transaction
			this.Transaction.Rollback();

			//Closing the connection
			this.Connection.Close();

			//Cleaning the memory
			this.Transaction = null;
		}

		/// <summary>
		/// Gets a value indicating if a transaction is currently active
		/// </summary>
		public override bool IsTransactionActive
		{
			get
			{
				return Transaction != null;
			}
		}

		#endregion

		#region Utilities

		/// <summary>
		/// Opens the current connection, only if it is not already openned
		/// </summary>
		protected override void OpenConnection()
		{
			if (Connection.State != ConnectionState.Open)
			{
				Connection.Open();
			}
		}

		/// <summary>
		/// Opens the current connection, only if it is not already openned
		/// </summary>
		protected override void CloseConnection()
		{
			if (Connection.State != ConnectionState.Closed && !IsTransactionActive) Connection.Close();
		}

		/// <summary>
		/// Returns a native Command corresponding to the database engine associated
		/// </summary>
		protected virtual MySqlPCL.MySqlParameter Parse(CommandParameter param)
		{
			MySqlPCL.MySqlParameter dbParam = new MySqlPCL.MySqlParameter();

			dbParam.Value = param.Value;
			dbParam.DbType = (System.Data.DbType) param.DbType;
			dbParam.Direction = (System.Data.ParameterDirection) param.Direction;
			dbParam.Size = param.Size;
			dbParam.ParameterName = param.Name;

			return dbParam;
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
	}
}