using OKHOSTING.Data.Validation;
using System;
using System.Collections.Generic;

namespace OKHOSTING.Sql
{
	/// <summary>
	/// Represents a generic SQL script command to be execeuted on a DataBase
	/// </summary>
	public class Command
	{
		public int Id { get; set; }

		/// <summary>
		/// A SQL script that you want to execute on a DataBase
		/// </summary>
		[RequiredValidator]
		[StringLengthValidator(StringLengthValidator.Unlimited)]
		public string Script { get; set; }

		/// <summary>
		/// Parameters for the script, will be passed on to the ICommand cobject
		/// </summary>
		public readonly List<CommandParameter> Parameters = new List<CommandParameter>();

		public Command()
		{
			Script = string.Empty;
		}

		public static implicit operator Command(String command)
		{
			return new Command() { Script = command };
		}

		public static Command operator +(Command command1, Command command2)
		{
			command1.Append(command2);

			return command1;
		}

		public void Append(Command command)
		{
			if (command == null)
			{
				//do nothing
				return;
			}

			Script += " " + command.Script;
			Parameters.AddRange(command.Parameters);
		}
	}
}