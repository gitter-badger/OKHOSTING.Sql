using OKHOSTING.Data.Validation;

namespace OKHOSTING.Sql.Schema
{
	/// <summary>
	/// A view in a DataBase
	/// </summary>
	public class View
	{
		public int Id { get; set; }
		/// <summary>
		/// The name of the table
		/// </summary>
		[RequiredValidator]
		[StringLengthValidator(100)]
		public string Name { get; set; }

		[StringLengthValidator(StringLengthValidator.Unlimited)]
		public string Description { get; set; }

		[RequiredValidator]
		[StringLengthValidator(StringLengthValidator.Unlimited)]
		public string Command { get; set; }

		[RequiredValidator]
		public DataBaseSchema DataBase { get; set; }
	}
}