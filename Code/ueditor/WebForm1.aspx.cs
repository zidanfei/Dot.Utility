using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ueditor
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(a.Value))
            {
                a.Value = "ff";
            }
            var aa = a.Value;
            var d = Request["editorValue"];
            a.Value = d;
        }

    }
}