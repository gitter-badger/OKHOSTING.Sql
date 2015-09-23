namespace OKHOSTING.Sql.Schema
{
	/// <summary>
	/// Action to be performed on a constraint
	/// </summary>
	public enum ConstraintAction
	{
		/// <summary>
		/// Set value to null
		/// </summary>
		Null,

		/// <summary>
		/// Set to default value
		/// </summary>
		Default,

		/// <summary>
		/// Restrict the operation, throw an exception
		/// </summary>
		Restrict,

		/// <summary>
		/// Casccade update / delete
		/// </summary>
		Cascade,

		/// <summary>
		/// Take no action
		/// </summary>
		NoAction
	}
}