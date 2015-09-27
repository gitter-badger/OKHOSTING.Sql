using System;

namespace OKHOSTING.Sql.Net4.Web.Services
{
	public struct SelectResult
	{
		public string[] ColumnNames;
		public string[] ColumnTypes;
		public string[][] Rows;
	}
}