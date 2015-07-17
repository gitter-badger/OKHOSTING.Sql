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

		public static Select Create<T>(params System.Linq.Expressions.Expression<Func<T, object>>[] memberExpressions)
		{
			Select select = new Select();
			select.From = typeof(T);
			Random random = new Random();

			foreach (var expression in memberExpressions)
			{
				string dataMemberString = DataMember<T>.GetMemberString(expression);
				List<System.Reflection.MemberInfo> nestedMemberInfos = DataMember.GetMemberInfos(typeof(T), dataMemberString).ToList();

				for (int i = 0; i < nestedMemberInfos.Count; i++)
				{
					System.Reflection.MemberInfo memberInfo = nestedMemberInfos[i];
					bool isTheFirstOne = i == 0;
					bool isTheLastOne = i == nestedMemberInfos.Count - 1;
					string currentExpression = string.Empty;

					for (int y = 0; y <= i; y++)
					{
						currentExpression += '.' + nestedMemberInfos[y].Name;
					}
					
					currentExpression = currentExpression.Trim('.');

					//if this is a native datamember, just add a SelectMember
					if (DataMember.IsMapped(select.From, currentExpression))
					{
						DataMember dmember = DataMember.GetMap(select.From, currentExpression);

						//this is a native member of this dataType
						SelectMember sm = new SelectMember(dmember, dmember.Member.Replace('.', '_') + random.Next());
						select.Members.Add(sm);

						//finish iteration here
						continue;
					}

					//if this is a dataMember from a base type, create join for that relationship
					foreach (DataType parent in select.From.GetBaseDataTypes().Skip(1))
					{
						//if this is a native datamember, just add a SelectMember
						if (DataMember.IsMapped(parent, currentExpression))
						{
							DataMember dmember = DataMember.GetMap(parent, currentExpression);

							SelectJoin join = select.Joins.Where(j => j.Type == parent && j.Alias == parent.InnerType.Name + "_base").SingleOrDefault();

							if (join == null)
							{
								join = new SelectJoin();
								join.JoinType = Sql.Operations.SelectJoinType.Inner;
								join.Type = parent;
								join.Alias = parent.InnerType.Name + "_base";

								var childPK = select.From.PrimaryKey.ToList();
								var joinPK = join.Type.PrimaryKey.ToList();

								for (int y = 0; y < joinPK.Count; y++)
								{
									//DataMember pk in join.Type.PrimaryKey
									var filter = new Filters.MemberCompareFilter(childPK[y], joinPK[y]);
									join.On.Add(filter);
								}

								select.Joins.Add(join);
							}

							//this is a native member of this dataType
							SelectMember sm = new SelectMember(dmember, dmember.Member.Replace('.', '_') + random.Next());
							join.Members.Add(sm);

							//finish iteration here
							continue;
						}

						//this is not a native or inherited DataMember, so we must detect the nested members and create the respective joins
						if (DataType.IsMapped(DataMember.GetReturnType(memberInfo)))
						{
							DataType foregignDataType = DataMember.GetReturnType(memberInfo);
							DataType referencingDataType = isTheFirstOne ? parent : DataMember.GetReturnType(nestedMemberInfos[i - 1]);
							bool foreignKeyIsMapped = true;

							foreach (DataMember foreignKey in foregignDataType.PrimaryKey)
							{
								DataMember localKey = referencingDataType.Members.Where(m => m.Member == currentExpression + "." + foreignKey.Member).SingleOrDefault();

								if (localKey == null)
								{
									foreignKeyIsMapped = false;
								}
							}

							if (foreignKeyIsMapped)
							{
								SelectJoin foreignJoin = select.Joins.Where(j => j.Type == foregignDataType && j.Alias == currentExpression.Replace('.', '_')).SingleOrDefault();

								if (foreignJoin == null)
								{
									foreignJoin = new SelectJoin();
									foreignJoin.JoinType = Sql.Operations.SelectJoinType.Inner;
									foreignJoin.Type = foregignDataType;
									foreignJoin.Alias = currentExpression.Replace('.', '_');

									foreach (DataMember foreignKey in foregignDataType.PrimaryKey)
									{
										DataMember localKey = referencingDataType.Members.Where(m => m.Member == currentExpression + "." + foreignKey.Member).Single();

										var filter = new Filters.MemberCompareFilter(localKey, foreignKey);
										foreignJoin.On.Add(filter);
									}
								}

								if (isTheLastOne)
								{
									DataMember dmember = DataMember.GetMap(referencingDataType, memberInfo.Name);

									SelectMember sm = new SelectMember(dmember, currentExpression.Replace('.', '_') + random.Next());
									foreignJoin.Members.Add(sm);
								}
							}
						}
					}
				}
			}

			return select;
		}
	}
}