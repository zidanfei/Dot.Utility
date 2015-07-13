<%@ Page Title="验证电话号码" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="VerifyPhoneNumber.aspx.cs" Inherits="WebForm.Account.VerifyPhoneNumber" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
 <h2><%: Title %>。</h2>
    <p class="text-danger">
        <asp:Literal runat="server" ID="ErrorMessage" />
    </p>
    <div class="form-horizontal">
        <h4>输入验证代码</h4>
        <hr />
        <asp:HiddenField runat="server" ID="PhoneNumber" />
        <asp:ValidationSummary runat="server" CssClass="text-danger" />
        <div class="form-group">
            <asp:Label runat="server" AssociatedControlID="Code" CssClass="col-md-2 control-label">代码</asp:Label>
            <div class="col-md-10">
                <asp:TextBox runat="server" ID="Code" CssClass="form-control" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="Code"
                    CssClass="text-danger" ErrorMessage="“代码”字段是必填字段。" />
            </div>
        </div>
        <div class="form-group">
            <div class="col-md-offset-2 col-md-10">
                <asp:Button runat="server" OnClick="Code_Click"
                    Text="提交" CssClass="btn btn-default" />
            </div>
        </div>
    </div>
</asp:Content>
