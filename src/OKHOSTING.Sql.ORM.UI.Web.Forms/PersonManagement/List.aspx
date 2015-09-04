<%@ Page Title="" Language="C#" MasterPageFile="~/General.Master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="OKHOSTING.Sql.ORM.UI.Web.PersonManagement.List" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
	
	<asp:GridView ID="grdList" runat="server" AutoGenerateColumns="false">
		<Columns>
			<asp:BoundField DataField="FirstName" HeaderText="First name" />
			<asp:BoundField DataField="LastName" HeaderText="Last name" />
			<asp:BoundField DataField="BirthDate" HeaderText="Birth date" />
			<asp:BoundField DataField="IsAlive" HeaderText="Is alive?" />
			<asp:TemplateField>
				<ItemTemplate>
					<a href="Detail.aspx?id=<%# DataBinder.Eval(Container.DataItem, "Id") %>">Details</a>
				</ItemTemplate>
			</asp:TemplateField>
			<asp:TemplateField>
				<ItemTemplate>
					<a href="Delete.aspx?id=<%# DataBinder.Eval(Container.DataItem, "Id") %>">Delete</a>
				</ItemTemplate>
			</asp:TemplateField>
		</Columns>
	</asp:GridView>
</asp:Content>
