using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Data;

namespace OKHOSTING.Sql.ORM
{
	/// <summary>
	/// A Type that is mapped to a database Table
	/// </summary>
	public class DataType
	{
		public DataType(Type innerType, Schema.Table table)
		{
			if (innerType == null)
			{
				throw new ArgumentNullException("innerType");
			}

			if (table == null)
			{
				throw new ArgumentNullException("table");
			}

			InnerType = innerType;
			Table = table;
		}

		public readonly List<DataMember> Members = new List<DataMember>();

		/// <summary>
		/// System.Type in wich this TypeMap<T> is created from
		/// </summary>
		public readonly Type InnerType;

		/// <summary>
		/// The table where objects of this Type will be stored
		/// </summary>
		public readonly Schema.Table Table;

		public IEnumerable<DataMember> PrimaryKey
		{
			get
			{
				return from m in Members where m.Column.IsPrimaryKey select m;
			}
		}

		public IEnumerable<DataMember> RegularValues
		{
			get
			{
				return from m in Members where !m.Column.IsPrimaryKey select m;
			}
		}

		public DataType BaseDataType
		{
			get
			{
				Type current;

				//Get all types in ascendent order (from base to child)
				current = this.InnerType.BaseType;

				while (current != null)
				{
					//see if this type is mapped
					if (IsMapped(current))
					{
						return current;
					}

					//Getting the parent of the current object
					current = current.BaseType;
				}

				return null;
			}
		}

		/// <summary>
		/// Returns a string representation of this TypeMapping
		/// </summary>
		public override string ToString()
		{
			return InnerType.FullName;
		}

		/// <summary>
		/// Returns all parent Types ordered from child to base
		/// </summary>
		public IEnumerable<DataType> GetBaseDataTypes()
		{
			Type current;

			//Get all types in ascendent order (from base to child)
			current = this.InnerType;

			while (current != null)
			{
				//see if this type is mapped
				if (IsMapped(current))
				{
					yield return current;
				}

				//Getting the parent of the current object
				current = current.BaseType;
			}
		}

		/// <summary>
		/// Searches for all TypeMaps inherited from this TypeMap 
		/// </summary>
		/// <returns>
		/// All TypeMap that directly inherit from the current TypeMap
		/// </returns>
		public IEnumerable<DataType> GetSubDataTypes()
		{
			//Crossing all loaded DataTypes
			foreach (DataType dt in DataTypes)
			{
				//Validating if the dataType has a Base Class
				if (dt.BaseDataType != null)
				{
					//Validating if the base class of the TypeMap<T>
					//is this TypeMap<T>
					if (dt.BaseDataType.Equals(this))
					{
						yield return dt;
					}
				}
			}
		}

		/// <summary>
		/// Searches for all DataTypes inherited from this TypeMap<T> in a recursive way
		/// </summary>
		/// <returns>
		/// All DataTypes that directly and indirectly inherit from the current TypeMap<T>. 
		/// Returns the hole tree of subclasses.
		/// </returns>
		public IEnumerable<DataType> GetSubDataTypesRecursive()
		{
			//Crossing all loaded DataTypes
			foreach (DataType tm in DataTypes)
			{
				//Validating if the dataType has a Base Class
				if (tm.BaseDataType != null)
				{
					//Validating if the base class of the TypeMap<T>
					//is this TypeMap<T>
					if (tm.BaseDataType.Equals(this))
					{
						yield return tm;

						foreach (var tm2 in tm.GetSubDataTypesRecursive())
						{
							yield return tm2;
						}
					}
				}
			}
		}

		#region Equality

		/// <summary>
		/// Compare this TypeMap<T>'s instance with another to see if they are the same
		/// </summary>
		public bool Equals(DataType typeMap)
		{
			//Validating if the argument is null
			if (typeMap == null) throw new ArgumentNullException("typeMap");

			//Comparing the InnerType types 
			return this.InnerType.Equals(typeMap.InnerType);
		}

		/// <summary>
		/// Compare this Type's instance with another to see if they are the same
		/// </summary>
		public bool Equals(Type type)
		{
			//Validating if the argument is null
			if (type == null) throw new ArgumentNullException("type");

			//Comparing the InnerType types 
			return this.InnerType.Equals(type);
		}

		/// <summary>
		/// Compare this Type's instance with another to see if they are the same
		/// </summary>
		public override bool Equals(object obj)
		{
			//Validating if the argument is null
			if (obj == null) throw new ArgumentNullException("obj");

			//Validating if the argument is a System.Type
			if (obj is Type)
			{
				return this.Equals((Type) obj);
			}
			//Validating if the argument is a OKHOSTING.Softosis.TypeMap<T>
			else if (obj is DataType)
			{
				return this.Equals((DataType) obj);
			}
			else
			{
				//The object is not a TypeMap<T> and is not a System.Type
				return false;
			}
		}

