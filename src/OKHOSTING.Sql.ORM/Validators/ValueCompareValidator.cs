
using OKHOSTING.Core.Data;
using System;

namespace OKHOSTING.Sql.ORM.Validators
{
	/// <summary>
	/// Defines a validation based on the comparison between
	/// an absolute value and a DataMember
	/// </summary>
	public class ValueCompareValidator : CompareValidator
	{
		/// <summary>
		/// Value used on the comparison
		/// </summary>
		public readonly IComparable ValueToCompare;

		#region Constructors

		/// <summary>
		/// Construct the validator
		/// </summary>
		/// <param name="op">
		/// Operator used on the comparison
		/// </param>
		/// <param name="valueToCompare">
		/// Value used on the comparison
		/// </param>
		public ValueCompareValidator(CompareOperator op, short valueToCompare) : this(op, (IComparable)valueToCompare) { }

		/// <summary>
		/// Construct the validator
		/// </summary>
		/// <param name="op">
		/// Operator used on the comparison
		/// </param>
		/// <param name="valueToCompare">
		/// Value used on the comparison
		/// </param>
		public ValueCompareValidator(CompareOperator op, int valueToCompare) : this(op, (IComparable)valueToCompare) { }

		/// <summary>
		/// Construct the validator
		/// </summary>
		/// <param name="op">
		/// Operator used on the comparison
		/// </param>
		/// <param name="valueToCompare">
		/// Value used on the comparison
		/// </param>
		public ValueCompareValidator(CompareOperator op, long valueToCompare) : this(op, (IComparable)valueToCompare) { }

		/// <summary>
		/// Construct the validator
		/// </summary>
		/// <param name="op">
		/// Operator used on the comparison
		/// </param>
		/// <param name="valueToCompare">
		/// Value used on the comparison
		/// </param>
		public ValueCompareValidator(CompareOperator op, ushort valueToCompare) : this(op, (IComparable)valueToCompare) { }

		/// <summary>
		/// Construct the validator
		/// </summary>
		/// <param name="op">
		/// Operator used on the comparison
		/// </param>
		/// <param name="valueToCompare">
		/// Value used on the comparison
		/// </param>
		public ValueCompareValidator(CompareOperator op, uint valueToCompare) : this(op, (IComparable)valueToCompare) { }

		/// <summary>
		/// Construct the validator
		/// </summary>
		/// <param name="op">
		/// Operator used on the comparison
		/// </param>
		/// <param name="valueToCompare">
		/// Value used on the comparison
		/// </param>
		public ValueCompareValidator(CompareOperator op, ulong valueToCompare) : this(op, (IComparable)valueToCompare) { }

		/// <summary>
		/// Construct the validator
		/// </summary>
		/// <param name="op">
		/// Operator used on the comparison
		/// </param>
		/// <param name="valueToCompare">
		/// Value used on the comparison
		/// </param>
		public ValueCompareValidator(CompareOperator op, byte valueToCompare) : this(op, (IComparable)valueToCompare) { }

		/// <summary>
		/// Construct the validator
		/// </summary>
		/// <param name="op">
		/// Operator used on the comparison
		/// </param>
		/// <param name="valueToCompare">
		/// Value used on the comparison
		/// </param>
		public ValueCompareValidator(CompareOperator op, sbyte valueToCompare) : this(op, (IComparable)valueToCompare) { }

		/// <summary>
		/// Construct the validator
		/// </summary>
		/// <param name="op">
		/// Operator used on the comparison
		/// </param>
		/// <param name="valueToCompare">
		/// Value used on the comparison
		/// </param>
		public ValueCompareValidator(CompareOperator op, float valueToCompare) : this(op, (IComparable)valueToCompare) { }

		/// <summary>
		/// Construct the validator
		/// </summary>
		/// <param name="op">
		/// Operator used on the comparison
		/// </param>
		/// <param name="valueToCompare">
		/// Value used on the comparison
		/// </param>
		public ValueCompareValidator(CompareOperator op, double valueToCompare) : this(op, (IComparable)valueToCompare) { }

		/// <summary>
		/// Construct the validator
		/// </summary>
		/// <param name="op">
		/// Operator used on the comparison
		/// </param>
		/// <param name="valueToCompare">
		/// Value used on the comparison
		/// </param>
		public ValueCompareValidator(CompareOperator op, decimal valueToCompare) : this(op, (IComparable)valueToCompare) { }

		/// <summary>
		/// Construct the validator
		/// </summary>
		/// <param name="op">
		/// Operator used on the comparison
		/// </param>
		/// <param name="valueToCompare">
		/// Value used on the comparison
		/// </param>
		public ValueCompareValidator(CompareOperator op, DateTime valueToCompare) : this(op, (IComparable)valueToCompare) { }

		/// <summary>
		/// Construct the validator
		/// </summary>
		/// <param name="op">
		/// Operator used on the comparison
		/// </param>
		/// <param name="valueToCompare">
		/// Value used on the comparison
		/// </param>
		public ValueCompareValidator(CompareOperator op, string valueToCompare) : this(op, (IComparable)valueToCompare) { }

		/// <summary>
		/// Construct the validator
		/// </summary>
		/// <param name="op">
		/// Operator used on the comparison
		/// </param>
		/// <param name="valueToCompare">
		/// Value used on the comparison
		/// </param>
		public ValueCompareValidator(CompareOperator op, bool valueToCompare) : this(op, (IComparable)valueToCompare) { }

		/// <summary>
		/// Construct the validator
		/// </summary>
		/// <param name="op">
		/// Operator used on the comparison
		/// </param>
		/// <param name="valueToCompare">
		/// Value used on the comparison
		/// </param>
		public ValueCompareValidator(CompareOperator op, char valueToCompare) : this(op, (IComparable)valueToCompare) { }


		/// <summary>
		/// Constructs the validator
		/// </summary>
		/// <param name="op">
		/// Operator used on the comparison
		/// </param>
		/// <param name="valueToCompare">
		/// Value used on the comparison
		/// </param>
		public ValueCompareValidator(CompareOperator op, IComparable valueToCompare): base(op)
		{
			//Validating if valueToCompare argument is null
			if (valueToCompare == null) throw new ArgumentNullException("valueToCompare");

			//Initializing validator
			this.ValueToCompare = valueToCompare;
		}

		#endregion 

		/// <summary>
		/// Performs the validation
		/// </summary>
		/// <returns>
		/// ValidationError object with the error founded if the validation fails,
		/// otherwise returns null
		/// </returns>
		public override ValidationError Validate(object obj)
		{
			//Validating
			return base.Validate(obj, ValueToCompare);
		}
	}
}