using System;
using OKHOSTING.Sql.ORM.Filters;
using OKHOSTING.Core.Data;

namespace OKHOSTING.Sql.ORM.Validators
{
	/// <summary>
	/// Defines a validation based on the comparison between
	/// two MemberMap's
	/// </summary>
	public class MemberCompareValidator : CompareValidator
	{
		/// <summary>
		/// MemberMap to compare with the MemberMap of the validator
		/// </summary>
		public readonly DataMember MemberToCompare;

		/// <summary>
		/// Constructs the validator
		/// </summary>
		/// <param name="op">
		/// Operator used on the comparison
		/// </param>
		/// <param name="dataValueToCompare">
		/// MemberMap to compare with the MemberMap of the validator
		/// </param>
		public MemberCompareValidator(CompareOperator op, DataMember memberToCompare): base(op)
		{
			//Validating if dataValueToCompare argument is null
			if (memberToCompare == null) throw new ArgumentNullException("memberToCompare");

			//Initializing validator
			this.MemberToCompare = memberToCompare;
		}

		/// <summary>
		/// Performs the validation
		/// </summary>
		/// <returns>
		/// ValidationError object with the error founded if the validation fails,
		/// otherwise returns null
		/// </returns>
		public override ValidationError Validate(object obj)
		{
			//Local Vars
			ValidationError error = null;

			//Converting the value to an IComparable interface
			IComparable memberValue = (IComparable) Member.GetValue(obj);

			//Validating
			error = base.Validate(obj, memberValue);
			
			//Returning the applicable error or null...
			return error;
		}
	}
}