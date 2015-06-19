using System;

namespace OKHOSTING.Sql.ORM.Validators
{
	/// <summary>
	/// Defines the behavior that must be have the validation classes
	/// </summary>
	public interface IValidator<T>
	{
		/// <summary>
		/// Performs the validation
		/// </summary>
		/// <returns>
		/// ValidationError object with the error founded if the validation fails,
		/// otherwise returns null
		/// </returns>
		ValidationError Validate(T obj);
	}
}