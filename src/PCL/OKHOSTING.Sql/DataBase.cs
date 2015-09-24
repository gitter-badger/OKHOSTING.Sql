﻿using System;
using System.Collections.Generic;

namespace OKHOSTING.Sql
{
	/// <summary>
	/// Implements methods for interact with database engines
	/// </summary>
	public abstract class DataBase
	{
		#region Fields and Properties

		public int Id { get; set; }

		public string Name { get; set; }

		public Schema.DataBaseSchema Schema { get; set; }

		public string ConnectionString
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
		public abstract int Execute(List<Command> commands);

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
		public abstract bool ExistsData(Command command);

		/// <summary>
		/// Returns a boolean value that indicates if is possible to connect
		/// to database defined at the connection string of the executor
		/// </summary>
		/// <returns>
		/// Boolean value that indicates the successfully or failed connection
		/// </returns>
		public abstract bool CanConnect();

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
		public abstract bool CanConnect(out Exception exceptionOccured);

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
	}
}