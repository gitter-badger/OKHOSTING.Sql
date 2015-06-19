using System;

namespace OKHOSTING.Sql.ORM.Validators
{
	/// <summary>
	/// Allows to perform custom validations over DataObjects data
	/// </summary>
	public class CustomValidator: MemberValidator
	{
		/// <summary>
		/// Error message to be showed when the validator fails
		/// </summary>
		public readonly string ErrorMessage;

		/// <summary>
		/// Event throwed on the validation
		/// </summary>
		public event ValidationEventHandler Validating;

		/// <summary>
		/// Construct the validator 
		/// </summary>
		/// <param name="errorMessage">
		/// Error message to be showed when the validator fails
		/// </param>
		/// <param name="validatingEventHandler">
		/// Delegate that points to the event that perform the validation
		/// </param>
		public CustomValidator(string errorMessage, ValidationEventHandler validatingEventHandler)
		{
			//Validating if the errorMessage argument is null
			if (ErrorMessage == null) throw new ArgumentNullException("errorMessage");

			//Initializing the error message
			this.ErrorMessage = errorMessage;
		}

		#region Validators Implementation

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

			//Validating if exists subscriptors to Validating event
			if (this.Validating != null)
			{
				//Creating the argument for the event
				ValidationEventArgs e = new ValidationEventArgs(this.ErrorMessage, obj);

				//Requesting validation to the client
				this.Validating(this, e);

				//If the validation fails, creating the respective error
				if (!e.IsValid) error = new ValidationError(this, e.ErrorMessage);
			}

			//Returning the applicable error or null
			return error;
		}

		#endregion
	
	}
}