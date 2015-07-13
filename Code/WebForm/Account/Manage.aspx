<%@ Page Title="管理帐户" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Manage.aspx.cs" Inherits="WebForm.Account.Manage" %>

<%@ Register Src="~/Account/OpenAuthProviders.ascx" TagPrefix="uc" TagName="OpenAuthProviders" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <h2><%: Title %>.</h2>

    <div>
        <asp:PlaceHolder runat="server" ID="successMessage" Visible="false" ViewStateMode="Disabled">
            <p class="text-success"><%: SuccessMessage %></p>
        </asp:PlaceHolder>
    </div>

    <div class="row">
        <div class="col-md-12">
            <div class="form-horizontal">
                <h4>更改你的帐户设置</h4>
                <hr />
                <dl class="dl-horizontal">
                    <dt>密码:</dt>
                    <dd>
                        <asp:HyperLink NavigateUrl="/Account/ManagePassword" Text="[更改]" Visible="false" ID="ChangePassword" runat="server" />
                        <asp:HyperLink NavigateUrl="/Account/ManagePassword" Text="[创建]" Visible="false" ID="CreatePassword" runat="server" />
                    </dd>
                    <dt>外部登录名:</dt>
                    <dd><%: LoginsCount %>
                        <asp:HyperLink NavigateUrl="/Account/ManageLogins" Text="[管理]" runat="server" />

                    </dd>
                    <%--
                        电话号码可以用作双因素身份验证系统中的第二个验证因素。
                        有关将此 ASP.NET 应用程序设置为使用 SMS 支持双因素身份验证的详细信息，
                        请参阅<a href="http://go.microsoft.com/fwlink/?LinkId=403804">本文</a>。
                        请在设置双因素身份验证后取消注释以下块
                    --%>
                    <%--
                    <dt>电话号码:</dt>
                    <% if (HasPhoneNumber)
                       { %>
                    <dd>
                        <asp:HyperLink NavigateUrl="/Account/AddPhoneNumber" runat="server" Text="[添加]" />
                    </dd>
                    <% }
                       else
                       { %>
                    <dd>
                        <asp:Label Text="" ID="PhoneNumber" runat="server" />
                        <asp:HyperLink NavigateUrl="/Account/AddPhoneNumber" runat="server" Text="[更改]" /> &nbsp;|&nbsp;
                        <asp:LinkButton Text="[删除]" OnClick="RemovePhone_Click" runat="server" />
                    </dd>
                    <% } %>
                    --%>

                    <dt>双重身份验证:</dt>
                    <dd>
                        <p>
                            未配置双因素身份验证提供程序。有关将此 ASP.NET 应用程序设置为支持双因素身份验证的详细信息，
                            请参阅<a href="http://go.microsoft.com/fwlink/?LinkId=403804">本文</a>。
                        </p>
                        <% if (TwoFactorEnabled)
                          { %> 
                        <%--
                        已启用
                        <asp:LinkButton Text="[禁用]" runat="server" CommandArgument="false" OnClick="TwoFactorDisable_Click" />
                        --%>
                        <% }
                          else
                          { %> 
                        <%--
                        已禁用
                        <asp:LinkButton Text="[启用]" CommandArgument="true" OnClick="TwoFactorEnable_Click" runat="server" />
                        --%>
                        <% } %>
                    </dd>
                </dl>
            </div>
        </div>
    </div>

</asp:Content>
