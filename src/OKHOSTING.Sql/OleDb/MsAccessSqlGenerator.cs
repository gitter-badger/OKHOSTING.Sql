using OKHOSTING.Sql.Operations;
using OKHOSTING.Sql.Schema;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using OKHOSTING.Core.Data;

namespace OKHOSTING.Sql.OleDb
{

	/// <summary>
	/// Generator of Sql sequences for MsAccess
	/// </summary>
	public class MsAccessSqlGenerator: SqlGeneratorBase
	{

		/// <summary>
		/// Constructs the Sql Generator
		/// </summary>
		public MsAccessSqlGenerator() : base() { }

		#region Properties

		/// <summary>
		/// Represent the opening character for enclosing table and column names
		/// </summary>
		/// <example>'[Name] (SQL Server), `Name` (MySQL), "Name" (Access)</example>
		protected override string NameEncloser_Begin
		{
			get
			{
				return "[";
			}
		}

		/// <summary>
		/// Represent the closing character for enclosing table and column names
		/// </summary>
		/// <example>'[Name] (SQL Server), `Name` (MySQL), "Name" (Access)</example>
		protected override string NameEncloser_End
		{
			get
			{
				return "]";
			}
		}


		/// <summary>
		/// Defines the name of the attribute used to generate autoincremental 
		/// fields (a null or string.Empty value defines that the Auto 
		/// Incremental Setting is not supported)
		/// </summary>
		protected override string AutoIncrementalSettingName
		{
			get
			{
				return "AUTOINCREMENT(1,1)";
			}
		}

		#endregion

		#region Format types

		/// <summary>
		/// Converts a Type to it's database-specific string representantion, so it can be included in a SQL script for creating or modifying tables
		/// </summary>
		/// <param name="value">
		/// Type to be converted to string
		/// </param>
		/// <returns>
		/// A database-specific string representation of the type
		/// </returns>
		/// <example>
		/// For typeof(Int32), "INTEGER" is returned
		/// </example>
		protected override string Format(DbType type)
		{
			//Validating that the type Argument is not null
			if (type == null) throw new ArgumentNullException("type");

			//Getting the type name in Database
			switch (type)
			{
				case DbType.Byte:
				case DbType.SByte:
					return "SMALLINT";

				case DbType.UInt16:
				case DbType.UInt32:
				case DbType.UInt64:
					return "INTEGER";

				case DbType.Int16:
					return "SMALLINT";

				case DbType.Int32:
				case DbType.Int64:
					return "INTEGER";

				case DbType.Currency:
				case DbType.Decimal:
					return "MONEY";

				case DbType.Single:
					return "REAL";

				case DbType.Double:
					return "FLOAT";

				case DbType.Boolean:
					return "BIT";

				case DbType.AnsiStringFixedLength:
				case DbType.StringFixedLength:
					return "CHAR";

				case DbType.AnsiString:
				case DbType.String:
					return "VARCHAR";

				case DbType.Time:
				case DbType.Date:
				case DbType.DateTime:
				case DbType.DateTime2:
				case DbType.DateTimeOffset:
					return "DATETIME";
			}

			//If the flow reaches the next line, the type is not 
			//recognized and then throw an exception
			throw new NotImplementedException("MsAccessFormatProvider not support type '" + type + "'");
		}

		/// <summary>
		/// Formats a string type depending of the lenght of the string type
		/// </summary>
		/// <param name="lenght">
		/// Lenght of the string type
		/// </param>
		/// <returns>
		/// A string representation af the text data type
		/// </returns>
		/// <example>
		/// if lenght is cero, "TEXT" is returned, is lengh is 20, CHAR(20) is returned
		/// </example>
		protected override string FormatStringType(int lenght)
		{
			//Validating the lenght of the string 
			if (lenght == 0)
				return "MEMO";
			else
				return this.Format(DbType.StringFixedLength) + " (" + lenght.ToString() + ")";
		}

		#endregion

