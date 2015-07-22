using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OKHOSTING.Sql.Schema
{
	/// <summary>
	/// Represents a DataBase trigger
	/// </summary>
	public class Trigger
	{
		public int Id { get; set; }
		public Table Table { get; set; }
		public string TriggerBody { get; set; }
		public DataBaseOperation TriggerEvent { get; set; }
	}
}