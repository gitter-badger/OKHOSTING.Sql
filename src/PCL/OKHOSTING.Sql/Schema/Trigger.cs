using OKHOSTING.Data.Validation;

namespace OKHOSTING.Sql.Schema
{
	/// <summary>
	/// Represents a DataBase trigger
	/// </summary>
	public class Trigger
	{
		public int Id { get; set; }

		[RequiredValidator]
		public Table Table { get; set; }

		[RequiredValidator]
		[StringLengthValidator(StringLengthValidator.Unlimited)]
		public string TriggerBody { get; set; }

		[RequiredValidator]
		public DataBaseOperation TriggerEvent { get; set; }
	}
}