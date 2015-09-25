using System;

namespace OKHOSTING.Sql
{
	/// <summary>
	/// The exception that is thrown when a bad-formed 
	/// Sql script is executed or when exists an error 
	/// on the execute
	/// </summary>
	public class SqlException: Exception
	{
		#region Fields 

		/// <summary>
		/// The Sql Script that was beeing executed when the exception was thrown
		/// </summary>
		public readonly Command Command;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the SqlException class
		/// </summary>
		/// <param name="script">The Sql script beeing executed</param>
		/// <param name="message">The message indicating the error ocurred</param>
		/// <param name="innerException">The exception that caused this exception to be thrown</param>
		public SqlException(Command command, string message, Exception innerException)
			: base(message, innerException)
		{
			this.Command = command;
		}
		/// <summary>
		/// Initializes a new instance of the SqlException class
		/// </summary>
		/// <param name="script">The Sql script beeing executed</param>
		/// <param name="message">The message indicating the error ocurred</param>
		public SqlException(Command command, string message)
			: base(message)
		{
			this.Command = command;
		}
		/// <summary>
		/// Initializes a new instance of the SqlException class
		/// </summary>
		/// <param name="script">The Sql script beeing executed</param>
		public SqlException(Command command)
			: base()
		{
			this.Command = command;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Returns a description of the exception including the executed script
		/// </summary>
		public override string Message
		{
			get
			{
				return base.Message + "\r\n\r\nExecuted Script:\r\n" + this.Command.Script + "\r\n\r\n";
			}
		}

		#endregion
	}
}