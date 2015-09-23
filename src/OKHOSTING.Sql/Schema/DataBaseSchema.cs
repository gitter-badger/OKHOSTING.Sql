using System;
using System.Collections.Generic;
using System.Linq;

namespace OKHOSTING.Sql.Schema
{
	public class DataBaseSchema
	{
		public int Id { get; set; }
		public readonly List<Table> Tables = new List<Table>();
		public readonly List<View> Views = new List<View>();
		public readonly List<User> Users = new List<User>();

		public Table this[string name]
		{
			get
			{
				name = name.ToLower();
				return Tables.Where(c => c.Name.ToLower() == name).Single();
			}
		}

		public ConstraintAction ParseConstraintAction(string action)
		{
			switch (action)
			{
				case "CASCADE":
					return ConstraintAction.Cascade;

				case "SET DEFAULT":
					return ConstraintAction.Default;

				case "NO ACTION":
					return ConstraintAction.NoAction;

				case "SET NULL":
					return ConstraintAction.Null;

				case "RESTRICT":
					return ConstraintAction.Restrict;

				default:
					throw new ArgumentOutOfRangeException("action");
			}
		}
	}
}