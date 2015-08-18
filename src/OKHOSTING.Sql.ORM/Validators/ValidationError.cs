using System;

namespace OKHOSTING.Sql.ORM.Validators
{
	/// <summary>
	/// Defines a validation error
	/// </summary>
	public class ValidationError
	{
		/// <summary>
		/// Reference to the validator that fails
		/// </summary>
		public readonly ValidatorBase Validator;

		/// <summary>
		/// Description of the error
		/// </summary>
		public readonly string Description;

		/// <summary>
		/// Constructs the ValidationError
		/// </summary>
		/// <param name="validator">
		/// Reference to the validator that fails
		/// </param>
		/// <param name="description">
		/// Description of the error
		/// </param>
		public ValidationError(ValidatorBase validator, string description)
		{
			Validator = validator;
			Description = description;
		}
	}
}