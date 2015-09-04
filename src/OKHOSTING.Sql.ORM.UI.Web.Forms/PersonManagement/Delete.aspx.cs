using System;
using OKHOSTING.Sql.ORM;

namespace OKHOSTING.Sql.ORM.UI.Web.PersonManagement
{
	public partial class Delete : System.Web.UI.Page
	{
		
		protected void Page_Load(object sender, EventArgs e)
		{

		}

		protected void cmdDelete_Click(object sender, EventArgs e)
		{
			DataType<Person> dtype = DataType<Person>.GetMap();
			Person instance = new Person();

			foreach (var member in dtype.AllMemberInfos)
			{
			}

			foreach (DataMember dmember in dtype.PrimaryKey)
			{
				object value = OKHOSTING.Core.Data.Converter.ChangeType(Request.QueryString[dmember.Member.Expression], dmember.Member.ReturnType);
				dmember.Member.SetValue(instance, value);
			}

			DataBase.Default.Delete(instance);
		}

		protected void cmdCancel_Click(object sender, EventArgs e)
		{
			Response.Redirect("List.aspx");
		}
	}
}