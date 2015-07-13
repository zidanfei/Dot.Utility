<%@ Page Title="帐户确认" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Confirm.aspx.cs" Inherits="WebForm.Account.Confirm" Async="true" %>

<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    <h2><%: Title %>。</h2>

    <div>
        <asp:PlaceHolder runat="server" ID="successPanel" ViewStateMode="Disabled" Visible="true">
            <p>
                感谢你确认帐户。单击 <asp:HyperLink ID="login" runat="server" NavigateUrl="~/Account/Login">此处</asp:HyperLink>  登录             
            </p>
        </asp:PlaceHolder>
        <asp:PlaceHolder runat="server" ID="errorPanel" ViewStateMode="Disabled" Visible="false">
            <p class="text-danger">
                出现错误。
            </p>
        </asp:PlaceHolder>
    </div>
</asp:Content>