		/// <summary>
		/// Serves as a hash function for DataTypes
		/// </summary>
		/// <remarks>Returns the InnerType.GetHashCode() value</remarks>
		public override int GetHashCode()
		{
			return InnerType.GetHashCode();
		}

		/// <summary>
		/// Determines whether an instance of the current 
		/// TypeMap<T> is assignable from another TypeMap<T>
		/// </summary>
		public bool IsAssignableFrom(DataType typeMap)
		{
			//Validating if the argument is null
			if (typeMap == null) throw new ArgumentNullException("typeMap");

			//Validating...
			return this.InnerType.IsAssignableFrom(typeMap.InnerType);
		}

		#endregion
		
		#region Static

		/// <summary>
		/// List of available type mappings, system-wide
		/// </summary>
		protected static readonly List<DataType> DataTypes = new List<DataType>();

		public static implicit operator DataType(Type type)
		{
			return GetMap(type);
		}

		public static bool IsMapped(Type type)
		{
			return DataTypes.Where(m => m.InnerType.Equals(type)).Count() > 0;
		}

		public static DataType GetMap(Type type)
		{
			return DataTypes.Where(m => m.InnerType.Equals(type)).Single();
		}

		/// <summary>
		/// Returns a collection of members that are mapable, 
		/// meaning they are fields or properties, public, non read-only, and non-static
		/// </summary>
		public static IEnumerable<System.Reflection.MemberInfo> GetMapableMembers(Type type)
		{
			foreach (var memberInfo in type.GetMembers(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public))
			{
				//ignore methods, events and other members
				if (!(memberInfo is System.Reflection.FieldInfo || memberInfo is System.Reflection.PropertyInfo))
				{
					continue;
				}

				//ignore readonly properties
				if (memberInfo is System.Reflection.PropertyInfo && ((System.Reflection.PropertyInfo)memberInfo).SetMethod == null)
				{
					continue;
				}

				//ignore readonly fields
				if (memberInfo is System.Reflection.FieldInfo && ((System.Reflection.FieldInfo)memberInfo).IsInitOnly)
				{
					continue;
				}

				//ignore inherited members, except for primary keys
				if (!memberInfo.DeclaringType.Equals(type) && !IsPrimaryKey(memberInfo))
				{
					continue;
				}

				yield return memberInfo;
			}
		}

		public static bool IsPrimaryKey(System.Reflection.MemberInfo memberInfo)
		{
			return memberInfo.Name.ToString().ToLower() == "id" || memberInfo.GetCustomAttributes(typeof(System.ComponentModel.DataAnnotations.KeyAttribute), false).Length > 0;
		}

		public static IEnumerable<DataType> DefaultMap(IEnumerable<Tuple<Type, Schema.Table>> types)
		{
			foreach (var tuple in types)
			{
				DefaultMap(tuple.Item1, tuple.Item2);
				yield return tuple.Item1;
			}
		}

		/// <summary>
		/// Creates a new DataType based on an existing Table, matching only members that have a column with the same name
		/// </summary>
		public static DataType DefaultMap(Type type, Schema.Table table)
		{
			if (IsMapped(type))
			{
				throw new ArgumentOutOfRangeException("type", "This Types is already mapped");
			}

			DataType dtype = new DataType(type, table);
			DataTypes.Add(dtype);

			foreach (var memberInfo in GetMapableMembers(type))
			{
				if (table.Columns.Where(c => c.Name == memberInfo.Name).Count() > 0)
				{
					DataMember member = new DataMember(dtype, table[memberInfo.Name], memberInfo.Name);
					dtype.Members.Add(member);
				}
			}

			return dtype;
		}

