using OKHOSTING.Core.Data.Validation;

namespace OKHOSTING.Sql.ORM.UI.Security
{
	/// <summary>
	/// Represents a DataType and a list of DataMembers that a user has acces to
	/// </summary>
	public class DbUserPermission : DbPermission
	{
		/// <summary>
		/// User which this permission is assigned to
		/// </summary>
		[RequiredValidator]
		public User User
		{
			get; set;
		}
	}
}