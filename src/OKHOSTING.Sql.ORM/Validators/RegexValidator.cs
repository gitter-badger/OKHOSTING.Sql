using System;
using System.Text.RegularExpressions;

namespace OKHOSTING.Sql.ORM.Validators
{
	
	/// <summary>
	/// Implements a validation of regular expressions
	/// </summary>
	/// <remarks>Applies only to string DataValues</remarks>
	public class RegexValidator: MemberValidator
	{
		/// <summary>
		/// Regular expression used to validate
		/// </summary>
		public readonly string Pattern;

		/// <summary>
		/// Constructs the validaror
		/// </summary>
		/// <param name="pattern">
		/// Regular expression used to validate
		/// </param>
		public RegexValidator(string pattern)
		{
			//Validating if the Pattern is null 
			if (pattern == null) throw new ArgumentNullException("pattern");

			//Initializing the validator
			this.Pattern = pattern;
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
			string currentValue = (string) Member.GetValue(obj);

			//if null, exit
			if (string.IsNullOrWhiteSpace(currentValue)) return null;

			//Performing the validation
			Regex regEx = new Regex(Pattern);
			
			//if doesnt match..
			if (!regEx.IsMatch(currentValue)) 
				error = new ValidationError(this, "The MemberMap " + Member + " doesn't match with the regular expression " + Pattern);

			//Returning the error or null
			return error;
		}
	}
}