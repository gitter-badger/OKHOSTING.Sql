using System;
using System.Linq;
using System.Collections.Generic;
using OKHOSTING.Data.Validation;

namespace OKHOSTING.Sql
{
	/// <summary>
	/// Implements methods for interact with database engines
	/// </summary>
	public abstract class DataBase: IDisposable
	{
		#region Fields and Properties

		public virtual int Id { get; set; }

		[StringLengthValidator(50)]
		public virtual string Name { get; set; }

		public virtual Schema.DataBaseSchema Schema 
		{ 
			get; 
			protected set; 
		}

		[RequiredValidator]
		[StringLengthValidator(250)]
		public virtual string ConnectionString
		{
			get; set;
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
		public virtual int Execute(Command command)
		{
			return Execute(new List<Command>() { command }).First();
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
		public abstract IEnumerable<int> Execute(IEnumerable<Command> commands);

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
		/// A System.Data.IDataReader with all data obtained
		/// </returns>
		public abstract IDataReader GetDataReader(Command command);

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
		public abstract object GetScalar(Command command);

		/// <summary>
		/// Executes a SQL Script and retrieves all data obteined
		/// </summary>
		/// <param name="command">
		/// Sql Sentence to execute for retrieve data
		/// </param>
		/// <returns>
		/// A System.Data.IDataTable with all data obtained
		/// </returns>
		public abstract IDataTable GetDataTable(Command command);

		#endregion

		#region Transactional Management

		/// <summary>
		/// Starts a transaction for all the querys executed throught the Executer
		/// </summary>
		public abstract void BeginTransaction();

		/// <summary> 
		/// Accepts the current transaction 
		/// </summary>
		public abstract void CommitTransaction();

		/// <summary> 
		/// Cancels the current transaction
		/// </summary>		
		public abstract void RollBackTransaction();

		/// <summary>
		/// Gets a value indicating if a transaction is currently active
		/// </summary>
		public abstract bool IsTransactionActive
		{
			get;
		}

		#endregion

		#region Utilities

		/// <summary>
		/// Opens the current connection, only if it is not already openned
		/// </summary>
		protected abstract void OpenConnection();

		/// <summary>
		/// Opens the current connection, only if it is not already openned
		/// </summary>
		protected abstract void CloseConnection();

		/// <summary>
		/// Returns a boolean value that indicates if is possible to connect
		/// to database defined at the connection string of the executor
		/// </summary>
		/// <returns>
		/// Boolean value that indicates the successfully or failed connection
		/// </returns>
		public virtual bool CanConnect()
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
		public virtual bool CanConnect(out Exception exceptionOccured)
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
			IDataReader reader = null;

			try
			{
				//Loading data reader
				reader = GetDataReader("SELECT * FROM " + name);

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
				if (reader != null && !reader.IsClosed) 
				{
					reader.Close ();
					reader.Dispose ();
				}
			}

			//Setting the return value
			return existsTable;
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
		public virtual bool ExistsData(Command command)
		{
			//Local Vars
			bool result = false;

			//Creating DataReader
			IDataReader reader = null;

			//Trying to read the data
			try
			{
				//Query the Database
				reader = GetDataReader(command);

				//Validating if exists data
				result = reader.Read();
			}
			catch
			{
				throw;
			}
			finally
			{
				if (reader != null && !reader.IsClosed)
				{
					reader.Close();
					reader.Dispose();
				}
			}

			//Returning value
			return result;
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

		public virtual void Dispose()
		{
			CloseConnection();
		}

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

		#region Static events

		/// <summary>
		/// Delegate used for the database creation. 
		/// </summary>
		public delegate DataBase SetupDataBaseEventHandler();

		/// <summary>
		/// Subscribe to this event to create the actual database that will be used in your apps, system-wide. Should only have 1 subscriber. If it has more it will return the last subscriber's result
		/// </summary>
		public static event SetupDataBaseEventHandler Setup;

		/// <summary>
		/// Allows you (or plugins) to perform adittional configurations on newly created databases
		/// </summary>
		public delegate void SettingUpDataBaseEventHandler(DataBase dataBase);

		/// <summary>
		/// Subscribe to this event to create the actual database that will be used in your projects. Allow for "plugins" to subscribe to dabase events and affect system wide behaviour
		/// </summary>
		public static event SettingUpDataBaseEventHandler SettingUp;

		/// <summary>
		/// Will create a ready to use database. 
		/// You should subscribeto Create and (optionally) Created events to return a fully configured database. Then just call this method from everywhere else.
		/// </summary>
		public static DataBase CreateDataBase()
		{
			if (DataBase.Setup == null)
			{
				throw new NullReferenceException("DataBase.Setup event has not subsrcibed method to actually create a configured DataBase. Subscribe to this event and create your own instance.");
			}

			DataBase db = DataBase.Setup();

			if (DataBase.SettingUp != null)
			{
				DataBase.SettingUp(db);
			}

			return db;
		}

		#endregion
	}
}