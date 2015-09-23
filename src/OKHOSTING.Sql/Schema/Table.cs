using System.Collections.Generic;
using System.Linq;

namespace OKHOSTING.Sql.Schema
{
	/// <summary>
	/// A table in a DataBase
	/// </summary>
	public class Table
	{
		public Table()
		{
		}

		public Table(string name)
		{
			Name = name;
		}

		public int Id { get; set; }

		/// <summary>
		/// The name of the table
		/// </summary>
		public string Name { get; set; }
		public string Description { get; set; }
		public DataBaseSchema DataBase { get; set; }

		public Column this[string name]
		{
			get
			{
				name = name.ToLower();
				return Columns.Where(c => c.Name.ToLower() == name).Single();
			}
		}

		public IEnumerable<Column> PrimaryKey
		{
			get
			{
				return (from c in Columns where c.IsPrimaryKey select c);
			}
		}

		public readonly List<Column> Columns = new List<Column>();
		public readonly List<ForeignKey> ForeignKeys = new List<ForeignKey>();
		public readonly List<CheckConstraint> CheckConstraints = new List<CheckConstraint>();
		public readonly List<Index> Indexes = new List<Index>();
		public readonly List<Trigger> Triggers = new List<Trigger>();
		
		/// <summary>
		/// Gets a value indicating whether this table has an autonumber column (identity or equivalent).
		/// </summary>
		public bool HasAutoNumberColumn 
		{
			get
			{
				return (from c in Columns where c.IsAutoNumber select c).FirstOrDefault() != null;
			}
		}

		/// <summary>
		/// Gets a value indicating whether this instance has a composite key.
		/// </summary>
		public bool HasCompositeKey 
		{
			get
			{
				return (from c in Columns where c.IsPrimaryKey select c).Count() > 1;
			}
		}

		public long IdentityIncrement { get; set; }
		public long IdentitySeed { get; set; }

		public override bool Equals(object obj)
		{
			if (obj is Table)
			{
				bool equals;
				equals = ((Table) obj).Name == Name;

				if (((Table)obj).DataBase != null && DataBase != null)
				{
					equals = equals && ((Table)obj).DataBase == DataBase;
				}
			}

			return base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return Name.GetHashCode();
		}

		public override string ToString()
		{
			return Name;
		}
	}
}