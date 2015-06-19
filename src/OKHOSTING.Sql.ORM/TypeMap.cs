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
	public abstract class TypeMap
	{
		protected TypeMap(Type innerType, Schema.Table table)
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

		public readonly List<MemberMap> Members = new List<MemberMap>();

		/// <summary>
		/// System.Type in wich this TypeMap<T> is created from
		/// </summary>
		public readonly Type InnerType;

		/// <summary>
		/// The table where objects of this Type will be stored
		/// </summary>
		public readonly Schema.Table Table;

		public IEnumerable<MemberMap> PrimaryKey
		{
			get
			{
				return from m in Members where m.Column.IsPrimaryKey select m;
			}
		}

		public IEnumerable<MemberMap> RegularValues
		{
			get
			{
				return from m in Members where !m.Column.IsPrimaryKey select m;
			}
		}

		public TypeMap BaseTypeMap
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
		/// Returns all parent Types ordered from base to child
		/// </summary>
		public IEnumerable<TypeMap> GetBaseTypeMaps()
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
		public IEnumerable<TypeMap> GetSubClassMaps()
		{
			//Crossing all loaded DataTypes
			foreach (TypeMap dt in Configuration.TypeMaps)
			{
				//Validating if the dataType has a Base Class
				if (dt.BaseTypeMap != null)
				{
					//Validating if the base class of the TypeMap<T>
					//is this TypeMap<T>
					if (dt.BaseTypeMap.Equals(this))
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
		public IEnumerable<TypeMap> GetSubClassMapsRecursive()
		{
			//Crossing all loaded DataTypes
			foreach (TypeMap tm in Configuration.TypeMaps)
			{
				//Validating if the dataType has a Base Class
				if (tm.BaseTypeMap != null)
				{
					//Validating if the base class of the TypeMap<T>
					//is this TypeMap<T>
					if (tm.BaseTypeMap.Equals(this))
					{
						yield return tm;

						foreach (var tm2 in tm.GetSubClassMapsRecursive())
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
		public bool Equals(TypeMap typeMap)
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
			else if (obj is TypeMap)
			{
				return this.Equals((TypeMap) obj);
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
		public bool IsAssignableFrom(TypeMap typeMap)
		{
			//Validating if the argument is null
			if (typeMap == null) throw new ArgumentNullException("typeMap");

			//Validating...
			return this.InnerType.IsAssignableFrom(typeMap.InnerType);
		}

		#endregion
		
		#region Events
		
		/// <summary>
		/// Delegate for BeforeSelect event
		/// </summary>
		public delegate void BeforeSelectEventHandler(TypeMap sender, SelectEventArgs e);

		/// <summary>
		/// Delegate for AfterSelect event
		/// </summary>
		public delegate void AfterSelectEventHandler(TypeMap sender, SelectEventArgs e);

		/// <summary>
		/// Delegate for BeforeSelectGroup event
		/// </summary>
		public delegate void BeforeSelectGroupEventHandler(TypeMap sender, SelectGroupEventArgs e);

		/// <summary>
		/// Delegate for AfterSelectGroup event
		/// </summary>
		public delegate void AfterSelectGroupEventHandler(TypeMap sender, SelectGroupEventArgs e);

		/// <summary>
		/// Delegate for insert, delete, and update operation events
		/// </summary>
		public delegate void OperationEventHandler(TypeMap sender, DataBaseOperationEventArgs e);

		/// <summary>
		/// Occurs before a Select operation is performed
		/// </summary>
		public event BeforeSelectEventHandler BeforeSelect;

		/// <summary>
		/// Occurs after a Select operation is performed
		/// </summary>
		public event AfterSelectEventHandler AfterSelect;

		/// <summary>
		/// Occurs before a Select Group operation is performed
		/// </summary>
		public event BeforeSelectGroupEventHandler BeforeSelectGroup;

		/// <summary>
		/// Occurs after a Select Group operation is performed
		/// </summary>
		public event AfterSelectGroupEventHandler AfterSelectGroup;

		/// <summary>
		/// Occurs before a Insert operation is performed
		/// </summary>
		public event OperationEventHandler BeforeInsert;

		/// <summary>
		/// Occurs after a Insert operation is performed
		/// </summary>
		public event OperationEventHandler AfterInsert;

		/// <summary>
		/// Occurs before a Update operation is performed
		/// </summary>
		public event OperationEventHandler BeforeUpdate;

		/// <summary>
		/// Occurs after a Update operation is performed
		/// </summary>
		public event OperationEventHandler AfterUpdate;

		/// <summary>
		/// Occurs before a Delete operation is performed
		/// </summary>
		public event OperationEventHandler BeforeDelete;

		/// <summary>
		/// Occurs after a Delete operation is performed
		/// </summary>
		public event OperationEventHandler AfterDelete;

		/// <summary>
		/// Raises the BeforeSelect event
		/// </summary>
		internal void OnBeforeSelect(SelectEventArgs e)
		{
			//Raise the TypeMap event
			if (BeforeSelect != null) BeforeSelect(this, e);

			//Raise the BaseTypeMap event
			if (BaseTypeMap != null) BaseTypeMap.OnBeforeSelect(e);
		}

		/// <summary>
		/// Raises the AfterSelect event
		/// </summary>
		internal void OnAfterSelect(SelectEventArgs e)
		{
			//Raise the TypeMap event
			if (AfterSelect != null) AfterSelect(this, e);

			//Raise the BaseTypeMap event
			if (BaseTypeMap != null) BaseTypeMap.OnAfterSelect(e);
		}

		/// <summary>
		/// Raises the BeforeSelectGroup event
		/// </summary>
		internal void OnBeforeSelectGroup(SelectGroupEventArgs e)
		{
			//Raise the TypeMap event
			if (BeforeSelectGroup != null) BeforeSelectGroup(this, e);

			//Raise the BaseTypeMap event
			if (BaseTypeMap != null) BaseTypeMap.OnBeforeSelectGroup(e);
		}

		/// <summary>
		/// Raises the AfterSelect event
		/// </summary>
		internal void OnAfterSelectGroup(SelectGroupEventArgs e)
		{
			//Raise the TypeMap event
			if (AfterSelectGroup != null) AfterSelectGroup(this, e);

			//Raise the BaseTypeMap event
			if (BaseTypeMap != null) BaseTypeMap.OnAfterSelectGroup(e);
		}

		/// <summary>
		/// Raises the BeforeInsert event
		/// </summary>
		internal void OnBeforeInsert(DataBaseOperationEventArgs e)
		{
			//Raise the TypeMap event
			if (BeforeInsert != null) BeforeInsert(this, e);

			//Raise the BaseTypeMap event
			if (BaseTypeMap != null) BaseTypeMap.OnBeforeInsert(e);
		}

		/// <summary>
		/// Raises the AfterInsert event
		/// </summary>
		internal void OnAfterInsert(DataBaseOperationEventArgs e)
		{
			//Raise the TypeMap event
			if (AfterInsert != null) AfterInsert(this, e);

			//Raise the BaseTypeMap event
			if (BaseTypeMap != null) BaseTypeMap.OnAfterInsert(e);
		}

		/// <summary>
		/// Raises the BeforeUpdate event
		/// </summary>
		internal void OnBeforeUpdate(DataBaseOperationEventArgs e)
		{
			//Raise the TypeMap event
			if (BeforeUpdate != null) BeforeUpdate(this, e);

			//Raise the BaseTypeMap event
			if (BaseTypeMap != null) BaseTypeMap.OnBeforeUpdate(e);
		}

		/// <summary>
		/// Raises the AfterUpdate event
		/// </summary>
		internal void OnAfterUpdate(DataBaseOperationEventArgs e)
		{
			//Raise the TypeMap event
			if (AfterUpdate != null) AfterUpdate(this, e);

			//Raise the BaseTypeMap event
			if (BaseTypeMap != null) BaseTypeMap.OnAfterUpdate(e);
		}

		/// <summary>
		/// Raises the BeforeDelete event
		/// </summary>
		internal void OnBeforeDelete(DataBaseOperationEventArgs e)
		{
			//Raise the TypeMap event
			if (BeforeDelete != null) BeforeDelete(this, e);

			//Raise the BaseTypeMap event
			if (BaseTypeMap != null) BaseTypeMap.OnBeforeDelete(e);
		}

		/// <summary>
		/// Raises the AfterDelete event
		/// </summary>
		internal void OnAfterDelete(DataBaseOperationEventArgs e)
		{
			//Raise the TypeMap event
			if (AfterDelete != null) AfterDelete(this, e);

			//Raise the BaseTypeMap event
			if (BaseTypeMap != null) BaseTypeMap.OnAfterDelete(e);
		}

		#endregion

		#region Static


		public static bool IsMapped(Type type)
		{
			return Configuration.TypeMaps.Where(m => m.InnerType.Equals(type)).Count() > 0;
		}

		public static TypeMap GetMap(Type type)
		{
			return Configuration.TypeMaps.Where(m => m.InnerType.Equals(type)).Single();
		}
		
		public static implicit operator TypeMap(Type type)
		{
			return GetMap(type);
		}

		#endregion
	}

	/// <summary>
	/// A Type that is mapped to a database Table
	/// </summary>
	public class TypeMap<T> : TypeMap
	{
		public TypeMap(Schema.Table table): base(typeof(T), table)
		{
			
		}

		public new readonly List<MemberMap<T>> Members = new List<MemberMap<T>>();

		public static TypeMap<T> GetMap()
		{
			return (TypeMap<T>) GetMap(typeof(T));
		}

		public static implicit operator TypeMap<T>(Type type)
		{
			return (TypeMap<T>) GetMap(type);
		}
	}
}