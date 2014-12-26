using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Aspose.EmailProcessing.Library;
using System.Web.UI.HtmlControls;
using Aspose.Email.Imap;
using Aspose.Email.Outlook.Pst;
using Aspose.Email.Outlook;
using Aspose.Email.Mail;

namespace Aspose.EmailProcessing
{
    public enum ExportType
    {
        PST = 1,
        PDF = 2,
        DOCX = 3,
        TIFF = 4,
        XPS = 5
    }

    public partial class MailBox : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
            ErrorsDiv.Visible = false;
            EmailViewDiv.Visible = false;
            EmailsListDiv.Visible = true;

            if (!Page.IsPostBack)
            {
                try
                {
                    //MailHelper.Current.PopulateFoldersList(ref MailFoldersRepeater);
                    MailHelper.Current.PopulateFoldersList(ref tvFolders);
                }
                catch (Exception ex)
                {
                    ErrorsDiv.Visible = true;
                    ErrorLiteral.Text = ex.ToString();
                }
            }
        }

        protected void MailFoldersRepeater_ItemCommand(Object Sender, RepeaterCommandEventArgs e)
        {
            //try
            //{
            //    if (e.CommandName == "ShowFolderDetails")
            //    {
            //        foreach (RepeaterItem item in MailFoldersRepeater.Items)
            //        {
            //            if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem)
            //            {
            //                HtmlGenericControl folderRowLi = item.FindControl("folderRowLi") as HtmlGenericControl;
            //                folderRowLi.Attributes.Remove("class");
            //            }
            //        }

            //        LinkButton FolderNameLinkButton = e.Item.FindControl("FolderNameLinkButton") as LinkButton;
            //        ViewState["SelectedFolderName"] = FolderNameLinkButton.Text;

            //        Label FolderUriLabel = e.Item.FindControl("FolderUriLabel") as Label;
            //        ViewState["SelectedFolderUri"] = FolderUriLabel.Text;

            //        HtmlGenericControl folderRowLi2 = e.Item.FindControl("folderRowLi") as HtmlGenericControl;
            //        folderRowLi2.Attributes.Add("class", "active");

            //        RenderSelectedMessages(FolderUriLabel.Text);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    ErrorsDiv.Visible = true;
            //    ErrorLiteral.Text = ex.ToString();
            //}
        }

        protected void RepeaterAttachments_ItemCommand(Object Sender, RepeaterCommandEventArgs e)
        {
            try
            {
                if (e.CommandName == "DownloadAttachment")
                {
                    LinkButton AttachmentLinkButton = e.Item.FindControl("AttachmentLinkButton") as LinkButton;
                    MailHelper.Current.DownloadAttachmentFromMessage(ViewState["SelectedFolderName"].ToString(), ViewState["MessageUniqueUri"].ToString(), AttachmentLinkButton.Text);
                    
                }
            }
            catch (Exception ex)
            {
                ErrorsDiv.Visible = true;
                ErrorLiteral.Text = ex.ToString();
            }
        }

        private void RenderSelectedMessages(string folderUri)
        {
            ViewState["SelectedFolderName"] = folderUri;
            int count = MailHelper.Current.ListMessagesInFolder(ref MessagesGridView, folderUri, txtSearch.Text);
            PagingDiv.Visible = ExportButtonsDiv.Visible = (count > 0) ? true : false;

            InfoDiv.Visible = false;
            SearchDiv.Visible = true;

            if (MessagesGridView.Rows.Count > 0)
            {
                MessagesGridView.UseAccessibleHeader = true;
                MessagesGridView.HeaderRow.TableSection = TableRowSection.TableHeader;
                //MessagesGridView.FooterRow.TableSection = TableRowSection.TableFooter;
            }
        }

        protected void LogoutButton_Click(object sender, EventArgs e)
        {
            if (HttpContext.Current.Session[Constants.MailHelperSession] != null)
            {
                HttpContext.Current.Session.Remove(Constants.MailHelperSession);
            }
            HttpContext.Current.Response.Redirect("~/Default.aspx");
        }

        private void ExportSelected(ExportType type)
        {
            try
            {
                ViewState["SelectedFolderName"] = tvFolders.SelectedNode.Text;
                ViewState["SelectedFolderUri"] = tvFolders.SelectedValue;
            
                List<string> messagesList = new List<string>();

                foreach (GridViewRow row in MessagesGridView.Rows)
                {
                    if (row.RowType == DataControlRowType.DataRow)
                    {
                        CheckBox chkRow = (row.Cells[0].FindControl("SelectedCheckBox") as CheckBox);
                        if (chkRow.Checked)
                        {
                            string UniqueUri = MessagesGridView.DataKeys[row.RowIndex].Value.ToString();
                            messagesList.Add(UniqueUri);
                        }
                    }
                }

                if (messagesList.Count() > 0)
                {
                    ErrorsDiv.Visible = false;

                    if (type == ExportType.PST)
                    {
                        MailHelper.Current.ExportSelectedMessagesToPST(ViewState["SelectedFolderName"].ToString(), messagesList);
                    }
                    // else if (type == ExportType.PDF)
                    else // Any format which involves Aspose.Words for .NET
                    {
                        MailHelper.Current.ExportSelectedMessagesToAsposeWords(ViewState["SelectedFolderName"].ToString(), messagesList, type);
                    }
                }
                else
                {
                    // Binding again to make sure paging works
                    RenderSelectedMessages(ViewState["SelectedFolderUri"].ToString());
                    ErrorsDiv.Visible = true;
                    ErrorLiteral.Text = "Please select one or more messages to export";
                }
            }
            catch (Exception ex)
            {
                ErrorsDiv.Visible = true;
                ErrorLiteral.Text = ex.ToString();
            }
        }

        private void ExportAll(ExportType type)
        {
            try
            {
                ViewState["SelectedFolderName"] = tvFolders.SelectedNode.Text;
                ViewState["SelectedFolderUri"] = tvFolders.SelectedValue;

                if (type == ExportType.PST)
                {
                    MailHelper.Current.ExportFolderToPST(ViewState["SelectedFolderUri"].ToString());
                }
                else if (type == ExportType.PDF)
                {
                    MailHelper.Current.ExportFolderToPDF(ViewState["SelectedFolderUri"].ToString());
                }

                
            }
            catch (Exception ex)
            {
                ErrorsDiv.Visible = true;
                ErrorLiteral.Text = ex.ToString();
            }
        }

        protected void PST_ExportSelectedLinkButton_Click(object sender, EventArgs e)
        {
            ExportSelected(ExportType.PST);
        }

        protected void PST_ExportAllLinkButton_Click(object sender, EventArgs e)
        {
            ExportAll(ExportType.PST);
        }

        protected void PDF_ExportSelectedLinkButton_Click(object sender, EventArgs e)
        {
            ExportSelected(ExportType.PDF);
        }

        protected void PDF_ExportAllLinkButton_Click(object sender, EventArgs e)
        {
            ExportSelected(ExportType.DOCX);
        }

        protected void LinkButton4_Click(object sender, EventArgs e)
        {
            ExportSelected(ExportType.TIFF);
        }

        protected void LinkButton3_Click(object sender, EventArgs e)
        {
            ExportSelected(ExportType.XPS);
        }

        protected void BackButton_Click(object sender, EventArgs e)
        {
            // Show the messages list
            EmailsListDiv.Visible = true;
            // Hide the view message dic
            EmailViewDiv.Visible = false;
            // Untick the checkbox
            CheckBox chkbox = MessagesGridView.SelectedRow.Cells[0].FindControl("SelectedCheckBox") as CheckBox;
            chkbox.Checked = false;
        }
        
        protected void Gridview_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            //Accessing BoundField Column
            string UniqueUri = MessagesGridView.DataKeys[MessagesGridView.SelectedIndex].Value.ToString();
            Aspose.Email.Mail.MailMessage message = 
                MailHelper.Current.GetMailMessage(ViewState["SelectedFolderName"].ToString(), UniqueUri);

            // Add the unique uri of the message to viewstate
            ViewState["MessageUniqueUri"] = UniqueUri;
            
            // Set subject
            EmailView_Subject.Text = message.Subject;
            
            // Set From
            EmailView_FromName.Text = message.From.DisplayName;
            EmailView_FromAddress.Text = "";
            // If Name is available, then show address in brackets
            if (message.From.DisplayName.Trim().Length > 0)
                EmailView_FromAddress.Text = "&lt;" + message.From.Address + "&gt;";
            else
                EmailView_FromName.Text = message.From.Address;

            // Set to
            EmailView_To.Text = "to ";
            int iTo = 0;
            foreach(MailAddress to in message.To)
            {
                if (iTo > 0)
                    EmailView_To.Text += ", ";

                // If it has display name, show in Name <email@domain.com> format
                if (to.DisplayName.Trim().Length > 0)
                    EmailView_To.Text += to.DisplayName + " &lt;" + to.Address + "&gt;";
                else
                    EmailView_To.Text += to.Address;

                iTo++;
            }
            
            // Set send date
            EmailView_SentDate.Text = message.Date.ToString("dddd") + ", " + message.Date.ToString();

            // Set the HTML Body
            //EmailView_MessageBody.Text = message.HtmlBody;
            EmailBodyIframe.Attributes["srcdoc"] = message.HtmlBody;
            EmailBodyIframe.Attributes["onload"] = "resizeIframe('EmailBodyIframe');";

            // Set attachments
            List<MessageAttachment> attachments = new List<MessageAttachment>();
            foreach(Attachment att in message.Attachments)
            {
                attachments.Add(new MessageAttachment(att.Name));
            }
            RepeaterAttachments.DataSource = attachments;
            RepeaterAttachments.DataBind();

            // Hide the messages list
            EmailsListDiv.Visible = false;
            // Show the view message div
            EmailViewDiv.Visible = true;
            // Tick the checkbox
            CheckBox chkbox = MessagesGridView.SelectedRow.Cells[0].FindControl("SelectedCheckBox") as CheckBox;
            chkbox.Checked = true;
        }

        protected void Gridview_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                switch (e.Row.RowType)
                {
                    case DataControlRowType.Header:
                        //...
                        break;
                    case DataControlRowType.DataRow:
                        //e.Row.Attributes["onmouseover"] = "this.style.cursor='pointer';this.style.textDecoration='underline';";
                        //e.Row.Attributes["onmouseout"] = "this.style.textDecoration='none';";
                        //e.Row.Attributes.Add("onclick", Page.ClientScript.GetPostBackEventReference(MessagesGridView, "Select$" + e.Row.RowIndex.ToString()));
                        break;
                }
            }
            catch
            {
                //...throw
            }
        }

        protected void tvFolders_SelectedNodeChanged(object sender, EventArgs e)
        {
            txtSearch.Text = "";
            RenderSelectedMessages(tvFolders.SelectedValue);
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            RenderSelectedMessages(tvFolders.SelectedValue);
        }
    }
}