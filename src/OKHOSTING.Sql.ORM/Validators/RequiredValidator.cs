using System;
using System.Reflection;

namespace OKHOSTING.Sql.ORM.Validators
{
	/// <summary>
	/// Validate if a Property of Field on a Class can be null
	/// </summary>
	public class RequiredValidator : MemberValidator
	{
		/// <summary>
		/// Constructs the attribute
		/// </summary>
		public RequiredValidator()
		{
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

			//Recover the value of MemberMap associated
			object currentValue = Member.GetValue(obj);

			//Validating if the value is null
			if (currentValue == null)
			{
				error = new ValidationError(this, this.Member + " cannot be null");
			}

			//if this is a string, do not allow null nor empty values
			else if (Member.ReturnType.Equals(typeof(string)))
			{
				if (string.IsNullOrWhiteSpace((string) currentValue))
				{
					error = new ValidationError(this, Member + " cannot be empty");
				}
			}

			//Returning the error or null
			return error;
		}
	}
}