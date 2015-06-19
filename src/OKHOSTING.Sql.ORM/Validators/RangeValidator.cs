using System;

namespace OKHOSTING.Sql.ORM.Validators
{
	/// <summary>
	/// Validate that the specified MemberMap be between 
	/// a Minimum and Maximum values
	/// </summary>
	public class RangeValidator: MemberValidator
	{
		/// <summary>
		/// Minimum value of the allowed range
		/// </summary>
		public readonly IComparable MinValue;
		
		/// <summary>
		/// Maximum value of the allowed range
		/// </summary>
		public readonly IComparable MaxValue;

		/// <summary>
		/// Construct the validator
		/// </summary>
		/// <param name="minValue">
		/// Minimum value of the allowed range
		/// </param>
		/// <param name="maxValue">
		/// Maximum value of the allowed range
		/// </param>
		public RangeValidator(short minValue, short maxValue) : this((IComparable)minValue, (IComparable)maxValue) { }

		/// <summary>
		/// Construct the validator
		/// </summary>
		/// <param name="minValue">
		/// Minimum value of the allowed range
		/// </param>
		/// <param name="maxValue">
		/// Maximum value of the allowed range
		/// </param>
		public RangeValidator(int minValue, int maxValue) : this((IComparable)minValue, (IComparable)maxValue) { }

		/// <summary>
		/// Construct the validator
		/// </summary>
		/// <param name="minValue">
		/// Minimum value of the allowed range
		/// </param>
		/// <param name="maxValue">
		/// Maximum value of the allowed range
		/// </param>
		public RangeValidator(long minValue, long maxValue) : this((IComparable)minValue, (IComparable)maxValue) { }

		/// <summary>
		/// Construct the validator
		/// </summary>
		/// <param name="minValue">
		/// Minimum value of the allowed range
		/// </param>
		/// <param name="maxValue">
		/// Maximum value of the allowed range
		/// </param>
		public RangeValidator(ushort minValue, ushort maxValue) : this((IComparable)minValue, (IComparable)maxValue) { }

		/// <summary>
		/// Construct the validator
		/// </summary>
		/// <param name="minValue">
		/// Minimum value of the allowed range
		/// </param>
		/// <param name="maxValue">
		/// Maximum value of the allowed range
		/// </param>
		public RangeValidator(uint minValue, uint maxValue) : this((IComparable)minValue, (IComparable)maxValue) { }

		/// <summary>
		/// Construct the validator
		/// </summary>
		/// <param name="minValue">
		/// Minimum value of the allowed range
		/// </param>
		/// <param name="maxValue">
		/// Maximum value of the allowed range
		/// </param>
		public RangeValidator(ulong minValue, ulong maxValue) : this((IComparable)minValue, (IComparable)maxValue) { }

		/// <summary>
		/// Construct the validator
		/// </summary>
		/// <param name="minValue">
		/// Minimum value of the allowed range
		/// </param>
		/// <param name="maxValue">
		/// Maximum value of the allowed range
		/// </param>
		public RangeValidator(byte minValue, byte maxValue) : this((IComparable)minValue, (IComparable)maxValue) { }

		/// <summary>
		/// Construct the validator
		/// </summary>
		/// <param name="minValue">
		/// Minimum value of the allowed range
		/// </param>
		/// <param name="maxValue">
		/// Maximum value of the allowed range
		/// </param>
		public RangeValidator(sbyte minValue, sbyte maxValue) : this((IComparable)minValue, (IComparable)maxValue) { }

		/// <summary>
		/// Construct the validator
		/// </summary>
		/// <param name="minValue">
		/// Minimum value of the allowed range
		/// </param>
		/// <param name="maxValue">
		/// Maximum value of the allowed range
		/// </param>
		public RangeValidator(float minValue, float maxValue) : this((IComparable)minValue, (IComparable)maxValue) { }

		/// <summary>
		/// Construct the validator
		/// </summary>
		/// <param name="minValue">
		/// Minimum value of the allowed range
		/// </param>
		/// <param name="maxValue">
		/// Maximum value of the allowed range
		/// </param>
		public RangeValidator(double minValue, double maxValue) : this((IComparable)minValue, (IComparable)maxValue) { }

		/// <summary>
		/// Construct the validator
		/// </summary>
		/// <param name="minValue">
		/// Minimum value of the allowed range
		/// </param>
		/// <param name="maxValue">
		/// Maximum value of the allowed range
		/// </param>
		public RangeValidator(decimal minValue, decimal maxValue) : this((IComparable)minValue, (IComparable)maxValue) { }

		/// <summary>
		/// Construct the validator
		/// </summary>
		/// <param name="minValue">
		/// Minimum value of the allowed range
		/// </param>
		/// <param name="maxValue">
		/// Maximum value of the allowed range
		/// </param>
		public RangeValidator(DateTime minValue, DateTime maxValue) : this((IComparable)minValue, (IComparable)maxValue) { }

		/// <summary>
		/// Construct the validator
		/// </summary>
		/// <param name="minValue">
		/// Minimum value of the allowed range
		/// </param>
		/// <param name="maxValue">
		/// Maximum value of the allowed range
		/// </param>
		public RangeValidator(string minValue, string maxValue) : this((IComparable)minValue, (IComparable)maxValue) { }

		/// <summary>
		/// Construct the validator
		/// </summary>
		/// <param name="minValue">
		/// Minimum value of the allowed range
		/// </param>
		/// <param name="maxValue">
		/// Maximum value of the allowed range
		/// </param>
		public RangeValidator(bool minValue, bool maxValue) : this((IComparable)minValue, (IComparable)maxValue) { }

		/// <summary>
		/// Construct the validator
		/// </summary>
		/// <param name="minValue">
		/// Minimum value of the allowed range
		/// </param>
		/// <param name="maxValue">
		/// Maximum value of the allowed range
		/// </param>
		public RangeValidator(char minValue, char maxValue) : this((IComparable)minValue, (IComparable)maxValue) { }

		/// <summary>
		/// Construct the validator
		/// </summary>
		/// <param name="minValue">
		/// Minimum value of the allowed range
		/// </param>
		/// <param name="maxValue">
		/// Maximum value of the allowed range
		/// </param>
		public RangeValidator(IComparable minValue, IComparable maxValue)
		{
			//Validating if the minValue or maxValue are nulls
			if (minValue == null) throw new ArgumentNullException("minValue");
			if (maxValue == null) throw new ArgumentNullException("maxValue");

			//Initializing validator
			this.MinValue = minValue;
			this.MaxValue = maxValue;
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

			//Getting the current value of the associated MemberMap
			IComparable val = (IComparable) Member.GetValue(obj);

			//Comparing the value with the minimum and maximum value
			int resultMin = val.CompareTo(MinValue);
			int resultMax = val.CompareTo(MaxValue);

			//Verifying if the range is fulfilled
			if (resultMin < 0 || resultMax > 0)
			{
				error = new ValidationError(this, "DataProperty value must be between " + MinValue + " and " + MaxValue + " (inclusive)");
			}

			//Returning the applicable error or null
			return error;
		}

		#endregion
	}
}