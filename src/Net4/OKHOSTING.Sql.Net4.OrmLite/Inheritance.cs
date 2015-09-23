using System;
using System.Collections.Generic;
using System.Linq;
using ServiceStack;
using ServiceStack.OrmLite;
using OKHOSTING.Core;

namespace OKHOSTING.Sql.Net4.OrmLite
{
	public class Inheritance
	{
		public readonly System.Data.IDbConnection Connection;

		public void Insert<TValue>(TValue obj)
		{
			var types = obj.GetType().GetAllParents();
			types.Reverse();

			var idProperty = obj.GetType().GetIdProperty();

			foreach(Type type in types)
			{
				Connection.InsertOnly<TValue>(obj, q => q.Insert(GetDefinedPropertiesAndKey(type)));

				if (type.BaseType == null && Attribute.IsDefined(idProperty, typeof(ServiceStack.DataAnnotations.AutoIncrementAttribute)))
				{
					var generatedId = Convert.ChangeType(Connection.LastInsertId(), idProperty.PropertyType);
					idProperty.SetValue(obj, generatedId);
				}
			}
		}

		public void Update<TValue>(TValue obj)
		{
			foreach (Type type in obj.GetType().GetAllParents())
			{
				Connection.UpdateOnly<TValue>(obj, q => q.Update(GetDefinedPropertiesAndKey(type)));
			}
		}

		public void Delete<TValue>(TValue obj)
		{
			foreach (Type type in obj.GetType().GetAllParents())
			{
				Connection.Delete<TValue>(obj);
			}
		}

		public void Select<TValue>()
		{
			//SqlExpression<TValue> join = Connection.Select<TValue>();

			//foreach (Type type in typeof(TValue).GetAllParents())
			//{
			//	join = join.Join(type, type, )
			//}
		}

		public void DropTable<TValue>(TValue obj)
		{
			foreach (Type type in obj.GetType().GetAllParents())
			{
				Connection.DropTable(type);
			}
		}

		public void CreateTable<TValue>(TValue obj)
		{
			var idProperty = obj.GetType().GetIdProperty();

			foreach (Type type in obj.GetType().GetAllParents())
			{
				Connection.CreateTable(true, type);
				
				//drop extra collumns
				foreach(var prop in type.GetProperties())
				{
					if (!prop.Equals(idProperty) && !prop.DeclaringType.Equals(type))
					{
						Connection.DropColumn(type, prop.Name);
					}
				}
			}
		}

		protected List<string> GetDefinedPropertiesAndKey(Type type)
		{
			List<string> result = new List<string>();

			foreach (var prop in type.GetProperties(System.Reflection.BindingFlags.DeclaredOnly))
			{
				result.Add(prop.Name);
			}

			var id = type.GetIdProperty();

			if (!result.Contains(id.Name))
			{
				result.Add(id.Name);
			}

			return result;
		}
	}
}