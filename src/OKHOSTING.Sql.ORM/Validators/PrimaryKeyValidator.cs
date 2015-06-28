using System;
using System.Linq;
using System.Collections.Generic;

namespace OKHOSTING.Sql.ORM.Validators
{
	/// <summary>
	/// Validate if the primary key of an DataObject is correctly defined
	/// </summary>
	public class PrimaryKeyValidator: IValidator
	{
		/// <summary>
		/// Constructs the validator
		/// </summary>
		public PrimaryKeyValidator()
		{
		}
		
		/// <summary>
		/// Performs the validation
		/// </summary>
		/// <returns>
		/// Error information if validation fails, otherwise null
		/// </returns>
		public ValidationError Validate(object obj)
		{
			//Local Vars
			NullPrimaryKeyError error = null;
			DataType dtype = obj.GetType();

			//Crossing the MemberMap's on the collection
			foreach (DataMember dv in dtype.PrimaryKey)
			{
				bool isNull;

				//if this is a string, do not allow null nor empty values
				if (dv.ReturnType.Equals(typeof(string)))
				{
					isNull = string.IsNullOrWhiteSpace((string) dv.GetValue(obj));
				}
				else
				{
					isNull = dv.GetValue(obj) == null;
				}

				if (isNull && !dv.Column.IsAutoNumber)
				{
					error = new NullPrimaryKeyError(dv, this, "PrimaryKey contains a null value");
				}
				
				//If an error exists, break and return the error
				if (error != null) return error;
			}
			
			//If no error was found, return null
			return null;
		}
	}
}