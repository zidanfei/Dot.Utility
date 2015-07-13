<%@ Page Title="密码已更改" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ResetPasswordConfirmation.aspx.cs" Inherits="WebForm.Account.ResetPasswordConfirmation" Async="true" %>

<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    <h2><%: Title %>.</h2>
    <div>
        <p>已更改你的密码。单击 <asp:HyperLink ID="login" runat="server" NavigateUrl="~/Account/Login">此处</asp:HyperLink> 登录 </p>
    </div>
</asp:Content>
