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
		protected DataBaseSchema(DataBase nativeDatabase)
		{
			DataBase = nativeDatabase;
		}

		public DataBase DataBase;
		public List<Table> Tables = new List<Table>();
		public List<View> Views = new List<View>();
		public List<User> Users = new List<User>();

		public Table this[string name]
		{
			get
			{
				name = name.ToLower();
				return Tables.Where(c => c.Name.ToLower() == name).Single();
			}
		}

		private ConstraintAction Parse(string action)
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
			DataBaseSchema schema = new DataBaseSchema(nativeDatabase);
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
				dbReader.
			}

			AutoMapper.Mapper.CreateMap<DatabaseSchema, DataBaseSchema>()
				.ForMember(sdb => sdb.DataBase, a => a.Ignore());

			AutoMapper.Mapper.CreateMap<DatabaseTable, Table>()
				.ForMember(t => t.DataBase, a => a.UseValue(schema))
				.ForMember(t => t.PrimaryKey, a => a.Ignore())
				.ForMember(t => t.IdentityIncrement, a => a.MapFrom(s => s.PrimaryKeyColumn.IdentityDefinition.IdentityIncrement))
				.ForMember(t => t.IdentitySeed, a => a.MapFrom(s => s.PrimaryKeyColumn.IdentityDefinition.IdentitySeed));

			AutoMapper.Mapper.CreateMap<DatabaseColumn, Column>()
				.ForMember(c => c.DbType, a => a.MapFrom(dbc => DataBase.Parse(dbc.DataType.GetNetType()))) //estaba comentado?
				.ForMember(c => c.Table, a => a.MapFrom(dbc => dbc.Table))
				.ForMember(c => c.IsNullable, a => a.MapFrom(dbc => dbc.Nullable));

			AutoMapper.Mapper.CreateMap<DatabaseIndex, Index>()
				.ForMember(i => i.Table, a => a.MapFrom(dbi => schemaReader.FindTableByName(dbi.TableName)))
				.ForMember(i => i.Direction, a => a.UseValue<SortDirection>(SortDirection.Ascending))
				.ForMember(i => i.Unique, a => a.MapFrom(dbi => dbi.IsUnique));

			AutoMapper.Mapper.CreateMap<DatabaseTrigger, Trigger>()
				.ForMember(i => i.Table, a => a.MapFrom(dbt => schemaReader.FindTableByName(dbt.TableName)));

			AutoMapper.Mapper.CreateMap<DatabaseConstraint, ForeignKey>()
				.ForMember(fk => fk.Table, a => a.MapFrom(dbc => schemaReader.FindTableByName(dbc.TableName)))
				.ForMember(fk => fk.RemoteTable, a => a.MapFrom(dbc => schemaReader.FindTableByName(dbc.RefersToTable)))
				.ForMember(fk => fk.UpdateAction, a => a.MapFrom(dbc => schema.Parse(dbc.UpdateRule)))
				.ForMember(fk => fk.DeleteAction, a => a.MapFrom(dbc => schema.Parse(dbc.DeleteRule)))
				.ForMember(fk => fk.Columns, a => a.Ignore());

			AutoMapper.Mapper.CreateMap<DatabaseConstraint, CheckConstraint>()
				.ForMember(cc => cc.Table, a => a.MapFrom(dcc => schemaReader.FindTableByName(dcc.TableName)));

			AutoMapper.Mapper.CreateMap<DatabaseView, View>()
				.ForMember(i => i.DataBase, a => a.UseValue<DataBaseSchema>(schema))
				.ForMember(i => i.Command, a => a.MapFrom(dbv => dbv.Sql));

			AutoMapper.Mapper.CreateMap<DatabaseUser, User>()
				.ForMember(u => u.DataBase, a => a.UseValue(schema));

			AutoMapper.Mapper.AssertConfigurationIsValid();

			AutoMapper.Mapper.Map<DatabaseSchema, DataBaseSchema>(schemaReader, schema);

			//fix foreign keys local and foreign tables manually since they fail
			foreach (Table t in schema.Tables)
			{
				foreach (Column c in t.Columns)
				{
					c.Table = t;
				}

				foreach (ForeignKey c in t.ForeignKeys)
				{
					c.Table = t;
				}

				foreach (CheckConstraint c in t.CheckConstraints)
				{
					c.Table = t;
				}

				foreach (Index c in t.Indexes)
				{
					c.Table = t;
				}

				foreach (Trigger c in t.Triggers)
				{
					c.Table = t;
				}
			}

			foreach (DatabaseTable t1 in schemaReader.Tables)
			{
				Table t2 = (from t in schema.Tables where t.Name == t1.Name select t).Single();

				foreach (DatabaseConstraint nativeFK in t1.ForeignKeys)
				{
					ForeignKey foreignKey = (from fk in t2.ForeignKeys where fk.Name == nativeFK.Name select fk).Single();

					for (int i = 0; i < nativeFK.Columns.Count; i++)
					{
						Column localColumn = (from c in foreignKey.Table.Columns where c.Name == nativeFK.Columns[i] select c).Single();
						Column foreignColumn = (from c in foreignKey.RemoteTable.Columns where c.Name == nativeFK.ReferencedColumns(schemaReader).ToArray()[i] select c).Single();

						foreignKey.Columns.Add(new Tuple<Column,Column>(localColumn, foreignColumn));
					}
				}
			}

			return schema;
		}
	}
}