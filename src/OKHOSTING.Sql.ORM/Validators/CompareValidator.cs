using System;
using OKHOSTING.Sql.ORM.Filters;
using OKHOSTING.Core.Data;

namespace OKHOSTING.Sql.ORM.Validators
{
	/// <summary>
	/// Base class for comparison validators
	/// </summary>
	public abstract class CompareValidator: MemberValidator
	{
		/// <summary>
		/// Operator to use in the validation 
		/// </summary>
		public readonly CompareOperator Operator;

		/// <summary>
		/// Constructs th validator
		/// </summary>
		/// <param name="op">
		/// Operator to use in the validation
		/// </param>
		protected CompareValidator(CompareOperator op)
		{
			this.Operator = op;
		}

		/// <summary>
		/// Compare the value of the associated MemberMap with the 
		/// specified value and using the indicated operator, returns
		/// an ValidationError if the validation fails, or null if it's success
		/// </summary>
		/// <param name="valueToCompare">
		/// Value for comparison
		/// </param>
		/// <returns>
		/// ValidationError if the validation fails, or null if it's success
		/// </returns>
		protected ValidationError Validate(object obj, IComparable valueToCompare)
		{
			//Local Vars
			ValidationError error = null;

			//Validating if the valueToCompare is null
			if (valueToCompare == null) throw new ArgumentNullException("valueToCompare");

			//Loading the value of associated MemberMap and comparing with the specified value
			IComparable toValidate = (IComparable) Member.GetValue(obj);
			int compareResult = toValidate.CompareTo(valueToCompare);
			
			//Perform the validation in function of the established operator
			switch(this.Operator)
			{
				case CompareOperator.Equal:
					if(compareResult != 0)
						error = new ValidationError(this, Member + " value must be equal than " + valueToCompare);
					break;
				
				case CompareOperator.NotEqual:
					if(compareResult == 0)
						error = new ValidationError(this, Member + " value must be different than " + valueToCompare);
					break;

				case CompareOperator.GreaterThan:
					if(compareResult <= 0)
						error = new ValidationError(this, Member + " value must be greater than than " + valueToCompare);
					break;

				case CompareOperator.GreaterThanEqual:
					if(compareResult < 0)
						error = new ValidationError(this, Member + " value must be greater or equal than " + valueToCompare);
					break;

				case CompareOperator.LessThan:
					if(compareResult >= 0)
						error = new ValidationError(this, this.Member + " value must be less than " + valueToCompare);
					break;

				case CompareOperator.LessThanEqual:
					if(compareResult > 0)
						error = new ValidationError(this, this.Member + " value must be less or equal than " + valueToCompare);
					break;
			}
			
			return error;
		}
	}
}