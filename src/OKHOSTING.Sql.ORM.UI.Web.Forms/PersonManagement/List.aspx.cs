using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace OKHOSTING.Sql.ORM.UI.Web.PersonManagement
{
	public partial class List : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			grdList.DataSource = DataBase.Default.Select<Person>();
			grdList.DataBind();
		}
	}
}