<%@ Page Title="电话号码" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AddPhoneNumber.aspx.cs" Inherits="WebForm.Account.AddPhoneNumber" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2><%: Title %>。</h2>

    <div class="form-horizontal">
        <h4>添加电话号码</h4>
        <hr />
        <asp:ValidationSummary runat="server" CssClass="text-danger" />
        <p class="text-danger">
            <asp:Literal runat="server" ID="ErrorMessage" />
        </p>
        <div class="form-group">
            <asp:Label runat="server" AssociatedControlID="PhoneNumber" CssClass="col-md-2 control-label">电话号码</asp:Label>
            <div class="col-md-10">
                <asp:TextBox runat="server" ID="PhoneNumber" CssClass="form-control" TextMode="Phone" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="PhoneNumber"
                    CssClass="text-danger" ErrorMessage="“电话号码”字段是必填字段。" />
            </div>
        </div>
        <div class="form-group">
            <div class="col-md-offset-2 col-md-10">
                <asp:Button runat="server" OnClick="PhoneNumber_Click"
                    Text="提交" CssClass="btn btn-default" />
            </div>
        </div>
    </div>
</asp:Content>
