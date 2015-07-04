using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

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
				current = this.InnerType;

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

		public static bool IsMapped(Type type)
		{
			return DataTypes.Where(m => m.InnerType.Equals(type)).Count() > 0;
		}

		public static DataType GetMap(Type type)
		{
			return DataTypes.Where(m => m.InnerType.Equals(type)).Single();
		}
		
		public static implicit operator DataType(Type type)
		{
			return GetMap(type);
		}

		/// <summary>
		/// List of available type mappings, system-wide
		/// </summary>
		protected static readonly List<DataType> DataTypes = new List<DataType>();

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

			foreach (var memberInfo in type.GetMembers(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.DeclaredOnly))
			{
				if ((memberInfo is System.Reflection.FieldInfo || memberInfo is System.Reflection.PropertyInfo) && table.Columns.Where(c => c.Name == memberInfo.Name).Count() > 0)
				{
					DataMember member = new DataMember(dtype, table[memberInfo.Name], memberInfo.Name);
					dtype.Members.Add(member);
				}
			}

			return dtype;
		}

		public static IEnumerable<DataType> DefaultMap(IEnumerable<Type> types)
		{
			foreach (Type t in types)
			{
				DefaultMap(t);
				yield return t;
			}

			//create foreign keys now that all tables are created
			foreach (Type t in types)
			{
			}
		}
		
			/// <summary>
		/// Creates a new DataType, creating as well a new Table with all members of type as columns
		/// </summary>
		public static DataType DefaultMap(Type type)
		{
			Schema.Table table = new Schema.Table();
			table.Name = type.Name;

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

				//is this a primary key?
				bool isPrimaryKey = memberInfo.Name.ToString().ToLower() == "id" || memberInfo.GetCustomAttributes(typeof(System.ComponentModel.DataAnnotations.KeyAttribute), false).Length > 0;

				//ignore inherited members, except for primary keys
				if (!memberInfo.DeclaringType.Equals(type) && !isPrimaryKey)
				{
					continue;
				}

				Schema.Column column = new Schema.Column();
				column.Name = memberInfo.Name;
				column.Table = table;
				column.DbType = Sql.DataBase.Parse(DataMember.GetReturnType(memberInfo));
				column.IsNullable = !Validators.RequiredValidator.IsRequired(memberInfo);

				if (isPrimaryKey)
				{
					column.IsPrimaryKey = true;
					column.IsNullable = false;
				}

				if (column.IsString)
				{
					column.Length = Validators.StringLengthValidator.GetMaxLenght(memberInfo);
				}

				table.Columns.Add(column);
			}

			if (table.PrimaryKey.Count() < 1)
			{
				throw new ArgumentException("Could not define a primary key for Type " + typeof(T).FullName, "T");
			}

			return DefaultMap(type, table);
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