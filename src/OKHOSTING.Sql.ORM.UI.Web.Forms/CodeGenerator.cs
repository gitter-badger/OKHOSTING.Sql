using System;
using System.Collections.Generic;
using System.IO;

namespace OKHOSTING.Sql.ORM.UI.Web.Forms
{
	public static class CodeGenerator
	{
		public static void Generate(IEnumerable<DataType> dtypes, string outputDirectory)
		{
			var session = new Dictionary<string, object>();

			foreach (DataType dtype in dtypes)
			{
				session["dtype"] = dtype;
				string directoryPath = Path.Combine(outputDirectory, dtype.InnerType.Name);

				if (!Directory.Exists(directoryPath))
				{
					Directory.CreateDirectory(directoryPath);
                }
				
				//Delete
				var deleteAspx = new Delete.aspx();
				deleteAspx.Session = session;
				File.WriteAllText(Path.Combine(directoryPath, "Delete.aspx"), deleteAspx.TransformText());

				var deleteAspxCs = new Delete.aspx_cs();
				deleteAspxCs.Session = session;
				File.WriteAllText(Path.Combine(directoryPath, "Delete.aspx.cs"), deleteAspxCs.TransformText());

				var deleteAspxDesignerCs = new Delete.aspx_designer_cs();
				deleteAspxDesignerCs.Session = session;
				File.WriteAllText(Path.Combine(directoryPath, "Delete.aspx.designer.cs"), deleteAspxDesignerCs.TransformText());

				//Detail
				var detailAspx = new Delete.aspx();
				detailAspx.Session = session;
				File.WriteAllText(Path.Combine(directoryPath, "Detail.aspx"), detailAspx.TransformText());

				var detailAspxCs = new Delete.aspx_cs();
				detailAspxCs.Session = session;
				File.WriteAllText(Path.Combine(directoryPath, "Detail.aspx.cs"), detailAspxCs.TransformText());

				var detailAspxDesignerCs = new Delete.aspx_designer_cs();
				detailAspxDesignerCs.Session = session;
				File.WriteAllText(Path.Combine(directoryPath, "Detail.aspx.designer.cs"), detailAspxDesignerCs.TransformText());

				//Edit
				var editAspx = new Delete.aspx();
				editAspx.Session = session;
				File.WriteAllText(Path.Combine(directoryPath, "Edit.aspx"), editAspx.TransformText());

				var editAspxCs = new Delete.aspx_cs();
				editAspxCs.Session = session;
				File.WriteAllText(Path.Combine(directoryPath, "Edit.aspx.cs"), editAspxCs.TransformText());

				var editAspxDesignerCs = new Delete.aspx_designer_cs();
				editAspxDesignerCs.Session = session;
				File.WriteAllText(Path.Combine(directoryPath, "Edit.aspx.designer.cs"), editAspxDesignerCs.TransformText());

				//List
				var listAspx = new Delete.aspx();
				listAspx.Session = session;
				File.WriteAllText(Path.Combine(directoryPath, "List.aspx"), listAspx.TransformText());

				var listAspxCs = new Delete.aspx_cs();
				listAspxCs.Session = session;
				File.WriteAllText(Path.Combine(directoryPath, "List.aspx.cs"), listAspxCs.TransformText());

				var listAspxDesignerCs = new Delete.aspx_designer_cs();
				listAspxDesignerCs.Session = session;
				File.WriteAllText(Path.Combine(directoryPath, "List.aspx.designer.cs"), listAspxDesignerCs.TransformText());
			}
		}
    }
}