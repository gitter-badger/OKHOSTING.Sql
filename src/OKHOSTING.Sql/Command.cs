using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OKHOSTING.Sql
{
	/// <summary>
	/// Represents a generic SQL script command to be execeuted on a DataBase
	/// </summary>
	public class Command
	{
		/// <summary>
		/// A SQL script that you want to execute on a DataBase
		/// </summary>
		public string Script { get; set; }

		/// <summary>
		/// Parameters for the script, will be passed on to the System.Data.ICommand cobject
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

		public static implicit operator String(Command command)
		{
			return command.Script;
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