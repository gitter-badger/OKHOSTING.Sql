using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace OKHOSTING.Sql.Net4.Web.Services
{
	/// <summary>
	/// Summary description for DataBaseService
	/// </summary>
	[WebService(Namespace = "http://tempuri.org/")]
	[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
	[System.ComponentModel.ToolboxItem(false)]
	// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
	// [System.Web.Script.Services.ScriptService]
	public class DataBaseService : System.Web.Services.WebService
	{
		private Sql.Net4.DataBase DataBase = null;

		[WebMethod]
		public int Execute(Command command)
		{
			return DataBase.Execute(command);
		}

		[WebMethod]
		public SelectResult Select(Command command)
		{
			IDataTable table = DataBase.GetDataTable(command);
			SelectResult result = new SelectResult();
			int columnCount = table.Schema.Count();
			result.ColumnNames = new string[columnCount];
			result.ColumnTypes = new string[columnCount];

			for (int row = 0; row < table.Count; row++)
			{
				result.Rows[row] = new string[columnCount];

				for (int column = 0; column < table.Schema.Count(); column++)
				{
					result.Rows[row][column] = OKHOSTING.Data.Convert.ChangeType<string>(table[row][column]);
				}
			}

			return result;
		}
	}
}
