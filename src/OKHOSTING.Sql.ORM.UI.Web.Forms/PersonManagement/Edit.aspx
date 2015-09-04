<%@ Page Title="" Language="C#" MasterPageFile="~/General.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="OKHOSTING.Sql.ORM.UI.Web.PersonManagement.Edit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
	<h1>Edit Person</h1>

	<table>
		<tr>
			<td>
				FirstName:
			</td>
			<td>
				<asp:TextBox runat="server" ID="ctrFirstName"></asp:TextBox>
			</td>
		</tr>
		<tr>
			<td>
				LastName:
			</td>
			<td>
				<asp:TextBox runat="server" ID="ctrLastName"></asp:TextBox>
			</td>
		</tr>
		<tr>
			<td>
				BirthDate:
			</td>
			<td>
				<asp:TextBox TextMode="Number" runat="server" ID="ctrBirthDate"></asp:TextBox>
				<asp:RequiredFieldValidator runat="server" ControlToValidate="" Display="Static" Text="Required" />
			</td>
		</tr>
		<tr>
			<td>
				IsAlive:
			</td>
			<td>
				<asp:CheckBox runat="server" ID="ctrIsAlive" />
			</td>
		</tr>
		<tr>
			<td colspan="2">
				<asp:Button runat="server" ID="cmdSave" Text="Save" OnClick="cmdSave_Click" />
			</td>
		</tr>
	</table>
</asp:Content>
