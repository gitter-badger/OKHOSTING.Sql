using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace OKHOSTING.Sql.ORM
{
	public class DataMember
	{
		public DataMember(DataType type, Schema.Column column, string member)
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
			var x = MemberInfos;

			Type = type;
			Column = column;
			Member = member;
		}

		/// <summary>
		/// The type map that contains this object
		/// </summary>
		public readonly DataType Type;
		
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
				var memberInfo = FinalMemberInfo;

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
		public IEnumerable<MemberInfo> MemberInfos
		{
			get
			{
				string[] splittedMembers = Member.Split(new[] { '.' }, StringSplitOptions.None);

				Type memberType = Type.InnerType;
				
				for (int x = 0; x < splittedMembers.Length; x++)
				{
					MemberInfo memberInfo = memberType.GetProperty(splittedMembers[x]);

					if (memberInfo == null)
					{
						memberInfo = memberType.GetField(splittedMembers[x]);
					
						if (memberInfo == null)
						{
							throw new ArgumentOutOfRangeException("Members", splittedMembers[x], "Type " + memberType + " does not contain a member with that name");
						}
					}
				
					memberType = GetReturnType(memberInfo);

					yield return memberInfo;
				}
			}
		}

		public MemberInfo FinalMemberInfo
		{
			get
			{
				return MemberInfos.Last();
			}
		}

		public IEnumerable<object> GetValues(object obj)
		{
			object result = obj;

			foreach (MemberInfo memberInfo in MemberInfos)
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
			var allMembers = MemberInfos.ToList();
			var allValues = GetValues(obj).ToList();

			//ensure all nested members are not null
			for(int i= 0; i < allValues.Count(); i++)
			{
				object val = allValues[i];
				MemberInfo member = allMembers[i];
				
				if (val == null)
				{
					object container = (i == 0)? obj : allValues[i -1];
					object newValue = Activator.CreateInstance(GetReturnType(member));

					SetValue(member, container, newValue);
				}
			}

			SetValue(FinalMemberInfo, obj, value);
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

		public static bool IsMapped(DataType typeMap, string member)
		{
			return typeMap.Members.Where(c => c.Member.Equals(member)).Count() > 0;
		}

		#endregion
	}

	public class DataMember<T> : DataMember
	{
		public DataMember(Schema.Column column, string member): base(typeof(T), column, member)
		{
		}

		public DataMember(Schema.Column column, Expression<Func<T, object>> memberExpression): base(typeof(T), column, GetMemberString(memberExpression))
		{
		}

		#region Static

		protected static string GetMemberString(System.Linq.Expressions.Expression<Func<T, object>> member)
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
			DataType<T> typeMapping = DataType<T>.GetMap();
			string member = GetMemberString(expression);

			return typeMapping.Members.Where(c => c.Member.Equals(member)).Count() > 0;
		}

		public static DataMember<T> GetMap(System.Linq.Expressions.Expression<Func<T, object>> expression)
		{
			DataType<T> typeMapping = DataType<T>.GetMap();
			string member = GetMemberString(expression);

			return typeMapping.Members.Where(c => c.Member.Equals(member)).Single();
		}

		public static implicit operator DataMember<T>(System.Linq.Expressions.Expression<Func<T, object>> expression)
		{
			return GetMap(expression);
		}

		#endregion
	}
}