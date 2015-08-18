using System;

namespace OKHOSTING.Sql.ORM.Validators
{
	/// <summary>
	/// Defines the behavior that must be have the validation classes
	/// </summary>
	public abstract class ValidatorBase
	{
		public int Id { get; set; }

		/// <summary>
		/// Performs the validation
		/// </summary>
		/// <returns>
		/// ValidationError object with the error founded if the validation fails,
		/// otherwise returns null
		/// </returns>
		public abstract ValidationError Validate(object obj);
	}

	public abstract class ValidatorBase<T> : ValidatorBase
	{
		/// <summary>
		/// Performs the validation
		/// </summary>
		/// <returns>
		/// ValidationError object with the error founded if the validation fails,
		/// otherwise returns null
		/// </returns>
		public abstract ValidationError Validate(T obj);

		public override ValidationError Validate(object obj)
		{
			return Validate((T) obj);
		}
	}
}