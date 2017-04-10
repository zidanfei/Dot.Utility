using Dot.Utility.Net;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Xml;

namespace Dot.HttpTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            string path = System.IO.Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "urls.xml");
            var root = XElement.Load(path);
            cmbTemplate.Items.Add(new { Key = "", Value = "--请选择--" });         
            foreach (var item in root.Elements("url"))
            {
                cmbTemplate.Items.Add(new { Key = item.Attribute("id").Value,
                    Value = item.Attribute("id").Value,
                    getUrl = item.Attribute("getUrl").Value,
                    userInfo = item.Attribute("userInfo").Value,
                    loginUrl = item.Attribute("loginUrl").Value,
                });
            }
            cmbTemplate.ValueMember = "Key";
            cmbTemplate.DisplayMember = "Value";
            cmbTemplate.Tag = root;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            string loginUrl = txtLoginUrl.Text;
            string conUlr = txtUrl.Text;
            string content = txtContent.Text;
            //GetHtml222(loginUrl, content, null);
            HttpHelper helper = new HttpHelper();
            CookieContainer credentials;
            var login = helper.Login(loginUrl, content, out credentials);
            var con = helper.Get(conUlr, null, null, credentials);
            txtResponse.Text = con;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cmbTemplate_SelectedIndexChanged(object sender, EventArgs e)
        {
            var root = cmbTemplate.Tag as XElement;
            var id = cmbTemplate.Text;
            var query = (from item in root.Elements("url")
                         where item.Attribute("id").Value.Equals(id, StringComparison.OrdinalIgnoreCase)
                         select new
                        {
                            id = item.Attribute("id").Value,
                            getUrl = item.Attribute("getUrl").Value,
                            userInfo = item.Attribute("userInfo").Value,
                            loginUrl = item.Attribute("loginUrl").Value,
                        }).FirstOrDefault();
            txtLoginUrl.Text = System.Web.HttpUtility.UrlDecode( query.loginUrl);
            txtContent.Text = System.Web.HttpUtility.UrlDecode(query.userInfo);
            txtUrl.Text = System.Web.HttpUtility.UrlDecode(query.getUrl); 

        }
    }
}
