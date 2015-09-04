using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace OKHOSTING.Sql.ORM.UI.Web.PersonManagement
{
	public partial class Detail : System.Web.UI.Page
	{
		Person person;

		protected void Page_Load(object sender, EventArgs e)
		{
			//person = new Person();
			//person.Id = (System.Int32)Core.Data.Converter.ChangeType(Request.QueryString["id"], typeof(System.Int32));

			//if (!IsPostBack)
			//{
			//	DataBase.Default.Select(person);

			//	lblFirstName.Text = person.FirstName;
			//	lblLastName.Text = person.LastName;
			//	lblBirthDate.Text = person.BirthDate.ToLongDateString();
			//	lblIsAlive.Text = person.IsAlive.ToString();
			//}
		}
	}
}