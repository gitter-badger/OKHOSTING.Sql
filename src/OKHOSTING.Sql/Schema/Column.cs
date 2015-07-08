using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OKHOSTING.Sql.Schema
{
	/// <summary>
	/// A column in a table, semi-cloned from DatabaseSchemaReader project
	/// </summary>
	[System.ComponentModel.DefaultProperty("Name")]
	public class Column
	{
		/// <summary>
		/// Name of the column
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the description.
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// Table where this column exist
		/// </summary>
		public Table Table { get; set; }

		/// <summary>
		/// Gets or sets the type of the data
		/// </summary>
		public DbType DbType { get; set; }

		/// <summary>
		/// Precision is the number of digits in a number. For example, the number 123.45 has a precision of 5 and a scale of 2.
		/// </summary>
		public int? Precision { get; set; }

		/// <summary>
		/// Scale is the number of digits to the right of the decimal point in a number. For example, the number 123.45 has a precision of 5 and a scale of 2.
		/// </summary>
		public int? Scale { get; set; }
		
		/// <summary>
		/// The Default value that will be used if a null value is provided during an INSERT operation for this column
		/// </summary>
		public object DefaultValue { get; set; }
		
		/// <summary>
		/// Gets or sets a value indicating whether this column is an autonumber column (identity or equivalent)
		/// </summary>
		public bool IsAutoNumber { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this instance is part of the primary key.
		/// </summary>
		public bool IsPrimaryKey { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this is nullable.
		/// </summary>
		public bool IsNullable { get; set; }

		/// <summary>
		/// Gets a value indicating whether this column is "computed" or "virtual".
		/// </summary>
		public bool IsComputed
		{
			get
			{
				return !string.IsNullOrWhiteSpace(ComputedDefinition);
			}
		}

		/// <summary>
		/// Gets or sets the "computed" (or "virtual") definition.
		/// </summary>
		public string ComputedDefinition { get; set; }
		
		/// <summary>
		/// Gets or sets the length if this is string (VARCHAR) or character (CHAR) type data. In SQLServer, a length of -1 indicates VARCHAR(MAX).
		/// </summary>
		public int? Length { get; set; }
		
		/// <summary>
		/// Gets or sets the ordinal (the order that the columns were defined in the database).
		/// </summary>
		public int Ordinal { get; set; }

		/// <summary>
		/// Gets a value indicating whether this column is part of a foreign key
		/// </summary>
		public bool IsForeignKey
		{
			get
			{
				return Table.ForeignKeys.Where(fk=> fk.Columns.Where(tuple=> tuple.Item1 == this).Count() > 0).Count() > 0;
			}
		}

		/// <summary>
		/// Gets a value indicating whether this column is part of an index
		/// </summary>
		public bool IsIndex
		{
			get
			{
				return (from index in Table.Indexes where index.Columns.Contains(this) select index).Count() > 0;
			}
		}

		public bool IsString
		{
			get
			{
				return
					DbType == System.Data.DbType.String ||
					DbType == System.Data.DbType.StringFixedLength ||
					DbType == System.Data.DbType.AnsiString ||
					DbType == System.Data.DbType.AnsiStringFixedLength ||
					DbType == System.Data.DbType.Xml;
			}
		}

		public bool IsNumeric
		{
			get
			{
				return IsIntegral || IsDecimal;
			}
		}

		public bool IsIntegral
		{
			get
			{
				return
					DbType == System.Data.DbType.Byte ||
					DbType == System.Data.DbType.SByte ||
					DbType == System.Data.DbType.Int16 ||
					DbType == System.Data.DbType.Int32 ||
					DbType == System.Data.DbType.Int64 ||
					DbType == System.Data.DbType.UInt16 ||
					DbType == System.Data.DbType.UInt32 ||
					DbType == System.Data.DbType.UInt64;
			}
		}

		public bool IsDecimal
		{
			get
			{
				return
					DbType == System.Data.DbType.Currency ||
					DbType == System.Data.DbType.Decimal ||
					DbType == System.Data.DbType.Single ||
					DbType == System.Data.DbType.Double ||
					DbType == System.Data.DbType.VarNumeric;
			}
		}

		public bool IsDate
		{
			get
			{
				return
					DbType == System.Data.DbType.Date ||
					DbType == System.Data.DbType.DateTime ||
					DbType == System.Data.DbType.DateTime2 ||
					DbType == System.Data.DbType.DateTimeOffset ||
					DbType == System.Data.DbType.Time;
			}
		}

		public override bool Equals(object obj)
		{
			if (obj is Column)
			{
				return ((Column)obj).Name == Name && ((Column)obj).Table == Table;
			}

			return base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return Name.GetHashCode() * Table.GetHashCode();
		}

		public override string ToString()
		{
			return Name;
		}
	}
}