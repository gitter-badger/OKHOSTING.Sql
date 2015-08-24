using DatabaseSchemaReader.DataSchema;
using OKHOSTING.Core;
using OKHOSTING.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OKHOSTING.Sql.Schema
{
	public class DataBaseSchema
	{
		public int Id { get; set; }
		public DataBase DataBase { get; set; }
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

		private ConstraintAction ParseConstraintAction(string action)
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

		/// <summary>
		/// Returns the full schema of the database including tables, indexes, foreign keys, etc.
		/// </summary>
		/// <remarks>
		/// It's very slow for large databases
		/// </remarks>
		internal static DataBaseSchema Load(DataBase nativeDatabase, string schemaProvider)
		{
			DataBaseSchema schema = new DataBaseSchema();
			DatabaseSchema schemaReader;

			using (var dbReader = new DatabaseSchemaReader.DatabaseReader(schema.DataBase.ConnectionString, schemaProvider))
			{
				dbReader.AllTables();
				dbReader.AllViews();

				try
				{
					dbReader.AllStoredProcedures();
				}
				catch { }

				try
				{
					dbReader.AllUsers();
				}
				catch { }

				schemaReader = dbReader.DatabaseSchema;
			}

			foreach (DatabaseTable dbt in schemaReader.Tables)
			{
				dbt.PrimaryKeyColumn.AddIdentity();

				var table = new Table()
				{
					Name = dbt.Name,
					DataBase = schema,
					IdentityIncrement = dbt.PrimaryKeyColumn.IdentityDefinition.IdentityIncrement,
					IdentitySeed = dbt.PrimaryKeyColumn.IdentityDefinition.IdentitySeed,
				};

				schema.Tables.Add(table);
			}

			foreach (DatabaseTable dbt in schemaReader.Tables)
			{
				var table = schema[dbt.Name];

				foreach (DatabaseColumn dbc in dbt.Columns)
				{
					table.Columns.Add(new Column()
					{
						Name = dbc.Name,
						//DbType = DataBase.Parse(dbc.DataType.GetNetType()),
						Table = table,
						Description = dbc.Description,
						IsNullable = dbc.Nullable,
						IsAutoNumber = dbc.IsAutoNumber,
						ComputedDefinition = dbc.ComputedDefinition,
						DefaultValue = dbc.DefaultValue,
						IsPrimaryKey = dbc.IsPrimaryKey,
						Length = (uint) dbc.Length,
						Ordinal = dbc.Ordinal,
						Precision = dbc.Precision,
						Scale = dbc.Scale,
					});
				}

				foreach (DatabaseIndex dbi in dbt.Indexes)
				{
					Index index = new Index()
					{
						Name = dbi.Name,
						Table = table,
						Direction = SortDirection.Ascending,
						Unique = dbi.IsUnique,
					};

					foreach (DatabaseColumn dbc in dbi.Columns)
					{
						index.Columns.Add(table[dbc.Name]);
					}
					
					table.Indexes.Add(index);
				}

				foreach (DatabaseTrigger dbtr in dbt.Triggers)
				{
					DataBaseOperation operation = DataBaseOperation.Insert;
					Enum.TryParse<DataBaseOperation>(dbtr.TriggerEvent, true, out operation);

					Trigger trigger = new Trigger()
					{
						TriggerBody = dbtr.TriggerBody,
						TriggerEvent = operation,
						Table = table,
					};

					table.Triggers.Add(trigger);
				}

				foreach (DatabaseConstraint dbcons in dbt.CheckConstraints)
				{
					if (dbcons.ConstraintType == ConstraintType.Check)
					{
						CheckConstraint constraint = new CheckConstraint()
						{
							Expression = dbcons.Expression,
							Table = table,
						};
						
						table.CheckConstraints.Add(constraint);
					}
					else if (dbcons.ConstraintType == ConstraintType.ForeignKey)
					{
						ForeignKey foreignKey = new ForeignKey()
						{
							Name= dbcons.Name,
							DeleteAction = schema.ParseConstraintAction(dbcons.DeleteRule),
							UpdateAction =schema.ParseConstraintAction(dbcons.UpdateRule),
							RemoteTable = schema[dbcons.RefersToTable],
							Table = table,
						};

						var referencedColumns = dbcons.ReferencedColumns(schemaReader).ToArray();
						for (int i = 0; i < dbcons.Columns.Count; i++)
						{
							foreignKey.Columns.Add(new Tuple<Column,Column>(table[dbcons.Columns[i]], foreignKey.RemoteTable[referencedColumns[i]]));
						}

						table.ForeignKeys.Add(foreignKey);
					}
				}
			}

			foreach (DatabaseView dbv in schemaReader.Views)
			{
				View view = new View()
				{
					Name = dbv.Name,
					Command = dbv.Sql,
					Description = dbv.Description,
					DataBase = schema,
				};

				schema.Views.Add(view);
			}

			foreach (DatabaseUser dbu in schemaReader.Users)
			{
				User user = new User()
				{
					Name = dbu.Name,
					DataBase = schema,
				};

				schema.Users.Add(user);
			}

			return schema;
		}
	}
}