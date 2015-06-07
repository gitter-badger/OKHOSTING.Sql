using OKHOSTING.Sql.Filters;
using OKHOSTING.Sql.Operations;
using OKHOSTING.Sql.Schema;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OKHOSTING.Sql
{
	/// <summary>
	/// A DataBase is an organized data structure that allows select/insert/update/delete 
	/// operations of DataObjects
	/// </summary>
	/// <remarks>
	/// A Database can be any type of DataObject container, like a Sql DataBase, MySql, DataSet, 
	/// a file system, a memory DataBase, a remote api, etc.
	/// </remarks>
	public abstract class SqlGeneratorDataBase
	{
		/// <summary>
		/// Constructs the class instance (protected for hide to clients)
		/// </summary>
		protected SqlGeneratorDataBase()
		{
			//Raise the DataBaseCreated event
			//DataBase.OnDataBaseCreated(this);
		}

		#region Select

		/// <summary>
		/// Load all the DataObjects of the specified Table
		/// </summary>
		/// <param name="table">
		/// Table of the DataObjects to load
		/// </param>
		/// <returns>
		/// IDataReader with all the DataObjects of the specified Table
		/// </returns>
		public virtual IDataReader Select(Table table)
		{
			return Select(table, null, null, null);
		}

		/// <summary>
		/// Load all the DataObjects of the specified Table
		/// </summary>
		/// <param name="table">
		/// Table of the DataObjects to load
		/// </param>
		/// <returns>
		/// IDataReader with all the DataObjects of the specified Table
		/// </returns>
		public virtual IDataReader Select(Table table, IEnumerable<FilterBase> filters)
		{
			return Select(table, filters, null, null);
		}

		/// <summary>
		/// Load all the DataObjects of the specified Table
		/// </summary>
		/// <param name="table">
		/// Table of the DataObjects to load
		/// </param>
		/// <param name="filters">
		/// Filters collection that the DataObjects to load 
		/// must fulfill
		/// </param>
		/// <param name="orderBy">
		/// Defines the order in which the results will be sorted
		/// </param>
		/// <returns>
		/// IDataReader with all the DataObjects of the specified Table
		/// </returns>
		public virtual IDataReader Select(Table table, IEnumerable<FilterBase> filters, List<OrderBy> orderBy)
		{
			return Select(table, filters, orderBy, null);
		}

		/// <summary>
		/// Searches for a DataObject with a primary key = id. It assumes table has a single primary key value of integral type
		/// </summary>
		/// <param name="id">Id of the DataObject to select</param>
		/// <returns>Found DataObject with all it's values</returns>
		public virtual IDataReader SelectById(Table table, int id)
		{
			DataObject dobj;

			dobj = DataObject.From(table);
			dobj.PrimaryKey[0].Value = TypeConverter.ChangeType(id, dobj.PrimaryKey[0].Column.ValueType);

			return Select(dobj);
		}

		/// <summary>
		/// Load the DataObjects of the specified Table that
		/// fulfills the indicated filters (combined with the And 
		/// Logical Operator)
		/// </summary>
		/// <param name="table">
		/// Table of the DataValues to load
		/// </param>
		/// <param name="dvalues">
		/// List of DataValues to load
		/// </param>
		/// <param name="orderName">
		/// Name of the order definition used to sort the IDataReader to return
		/// (or null if not used)
		/// </param>
		/// <param name="filters">
		/// Filters collection that the DataObjects to load 
		/// must fulfill
		/// </param>
		/// <returns>
		/// IDataReader with the loaded DataObjects
		/// </returns>
		public virtual IDataReader Select(Table table, IEnumerable<FilterBase> filters, List<OrderBy> orderBy, List<Column> dvalues)
		{
			return Select(table, filters, orderBy, dvalues, null);
		}

		/// <summary>
		/// Load the DataObjects of the specified Table that
		/// fulfills the indicated filters (combined with the And 
		/// Logical Operator)
		/// </summary>
		/// <param name="table">
		/// Table of the DataValues to load
		/// </param>
		/// <param name="dvalues">
		/// List of DataValues to load
		/// </param>
		/// <param name="orderBy">
		/// Sorts the result by one or more DataValues, ascending or descending
		/// </param>
		/// <param name="filters">
		/// Filters collection that the DataObjects to load must fulfill
		/// </param>
		/// <param name="limit">
		/// Used for paging, to return only the DataObjects between an index and another
		/// </param>
		/// <returns>
		/// IDataReader with the loaded DataObjects
		/// </returns>
		public abstract IDataReader Select(Table table, IEnumerable<FilterBase> filters, List<OrderBy> orderBy, List<Column> dvalues, SelectLimit limit);

		/// <summary>
		/// Returns a DataTable with the results of the 
		/// specified select with aggregate functions
		/// </summary>
		/// <param name="aggegateSelectFields">
		/// Aggregate fields definitions
		/// </param>
		/// <param name="table">
		/// Table to query
		/// </param>
		/// <returns>
		/// DataTable with the results of the 
		/// specified select with aggregate functions
		/// </returns>
		public virtual DataTable SelectGroup(Table table, List<SelectAggregateColumn> aggregateSelectFields)
		{
			return SelectGroup(table, aggregateSelectFields, null);
		}

		/// <summary>
		/// Returns a DataTable with the results of the 
		/// specified select with aggregate functions
		/// </summary>
		/// <param name="aggegateSelectFields">
		/// Aggregate fields definitions
		/// </param>
		/// <param name="table">
		/// Table to query
		/// </param>
		/// <param name="dataValuesToGroup">
		/// Columns for group by
		/// </param>
		/// <returns>
		/// DataTable with the results of the 
		/// specified select with aggregate functions
		/// </returns>
		public virtual DataTable SelectGroup(Table table, List<SelectAggregateColumn> aggregateSelectFields, List<Column> dataValuesToGroup)
		{
			return SelectGroup(table, aggregateSelectFields, dataValuesToGroup);
		}

		/// <summary>
		/// Returns a DataTable with the results of the 
		/// specified select with aggregate functions
		/// </summary>
		/// <param name="aggegateSelectFields">
		/// Aggregate fields definitions
		/// </param>
		/// <param name="table">
		/// Table to query
		/// </param>
		/// <param name="filters">
		/// Filters to apply on Select
		/// </param>
		/// <param name="dataValuesToGroup">
		/// Columns for group by
		/// </param>
		/// <param name="orderBy">
		/// Defines the order in which the results will be sorted
		/// </param>
		/// <returns>
		/// DataTable with the results of the 
		/// specified select with aggregate functions
		/// </returns>
		public abstract DataTable SelectGroup(Table table, List<SelectAggregateColumn> aggregateSelectFields, List<Column> dataValuesToGroup, IEnumerable<FilterBase> filters, List<OrderBy> orderBy);

		#endregion

		#region Search

		/// <summary>
		/// Do a generical search of the entities with the specified Table
		/// searching in all it DataValues the specified string with the like 
		/// pattern. This search can use an excessive amount of system resources
		/// reason why it's use is recommended only for small sets of data
		/// </summary>
		/// <param name="table">
		/// Table for search
		/// </param>
		/// <param name="searchedString">
		/// String that will be searched
		/// </param>
		/// <returns>
		/// DataObject collection with all the DataObjects that contains the 
		/// searchedString on it's DataValues
		/// </returns>
		public virtual IDataReader Search(Table table, string searchedString)
		{
			return Search(table, searchedString, false, null, null, null);
		}

		/// <summary>
		/// Do a generical search of the entities with the specified Table
		/// searching in all it DataValues the specified string with the like 
		/// pattern. This search can use an excessive amount of system resources
		/// reason why it's use is recommended only for small sets of data
		/// </summary>
		/// <param name="table">
		/// Table for search
		/// </param>
		/// <param name="searchedString">
		/// String that will be searched
		/// </param>
		/// <param name="deepSearch">
		/// If set to true, the search looks into inbound and oubound 
		/// foreign keys ass well as the main table, 
		/// if false, will only search into the main table
		/// </param>
		/// <returns>
		/// DataObject collection with all the DataObjects that contains the 
		/// searchedString on it's DataValues
		/// </returns>
		public virtual IDataReader Search(Table table, string searchedString, bool deepSearch, List<OrderBy> orderBy, List<Column> dvalues, SelectLimit limit)
		{
			//Local Vars
			OrFilter orAux;

			//Validating if the table argument is null
			if (table == null) throw new ArgumentNullException("table", "Argument cannot be null");

			//Creating the Or logical filters for the Query
			OrFilter or = GetSearchFilter(table, searchedString);

			//Searching in all outbound foreign keys
			if (deepSearch)
			{
				foreach (Column column in table.GetOutboundForeignKeys())
				{
					//Creating the Or Logical filter with the Table of the aggregate DataObject
					orAux = GetSearchFilter(column.ValueType, searchedString);

					//Validating the filter is valid
					if (orAux != null)
					{
						//Select all foreign DataObjects that match the criteria and 
						//add them as foreign key filters to the main search
						foreach (DataObject dobj in Select(column.ValueType, new List<FilterBase>(orAux)))
						{
							//Creating the Foreign Key filter and adding criteria to main search
							ForeignKeyFilter fkf = new ForeignKeyFilter(column, dobj);
							or.InnerFilters.Add(fkf);
						}
					}
				}
			}

			//Querying the Database...
			IDataReader result = Select(table, new List<FilterBase>(or), orderBy, dvalues);

			//Searching in the inbound Foreign Keys
			if (deepSearch)
			{
				foreach (Column column in table.GetInboundForeignKeys())
				{
					//ommit DataObject generic foreign keys
					if (!Table.IsDataObjectSubClass(column.ValueType)) continue;

					//Creating the Or Logical filter with the Table of the aggregate collection
					orAux = GetSearchFilter(column.DeclaringDataType, searchedString);

					//Validating the filter is valid
					if (orAux != null)
					{
						//Select all related DataObjects that match the criteria and 
						//add them as foreign key filters to the main search
						foreach (DataObject dobj in Select(column.DeclaringDataType, new List<FilterBase>(orAux)))
						{
							//Loading the current DataObject
							DataObject temp = (DataObject)dobj.GetValue(column);

							//if null value, continue
							if (temp == null) continue;

							//load temp from database to be sure it is of the corresponding table
							IDataReader tempCollection = Select(table, new List<FilterBase>(){ new PrimaryKeyFilter(temp) });
							
							//if no object was found, continue
							if (tempCollection.Count == 0) continue;
							
							//get firs result
							temp = tempCollection[0];

							//validating if the IDataReader to return already contains the current DataObject
							if (!result.Contains(temp))
							{
								//adding temp to the result collection
								result.Add(temp);
							}
						}
					}
				}
			}

			//Returning the collection
			if (limit != null)
			{
				IDataReader range = new IDataReader();
				range.AddRange(result.GetRange(limit.From, limit.Count));
				return range;
			}
			else
			{
				return result;
			}
		}

		#endregion

		#region Insert

		/// <summary>
		/// Inserts a DataObject into the DataBase
		/// </summary>
		/// <param name="dobj">
		/// DataObject to be inserted
		/// </param>
		public abstract void Insert(Insert dobj);

		#endregion

		#region Update

		/// <summary>
		/// Update the entities of the specified Table
		/// </summary>
		/// <param name="table">
		/// Table of the entities to update
		/// </param>
		/// <param name="dvalues">
		/// DataValues that will be updated with it's new values
		/// </param>
		/// <returns>
		/// The total of rows affected by the update
		/// </returns>
		public virtual void Update(Table table, List<DataValueInstance> dvalues)
		{
			//Updating and returning the affected rows...
			this.Update(table, dvalues, null);
		}

		/// <summary>
		/// Update the entities of the specified Table  
		/// that fulfill the indicated filter collection
		/// </summary>
		/// <param name="table">
		/// Table of the entities to update
		/// </param>
		/// <param name="dvalues">
		/// DataValues that will be updated with it's new values
		/// </param>
		/// <param name="filters">
		/// Filters that must fulfill the entities to update (are 
		/// combined with the And Logical Operator)
		/// </param>
		/// <returns>
		/// The total of rows affected by the update
		/// </returns>
		public virtual void Update(Table table, List<DataValueInstance> dvalues, IEnumerable<FilterBase> filters)
		{
			//Validating if the table and dataValues arguments are null
			if (table == null) throw new ArgumentNullException("table");
			if (dvalues == null) throw new ArgumentNullException("dvalues");
			
			//default values
			if (filters == null) filters = new List<FilterBase>();

			//Loading the DataObjects to update
			IDataReader dobjs = this.Select(table, filters);

			//create Column collection for massive update
			List<Column> _values = new List<Column>();
			
			//Crossing the DataValues collection
			foreach (DataValueInstance dvi in dvalues)
			{
				//update values in memory for all DataObjects
				foreach (DataObject dobj in dobjs)
				{
					dvi.Column.SetValue(dobj, dvi.Value);
				}

				//add to values collection
				_values.Add(dvi.Column);
			}

			//massive update
			Update(dobjs, _values);
		}

		/// <summary>
		/// Updates a full DataObject collection
		/// </summary>
		/// <param name="dobjs">
		/// DataObject collection to be updated
		/// </param>
		public virtual void Update(IDataReader dobjs)
		{
			this.Update(dobjs, null);
		}

		/// <summary>
		/// Updates a full DataObject collection into the DataBase
		/// </summary>
		/// <param name="dobjs">DataObject collection to be updated</param>
		/// <param name="dvalues">List of DataValues that will be updated</param>
		public virtual void Update(IDataReader dobjs, List<Column> dvalues)
		{
			//Validating if the IDataReader is null
			if (dobjs == null) throw new ArgumentNullException("dobjs");
			
			//default values
			if (dvalues == null) dvalues = dobjs.Table.AllValues;

			//Crossing the DataObjects on the collection
			foreach (DataObject dobj in dobjs)
			{
				//Updating DataObject
				this.Update(dobj, dvalues);
			}
		}

		/// <summary>
		/// Updates a DataObject on the DataBase
		/// </summary>
		/// <param name="dobj">
		/// DataObject to be updated
		/// </param>
		public virtual void Update(DataObject dobj)
		{
			Update(dobj, null);
		}

		/// <summary>
		/// Updates a DataObject on the DataBase
		/// </summary>
		/// <param name="dobj">
		/// DataObject to be updated
		/// </param>
		/// <param name="dvalues">List of DataValues that will be updated</param>
		public abstract void Update(DataObject dobj, List<Column> dvalues);

		#endregion

		#region Delete

		/// <summary>
		/// Delete the entities of the specified Table
		/// </summary>
		/// <param name="table">
		/// Table of the entities to delete
		/// </param>
		/// <returns>
		/// The total of rows affected by the delete
		/// </returns>
		public virtual void Delete(Table table)
		{
			//Deleting and returning the affected rows...
			Delete(table, null);
		}

		/// <summary>
		/// Deletes the entities of the specified Table 
		/// that fulfill the indicated filter collection 
		/// </summary>
		/// <param name="table">
		/// Table of the entities to delete
		/// </param>
		/// <param name="filters">
		/// Filters that must fulfill the entities to delete (are 
		/// combined with the And Logical Operator)
		/// </param>
		/// <returns>
		/// The total of rows affected by the delete
		/// </returns>
		public virtual void Delete(Table table, IEnumerable<FilterBase> filters)
		{
			//Validating if the table argument is null
			if (table == null) throw new ArgumentNullException("table", "Argument cannot be null");
			this.Delete(this.Select(table, filters));
		}

		#endregion

		#region Utilities

		/// <summary>
		/// Returns the number of DataObjects found in the DataBase
		/// </summary>
		/// <param name="table">Table that will be searched in the Database</param>
		/// <returns>Number of DataObjects found in the DataBase</returns>
		public virtual ulong Count(Table table)
		{
			return Count(table, null);
		}
		
		/// <summary>
		/// Returns the number of DataObjects found in the DataBase
		/// </summary>
		/// <param name="table">Table that will be searched in the Database</param>
		/// <param name="filters">Optional filters</param>
		/// <returns>Number of DataObjects found in the DataBase</returns>
		public virtual ulong Count(Table table, IEnumerable<FilterBase> filters)
		{
			return (ulong) Select(table, filters).Count;
		}

		/// <summary>
		/// Returns the Date and Hour from the database Server
		/// </summary>
		/// <returns>
		/// Date and Hour from the database Server
		/// </returns>
		public virtual DateTime DateTimeOnDBServer()
		{
			return DateTime.Now;
		}

		/// <summary>
		/// Returns a Global Unique Identifier from the Database
		/// </summary>
		/// <returns>
		/// Global Unique Identifier 
		/// </returns>
		public virtual string GetUniqueIdentifier()
		{
			return new Guid().ToString();
		}

		/// <summary>
		/// Performs initial tasks so the current DataBase can perform operations on these DataTypes
		/// </summary>
		/// <param name="dtypes">List of DataTypes that will be supported by the current DataBase</param>
		/// <remarks>This method should only be executed on system setup</remarks>
		public abstract void Setup(List<Table> dtypes);

		/// <summary>
		/// Verifies that all DataTypes are crreclty setup in the Database
		/// by performing a simple select operation on each one. 
		/// If errors are found, a list of exceptions is returned
		/// </summary>
		/// <param name="dtypes">List of DataTypes that will be verified by the current DataBase</param>
		/// <returns>A list of exceptions found in the current setup, if any</returns>
		/// <remarks>This method should only be executed after a system setup or system update</remarks>
		public virtual List<DataTypeNotSupportedException> VerifySetup(List<Table> dtypes)
		{
			List<DataTypeNotSupportedException> errors = new List<DataTypeNotSupportedException>();

			//make a simple select to see if all the collumns are defined for each Table
			foreach (Table table in dtypes)
			{
				try
				{
					Select(table);
				}
				catch (Exception e)
				{
					errors.Add(new DataTypeNotSupportedException(table, e.Message, e));
				}
			}

			return errors;
		}

		#endregion

		#region Protected methods

		/// <summary>
		/// Return an Or Logical filter with the structure 
		/// "DataValue1 like '%' + filter + '%' or DataValue2 like '%' + filter + '%' or ..."
		/// with all the DataValues of the specified Table
		/// </summary>
		/// <param name="type">
		/// Table used to create the filter
		/// </param>
		/// <param name="likePattern">
		/// String used as Like Pattern on the filter
		/// </param>
		/// <returns>
		/// Or Logical filter with the structure 
		/// "DataValue1 like '%' + filter + '%' or DataValue2 like '%' + filter + '%' or ..."
		/// with all the DataValues of the specified Table
		/// </returns>
		protected OrFilter GetSearchFilter(Table type, string likePattern)
		{
			//Creating Or Logic Filter
			OrFilter or = new OrFilter();

			//Crossing the DataValues on members of Table
			foreach (Column column in type.AllValues)
			{
				//LIKE filter for a string value
				if (column.ValueType.Equals(typeof(string)))
				{
					//Creating Like filter and adding to Or Filter
					or.InnerFilters.Add(new LikeFilter(column, "%" + likePattern + "%"));
				}

				//Compare filter for a numeric value
				else if (column.IsNumeric)
				{
					//integral value
					if (column.ValueType.IsIntegral())
					{
						int pattern;
						if (Int32.TryParse(likePattern, out pattern))
						{
							or.InnerFilters.Add(new ValueCompareFilter(column, pattern, CompareOperator.Equal));
						}
					}

					//decimal value
					else
					{
						decimal pattern;
						if (decimal.TryParse(likePattern, out pattern))
						{
							or.InnerFilters.Add(new ValueCompareFilter(column, pattern, CompareOperator.Equal));
						}
					}
				}
			}

			//Establishing Or logical filter to null if dont have inner filters
			if (or.InnerFilters.Count == 0) or = null;

			//Return Or Logical Filter
			return or;
		}

		/// <summary>
		/// Returns the Filter to use for discard logical deleted entities
		/// </summary>
		/// <param name="table">
		/// Table to be query
		/// </param>
		/// <returns>
		/// ValueCompareFilter with the filter criteria
		/// </returns>
		protected ValueCompareFilter GetDeletedFilter(Table table)
		{
			return new ValueCompareFilter((Column)table["Deleted"], false, CompareOperator.Equal);
		}

		/// <summary>
		/// Returns a value that indicates if must be filtered 
		/// the logical deleted entities of the specified Table
		/// </summary>
		/// <param name="table">
		/// Table to validate
		/// </param>
		/// <returns>
		/// true if the logical deleted entities must be filtered, otherwise false
		/// </returns>
		protected bool MustFilterDeleted(Table table)
		{
			return IsIDeleted(table) && !IncludeLogicalDeleted;
		}

		/// <summary>
		/// Retuns a value that indicates if the specified Table 
		/// implement the logical delete behavior
		/// </summary>
		/// <param name="table">
		/// Table to validate
		/// </param>
		/// <returns>
		/// true if the specified Table implements IDeleted, otherwise false
		/// </returns>
		protected bool IsIDeleted(Table table)
		{
			return typeof(IDeleted).IsAssignableFrom(table.InnerType);
		}

		#endregion

		#region Events

		/// <summary>
		/// Delegate for DataBaseCreated event
		/// </summary>
		public delegate void DataBaseCreatedEventHandler(DataBase sender);

		/// <summary>
		/// Delegate for BeforeSelect event
		/// </summary>
		public delegate void BeforeSelectEventHandler(DataBase sender, SelectEventArgs e);
		
		/// <summary>
		/// Delegate for AfterSelect event
		/// </summary>
		public delegate void AfterSelectEventHandler(DataBase sender, SelectEventArgs e);

		/// <summary>
		/// Delegate for BeforeSelectGroup event
		/// </summary>
		public delegate void BeforeSelectGroupEventHandler(DataBase sender, SelectGroupEventArgs e);

		/// <summary>
		/// Delegate for AfterSelectGroup event
		/// </summary>
		public delegate void AfterSelectGroupEventHandler(DataBase sender, SelectGroupEventArgs e);
		
		/// <summary>
		/// Delegate for insert, delete, and update operation events
		/// </summary>
		public delegate void OperationEventHandler(DataBase sender, CommandEventArgs e);

		/// <summary>
		/// Occurs when a new DataBase instance is created in the current application
		/// </summary>
		public static event DataBaseCreatedEventHandler DataBaseCreated;

		/// <summary>
		/// Occurs before a Select operation is performed
		/// </summary>
		public event BeforeSelectEventHandler BeforeSelect;

		/// <summary>
		/// Occurs after a Select operation is performed
		/// </summary>
		public event AfterSelectEventHandler AfterSelect;

		/// <summary>
		/// Occurs before a Select Group operation is performed
		/// </summary>
		public event BeforeSelectGroupEventHandler BeforeSelectGroup;

		/// <summary>
		/// Occurs after a Select Group operation is performed
		/// </summary>
		public event AfterSelectGroupEventHandler AfterSelectGroup;

		/// <summary>
		/// Occurs before a Insert operation is performed
		/// </summary>
		public event OperationEventHandler BeforeInsert;

		/// <summary>
		/// Occurs after a Insert operation is performed
		/// </summary>
		public event OperationEventHandler AfterInsert;

		/// <summary>
		/// Occurs before a Update operation is performed
		/// </summary>
		public event OperationEventHandler BeforeUpdate;

		/// <summary>
		/// Occurs after a Update operation is performed
		/// </summary>
		public event OperationEventHandler AfterUpdate;

		/// <summary>
		/// Occurs before a Delete operation is performed
		/// </summary>
		public event OperationEventHandler BeforeDelete;

		/// <summary>
		/// Occurs after a Delete operation is performed
		/// </summary>
		public event OperationEventHandler AfterDelete;

		/// <summary>
		/// Raises the DataBaseCreated event
		/// </summary>
		protected static void OnDataBaseCreated(DataBase dataBase)
		{
			//Raise the DataBase event
			if (DataBase.DataBaseCreated != null) DataBase.DataBaseCreated(dataBase);
		}

		/// <summary>
		/// Raises the BeforeSelect event. Also, adds filters to affect (or not) logical deleted DataObjects
		/// </summary>
		protected virtual void OnBeforeSelect(SelectEventArgs e)
		{
			//check if Table is supported
			//if (!SupportedDataTypes.Contains(e.Table)) throw new DataTypeNotSupportedException(e.Table);

			//If Table implements IDeleted and IncludeLogicalDeleted, add filter so logically deleted objects are not retrieved
			if (MustFilterDeleted(e.Table))
			{
				e.Filters.Add(GetDeletedFilter(e.Table));
			}

			//Raise the DataBase event
			if (BeforeSelect != null) BeforeSelect(this, e);

			//Raise the Table event
			e.Table.OnBeforeSelect(e);
		}

		/// <summary>
		/// Raises the AfterSelect event
		/// </summary>
		protected virtual void OnAfterSelect(SelectEventArgs e)
		{
			//Raise the DataBase event
			if (AfterSelect != null) AfterSelect(this, e);

			//Raise the Table event
			e.Table.OnAfterSelect(e);

			//Raise the DataObject event
			foreach (DataObject dobj in e.Result)
			{
				dobj.OnAfterSelect();
			}
		}

		/// <summary>
		/// Raises the BeforeSelect event. Also, adds filters to affect (or not) logical deleted DataObjects
		/// </summary>
		protected virtual void OnBeforeSelectGroup(SelectGroupEventArgs e)
		{
			//If Table implements IDeleted and IncludeLogicalDeleted, add filter so logically deleted objects are not retrieved
			if (MustFilterDeleted(e.Table))
			{
				e.Filters.Add(GetDeletedFilter(e.Table));
			}

			//Raise the DataBase event
			if (BeforeSelectGroup != null) BeforeSelectGroup(this, e);

			//Raise the Table event
			e.Table.OnBeforeSelectGroup(e);
		}

		/// <summary>
		/// Raises the AfterSelect event
		/// </summary>
		protected virtual void OnAfterSelectGroup(SelectGroupEventArgs e)
		{
			//Raise the DataBase event
			if (AfterSelectGroup != null) AfterSelectGroup(this, e);

			//Raise the Table event
			e.Table.OnAfterSelectGroup(e);
		}

		/// <summary>
		/// Raises the BeforeInsert event
		/// </summary>
		protected virtual void OnBeforeInsert(CommandEventArgs e)
		{
			//check if Table is supported
			//if (!SupportedDataTypes.Contains(e.DataObject.Table)) throw new DataTypeNotSupportedException(e.DataObject.Table);

			//Raise the DataBase event
			if (BeforeInsert != null) BeforeInsert(this, e);

			//Raise the Table event
			e.DataObject.Table.OnBeforeInsert(e);

			//Raise the DataObject event
			e.DataObject.OnBeforeInsert();
		}

		/// <summary>
		/// Raises the AfterInsert event
		/// </summary>
		protected virtual void OnAfterInsert(CommandEventArgs e)
		{
			//Raise the DataObject event
			e.DataObject.OnAfterInsert();

			//Raise the Table event
			e.DataObject.Table.OnAfterInsert(e);

			//Raise the DataBase event
			if (AfterInsert != null) AfterInsert(this, e);
		}

		/// <summary>
		/// Raises the BeforeUpdate event
		/// </summary>
		protected virtual void OnBeforeUpdate(CommandEventArgs e)
		{
			//check if Table is supported
			//if (!SupportedDataTypes.Contains(e.DataObject.Table)) throw new DataTypeNotSupportedException(e.DataObject.Table);

			//Raise the DataBase event
			if (BeforeUpdate != null) BeforeUpdate(this, e);

			//Raise the Table event
			e.DataObject.Table.OnBeforeUpdate(e);

			//Raise the DataObject event
			e.DataObject.OnBeforeUpdate();
		}

		/// <summary>
		/// Raises the AfterUpdate event
		/// </summary>
		protected virtual void OnAfterUpdate(CommandEventArgs e)
		{
			//Raise the DataBase event
			if (AfterUpdate != null) AfterUpdate(this, e);

			//Raise the Table event
			e.DataObject.Table.OnAfterUpdate(e);

			//Raise the DataObject event
			e.DataObject.OnAfterUpdate();
		}

		/// <summary>
		/// Raises the BeforeDelete event
		/// </summary>
		protected virtual void OnBeforeDelete(CommandEventArgs e)
		{
			//check if Table is supported
			//if (!SupportedDataTypes.Contains(e.DataObject.Table)) throw new DataTypeNotSupportedException(e.DataObject.Table);

			//Raise the DataBase event
			if (BeforeDelete != null) BeforeDelete(this, e);

			//Raise the Table event
			e.DataObject.Table.OnBeforeDelete(e);

			//Raise the DataObject event
			e.DataObject.OnBeforeDelete();
			
			//Look for inherited objects and delete them
			Delete(this.SearchInheritedFrom(e.DataObject));
		}

		/// <summary>
		/// Raises the AfterDelete event
		/// </summary>
		protected virtual void OnAfterDelete(CommandEventArgs e)
		{
			//Raise the DataBase event
			if (AfterDelete != null) AfterDelete(this, e);

			//Raise the Table event
			e.DataObject.Table.OnAfterDelete(e);

			//Raise the DataObject event
			e.DataObject.OnAfterDelete();
		}

		#endregion

		#region Transactions

		/// <summary>
		/// Gets a value indicating if a transaction is currently active
		/// </summary>
		public abstract bool IsTransactionActive
		{
			get;
		}

		/// <summary>
		/// Begins a new transaction
		/// </summary>
		public abstract void BeginTransaction();

		/// <summary>
		/// Commits the current transaction
		/// </summary>
		public abstract void CommitTransaction();

		/// <summary>
		/// Rolls back the current transaction
		/// </summary>
		public abstract void RollBackTransaction();

		#endregion

		#region Not yet implemented

		/*
		This methods are not yet finished and should not be used
		
		/// <summary>
		/// Inserts a child DataObject into the DataBase
		/// but a parent DataObject must be already registered
		/// </summary>
		/// <param name="dobj">
		/// Child DataObject to be inserted
		/// </param>
		/// <remarks>
		/// Usefull to create a child object when the base object already exist.
		/// For example, you have a "Person" that is already registered, but you want to convert
		/// that Person into a "User" (Person is the base class and User inherits from Person).
		/// Use this method to create the User without the need of previously deleting the Person
		/// Only the higher level Table is inserted.
		/// </remarks>
		public abstract void InsertChild(DataObject dobj);

		/// <summary>
		/// Deletes a child DataObject into from the DataBase
		/// but without deleting the parent DataObject
		/// </summary>
		/// <param name="dobj">
		/// Child DataObject to be deleted
		/// </param>
		/// <remarks>
		/// Usefull to delete a child object without deleting the base object.
		/// For example, you have a "User" already registered, but you want to convert
		/// that User into a "Person" (Person is the base class and User inherits from Person).
		/// Use this method to delete the User without the need of deleting the Person.
		/// Only the higher level Table is deleted.
		/// </remarks>
		public abstract void DeleteChild(DataObject dobj);
		 
		*/

		#endregion
	}
}