using System;
using OKHOSTING.Sql.Operations;
using OKHOSTING.Sql.Schema;

namespace OKHOSTING.Sql.Net4.SqlServer
{
	/// <summary>
	/// Provides formatting methods for Sql Server DataBases
	/// </summary>
	public class SqlGenerator : SqlGeneratorBase
	{
		/// <summary>
		/// Creates a new instance
		/// </summary>
		public SqlGenerator()
		{
		}

		#region Properties

		/// <summary>
		/// Represents the secuence of characters used to separate SQL scripts
		/// </summary>
		protected override string ScriptSeparator
		{
			get
			{				
                return " ";
			}
		}

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
				return "IDENTITY";
			}
		}

		#endregion

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
			//Getting the type name in Database
			switch (type)
			{
				case DbType.Byte:
				case DbType.SByte:
					return "TINYINT";

				case DbType.UInt16:
					return "INTEGER";

				case DbType.UInt32:
				case DbType.UInt64:
					return "BIGINT";

				case DbType.Int16:
					return "SMALLINT";

				case DbType.Int32:
					return "INTEGER";

				case DbType.Int64:
					return "BIGINT";

				case DbType.Currency:
				case DbType.Decimal:
					return "DECIMAL (18,3)";

				case DbType.Single:
					return "FLOAT";

				case DbType.Double:
					return "REAL";

				case DbType.Boolean:
					return "BIT";

				case DbType.AnsiStringFixedLength:
				case DbType.StringFixedLength:
					return "CHAR";

				case DbType.AnsiString:
				case DbType.String:
					return "VARCHAR";

				case DbType.Time:
				case DbType.DateTime:
				case DbType.DateTime2:
				case DbType.DateTimeOffset:
					return "DATETIME";
			}

			//If the flow reaches the next line, the type is not 
			//recognized and then throw an exception
			throw new ArgumentOutOfRangeException("type", "SqlServerFormatProvider not support type '" + type + "'");
		}

		/// <summary>
		/// Creates a Insert Sentence for the specified TypeMap<T> with 
		/// the values of the Object indicated
		/// </summary>
		/// <param name="dtype">
		/// TypeMap<T> for Insert sentence create
		/// </param>
		/// <param name="dobj">
		/// Object instance with the values to use in the Insert sentence
		/// </param>
		/// <returns>
		/// Insert Sql Sentence 
		/// </returns>
		public override Command Insert(Insert insert)
		{
			//Validating if dtype or dobj arguments are null
			if (insert == null) throw new ArgumentNullException("insert");

			//Creating column names and values script
			string columnsList = string.Empty, valuesList = string.Empty;
			Command command = new Command();

			foreach (var cv in insert.Values)
			{
				//do not include the column name or value if this is an autonumber
				if
				(
					cv.Column.IsAutoNumber &&
					cv.Value == null
				)
				{
					//do nothing
				}
				//otherwise, treat normally
				else
				{
					//CommandParameter param = new CommandParameter(cv.Value, null, cv.Column.DbType);
					CommandParameter param = CreateCommandParameter(cv);
					command.Parameters.Add(param);

					columnsList += EncloseName(cv.Column.Name) + ", ";
					valuesList +=  param.Name + ", ";
				}
			}

			//Removing last ", "
			columnsList = columnsList.Remove(columnsList.Length - 2, 2);
			valuesList = valuesList.Remove(valuesList.Length - 2, 2);

			//Creating Insert Sentence and return it
			command.Script =
				"INSERT INTO " + EncloseName(insert.Table.Name) + " " +
				EncloseOnParenthesis(columnsList) +
				" VALUES " +
				EncloseOnParenthesis(valuesList);

			return command;
		}
		
		/// <summary>
		/// Returns the Sql sentence for creating an index in the specified TypeMap<T>. Can be used when creatingt a table or after creation, for adding an index
		/// </summary>
		/// <param name="index">Index that will be added</param>
		protected override Command IndexDefinition(Index index)
		{
			//local vars
			Command sql = new Command();

			//Creating the sql 
			sql += 
				"CONSTRAINT " + 
				EncloseName(index.Name) + 
				" UNIQUE (";

			//add columns to sql
			foreach (var column in index.Columns)
			{
				sql += this.EncloseName(column.Name) + ", ";
			}

			//Enclosing the field lists
			sql.Script = sql.Script.TrimEnd(',', ' ');
			sql += ")";

			return sql;
		}

		/// <summary>
		/// Returns the name of the function used to retrieve the latest
		/// auto-generated primary key on the session
		/// </summary>
		/// <param name="dtype">TypeMap<T> which table will be scanned for the last autogenerated primary key</param>
		/// <returns>Sql function that returns a single value containing the last auto generated primary key on a table</returns>
		protected override Command AutoIncrementalFunction(Table table)
		{
			return string.Format("IDENT_CURRENT({0})", EncloseName(table.Name));
		}
	}
}