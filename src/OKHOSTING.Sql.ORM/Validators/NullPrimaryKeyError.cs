using System;

namespace OKHOSTING.Sql.ORM.Validators
{
	/// <summary>
	/// Defines a primary key validation error
	/// </summary>
	public class NullPrimaryKeyError : ValidationError
	{
		/// <summary>
		/// Member that is wrong defined and that 
		/// is part of the primary key
		/// </summary>
		public readonly DataMember Member;

		/// <summary>
		/// Constructs the error object
		/// </summary>
		/// <param name="dataValue">
		/// Member that is wrong defined and that 
		/// is part of the primary key
		/// </param>
		/// <param name="validator">
		/// Reference to the validator that fails
		/// </param>
		/// <param name="message">
		/// Description of the error
		/// </param>
		public NullPrimaryKeyError(DataMember member, ValidatorBase validator, string message): base(validator, message)
		{
			//Validating if the dataValue argument is null
			if (member == null) throw new ArgumentNullException("member");

			//Initializing the error
			this.Member = member;
		}
	}
}