		/// <summary>
		/// Creates a list of new DataTypes, creating as well a list of new Tables with all members of type as columns
		/// </summary>
		public static IEnumerable<DataType> DefaultMap(IEnumerable<Type> types)
		{
			//map primary keys first, so we allow to foreign keys and inheritance to be correctly mapped
			foreach (Type type in types)
			{
				Schema.Table table = new Schema.Table(type.Name);
				DataType dtype = new DataType(type, table);
				DataTypes.Add(dtype);

				foreach (var memberInfo in GetMapableMembers(type).Where(m => IsPrimaryKey(m)))
				{
					Schema.Column column = new Schema.Column();
					column.Name = memberInfo.Name;
					column.Table = table;
					column.IsPrimaryKey = true;
					column.IsNullable = false;
					column.DbType = Sql.DataBase.Parse(DataMember.GetReturnType(memberInfo)); //only atomic values supported at the moment

					if (column.IsString)
					{
						column.Length = Validators.StringLengthValidator.GetMaxLenght(memberInfo);
					}

					//only autoincrement the base table
					if (dtype.BaseDataType == null && column.IsIntegral)
					{
						column.IsAutoNumber = true;
					}

					table.Columns.Add(column);

					//create datamember
					DataMember dmember = new DataMember(type, column, memberInfo.Name);
					dtype.Members.Add(dmember);
				}
			}

			foreach (Type type in types)
			{
				DataType dtype = type;

				//create inheritance foreign keys
				if (dtype.BaseDataType != null)
				{
					Schema.ForeignKey foreignKey = new Schema.ForeignKey();
					foreignKey.Table = dtype.Table;
					foreignKey.RemoteTable = dtype.BaseDataType.Table;
					foreignKey.Name = "FK_" + dtype.Table.Name + "_" + dtype.BaseDataType.Table.Name;

					//we asume that primary keys on parent and child tables have the same number and order of related columns
					for (int i = 0; i < dtype.PrimaryKey.Count(); i++)
					{
						DataMember pk = dtype.PrimaryKey.ToArray()[i];
						DataMember basePk = dtype.BaseDataType.PrimaryKey.ToArray()[i];

						foreignKey.Columns.Add(new Tuple<Schema.Column, Schema.Column>(pk.Column, basePk.Column));
					}

					dtype.Table.ForeignKeys.Add(foreignKey);
				}

				//map non primary key members now
				foreach (var memberInfo in GetMapableMembers(type).Where(m => !IsPrimaryKey(m)))
				{
					Type returnType = DataMember.GetReturnType(memberInfo);

					//its a persistent type, with it's own table, map primary keys
					if (IsMapped(returnType))
					{
						//we asume this datatype is already mapped along with it's primery key
						DataType returnDataType = returnType;

						Schema.ForeignKey foreignKey = new Schema.ForeignKey();
						foreignKey.Table = dtype.Table;
						foreignKey.RemoteTable = returnDataType.Table;
						foreignKey.Name = "FK_" + dtype.Table.Name + "_" + memberInfo.Name;

						foreach (DataMember pk in returnDataType.PrimaryKey)
						{
							Schema.Column column = new Schema.Column();
							column.Name = memberInfo.Name + "_" + pk.Member.Replace('.', '_');
							column.Table = dtype.Table;
							column.IsPrimaryKey = false;
							column.IsNullable = !Validators.RequiredValidator.IsRequired(memberInfo);
							column.DbType = Sql.DataBase.Parse(pk.ReturnType);

							if (column.IsString)
							{
								column.Length = Validators.StringLengthValidator.GetMaxLenght(pk.FinalMemberInfo);
							}

							dtype.Table.Columns.Add(column);
							foreignKey.Columns.Add(new Tuple<Schema.Column, Schema.Column>(column, pk.Column));
							
							//create datamember
							DataMember dmember = new DataMember(type, column, memberInfo.Name + "." + pk.Member);
							dtype.Members.Add(dmember);
						}

						dtype.Table.ForeignKeys.Add(foreignKey);
					}
					//just map as a atomic value
					else
					{
						Schema.Column column = new Schema.Column();
						column.Name = memberInfo.Name;
						column.Table = dtype.Table;
						column.IsNullable = !Validators.RequiredValidator.IsRequired(memberInfo);
						column.IsPrimaryKey = false;

						//create datamember
						DataMember dmember = new DataMember(type, column, memberInfo.Name);
						dtype.Members.Add(dmember);

						//is this a regular atomic value?
						if (Sql.DataBase.DbTypeMap.ContainsValue(returnType) && returnType != typeof(object))
						{
							column.DbType = Sql.DataBase.Parse(returnType);
						}
						//this is an non-atomic object, but its not mapped as a DataType, so we serialize it as json
						else
						{
							column.DbType = DbType.String;
							dmember.Converter = new Converters.Json(returnType);
						}

						if (column.IsString)
						{
							column.Length = Validators.StringLengthValidator.GetMaxLenght(memberInfo);
						}

						dtype.Table.Columns.Add(column);
					}
				}

				yield return dtype;
			}
		}
		
		#endregion
	}

	/// <summary>
	/// A Type that is mapped to a database Table
	/// </summary>
	public class DataType<T> : DataType
	{
		public DataType(Schema.Table table): base(typeof(T), table)
		{
			
		}

		public new readonly List<DataMember<T>> Members = new List<DataMember<T>>();

		public static DataType<T> GetMap()
		{
			return (DataType<T>) GetMap(typeof(T));
		}

		public static implicit operator DataType<T>(Type type)
		{
			return (DataType<T>) GetMap(type);
		}
	}
}