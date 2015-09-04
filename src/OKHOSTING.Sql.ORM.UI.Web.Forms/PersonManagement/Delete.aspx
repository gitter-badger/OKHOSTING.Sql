<%@ Page Title="" Language="C#" MasterPageFile="~/General.Master" AutoEventWireup="true" CodeBehind="Delete.aspx.cs" Inherits="OKHOSTING.Sql.ORM.UI.Web.PersonManagement.Delete" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
	<h1>Delete</h1>
	<asp:CheckBox runat="server" Text="I confirm I want to delete this record" />
	<br />
	<asp:Button runat="server" ID="cmdDelete" Text="Delete" OnClick="cmdDelete_Click" />
	<asp:Button runat="server" ID="cmdCancel" Text="Cancel" OnClick="cmdCancel_Click" />
</asp:Content>
