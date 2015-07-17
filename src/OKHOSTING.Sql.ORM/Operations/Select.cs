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
	}

	public class Select<T> : Select
	{
		public Select()
		{
			From = typeof(T);
		}

		public void AddMembers(params System.Linq.Expressions.Expression<Func<T, object>>[] memberExpressions)
		{
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
					if (DataMember.IsMapped(From, currentExpression))
					{
						DataMember dmember = DataMember.GetMap(From, currentExpression);

						//this is a native member of this dataType
						SelectMember sm = new SelectMember(dmember, dmember.Member.Replace('.', '_'));
						Members.Add(sm);

						//finish iteration here
						continue;
					}

					//if this is a dataMember from a base type, create join for that relationship
					foreach (DataType parent in From.GetBaseDataTypes())
					{
						//if this is a native datamember, just add a SelectMember
						if (DataMember.IsMapped(parent, currentExpression))
						{
							DataMember dmember = DataMember.GetMap(parent, currentExpression);

							SelectJoin join = Joins.Where(j => j.Type == parent && j.Alias == parent.InnerType.Name + "_base").SingleOrDefault();

							if (join == null)
							{
								join = new SelectJoin();
								join.JoinType = Sql.Operations.SelectJoinType.Inner;
								join.Type = parent;
								join.Alias = parent.InnerType.Name + "_base";

								var childPK = From.PrimaryKey.ToList();
								var joinPK = join.Type.PrimaryKey.ToList();

								for (int y = 0; y < joinPK.Count; y++)
								{
									//DataMember pk in join.Type.PrimaryKey
									var filter = new Filters.MemberCompareFilter()
									{
										 Member = childPK[y],
										 MemberToCompare = joinPK[y],
										 MemberToCompareTypeAlias = join.Alias,
									};

									join.On.Add(filter);
								}

								Joins.Add(join);
							}

							//this is a native member of this dataType
							SelectMember sm = new SelectMember(dmember, dmember.Member.Replace('.', '_'));
							join.Members.Add(sm);

							//finish iteration here
							break;
						}

						DataType referencingDataType = isTheFirstOne ? parent : DataMember.GetReturnType(nestedMemberInfos[i - 1]);

						//this is not a native or inherited DataMember, so we must detect the nested members and create the respective joins
						if (!isTheLastOne && DataType.IsMapped(DataMember.GetReturnType(memberInfo)))
						{
							DataType foreignDataType = DataMember.GetReturnType(memberInfo);
							bool foreignKeyIsMapped = true;

							foreach (DataMember foreignKey in foreignDataType.PrimaryKey)
							{
								DataMember localKey = referencingDataType.Members.Where(m => m.Member == memberInfo.Name + "." + foreignKey.Member).SingleOrDefault();

								if (localKey == null)
								{
									foreignKeyIsMapped = false;
								}
							}

							if (foreignKeyIsMapped)
							{
								SelectJoin foreignJoin = Joins.Where(j => j.Type == foreignDataType && j.Alias == currentExpression.Replace('.', '_')).SingleOrDefault();

								if (foreignJoin == null)
								{
									foreignJoin = new SelectJoin();
									foreignJoin.JoinType = Sql.Operations.SelectJoinType.Inner;
									foreignJoin.Type = foreignDataType;
									foreignJoin.Alias = currentExpression.Replace('.', '_');

									string pastJoinAlias = string.Empty;

									if (!isTheFirstOne)
									{
										for (int y = 0; y <= i - 1; y++)
										{
											pastJoinAlias += '.' + nestedMemberInfos[y].Name;
										}

										pastJoinAlias = pastJoinAlias.Trim('.');
									}
									else if(parent != From)
									{
										pastJoinAlias = parent.InnerType.Name + "_base";
									}

									foreach (DataMember foreignKey in foreignDataType.PrimaryKey)
									{
										DataMember localKey = referencingDataType.Members.Where(m => m.Member == memberInfo.Name + "." + foreignKey.Member).SingleOrDefault();

										var filter = new Filters.MemberCompareFilter()
										{
											Member = localKey,
											MemberToCompare = foreignKey,
											TypeAlias = pastJoinAlias,
											MemberToCompareTypeAlias = foreignJoin.Alias,
										};

										foreignJoin.On.Add(filter);
									}

									Joins.Add(foreignJoin);
								}

								break;
							}
						}

						if (!isTheFirstOne && isTheLastOne)
						{
							DataMember dmember = DataMember.GetMap(referencingDataType, memberInfo.Name);
							SelectJoin foreignJoin = Joins.Where(j => j.Type == referencingDataType && j.Alias == currentExpression.Replace("." + memberInfo.Name, string.Empty).Replace('.', '_')).SingleOrDefault();
							SelectMember sm = new SelectMember(dmember, currentExpression.Replace('.', '_'));
							foreignJoin.Members.Add(sm);
							break;
						}
					}
				}
			}
		}
	}
}