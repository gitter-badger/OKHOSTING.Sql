using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace OKHOSTING.Sql.ORM
{
	public abstract class MemberMap
	{
		/// <summary>
		/// The type map that contains this object
		/// </summary>
		public readonly TypeMap Type;
		
		/// <summary>
		/// The database column where this field will be stored
		/// </summary>
		public readonly Schema.Column Column;

		/// <summary>
		/// String representing the member (property or field) that is being mapped
		/// </summary>
		public readonly string Member;

		public Type ReturnType
		{
			get
			{
				var memberInfo = GetMembers().Last();

				if (memberInfo is FieldInfo)
				{
					return ((FieldInfo)memberInfo).FieldType;
				}
				else
				{
					return ((PropertyInfo)memberInfo).PropertyType;
				}
			}
		}

		public readonly List<Validators.IValidator> Validators = new List<Validators.IValidator>();

		protected MemberMap(TypeMap type, Schema.Column column, string member)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}

			if (column == null)
			{
				throw new ArgumentNullException("column");
			}

			if (string.IsNullOrWhiteSpace(member))
			{
				throw new ArgumentNullException("member");
			}

			if (column.Table != type.Table)
			{
				throw new ArgumentOutOfRangeException("column", column, "This column does not belong the the Table that TypeMap is mapped to");
			}

			//validate member expression
			GetMembers();

			Type = type;
			Column = column;
			Member = member;
		}

		public override string ToString()
		{
			return Member;
		}

		/// <summary>
		/// Returns a list of members represented in the member string
		/// </summary>
		/// <param name="memberPath"></param>
		/// <returns></returns>
		/// <remarks>
		/// http://www.java2s.com/Code/CSharp/Reflection/GetPropertyfromPropertypath.htm
		/// </remarks>
		public IEnumerable<MemberInfo> GetMembers()
		{
			string[] splittedMembers = Member.Split(new[] { '.' }, StringSplitOptions.None);

			Type memberType = Type.InnerType;
			MemberInfo memberInfo = memberType.GetProperty(splittedMembers[0]);

			if (memberInfo == null)
			{
				throw new ArgumentOutOfRangeException("Members", splittedMembers[0], "Type " + memberType + " does not contain a member with that name");
			}

			memberType = GetReturnType(memberInfo);

			yield return memberInfo;

			for (int x = 1; x < splittedMembers.Length; ++x)
			{
				memberInfo = memberType.GetProperty(splittedMembers[x]);

				if (memberInfo == null)
				{
					throw new ArgumentOutOfRangeException("Members", splittedMembers[x], "Type " + memberType + " does not contain a member with that name");
				}
				
				memberType = GetReturnType(memberInfo);

				yield return memberInfo;
			}
		}

		public IEnumerable<object> GetValues(object obj)
		{
			object result = obj;

			foreach (MemberInfo memberInfo in GetMembers())
			{
				if (result != null)
				{
					result = GetValue(memberInfo, result);
				}

				yield return result;
			}
		}

		public object GetValue(object obj)
		{
			return GetValues(obj).Last();
		}

		public void SetValue(object obj, object value)
		{
			MemberInfo finalMember = GetMembers().Last();
			SetValue(finalMember, obj, value);
		}

		#region Static

		public static Type GetReturnType(MemberInfo memberInfo)
		{
			if (memberInfo is FieldInfo)
			{
				return ((FieldInfo)memberInfo).FieldType;
			}
			else
			{
				return ((PropertyInfo)memberInfo).PropertyType;
			}
		}

		/// <summary>
		/// Returns the value of this DataMember
		/// </summary>
		/// <param name="obj">
		/// Object that will be examined
		/// </param>
		/// <returns>
		/// The value of the current DataMember in the specified DataObject
		/// </returns>
		public static object GetValue(MemberInfo memberInfo, object obj)
		{
			if (memberInfo is FieldInfo)
			{
				return ((FieldInfo) memberInfo).GetValue(obj);
			}
			else
			{
				return ((PropertyInfo) memberInfo).GetValue(obj);
			}
		}

		/// <summary>
		/// Sets the value for this DataValue
		/// </summary>
		/// <param name="obj">
		/// Object that will be changed
		/// </param>
		/// <param name="value">
		/// The value that will be set to the DataMember
		/// </param>
		public static void SetValue(MemberInfo memberInfo, object obj, object value)
		{
			if (memberInfo is FieldInfo)
			{
				((FieldInfo) memberInfo).SetValue(obj, value);
			}
			else
			{
				((PropertyInfo) memberInfo).SetValue(obj, value);
			}
		}

		public static bool IsMapped(TypeMap typeMap, string member)
		{
			return typeMap.Members.Where(c => c.Member.Equals(member)).Count() > 0;
		}

		#endregion
	}

	public class MemberMap<T> : MemberMap
	{
		/// <summary>
		/// The member expression that indicates where this field will be stored in runtime
		/// </summary>
		public readonly Expression<Func<T, object>> MemberExpression;

		public MemberMap(Schema.Column column, Expression<Func<T, object>> memberExpression): base(typeof(T), column, GetMemberString(memberExpression))
		{
		}

		#region Static

		public static string GetMemberString(System.Linq.Expressions.Expression<Func<T, object>> member)
		{
			if (member.Body is UnaryExpression)
			{
				UnaryExpression unex = (UnaryExpression) member.Body;
				if (unex.NodeType == ExpressionType.Convert)
				{
					Expression ex = unex.Operand;
					MemberExpression mex = (MemberExpression) ex;
					return mex.Member.Name;
				}
			}

			MemberExpression lambdaMemberExpression = (MemberExpression) member.Body;
			MemberExpression lambdaMemberExpressionOriginal = lambdaMemberExpression;

			string path = "";

			while (lambdaMemberExpression.Expression.NodeType == ExpressionType.MemberAccess)
			{
				var propInfo = lambdaMemberExpression.Expression.GetType().GetProperty("Member");
				var propValue = propInfo.GetValue(lambdaMemberExpression.Expression, null) as MemberInfo;

				path = propValue.Name + "." + path;

				lambdaMemberExpression = lambdaMemberExpression.Expression as MemberExpression;
			}

			return path + lambdaMemberExpressionOriginal.Member.Name;
		}

		public static bool IsMapped(System.Linq.Expressions.Expression<Func<T, object>> expression)
		{
			TypeMap<T> typeMapping = TypeMap.GetMap<T>();
			return typeMapping.Members.Where(c => c.MemberExpression.Equals(expression)).Count() > 0;
		}

		public static MemberMap<T> From(System.Linq.Expressions.Expression<Func<T, object>> expression)
		{
			TypeMap<T> typeMapping = TypeMap.GetMap<T>();

			return typeMapping.Members.Where(c => c.MemberExpression.Equals(expression)).Single();
		}

		public static implicit operator MemberMap<T>(System.Linq.Expressions.Expression<Func<T, object>> expression)
		{
			return From(expression);
		}

		public static implicit operator System.Linq.Expressions.Expression<Func<T, object>>(MemberMap<T> member)
		{
			return member.MemberExpression;
		}

		#endregion
	}
}