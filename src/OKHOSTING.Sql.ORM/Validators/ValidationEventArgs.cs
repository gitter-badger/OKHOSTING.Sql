using System;

namespace OKHOSTING.Sql.ORM.Validators
{
	/// <summary>
	/// Argument for custom validation events
	/// </summary>
	public class ValidationEventArgs : EventArgs
	{
		/// <summary>
		/// true if validation is successfully evaluated, otherwise false
		/// </summary>
		public bool IsValid;

		/// <summary>
		/// Error message to show if the validation fails
		/// </summary>
		public string ErrorMessage;

		/// <summary>
		/// Stores the value of the MemberMap to validate
		/// </summary>
		public object Value;

		/// <summary>
		/// Constructs the argument
		/// </summary>
		/// <param name="errorMessage">
		/// Error message to show if the validation fails
		/// </param>
		/// <param name="value">
		/// Stores the value of the MemberMap to validate
		/// </param>
		public ValidationEventArgs(string errorMessage, object value)
		{
			IsValid = false;
			ErrorMessage = errorMessage;
			Value = value;
		}
	}
}