		/// <summary>
		/// Returns the From clausule for a Select statement of the
		/// specified TypeMap<T>, including the respective inner joins
		/// to the related entities
		/// </summary>
		/// <param name="dtype">
		/// TypeMap<T> wich you desire the From clausule
		/// </param>
		/// <returns>
		/// From clausule for a Select statement of the
		/// specified TypeMap<T>, including the respective inner joins
		/// to the related entities
		/// </returns>
		protected override Command FromClause(Select select)
		{
			//Validating if dtype argument is null
			if (select == null)
			{
				throw new ArgumentNullException("select");
			}
			
			//Getting the number of parent DataTypes that have the specified TypeMap<T>
			int parentDataTypes = select.Joins.Count();

			//Initializing the sql sequence
			Command command = 
				"FROM " + 
				new String('(', parentDataTypes - 1) + 
				EncloseName(select.From.Name);

			//Crossing the parent DataTypes of the specified type
			foreach(SelectJoin join in select.Joins)
			{
				//Creating the respective inner join clausule
				command += " INNER JOIN " + EncloseName(join.Table.Name) + " ON ";

				//Creating the link conditions of the join (ON clausule body)
				command += Filter(join.On, LogicalOperator.And);

				//Removing the last " AND "
				command.Script = command.Script.Remove(command.Script.Length - 5, 5);

				//Closing the inner join
				command += ")";
			}

			//Retrieving sql sentence
			return command;
		}

		/// <summary>
		/// Returns the Sql sentece for delete all the data of
		/// the specified TypeMap<T>
		/// </summary>
		/// <param name="dtype">
		/// TypeMap<T> to delete
		/// </param>
		/// <returns>
		/// Sql sentece for delete all the data of
		/// the specified TypeMap<T>
		/// </returns>
		public override Command Delete(Delete delete)
		{
			//Validating if the specified DataObject is null
			if (delete == null)
			{
				throw new ArgumentNullException("delete");
			}

			Command command = "DELETE FROM " + EncloseName(delete.From.Name);

			command.Append(WhereClause(delete.Where, LogicalOperator.And));

			//Retrieving sql sentences for delete
			return command;
		}

		/// <summary>
		/// Returns the name of the function used to retrieve the latest
		/// auto-generated primary key on the session
		/// </summary>
		/// <param name="dtype">TypeMap<T> which table will be scanned for the last autogenerated primary key</param>
		/// <returns>Sql function that returns a single value containing the last auto generated primary key on a table</returns>
		protected override Command AutoIncrementalRetrieveFunction(Table table)
		{
			return "@@IDENTITY";
		}

		/// <summary>
		/// Returns the DML for create the column corresponding 
		/// with the specified DataValue
		/// </summary>
		/// <param name="dataValue">
		/// DataValue for DML column creation
		/// </param>
		/// <returns>
		/// DML for create the column corresponding 
		/// with the specified DataValue
		/// </returns>
		protected override Command CreateColumnClause(Column column)
		{
			//if this is not an autoincrement column, use the base method
			if (!column.IsAutoNumber)
			{
				return base.CreateColumnClause(column);
			}

			//Append the column Name 
			string columnDefinition = EncloseName(column.Name) + " ";

			//use Autoincrement as the TypeMap<T>
			columnDefinition += " " + AutoIncrementalSettingName;

			//Append the NULL flag
			if (!column.IsNullable) columnDefinition += " NOT " + Null;
			else columnDefinition += " " + Null;

			//Returning the column definition 
			return columnDefinition;
		}

		/// <summary>
		/// Returns all the scripts necesary to create all indexes in a datatype
		/// </summary>
		/// <param name="dtype">TypeMap<T> which indexes will be generated</param>
		/// <param name="inherits">If true, scripts for all base DataTypes indexes are generated too</param>
		/// <returns>Sql script for creating indexes</returns>
		public override Command Create(Index index)
		{
			string sql = string.Empty;
			
			sql +=
				"CREATE " +
				(index.Unique ? "UNIQUE " : string.Empty) +
				"INDEX " +
				this.EncloseName(index.Name) +
				" ON " +
				EncloseName(index.Table.Name) +
				"(";

			//add columns to sql
			foreach (var c in index.Columns)
			{
				sql += this.EncloseName(c.Name) + ", ";
			}

			//Enclosing the field lists
			sql = sql.TrimEnd(',', ' ');
			sql += ")";

			return sql;
		}

		/// <summary>
		/// Returns the Sql sentence for creating an index in the specified TypeMap<T>. Can be used when creating a table or after creation, for adding an index
		/// </summary>
		/// <param name="index">Index that will be added</param>
		protected override Command IndexDefinition(Index index)
		{
			//this method should not be used on Access databases
			return string.Empty;
		}
	}
}