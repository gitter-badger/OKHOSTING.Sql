namespace OKHOSTING.Sql.Schema
{
	/// <summary>
	/// Operations that can be perfomed on a table
	/// </summary>
	public enum DataBaseOperation
	{
		Insert = 1,
		Update = 2,
		Delete = 3,
		Select = 4,

		Create = 5,
		Drop = 6,
		Alter = 7,
	}
}