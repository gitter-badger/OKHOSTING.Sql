using System;
using OKHOSTING.Sql.ORM.Filters;
using OKHOSTING.Core.Data;

namespace OKHOSTING.Sql.ORM.Validators
{
	/// <summary>
	/// Indicates if a Property of Field of type string, must have 
	/// an specific length
	/// </summary>
	/// <remarks>Applies only to string DataValues</remarks>
	public class StringLengthValidator: MemberValidator
	{
		/// <summary>
		/// Specify the maximum length that a string can contain
		/// </summary>
		public readonly int MaxLength;

		/// <summary>
		/// Specify the minimum length that a string can contain
		/// </summary>
		public readonly int MinLength;

		/// <summary>
		/// Specify the Operator to use in the validation 
		/// of the length 
		/// </summary>
		public readonly CompareOperator Operator;

		/// <summary>
		/// Defines if an string.Empty value is valid
		/// </summary>
		public readonly bool AllowEmpty;

		/// <summary>
		/// Constructs the validator 
		/// </summary>
		/// <param name="op">
		/// Operator used in the validation
		/// </param>
		/// <param name="length">
		/// Length for the validation
		/// </param>
		public StringLengthValidator(CompareOperator op, int length) : this(op, length, true) { }
		
		/// <summary>
		/// Constructs the validator 
		/// </summary>
		/// <param name="op">
		/// Operator used in the validation
		/// </param>
		/// <param name="length">
		/// Length for the validation
		/// </param>
		/// <param name="allowEmpty">
		/// Defines if an string.Empty value is valid
		/// </param>
		public StringLengthValidator(CompareOperator op, int length, bool allowEmpty)
		{
			this.MaxLength = length;
			this.Operator = op;
			this.AllowEmpty = allowEmpty;
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

			//Getting the value of the MemberMap
			string currentValue = (string) Member.GetValue(obj);

			//if it's null, omit validation
			if (currentValue == null) return null;

			//Perform the applicable validation for the specified operator
			switch (this.Operator)
			{
				case CompareOperator.Equal:
					if (!(currentValue.Length == this.MaxLength))
						error = new ValidationError(this, "String length must be equal than " + MaxLength + " on field " + Member);
					break;

				case CompareOperator.NotEqual:
					if (!(currentValue.Length != this.MaxLength))
						error = new ValidationError(this, "String length must be different than " + MaxLength + " on field " + Member);
					break;

				case CompareOperator.GreaterThan:
					if (!(currentValue.Length > this.MaxLength))
						error = new ValidationError(this, "String length must be greater than than " + MaxLength + " on field " + Member);
					break;

				case CompareOperator.GreaterThanEqual:
					if (!(currentValue.Length >= this.MaxLength))
						error = new ValidationError(this, "String length must be greater or equal than " + MaxLength + " on field " + Member);
					break;

				case CompareOperator.LessThan:
					if (!(currentValue.Length < this.MaxLength))
						error = new ValidationError(this, "String length must be less than " + MaxLength + " on field " + Member);
					break;

				case CompareOperator.LessThanEqual:
					if (!(currentValue.Length <= this.MaxLength))
						error = new ValidationError(this, "String length must be less or equal than " + MaxLength + " on field " + Member);
					break;
			}

			//Validating if the string.Empty value is a valid value (only if dont exists errors))
			if (error == null && !this.AllowEmpty)
			{
				if (currentValue.Trim() == string.Empty)
					error = new ValidationError(this, "String can't be an empty string on field " + Member);
			}

			//Returning the error or null
			return error;
		}

		/// <summary>
		/// Gets the max lenght of a string DataValue
		/// </summary>
		/// <param name="dmember">String DataValue that has a StringLengthValidator attribute</param>
		/// <returns>Maximum lenght of the string DataValue. 0 if no max lenght is defined.</returns>
		public static int GetMaxLenght(System.Reflection.MemberInfo member)
		{
			//Validating if the MemberInfo is null
			if (member == null) throw new ArgumentNullException("member");

			//Recovering the attributes of type DataMemberAttribute declared in the MemberInfo
			object[] attributes = member.GetCustomAttributes(typeof(StringLengthValidator), false);

			foreach (StringLengthValidator validator in attributes)
			{
				if (validator.Operator == CompareOperator.Equal)
					return validator.MaxLength;
				else if (validator.Operator == CompareOperator.LessThan)
					return validator.MaxLength - 1;
				else if (validator.Operator == CompareOperator.LessThanEqual)
					return validator.MaxLength;
			}

			//try with StringLengthAttribute
			attributes = member.GetCustomAttributes(typeof(System.ComponentModel.DataAnnotations.StringLengthAttribute), false);

			foreach (System.ComponentModel.DataAnnotations.StringLengthAttribute validator in attributes)
			{
				return validator.MaximumLength;
			}

			//if operator is not one of the previous, return null
			return 0;
		}

		/// <summary>
		/// Gets the min lenght of a string DataValue
		/// </summary>
		/// <param name="dmember">String DataValue that has a StringLengthValidator attribute</param>
		/// <returns>Maximum lenght of the string DataValue. Null if no max lenght is defined.</returns>
		public static int GetMinLenght(System.Reflection.MemberInfo member)
		{
			//Validating if the MemberInfo is null
			if (member == null) throw new ArgumentNullException("member");

			//Recovering the attributes of type DataMemberAttribute declared in the MemberInfo
			object[] attributes = member.GetCustomAttributes(typeof(StringLengthValidator), false);

			foreach (StringLengthValidator validator in attributes)
			{
				if (validator.Operator == CompareOperator.Equal)
					return validator.MaxLength;
				else if (validator.Operator == CompareOperator.GreaterThan)
					return validator.MaxLength + 1;
				else if (validator.Operator == CompareOperator.GreaterThanEqual)
					return validator.MaxLength;
			}

			//try with StringLengthAttribute
			attributes = member.GetCustomAttributes(typeof(System.ComponentModel.DataAnnotations.StringLengthAttribute), false);

			foreach (System.ComponentModel.DataAnnotations.StringLengthAttribute validator in attributes)
			{
				return validator.MinimumLength;
			}

			//if operator is not one of the previous, return null
			return 0;
		}
	}
}