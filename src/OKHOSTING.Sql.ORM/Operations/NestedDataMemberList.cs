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
	public class NestedDataMemberList<T> : IEnumerable<NestedDataMember>
	{
		protected readonly List<NestedDataMember> DataMembers = new List<NestedDataMember>();

		public void Add(string dataMember)
		{
			DataMembers.Add(new NestedDataMember(typeof(T), dataMember));
		}

		public void Add(Expression<Func<T, object>> dataMember)
		{
			DataMembers.Add(new NestedDataMember<T>(dataMember));
		}

		public void Add(DataMember dataMember)
		{
			DataMembers.Add(new NestedDataMember(dataMember.Type, dataMember.Member));
		}

		public void Add(NestedDataMember dataMember)
		{
			DataMembers.Add(dataMember);
		}

		public void Add(NestedDataMember<T> dataMember)
		{
			DataMembers.Add(dataMember);
		}

		public void AddRange(params Expression<Func<T, object>>[] dataMembers)
		{
			foreach (var dm in dataMembers)
			{
				DataMembers.Add(new NestedDataMember<T>(dm));
			}
		}

		public void AddRange(params string[] dataMembers)
		{
			foreach (var dm in dataMembers)
			{
				DataMembers.Add(new NestedDataMember(typeof(T), dm));
			}
		}

		public IEnumerator<NestedDataMember> GetEnumerator()
		{
			foreach (var dm in DataMembers)
			{
				yield return dm;
			}
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}