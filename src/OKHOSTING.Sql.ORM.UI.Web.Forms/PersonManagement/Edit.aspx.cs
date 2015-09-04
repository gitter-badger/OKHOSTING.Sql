using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace OKHOSTING.Sql.ORM.UI.Web.PersonManagement
{
	public partial class Edit : System.Web.UI.Page
	{
		Person person;

		protected void Page_Load(object sender, EventArgs e)
		{
			person = new Person();
			person.Id = (System.Int32)Core.Data.Converter.ChangeType(Request.QueryString["id"], typeof(System.Int32));

			if (!IsPostBack)
			{
				DataBase.Default.Select(person);

				ctrFirstName.Text = person.FirstName;
				ctrLastName.Text = person.LastName;
				ctrBirthDate.Text = person.BirthDate.ToLongDateString();
				ctrIsAlive.Checked = person.IsAlive;
			}
		}

		protected void cmdSave_Click(object sender, EventArgs e)
		{
			person.FirstName = ctrFirstName.Text;
			person.LastName = ctrLastName.Text;
			person.BirthDate = DateTime.Parse(ctrBirthDate.Text);
			person.IsAlive = ctrIsAlive.Checked;

			DataBase.Default.Update(person);
			DropDownList ddl = new DropDownList();
			Response.Redirect("list.aspx");
		}
	}
}