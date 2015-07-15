using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OKHOSTING.Sql.ORM.Operations
{
	/// <summary>
	/// A select SQL query
	/// </summary>
	public class Select
	{
		public DataType From { get; set; }
		public SelectLimit Limit { get; set; }
		public readonly List<SelectMember> Members = new List<SelectMember>();
		public readonly List<SelectJoin> Joins = new List<SelectJoin>();
		public readonly List<Filters.FilterBase> Where = new List<Filters.FilterBase>();
		public readonly List<OrderBy> OrderBy = new List<OrderBy>();

		public static Select Create<T>(params System.Linq.Expressions.Expression<Func<T, object>>[] members)
		{
			Select select = new Select();
			select.From = typeof(T);
			Random random = new Random();

			foreach (var member in members)
			{
				string memberString = DataMember<T>.GetMemberString(member);
				var nestedDataMembers = DataMember<T>.GetNestedDataMebers(select.From, memberString).ToList();

				foreach (DataMember dmember in nestedDataMembers)
				{
					bool isTheFirstOne = nestedDataMembers.IndexOf(dmember) == 0;
					bool isTheLastOne = nestedDataMembers.IndexOf(dmember) == nestedDataMembers.Count - 1;

					//if this is the first member, it must be of the current type or an inherited primary key
					//this is a member declared on the current datatype
					if (isTheFirstOne && isTheLastOne && dmember.Type == select.From)
					{
						//this is a native member of this dataType
						SelectMember sm = new SelectMember(dmember, dmember.Member.Replace('.', '_') + random.Next());
						select.Members.Add(sm);
					}
					//this is an inheritance foreign key
					else if (isTheFirstOne && dmember.Type.InnerType.IsSubclassOf(select.From.InnerType))
					{
						SelectJoin join = select.Joins.Where(j => j.Type == dmember.Type && j.Alias == dmember.Type.InnerType.Name + "_base").SingleOrDefault();

						if (join == null)
						{
							join = new SelectJoin();
							join.JoinType = Sql.Operations.SelectJoinType.Inner;
							join.Type = dmember.Type;
							join.Alias = dmember.Type.InnerType.Name + "_base";

							var childPK = select.From.PrimaryKey.ToList();
							var joinPK = join.Type.PrimaryKey.ToList();
							
							for (int i = 0; i < joinPK.Count; i++)
							{
								//DataMember pk in join.Type.PrimaryKey
								var filter = new Filters.MemberCompareFilter(childPK[i], joinPK[i]);
								join.On.Add(filter);
							}

							select.Joins.Add(join);
						}

						if (isTheLastOne)
						{
							//this is a native member of this dataType
							SelectMember sm = new SelectMember(dmember, dmember.Member.Replace('.', '_') + random.Next());
							join.Members.Add(sm);
						}
					}
					//this is a nested dataMember
					else
					{
						DataType container = isTheFirstOne? select.From : nestedDataMembers[nestedDataMembers.IndexOf(dmember) - 1].ReturnType;
						SelectJoin join = select.Joins.Where(j => j.Type.InnerType == dmember.ReturnType && j.Alias == dmember.Member.Replace('.', '_')).SingleOrDefault();

						if (join == null)
						{
							join = new SelectJoin();
							join.JoinType = Sql.Operations.SelectJoinType.Inner;
							join.Type = dmember.ReturnType;
							join.Alias = dmember.Member.Replace('.', '_');

							var joinPK = join.Type.PrimaryKey.ToList();

							for (int i = 0; i < joinPK.Count; i++)
							{
								var foreignPK = container.Members.Where(m => m.Member.StartsWith(dmember.Member)).ToList();

								//DataMember pk in join.Type.PrimaryKey
								var filter = new Filters.MemberCompareFilter(childPK[i], joinPK[i]);
								join.On.Add(filter);
							}
						}

						if (isTheLastOne)
						{
							//this is a native member of this dataType
							SelectMember sm = new SelectMember(dmember, dmember.Member.Replace('.', '_') + random.Next());
							join.Members.Add(sm);
						}
					}
				}
			}

			return select;
		}
	}
}