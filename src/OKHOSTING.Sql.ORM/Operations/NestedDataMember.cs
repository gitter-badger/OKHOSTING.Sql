using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace OKHOSTING.Sql.ORM.Operations
{
	/// <summary>
	/// Representa an expression of a datamember that can have nested memebers, pe>
	/// Person.Address.Suburb, Person.Address.Country.Name
	/// </summary>
	/// <remarks>
	/// Usefull to indicate members affected byu an operation, even if they belong to nested properties
	/// </remarks>
	public class NestedDataMember
	{
		public readonly DataType DataType;
		public readonly string Expression;

		public NestedDataMember(DataType dtype, string expression)
		{
			if (dtype == null)
			{
				throw new ArgumentNullException("dtype");
			}

			if (string.IsNullOrWhiteSpace(expression))
			{
				throw new ArgumentNullException("expression");
			}

			DataType = dtype;
			Expression = expression;
		}

		public IEnumerable<DataMember> DataMembers
		{
			get
			{
				DataType dtype = DataType;
				string[] splitted = Expression.Split('.');

				for (int x = 0; x < splitted.Length; )
				{
					string merged = string.Empty;
					bool found = false;

					for (int y = x; y < splitted.Length; y++)
					{
						merged = merged + '.' + splitted[y];
						merged = merged.Trim('.');

						DataMember member = dtype.Members.Where(m => m.Member == merged).SingleOrDefault();

						if (member != null)
						{
							yield return member;
							dtype = member.ReturnType;
							x++;
							found = true;
							break;
						}
					}

					if (!found)
					{
						throw new ArgumentOutOfRangeException("Expression", Expression, "Cant find a sequence of DataMembers in expression");
					}
				}
			}
		}
	}

	/// <summary>
	/// Representa an expression of a datamember that can have nested memebers, pe>
	/// Person.Address.Suburb, Person.Address.Country.Name
	/// </summary>
	/// <remarks>
	/// Usefull to indicate members affected byu an operation, even if they belong to nested properties
	/// </remarks>
	public class NestedDataMember<T>: NestedDataMember
	{
		public readonly Expression<Func<T, object>> Expression;

		public NestedDataMember(Expression<Func<T, object>> expression): base(typeof(T), DataMember<T>.GetMemberString(expression))
		{
			Expression = expression;
		}
	}
}