﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using OKHOSTING.Core.Data.Validation;

namespace OKHOSTING.Sql.ORM
{
	/// <summary>
	/// A Type that is mapped to a database Table
	/// </summary>
	public class DataType
	{
		public DataType()
		{
		}

		public DataType(Type innerType, Schema.Table table)
		{
			if (innerType == null)
			{
				throw new ArgumentNullException("innerType");
			}

			InnerType = innerType;

			if (table == null)
			{
				CreateTable();
			}
			else
			{
				Table = table;
			}
		}

		public DataType(Type innerType): this(innerType, null)
		{
		}

		#region Properties

		public readonly List<DataMember> DataMembers = new List<DataMember>();

		public readonly List<ValidatorBase> Validators = new List<ValidatorBase>();

		/// <summary>
		/// System.Type in wich this TypeMap<T> is created from
		/// </summary>
		public System.Type InnerType { get; set; }

		/// <summary>
		/// The table where objects of this Type will be stored
		/// </summary>
		public Schema.Table Table { get; set; }

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

		public DataMember this[string name]
		{
			get
			{
				return AllDataMembers.Where(m => m.Member.Expression == name).Single();
			}
		}

		/// <summary>
		/// Returns all DataMembers, including those inherited from base classes. 
		/// </summary>
		/// <remarks>
		/// Does not duplicate the primary key by omitting base classes primary keys
		/// </remarks>
		public IEnumerable<DataMember> AllDataMembers
		{
			get
			{
				foreach (DataType parent in BaseDataTypes)
				{
					foreach (DataMember dmember in parent.DataMembers)
					{
						//Do not duplicate the primary key by omitting base classes primary keys
						if (parent != this && dmember.Column.IsPrimaryKey)
						{
							continue;
						}

						yield return dmember;
					}
				}
			}
		}

		public IEnumerable<DataMember> PrimaryKey
		{
			get
			{
				return from m in DataMembers where m.Column.IsPrimaryKey select m;
			}
		}

		/// <summary>
		/// Returns the list of immediate members that are mapped to the database, including foreign keys.
		/// This does not contains inherited members except for the primary key
		/// </summary>
		public IEnumerable<System.Reflection.MemberInfo> MemberInfos
		{
			get
			{
				foreach (var member in GetMapableMembers(InnerType))
				{
					if (this.IsMapped(member.Name))
					{
						yield return member;
					}
					else if (IsMapped(MemberExpression.GetReturnType(member)) && IsForeignKey(member))
					{
						yield return member;
					}
				}
			}
		}

		/// <summary>
		/// Returns the list of immediate members that are mapped to the database, including foreign keys.
		/// This do contains all inherited members 
		/// </summary>
		public IEnumerable<System.Reflection.MemberInfo> AllMemberInfos
		{
			get
			{
				foreach (DataType parent in BaseDataTypes)
				{
					foreach (var member in parent.MemberInfos)
					{
						//Do not duplicate the primary key by omitting base classes primary keys
						if (parent != this && !DataMember.IsPrimaryKey(member))
						{
							continue;
						}

						yield return member;
					}
				}
			}
		}
		
		/// <summary>
		/// Returns all parent Types ordered from child to base
		/// </summary>
		public IEnumerable<DataType> BaseDataTypes
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
						yield return current;
					}

