using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OKHOSTING.Sql.Schema;
using OKHOSTING.Sql.Filters;
using OKHOSTING.Sql.Operations;

namespace OKHOSTING.Sql.Tests
{
	[TestClass]
	public class ExistingDataBaseMySql
	{
		public Net4.MySql.DataBase Connect()
		{
			return new Net4.MySql.DataBase() { ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["mysql"].ConnectionString };
		}

		[TestMethod]
		public void SimpleSelect()
		{
			Net4.MySql.DataBase db = Connect();
			var generator = new MySql.SqlGenerator();

			db.LoadSchema();

			Table historicoVentas = db.Schema.Tables.Where(t => t.Name == "historicoventa").Single();

			Select select = new Select();
			select.Table = historicoVentas;
			select.Columns.Add(new SelectColumn(historicoVentas["fecha"]));
			select.Columns.Add(new SelectColumn(historicoVentas["tipocambio"]));
			select.Columns.Add(new SelectColumn(historicoVentas["precio"]));

			Table cliente = db.Schema.Tables.Where(t => t.Name == "cliente").Single();

			SelectJoin joinCliente = new SelectJoin();
			joinCliente.Table = cliente;
			joinCliente.Columns.Add(new SelectColumn(cliente["RasonSocial"]));
			joinCliente.On.Add(new ColumnCompareFilter() { Column = historicoVentas["cliente"], ColumnToCompare = cliente["oid"] });

			select.Where.Add(new RangeFilter() { Column = historicoVentas["fecha"], MinValue = DateTime.Now.AddYears(-1), MaxValue = DateTime.Now });

			Command command = generator.Select(select);
			var result = db.GetDataTable(command);
		}
	}
}
