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
		/// DataBaseOperation that will be performed. Affects the way validation is done.
		/// </summary>
		public readonly DataBaseOperation Operation;

		/// <summary>
		/// Constructs the validator
		/// </summary>
		/// <param name="dobj">
		/// DataObject for primary key validation
		/// </param>
		/// <param name="operation">DataBaseOperation that will be performed. Affects the way validation is done.</param>
		public PrimaryKeyValidator(DataBaseOperation operation)
		{
			this.Operation = operation;
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
			TypeMap dtype = obj.GetType();

			//Crossing the MemberMap's on the collection
			foreach (MemberMap dv in dtype.PrimaryKey)
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

				if (isNull)
				{
					if (!(Operation == DataBaseOperation.Insert && dv.Column.IsAutoNumber))
					{
						error = new NullPrimaryKeyError(dv, this, "PrimaryKey contains a null value");
					}
				}
				
				//If an error exists, break and return the error
				if (error != null) return error;
			}
			
			//If no error was found, return null
			return null;
		}
	}
}