					//Getting the parent of the current object
					current = current.BaseType;
				}
			}
		}

		/// <summary>
		/// Searches for all TypeMaps inherited from this TypeMap 
		/// </summary>
		/// <returns>
		/// All TypeMap that directly inherit from the current TypeMap
		/// </returns>
		public IEnumerable<DataType> SubDataTypes
		{
			get
			{
				//Crossing all loaded DataTypes
				foreach (DataType dt in AllDataTypes)
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
		}

		/// <summary>
		/// Searches for all DataTypes inherited from this TypeMap in a recursive way
		/// </summary>
		/// <returns>
		/// All DataTypes that directly and indirectly inherit from the current TypeMap. 
		/// Returns the hole tree of subclasses.
		/// </returns>
		public IEnumerable<DataType> SubDataTypesRecursive
		{
			get
			{
				//Crossing all loaded DataTypes
				foreach (DataType tm in AllDataTypes)
				{
					//Validating if the dataType has a Base Class
					if (tm.BaseDataType != null)
					{
						//Validating if the base class of the TypeMap<T>
						//is this TypeMap<T>
						if (tm.BaseDataType.Equals(this))
						{
							yield return tm;

							foreach (var tm2 in tm.SubDataTypesRecursive)
							{
								yield return tm2;
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// Returns a list of members that are foreign keys of this datatype, across all DataTypes registered
		/// </summary>
		/// <returns></returns>
		public IEnumerable<System.Reflection.MemberInfo> InboundForeingKeys
		{
			get
			{
				foreach (DataType dtype in AllDataTypes)
				{
					foreach (System.Reflection.MemberInfo member in dtype.MemberInfos)
					{
						//Validating if the dataType has a Base Class
						if (MemberExpression.GetReturnType(member).Equals(InnerType) && dtype.IsForeignKey(member))
						{
							yield return member;
						}
					}
				}
			}
		}

		#endregion

		#region Methods

		public bool IsMapped(string member)
		{
			return DataMembers.Where(m => m.Member.Expression == member).Count() > 0;
		}

		public DataMember AddMember(string member)
		{
			return AddMember(member, null);
		}

		public DataMember AddMember(string member, Schema.Column column)
		{
			var genericDataMemberType = typeof(DataMember<>).MakeGenericType(InnerType);

			var constructor = genericDataMemberType.GetConstructor(
			  System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.CreateInstance | System.Reflection.BindingFlags.Instance,
			  null,
			  new[] { typeof(string), typeof(Schema.Column) },
			  null
			);

			DataMember genericDataMember = (DataMember) constructor.Invoke(new object[] { member, column });

			DataMembers.Add(genericDataMember);

			return genericDataMember;
		}

		/// <summary>
		/// Returns a string representation of this TypeMapping
		/// </summary>
		public override string ToString()
		{
			return InnerType.FullName;
		}

		/// <summary>
		/// Returns true if a member is properly mapped to be stored as a foreign key, with the primary key fully mapped
		/// </summary>
		public bool IsForeignKey(System.Reflection.MemberInfo member)
		{
			System.Type type = MemberExpression.GetReturnType(member);
			
			if (!DataType.IsMapped(type))
			{
				return false;
			}

			DataType dtype = type;

			foreach (var pk in dtype.PrimaryKey)
			{
				if (!this.IsMapped(member.Name + "." + pk.Member))
				{
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// Creates (in memory, not in DB) a new table for this DataType and creates columns for it's datamembers
		/// </summary>
		public void CreateTable()
		{
			Table = new Schema.Table(InnerType.Name);

			foreach (DataMember dm in DataMembers)
			{
				if (dm.Column == null)
				{
					dm.CreateColumn();
				}
			}
		}

		public virtual IEnumerable<ValidationError> Validate(object obj)
		{
			List<ValidationError> errors = new List<ValidationError>();

			foreach (var validator in Validators)
			{
				var error = validator.Validate(obj);

				if (error != null)
				{
					yield return error;
				}
			}
		}

		#endregion

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
			//Validating if the argument is a DataType
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

		public static readonly List<DataType> AllDataTypes = new List<DataType>();

		/// <summary>
		/// List of available type mappings, system-wide
		/// </summary>
		public static implicit operator DataType(Type type)
		{
			return GetMap(type);
		}

		public static bool IsMapped(Type type)
		{
			return AllDataTypes.Where(m => m.InnerType.Equals(type)).Count() > 0;
		}

		public static DataType GetMap(Type type)
		{
			return AllDataTypes.Where(m => m.InnerType.Equals(type)).Single();
		}

		public static IEnumerable<DataType> DefaultMap(params Tuple<Type, Schema.Table>[] types)
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
		public static DataType DefaultMap(Type type)
		{
			return DefaultMap(new Type[] { type }).First();
		}

		/// <summary>
		/// Creates a new DataType based on an existing Table, matching only members that have a column with the same name
		/// </summary>
		public static DataType DefaultMap(Type type, Schema.Table table)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			
			if (table == null)
			{
				throw new ArgumentNullException("table");
			}

			if (IsMapped(type))
			{
				throw new ArgumentOutOfRangeException("type", "This Types is already mapped");
			}

			DataType dtype = new DataType(type, table);

			foreach (var memberInfo in GetMapableMembers(type))
			{
				if (dtype.Table.Columns.Where(c => c.Name == memberInfo.Name).Count() > 0)
				{
					dtype.AddMember(memberInfo.Name, dtype.Table[memberInfo.Name]);
				}
			}

			return dtype;
		}

		/// <summary>
		/// Creates a list of new DataTypes, creating as well a list of new Tables with all members of type as columns
		/// </summary>
		public static IEnumerable<DataType> DefaultMap(IEnumerable<Type> types)
		{
			//we will store here all types that are actually persistent
			List<DataType> persistentTypes = new List<DataType>();

			//map primary keys first, so we allow to foreign keys and inheritance to be correctly mapped
			foreach (Type type in types)
			{
				//skip enums and interfaces
				if (type.IsEnum || type.IsInterface)
				{
					continue;
				}

				//ignore types with no primary key
				var pk = GetMapableMembers(type).Where(m => DataMember.IsPrimaryKey(m));

				if (pk.Count() == 0)
				{
					continue;
				}

				Schema.Table table = new Schema.Table(type.Name);
				DataType dtype = new DataType(type, table);
				AllDataTypes.Add(dtype);

				foreach (var memberInfo in pk)
				{
					//create datamember
					dtype.AddMember(memberInfo.Name);
				}

				persistentTypes.Add(dtype);
			}

			foreach (DataType dtype in persistentTypes)
			{
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
				foreach (var memberInfo in GetMapableMembers(dtype.InnerType).Where(m => !DataMember.IsPrimaryKey(m) && !MemberExpression.IsCollection(m)))
				{
					Type returnType = MemberExpression.GetReturnType(memberInfo);

					//its a persistent type, with it's own table, map primary keys
					if (IsMapped(returnType))
					{
						//we asume this datatype is already mapped along with it's primery key
						DataType returnDataType = returnType;

						Schema.ForeignKey foreignKey = new Schema.ForeignKey();
						foreignKey.Table = dtype.Table;
						foreignKey.RemoteTable = returnDataType.Table;
						foreignKey.Name = "FK_" + dtype.Table.Name + "_" + memberInfo.Name;

						foreach (DataMember pk in returnDataType.PrimaryKey.ToList())
						{
							Schema.Column column = new Schema.Column();
							column.Name = memberInfo.Name + "_" + pk.Member.Expression.Replace('.', '_');
							column.Table = dtype.Table;
							column.IsPrimaryKey = false;
							column.IsNullable = !RequiredValidator.IsRequired(memberInfo);
							column.DbType = Sql.DataBase.Parse(pk.Member.ReturnType);

							if (column.IsString)
							{
								column.Length = StringLengthValidator.GetMaxLenght(pk.Member.FinalMemberInfo);
							}

							dtype.Table.Columns.Add(column);
							foreignKey.Columns.Add(new Tuple<Schema.Column, Schema.Column>(column, pk.Column));

							//create datamember
							dtype.AddMember(memberInfo.Name + "." + pk.Member, column);
						}

						dtype.Table.ForeignKeys.Add(foreignKey);
					}
					//just map as a atomic value
					else
					{
						Schema.Column column = new Schema.Column();
						column.Name = memberInfo.Name;
						column.Table = dtype.Table;
						column.IsNullable = !Core.Data.Validation.RequiredValidator.IsRequired(memberInfo);
						column.IsPrimaryKey = false;

						//create datamember
						DataMember dmember = dtype.AddMember(memberInfo.Name, column);

						//is this a regular atomic value?
						if (Sql.DataBase.DbTypeMap.ContainsValue(returnType) && returnType != typeof(object))
						{
							column.DbType = Sql.DataBase.Parse(returnType);
						}
						else if (returnType.IsEnum)
						{
							column.DbType = DbType.Int32;
						}
						//this is an non-atomic object, but its not mapped as a DataType, so we serialize it as json
						else
						{
							column.DbType = DbType.String;
							dmember.Converter = new Conversions.Json<object>();
						}

						if (column.IsString)
						{
							column.Length = Core.Data.Validation.StringLengthValidator.GetMaxLenght(memberInfo);
						}

						dtype.Table.Columns.Add(column);
					}
				}

				yield return dtype;
			}
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

				//ignore readonly properties and fields, except for collections
				if (MemberExpression.IsReadOnly(memberInfo) && !MemberExpression.IsCollection(memberInfo))
				{
					continue;
				}

				//ignore inherited members, except for primary keys
				if (!memberInfo.DeclaringType.Equals(type) && !DataMember.IsPrimaryKey(memberInfo))
				{
					continue;
				}

				yield return memberInfo;
			}
		}

		#endregion
	}

	/// <summary>
	/// A Type that is mapped to a database Table
	/// </summary>
	public class DataType<T> : DataType
	{
		public DataType(): base(typeof(T))
		{
		}

		public DataType(Schema.Table table): base(typeof(T), table)
		{
		}

		public new IEnumerable<DataMember<T>> Members
		{
			get
			{
				foreach (DataMember dmember in base.DataMembers)
				{
					yield return (DataMember<T>) dmember;
				}
			}
		}

		public DataMember<T> AddMember(System.Linq.Expressions.Expression<Func<T, object>> expression)
		{
			return (DataMember<T>) AddMember(expression, null);
		}

		public DataMember<T> AddMember(System.Linq.Expressions.Expression<Func<T, object>> expression, Schema.Column column)
		{
			return (DataMember<T>) AddMember(MemberExpression<T>.GetMemberString(expression), column);
		}

		public DataMember<T> this[System.Linq.Expressions.Expression<Func<T, object>> expression]
		{
			get
			{
				return (DataMember<T>) this[MemberExpression<T>.GetMemberString(expression)];
			}
		}

		public bool IsMapped(System.Linq.Expressions.Expression<Func<T, object>> memberExpression)
		{
			return IsMapped(MemberExpression<T>.GetMemberString(memberExpression));
		}

		public virtual IEnumerable<ValidationError> Validate(T obj)
		{
			return Validate((object) obj);
		}

		#region Static

		public static DataType<T> GetMap()
		{
			var dtype = GetMap(typeof(T));

			if (dtype is DataType<T>)
			{
				return (DataType<T>) dtype;
			}
			else
			{
				//make generic
				return ToGeneric(dtype);
			}
		}

		public static DataType<T> ToGeneric(DataType dtype)
		{
			if (dtype is DataType<T>)
			{
				return (DataType<T>) dtype;
			}

			var genericDataTypeType = typeof(DataType<>).MakeGenericType(dtype.InnerType);
			var constructor = genericDataTypeType.GetConstructor(
			  System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.CreateInstance | System.Reflection.BindingFlags.Instance,
			  null,
			  new[] { typeof(Schema.Table) },
			  null
			);

			DataType genericDataType = (DataType) constructor.Invoke(new object[] { dtype.Table });

			foreach (var member in dtype.DataMembers)
			{
				if (member is DataMember<T>)
				{
					genericDataType.DataMembers.Add(member);
				}
				else
				{
					genericDataType.DataMembers.Add(DataMember<T>.ToGeneric(member));
				}
			}

			foreach (var val in dtype.Validators)
			{
				genericDataType.Validators.Add(val);
			}

			//replace in DataTypes
			var dtypeMapped = AllDataTypes.Where(dt => dt.InnerType == dtype.InnerType).SingleOrDefault();
			
			if (dtypeMapped != null)
			{
				AllDataTypes.Remove(dtypeMapped);
				AllDataTypes.Add(genericDataType);
			}

			return (DataType<T>) genericDataType;
		}

		#endregion
	}
}