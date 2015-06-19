using System;
using System.IO;

namespace OKHOSTING.Sql.ORM.Validators
{
	/// <summary>
	/// Validates that a string DataMember contains a valid path to a file.
	/// Path can absolute or relative to the "/Custom" directory.
	/// </summary>
	/// <remarks>Applies only to string DataValues</remarks>
	/// <example>
	/// c:\myfolder\myfile.jpg --> absolute path
	/// /myfolder/myfile.jpg --> relative path (starting at /Custom directory)
	/// </example>
	public class FileValidator : MemberValidator
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
			if (!File.Exists(path))
			{
				error = new ValidationError(this, "File '" + path + "' does not exists");
			}

			//Returning the error or null
			return error;
		}
	}
}