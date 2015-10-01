using System;
using System.Data;
using System.Data.Common;
using System.Collections.Generic;

namespace OKHOSTING.Sql.Net4
{
	/// <summary>
	/// Implements methods for interact with database engines
	/// </summary>
	public abstract class DataBase: Sql.DataBase
	{
		#region Constructors and destructors

		/// <summary>
		/// Initializes a new instance of the OKHOSTING.Data.SqlBases.BaseExecuter class
		/// </summary>
		/// <param name="ConnectionString"> 
		/// The connection string to use to connect to the DataBase
		/// </param>
		protected DataBase(DbProviderFactory providerFactory)
		{
			if (providerFactory == null)
			{
				throw new ArgumentNullException("providerFactory");
			}

			//Assigning the Connection String
			ProviderFactory = providerFactory;
			Connection = providerFactory.CreateConnection();
		}

		/// <summary>
		/// Destroy the Executer
		/// </summary>
		~DataBase()
		{
			//Canceling current transaction 
			if (Transaction != null) Transaction.Rollback();

			//Cleaning the memory
			Transaction = null;
		}

		#endregion

		#region Fields and Properties

		/// <summary>
		/// Object used for lock operations
		/// </summary>
		private readonly object Locker = new object();

		/// <summary>
		/// Connection in which all the operations are performed
		/// </summary>
		protected readonly DbConnection Connection;

		/// <summary>
		/// Transaction in which all the operations are performed, after BeginTrans() is called
		/// </summary>
		protected DbTransaction Transaction;

		/// <summary>
		/// Used to create connections, commands and parameters for a specific database engine
		/// </summary>
		protected DbProviderFactory ProviderFactory;

		protected OKHOSTING.Sql.Schema.DataBaseSchema _Schema;

		public override string ConnectionString
		{
			get
			{
				return base.ConnectionString;
			}
			set
			{
				base.ConnectionString = value;

				if (Connection != null)
				{
					Connection.ConnectionString = value;
				}
			}
		}

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
					DbCommand dbCommand = ProviderFactory.CreateCommand();
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
			DbDataReader reader = null;
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
			return new DataReader(this, (DbDataReader) e.Result);
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
						tableToReturn = new System.Data.DataTable();
						adapter.Fill(tableToReturn);
					}

					//raising events
					e.Result = tableToReturn;
					OnAfterGetDataTable(e);

					//Returning the data to the caller
					return new DataTable(this, (System.Data.DataTable) e.Result);
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

		public override void Dispose()
		{
			base.Dispose();

			if (Transaction != null)
			{
				Transaction.Dispose();
			}

			if (Connection != null)
			{
				Connection.Dispose();
			}
		}

		#endregion

		#region Abstract functionality (for implementation on child classes)

		protected abstract string SchemaProvider
		{
			get;
		}

		#endregion
	}
}