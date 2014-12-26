using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Aspose.EmailProcessing.Library;

namespace Aspose.EmailProcessing
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
                LoginButton.Attributes.Add("class", "btn btn-primary");
        }

        protected void SSLEnabledCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            SSLPortDiv.Visible = SSLEnabledCheckBox.Checked;
        }

        protected void MailServerDropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (MailServerDropDownList.SelectedValue.Equals("IMAP"))
            {
                ServerURLTextBox.Text = "imap.domain.com";
                SSLEnabledRow.Visible = true;
                DomainRow.Visible = false;
                SSLPortTextBox.Text = "993";
            }
            else if (MailServerDropDownList.SelectedValue.Equals("POP3"))
            {
                ServerURLTextBox.Text = "pop.domain.com";
                SSLEnabledRow.Visible = true;
                DomainRow.Visible = false;
                SSLPortTextBox.Text = "995";
            }
            else
            {
                ServerURLTextBox.Text = "https://exchange.domain.com/ews/exchange.asmx";
                SSLEnabledCheckBox.Checked = SSLPortDiv.Visible = SSLEnabledRow.Visible = false;
                DomainRow.Visible = true;
            }
        }

        protected void LoginButton_Click(object sender, EventArgs e)
        {
            MailTypeEnum mType = SelectedMailType;

            string serverURL = ServerURLTextBox.Text.Trim();
            string domain = DomainTextBox.Text.Trim();
            string username = UsernameTextBox.Text.Trim();
            string password = PasswordTextBox.Text.Trim();
            bool sslEnabled = SSLEnabledCheckBox.Checked;
            int sslPort = Convert.ToInt32(SSLPortTextBox.Text.Trim());

            MailHelper mailHelper = null;

            switch (mType)
            {
                case MailTypeEnum.ExchangeServer:
                    mailHelper = new ExchangeHelper(serverURL, domain, username, password, mType);
                    break;
                case MailTypeEnum.IMAP:
                    mailHelper = new IMAPHelper(serverURL, username, password, sslEnabled, sslPort, mType);
                    break;
                case MailTypeEnum.POP3:
                    mailHelper = new POP3Helper(serverURL, username, password, sslEnabled, sslPort, mType);
                    break;
                default:
                    return;
            }

            if (mailHelper.VerfiyCredentials())
            {
                Session[Constants.MailHelperSession] = mailHelper;
                Response.Redirect("~/MailBox.aspx");
            }
            else
            {
                ConnectionError.Visible = true;
            }
        }

        protected MailTypeEnum SelectedMailType
        {
            get
            {
                MailTypeEnum mType = MailTypeEnum.ExchangeServer;

                if (MailServerDropDownList.SelectedValue.Equals("IMAP"))
                    mType = MailTypeEnum.IMAP;
                else if (MailServerDropDownList.SelectedValue.Equals("POP3"))
                    mType = MailTypeEnum.POP3;

                return mType;
            }
        }
    }
}