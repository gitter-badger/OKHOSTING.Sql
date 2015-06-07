using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OKHOSTING.Sql
{
	/// <summary>
	/// Represents a generic command parameter to be used on command executions
	/// </summary>
	public class CommandParameter
	{
		public CommandParameter()
		{
		}

		public CommandParameter(object value): this(value, null)
		{
		}

		public CommandParameter(object value, string name)
		{
			Name = name;
			Value = value;

			if (string.IsNullOrWhiteSpace(name))
			{
				CreateRandomName();
			}
			else
			{
				Name = name;
			}

			if (value != null)
			{
				DbType = DataBase.Parse(value.GetType());
			}
		}

		public CommandParameter(object value, string name, System.Data.DbType dbType): this(value, name, dbType, 0)
		{
		}

		public CommandParameter(object value, string name, System.Data.DbType dbType, int size)
		{
			if (string.IsNullOrWhiteSpace(name))
			{
				CreateRandomName();
			}
			else
			{
				Name = name;
			}

			Value = value;
			DbType = dbType;
			Size = size;
		}

		/// <summary>
		/// Name of the parameter
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// DbType 
		/// </summary>
		public System.Data.DbType DbType { get; set; }

		/// <summary>
		/// Optional size, used for string max lenght
		/// </summary>
		public int Size { get; set; }

		/// <summary>
		/// Actual value of the parameter, can be null
		/// </summary>
		public object Value { get; set; }

		/// <summary>
		/// Direction of the parameter
		/// </summary>
		public System.Data.ParameterDirection Direction { get; set; }

		private static readonly Random Random = new Random();
		
		public void CreateRandomName()
		{
			Name = DbType + Random.Next().ToString();
		}
	}
}