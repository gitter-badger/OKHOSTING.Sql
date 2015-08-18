using System;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;

namespace OKHOSTING.Sql.ORM.Validators
{
	/// <summary>
	/// Validates a specific MemberMap. CHild classes will be able to perform a validation on a specific MemberMap
	/// </summary>
	public abstract class MemberValidator : ValidatorBase
	{
		/// <summary>
		/// MemberMap that implements the DataValueValidator
		/// </summary>
		public readonly DataMember Member;
	}
}