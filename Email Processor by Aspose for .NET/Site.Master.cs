using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Aspose.EmailProcessing
{
    public partial class Site : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.RawUrl.ToLower().EndsWith("mailbox.aspx"))
            {
                TopHyperLink.Text = "Logout"; ;
                TopHyperLink.NavigateUrl = "~/Default.aspx";
            }
            else
            {
                TopHyperLink.Text = "Login"; ;
                TopHyperLink.NavigateUrl = "~/Login.aspx";
            }
        }
    }
}