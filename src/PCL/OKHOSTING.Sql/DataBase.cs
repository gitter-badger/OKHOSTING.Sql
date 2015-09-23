﻿using System;
using System.Collections.Generic;

namespace OKHOSTING.Sql.Net4
{
	/// <summary>
	/// Implements methods for interact with database engines
	/// </summary>
	public abstract class DataBase
	{
		#region Fields and Properties

		/// <summary>
		/// Object used for lock operations
		/// </summary>
		private readonly object Locker = new object();

		public int Id { get; set; }

		public string Name { get; set; }

		public Schema.DataBaseSchema Schema { get; set; }

		string _ConnectionString;

		public string ConnectionString
		{
			get
			{
				return _ConnectionString;
			}
			set
			{
				_ConnectionString = value;
			}
		}

		#endregion

		#region Operations

		/// <summary>
		/// Executes a SQL Script that doesn't retun rows
		/// </summary>
		/// <param name="command">
		/// The SQL script to be executed
		/// </param>
		/// <returns>
		/// An int indicating the number of affected rows
		/// </returns>
		public int Execute(Command command)
		{
			return Execute(new List<Command>() { command });
		}

		/// <summary>
		/// Executes a SQL Script that doesn't retun rows
		/// </summary>
		/// <param name="command">
		/// A list of SQL scripts to be executed
		/// </param>
		/// <returns>
		/// An int indicating the number of affected rows
		/// </returns>
		public int Execute(List<Command> commands)
		{
			//Local Vars
			int rowsAffected = 0;
			Command current = new Command();

			//Validating that exists at least one script
			if (commands == null) throw new ArgumentNullException("commands");

			lock (Locker)
			{
				try
				{
					//Initializing command and connection
					DbCommand dbCommand = ProviderFactory.CreateCommand();
					dbCommand.Connection = Connection;
					dbCommand.Transaction = Transaction;
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
			}
			//Returning value
			return rowsAffected;
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
		public DbDataReader GetDataReader(Command command)
		{
			//Local Vars
			DbDataReader reader = null;

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
						//Initializing command and connection
						DbCommand dbCommand = ProviderFactory.CreateCommand();
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
			return reader;
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
		public object GetScalar(Command command)
		{
			//Local Vars
			object value = null;

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
						//Initializing command and connection
						DbCommand dbCommand = ProviderFactory.CreateCommand();

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
			return value;
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
		public DataTable GetDataTable(Command command)
		{
			//Local Vars
			DataTable tableToReturn = null;
			DbDataAdapter adapter;

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
						DbCommand dbCommand = ProviderFactory.CreateCommand();
						dbCommand.Connection = Connection;
						dbCommand.CommandText = command.Script;
						dbCommand.Transaction = Transaction;

						foreach (CommandParameter param in command.Parameters)
						{
							dbCommand.Parameters.Add(Parse(param));
						}

						OpenConnection();

						//Configuring the adapter
						adapter = ProviderFactory.CreateDataAdapter();
						adapter.SelectCommand = dbCommand;

						//Loading the data
						tableToReturn = new DataTable();
						adapter.Fill(tableToReturn);
					}

					//raising events
					e.Result = tableToReturn;
					OnAfterGetDataTable(e);

					//Returning the data to the caller
					return tableToReturn;
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
		public void BeginTransaction()
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
		public void CommitTransaction()
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
		public void RollBackTransaction()
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
		public bool IsTransactionActive
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
		protected void OpenConnection()
		{
			if (Connection.State != ConnectionState.Open)
			{
				Connection.Open();
			}
		}

		/// <summary>
		/// Opens the current connection, only if it is not already openned
		/// </summary>
		protected void CloseConnection()
		{
			if (Connection.State != ConnectionState.Closed && !IsTransactionActive) Connection.Close();
		}

		/// <summary>
		/// Returns a native Command corresponding to the database engine associated
		/// </summary>
		protected virtual DbParameter Parse(CommandParameter param)
		{
			DbParameter dbParam = ProviderFactory.CreateParameter();

			dbParam.Value = param.Value;
			dbParam.DbType = Parse(param.DbType);
			dbParam.Direction = Parse(param.Direction);
			dbParam.Size = param.Size;
			dbParam.ParameterName = param.Name;

			return dbParam;
		}

		protected virtual System.Data.ParameterDirection Parse(ParameterDirection direction)
		{
			return (System.Data.ParameterDirection) Enum.Parse(typeof(System.Data.ParameterDirection), direction.ToString());
		}

		protected virtual System.Data.DbType Parse(DbType dbType)
		{
			return (System.Data.DbType) Enum.Parse(typeof(System.Data.DbType), dbType.ToString());
		}

		/// <summary>
		/// Returns a boolean value that indicates if the 
		/// SQL Sentence return data
		/// </summary>
		/// <param name="command">
		/// Sentence to verify
		/// </param>
		/// <returns>
		/// Boolean value that indicates if the 
		/// SQL Sentence return data
		/// </returns>
		public bool ExistsData(Command command)
		{
			//Local Vars
			bool result = false;

			//Creating DataReader
			DbDataReader reader = null;

			//Trying to read the data
			try
			{
				//Query the Database
				reader = GetDataReader(command);

				//Validating if exists data
				result = (reader.Read());
			}
			catch
			{
				throw;
			}
			finally
			{
				if (reader != null && !reader.IsClosed) reader.Close();
			}

			//Returning value
			return result;
		}

		/// <summary>
		/// Returns a value that indicates how many records returns
		/// the query specified
		/// </summary>
		/// <param name="SQLSentence">
		/// Sql Sentence for query
		/// </param>
		/// <returns>
		/// Number of Records that returns the query
		/// </returns>
		public int NumberOfRecords(string sqlSentence)
		{
			//Local Vars
			int result = 0;

			//Geting data requested and number of rows affected 
			DataTable tblData = this.GetDataTable(sqlSentence);
			result = tblData.Rows.Count;

			//Returning value
			return result;
		}

		/// <summary>
		/// Returns a boolean value that indicates if is possible to connect
		/// to database defined at the connection string of the executor
		/// </summary>
		/// <returns>
		/// Boolean value that indicates the successfully or failed connection
		/// </returns>
		public bool CanConnect()
		{
			//Calling to the corresponding overload
			Exception ExceptionOccured;
			return CanConnect(out ExceptionOccured);
		}

		/// <summary>
		/// Returns a boolean value that indicates if is possible to connect
		/// to database defined at the connection string of the executor
		/// </summary>
		/// <param name="ExceptionOccured">
		/// Out parameter that expose the error on the connection when this exists, 
		/// or null if dont occurs
		/// </param>
		/// <returns>
		/// Boolean value that indicates the successfully or failed connection
		/// </returns>
		public bool CanConnect(out Exception exceptionOccured)
		{
			//Local vars
			bool success;

			//Initializing the out parameters
			exceptionOccured = null;

			try
			{
				//Trying to connect to database
				OpenConnection();

				//Establishing that the connection was successfully
				success = true;
			}
			catch (System.Exception ex)
			{
				//Establishing exception to return 
				exceptionOccured = ex;

				//Establishing that the connection was failed
				success = false;
			}

			finally
			{
				//Closing the conection (if apply)
				CloseConnection();
			}

			//Returning the value
			return success;
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
		public virtual bool ExistsTable(string name)
		{
			//Local vars
			bool existsTable = false;
			DbDataReader reader = null;

			try
			{
				//Loading data reader
				reader = GetDataReader("select * from " + name);

				//Validating if was possible to load the reader
				if (reader != null)
				{
					//If the previous query executes it successfully, 
					//then the table exists
					existsTable = true;

					//Closing the DataReader and it underlying connection
					reader.Close();
				}
				else
				{
					existsTable = false;
				}
			}
			catch
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

		#endregion

		#region Abstract functionality (for implementation on child classes)

		protected abstract string SchemaProvider
		{
			get;
		}

		/// <summary>
		/// Returns the Date and Hour from the database Server
		/// </summary>
		/// <returns>
		/// Date and Hour from the database Server
		/// </returns>
		public abstract DateTime DateTimeOnDBServer();

		/// <summary>
		/// Returns a Global Unique Identifier from the Database
		/// </summary>
		/// <returns>
		/// Global Unique Identifier 
		/// </returns>
		public abstract string GetUniqueIdentifier();

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
		public abstract bool ExistsConstraint(string Name);

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
		public abstract bool ExistsIndex(string Name);

		#endregion

		#region Events

		/// <summary>
		/// Delegate for the database interaction events
		/// </summary>
		public delegate void DataBaseOperationEventHandler(DataBase sender, CommandEventArgs DatabaseInteractionArgs);

		/// <summary>
		/// Event thrown before of execute sentences against the database
		/// </summary>
		public event DataBaseOperationEventHandler BeforeExecute;

		/// <summary>
		/// Event thrown after of execute sentences against the database
		/// </summary>
		public event DataBaseOperationEventHandler AfterExecute;

		/// <summary>
		/// Event thrown before get a data table
		/// </summary>
		public event DataBaseOperationEventHandler BeforeGetDataTable;

		/// <summary>
		/// Event thrown after get a data table
		/// </summary>
		public event DataBaseOperationEventHandler AfterGetDataTable;

		/// <summary>
		/// Event thrown before get a data reader
		/// </summary>
		public event DataBaseOperationEventHandler BeforeGetDataReader;

		/// <summary>
		/// Event thrown after get a data reader
		/// </summary>
		public event DataBaseOperationEventHandler AfterGetDataReader;

		/// <summary>
		/// Raises the BeforeExecute event
		/// </summary>
		public virtual void OnBeforeExecute(CommandEventArgs e)
		{
			if (BeforeExecute != null) BeforeExecute(this, e);
		}

		/// <summary>
		/// Raises the AfterExecute event
		/// </summary>
		public virtual void OnAfterExecute(CommandEventArgs e)
		{
			if (AfterExecute != null) AfterExecute(this, e);
		}

		/// <summary>
		/// Raises the BeforeGetDataTable event
		/// </summary>
		public virtual void OnBeforeGetDataTable(CommandEventArgs e)
		{
			if (BeforeGetDataTable != null) BeforeGetDataTable(this, e);
		}

		/// <summary>
		/// Raises the AfterGetDataTable event
		/// </summary>
		public virtual void OnAfterGetDataTable(CommandEventArgs e)
		{
			if (AfterGetDataTable != null) AfterGetDataTable(this, e);
		}

		/// <summary>
		/// Raises the BeforeGetDataReader event
		/// </summary>
		public virtual void OnBeforeGetDataReader(CommandEventArgs e)
		{
			if (BeforeGetDataReader != null) BeforeGetDataReader(this, e);
		}

		/// <summary>
		/// Raises the AfterGetDataReader event
		/// </summary>
		public virtual void OnAfterGetDataReader(CommandEventArgs e)
		{
			if (AfterGetDataReader != null) AfterGetDataReader(this, e);
		}

		#endregion

		#region Static

		#endregion
	}
}