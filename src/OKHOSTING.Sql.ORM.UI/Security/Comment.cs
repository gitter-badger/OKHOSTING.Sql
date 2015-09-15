using System;
using OKHOSTING.Core.Data.Validation;

namespace OKHOSTING.Sql.ORM.UI.Security
{
	/// <summary>
	/// A comment of a user on a DataObject, usefull to create comments on any type of DataObject
	/// </summary>
	public class Comment
	{
		/// <summary>
		/// Unique Id
		/// </summary>
		public int Id;

		/// <summary>
		/// User that writes the comment
		/// </summary>
		[RequiredValidator]
		public User User;

		/// <summary>
		/// Date and time of the coment
		/// </summary>
		[RequiredValidator]
		public DateTime Date;

		/// <summary>
		/// DataObject which the comment is about
		/// </summary>
		[RequiredValidator]
		public DataObject DataObject;

		/// <summary>
		/// The comment itself
		/// </summary>
		[RequiredValidator]
		public string Notes;

		/// <summary>
		/// Sets User and Date before inserting
		/// </summary>
		protected override void OnBeforeInsert()
		{
			User = User.Current;
			Date = DateTime.Now;

			base.OnBeforeInsert();
		}
	}
}
