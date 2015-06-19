using System;
using System.IO;

namespace OKHOSTING.Sql.ORM.Validators
{
	/// <summary>
	/// Validates that a string DataMember contains a valid path to a directory.
	/// Path can absolute or relative to the "/Custom" directory.
	/// </summary>
	/// <remarks>Applies only to string DataValues</remarks>
	/// <example>
	/// c:\myfolder\ --> absolute path
	/// /myfolder/ --> relative path (starting at /Custom directory)
	/// </example>
	public class DirectoryValidator : MemberValidator
	{
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
			string path;
			
			//Getting the value of the DataMember
			string currentValue = (string) Member.GetValue(obj);

			//if null, exit
			if (string.IsNullOrWhiteSpace(currentValue)) return null;

			//absolute path
			if (currentValue.Contains(":"))
			{
				path = currentValue;
			}
			//relative path
			else
			{
				path = AppDomain.CurrentDomain.BaseDirectory + @"Custom\" + currentValue.TrimStart('/', '\\');
			}

			//validate file path
			if (!Directory.Exists(path))
			{
				error = new ValidationError(this, "Directory '" + path + "' does not exists");
			}

			//Returning the error or null
			return error;
		}
	}
}