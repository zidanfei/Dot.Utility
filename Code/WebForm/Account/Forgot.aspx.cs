using System;
using System.Web;
using System.Web.UI;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Owin;
using WebForm.Models;

namespace WebForm.Account
{
    public partial class ForgotPassword : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void Forgot(object sender, EventArgs e)
        {
            if (IsValid)
            {
                // 验证用户的电子邮件地址
                var manager = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();
                ApplicationUser user = manager.FindByName(Email.Text);
                if (user == null || !manager.IsEmailConfirmed(user.Id))
                {
                    FailureText.Text = "用户不存在或未确认。";
                    ErrorMessage.Visible = true;
                    return;
                }
                // 有关如何启用帐户确认和密码重置的详细信息，请访问 http://go.microsoft.com/fwlink/?LinkID=320771
                // 发送包含此代码和重定向到“重置密码”页的电子邮件
                //string code = manager.GeneratePasswordResetToken(user.Id);
                //string callbackUrl = IdentityHelper.GetResetPasswordRedirectUrl(code, Request);
                //manager.SendEmail(user.Id, "重置密码", "请通过单击 <a href=\"" + callbackUrl + "\">此处</a> 来重置你的密码。");
                loginForm.Visible = false;
                DisplayEmail.Visible = true;
            }
        }
    }
}