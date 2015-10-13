using OKHOSTING.Data.Validation;

namespace OKHOSTING.Sql.Schema
{
	/// <summary>
	/// A user that has access to a database
	/// </summary>
	public class User
	{
		public int Id { get; set; }

		[RequiredValidator]
		[StringLengthValidator(100)]
		public string Name { get; set; }

		[RequiredValidator]
		public DataBaseSchema DataBase { get; set; }
	}
}