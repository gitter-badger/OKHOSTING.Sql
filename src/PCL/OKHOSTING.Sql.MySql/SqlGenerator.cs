using OKHOSTING.Sql.Operations;
using OKHOSTING.Sql.Schema;
using System;

namespace OKHOSTING.Sql.MySql
{

	/// <summary>
	/// Generator of Sql sequences for MySql RDMS
	/// </summary>
	public class SqlGenerator: SqlGeneratorBase
	{
		/// <summary>
		/// Constructs the class
		/// </summary>
		public SqlGenerator() : base() { }

		/// <summary>
		/// Returns the Sql sentece for delete all the data of
		/// the specified Table
		/// </summary>
		/// <param name="table">
		/// Table to delete
		/// </param>
		/// <returns>
		/// Sql sentece for delete all the data of
		/// the specified Table
		/// </returns>
		public override Command Delete(Delete delete)
		{
			//Validating if the table argument is null
			if (delete == null)
			{
				throw new ArgumentNullException("delete");
			}

			//Returning the sql sentence
			return "DELETE FROM " + base.EncloseName(delete.Table.Name) + WhereClause(delete.Where);
		}

		#region Properties

		/// <summary>
		/// Represent the opening character for enclosing table and column names
		/// </summary>
		/// <example>'[Name] (SQL Server), `Name` (MySQL), "Name" (Access)</example>
		protected override string NameEncloser_Begin
		{
			get
			{
				return "`";
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
				return "`";
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
				return "AUTO_INCREMENT";
			}
		}

		#endregion

		#region Format values

		/// <summary>
		/// Returns the name of the function used to retrieve the latest
		/// auto-generated primary key on the session
		/// </summary>
		/// <param name="table">Table which table will be scanned for the last autogenerated primary key</param>
		/// <returns>Sql function that returns a single value containing the last auto generated primary key on a table</returns>
		protected override Command AutoIncrementalFunction(Table table)
		{
			return "LAST_INSERT_ID()";
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
			switch (type)
			{
				case DbType.Binary:
				case DbType.Object:
					return "BLOB";

				case DbType.Byte:
					return "TINYINT UNSIGNED";
				case DbType.SByte:
					return "TINYINT";

				case DbType.UInt16:
					return "SMALLINT UNSIGNED";
				case DbType.Int16:
					return "SMALLINT";

				case DbType.Int32:
					return "INTEGER";
				case DbType.UInt32:
					return "INTEGER UNSIGNED";

				case DbType.UInt64:
					return "BIGINT UNSIGNED";

				case DbType.VarNumeric:
				case DbType.Int64:
					return "BIGINT";

				case DbType.Currency:
				case DbType.Decimal:
					return "DECIMAL(10,2)";

				case DbType.Single:
					return "FLOAT";

				case DbType.Double:
					return "DOUBLE";

				case DbType.Boolean:
					return "BOOLEAN";

				case DbType.AnsiStringFixedLength:
				case DbType.StringFixedLength:
					return "CHAR";

				case DbType.AnsiString:
				case DbType.String:
				case DbType.Guid:
				case DbType.Xml:
					return "VARCHAR";

				case DbType.Date:
					return "DATE";

				case DbType.DateTime:
				case DbType.DateTime2:
				case DbType.DateTimeOffset:
					return "DATETIME";

				case DbType.Time:
					return "TIME";
			}

			//If the flow reaches the next line, the type is not 
			//recognized and then throw an exception
			throw new NotImplementedException("MySQLGenerator not support type '" + type + "'");
		}

		#endregion
	}
}