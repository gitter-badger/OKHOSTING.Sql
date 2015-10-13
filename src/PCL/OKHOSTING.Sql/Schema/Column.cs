using OKHOSTING.Data.Validation;
using System.Linq;

namespace OKHOSTING.Sql.Schema
{
	/// <summary>
	/// A column in a table, semi-cloned from DatabaseSchemaReader project
	/// </summary>
	public class Column
	{
		public int Id { get; set; }

		/// <summary>
		/// Name of the column
		/// </summary>
		[RequiredValidator]
		[StringLengthValidator(200)]
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the description.
		/// </summary>
		[StringLengthValidator(StringLengthValidator.Unlimited)]
		public string Description { get; set; }

		/// <summary>
		/// Table where this column exist
		/// </summary>
		[RequiredValidator]
		public Table Table { get; set; }

		/// <summary>
		/// Gets or sets the type of the data
		/// </summary>
		[RequiredValidator]
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
		[RequiredValidator]
		public bool IsAutoNumber { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this instance is part of the primary key.
		/// </summary>
		[RequiredValidator]
		public bool IsPrimaryKey { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this is nullable.
		/// </summary>
		[RequiredValidator]
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
		[StringLengthValidator(100)]
		public string ComputedDefinition { get; set; }
		
		/// <summary>
		/// Gets or sets the length if this is string (VARCHAR) or character (CHAR) type data. In SQLServer, a length of 0 or null indicates VARCHAR(MAX).
		/// </summary>
		public uint? Length { get; set; }
		
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
					DbType == DbType.String ||
					DbType == DbType.StringFixedLength ||
					DbType == DbType.AnsiString ||
					DbType == DbType.AnsiStringFixedLength ||
					DbType == DbType.Xml;
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
					DbType == DbType.Byte ||
					DbType == DbType.SByte ||
					DbType == DbType.Int16 ||
					DbType == DbType.Int32 ||
					DbType == DbType.Int64 ||
					DbType == DbType.UInt16 ||
					DbType == DbType.UInt32 ||
					DbType == DbType.UInt64;
			}
		}

		public bool IsDecimal
		{
			get
			{
				return
					DbType == DbType.Currency ||
					DbType == DbType.Decimal ||
					DbType == DbType.Single ||
					DbType == DbType.Double ||
					DbType == DbType.VarNumeric;
			}
		}

		public bool IsDate
		{
			get
			{
				return
					DbType == DbType.Date ||
					DbType == DbType.DateTime ||
					DbType == DbType.DateTime2 ||
					DbType == DbType.DateTimeOffset ||
					DbType == DbType.Time;
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