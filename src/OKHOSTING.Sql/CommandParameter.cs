using System;

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
				DbType = DbTypeMapper.Parse(value.GetType());
			}
		}

		public CommandParameter(object value, string name, DbType dbType): this(value, name, dbType, 0)
		{
		}

		public CommandParameter(object value, string name, DbType dbType, int size)
		{
			Value = value;
			DbType = dbType;
			Size = size; 
			
			if (string.IsNullOrWhiteSpace(name))
			{
				CreateRandomName();
			}
			else
			{
				Name = name;
			}
		}

		public int Id { get; set; }

		/// <summary>
		/// Name of the parameter
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// DbType 
		/// </summary>
		public DbType DbType { get; set; }

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
		public ParameterDirection Direction { get; set; }

		private static readonly Random Random = new Random();
		
		public void CreateRandomName()
		{
			Name = "@" + DbType + Random.Next().ToString();
		}
	}
}