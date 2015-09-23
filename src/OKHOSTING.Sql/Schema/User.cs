namespace OKHOSTING.Sql.Schema
{
	/// <summary>
	/// A user that has access to a database
	/// </summary>
	public class User
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public DataBaseSchema DataBase { get; set; }
	}
}