using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Aspose.Email.Exchange;
using Aspose.Email.Mail;
using System.Web.UI.WebControls;
using System.IO;
using System.Net;
using Aspose.Email.Pop3;
using Aspose.Email.Outlook.Pst;
using Aspose.Email.Outlook;
using Aspose.Words;


namespace Aspose.EmailProcessing.Library
{
    public class POP3Helper : MailHelper
    {
        public bool SSLEnabled { get; set; }
        public int SSLPort { get; set; }

        public POP3Helper(string su, string u, string p, bool ssl, int port, MailTypeEnum mte)
        {
            ServerURL = su; Username = u; Password = p; SSLEnabled = ssl; SSLPort = port; MailType = mte;
        }

        private Pop3Client client
        {
            get { return (Pop3Client)MailClient; }
        }

        public override bool VerfiyCredentials()
        {
            try
            {
                Pop3Client client = new Pop3Client(ServerURL, Username, Password);
                if (SSLEnabled)
                {
                    client.Port = SSLPort;
                    client.EnableSsl = true;
                    client.SecurityMode = Pop3SslSecurityMode.Implicit;
                }
                int messageCount = client.GetMessageCount();
                MailClient = client;
            }
            catch (Exception)
            {
                HttpContext.Current.Session.Remove(Constants.MailHelperSession);
                return false;
            }
            return true;
        }

        public override void PopulateFoldersList(ref Repeater repater)
        {
            try
            {
                // POP3 doesn’t support listing folders and fetches messages from inbox only.

                List<MailFolder> foldersList = new List<MailFolder>();

                // inbox
                // drafts
                // sent items
                // deleted items

                foldersList.Add(new MailFolder("Inbox", "Inbox"));
                repater.DataSource = foldersList;
                repater.DataBind();
            }
            catch (System.Exception) { }
        }

        public override void PopulateFoldersList(ref TreeView tvFolders)
        {
            try
            {
                // POP3 doesn’t support listing folders and fetches messages from inbox only.

                List<MailFolder> foldersList = new List<MailFolder>();

                // inbox
                // drafts
                // sent items
                // deleted items

                tvFolders.Nodes.Add(new TreeNode("Inbox", "Inbox"));
            }
            catch (System.Exception) { }
        }

        public override MailMessage GetMailMessage(string folderName, string UniqueUri)
        {
            return client.FetchMessage(Convert.ToInt32(UniqueUri));
        }

        public override void DownloadAttachmentFromMessage(string folderName, string messageUniqueUri, string attachmentName)
        {
            if (!Directory.Exists(HttpContext.Current.Server.MapPath("~/output")))
                Directory.CreateDirectory(HttpContext.Current.Server.MapPath("~/output"));

            string outputFileName = System.Guid.NewGuid().ToString();
            // Get the message
            MailMessage message = client.FetchMessage(Convert.ToInt32(messageUniqueUri));
            // Get the attachment
            Attachment attachment = null;
            foreach (Attachment att in message.Attachments)
            {
                if (att.Name == attachmentName)
                    attachment = att;
            }

            // Add the extention to the output file name
            string savepath = HttpContext.Current.Server.MapPath(string.Format("~/output/{0}", attachment.Name));
            attachment.Save(savepath);

            // send the attachment to browser
            HttpContext.Current.Response.Clear();

            if (HttpContext.Current.Request.Browser != null && HttpContext.Current.Request.Browser.Browser == "IE")
                savepath = HttpUtility.UrlPathEncode(savepath);

            // Response.Cache.SetCacheability(HttpCacheability.Public); // that's upon you
            HttpContext.Current.Response.ContentType = "application/octet-stream";
            HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment;filename=\"" + savepath + "\"");
            // ^                   ^
            // Response.AddHeader("Content-Length", new FileInfo(sFilePath).Length.ToString()); // upon you 
            HttpContext.Current.Response.WriteFile(savepath);

            HttpContext.Current.Response.Flush();
            HttpContext.Current.Response.End();
        }

        public override int ListMessagesInFolder(ref GridView gridView, string folderUri, string searchText)
        {
            try
            {
                Email.MailQuery mQuery = Common.GetMailQuery(searchText);
                List<Message> messagesList = new List<Message>();

                Pop3MessageInfoCollection msgCollection = null;
                if (mQuery == null)
                    msgCollection = client.ListMessages();
                else
                    msgCollection = client.ListMessages(mQuery);
                int messageCount = 1;
                foreach (Pop3MessageInfo msgInfo in msgCollection)
                {
                    if (messageCount > Common.MaxNumberOfMessages)
                        break;
                    //Retrieve the message in a MailMessage class
                    messagesList.Add(new Message
                        (msgInfo.UniqueId.ToString(), 
                        Common.FormatDate(msgInfo.Date), 
                        Common.FormatSubject(msgInfo.Subject),
                        Common.FormatSender(msgInfo.From)
                        ));
                    messageCount++;
                }

                gridView.DataSource = messagesList;
                gridView.DataBind();

                return messagesList.Count;
            }
            catch (Exception) { }

            return 0;
        }


        # region PST Export methods

        public override void ExportFolderToPST(string folderName)
        {
            if (!Directory.Exists(HttpContext.Current.Server.MapPath("~/output")))
                Directory.CreateDirectory(HttpContext.Current.Server.MapPath("~/output"));

            //Create PST
            string outputFileName = System.Guid.NewGuid().ToString() + ".pst";
            PersonalStorage pst = PersonalStorage.Create(HttpContext.Current.Server.MapPath(string.Format("~/output/{0}", outputFileName)), FileFormatVersion.Unicode);
            pst.RootFolder.AddSubFolder(folderName);
            FolderInfo outfolder = pst.RootFolder.GetSubFolder(folderName);

            int messageCount = client.GetMessageCount();
            for (int i = 1; i <= messageCount; i++)
            {
                //Retrieve the message in a MailMessage class
                MailMessage mail = client.FetchMessage(i);
                outfolder.AddMessage(MapiMessage.FromMailMessage(mail));
            }
            pst.Dispose();

            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.ContentType = "application/octet-stream";
            HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=" + outputFileName);
            HttpContext.Current.Response.WriteFile(HttpContext.Current.Server.MapPath(string.Format("~/output/{0}", outputFileName)));
            HttpContext.Current.Response.End();
        }

        public override void ExportSelectedMessagesToPST(string folderName, List<string> messagesUrisList)
        {
            if (!Directory.Exists(HttpContext.Current.Server.MapPath("~/output")))
                Directory.CreateDirectory(HttpContext.Current.Server.MapPath("~/output"));

            //Create PST
            string outputFileName = System.Guid.NewGuid().ToString() + ".pst";
            PersonalStorage pst = PersonalStorage.Create(HttpContext.Current.Server.MapPath(string.Format("~/output/{0}", outputFileName)), FileFormatVersion.Unicode);
            pst.RootFolder.AddSubFolder(folderName);
            FolderInfo outfolder = pst.RootFolder.GetSubFolder(folderName);

            foreach (string mUri in messagesUrisList)
            {
                MailMessage mail = client.FetchMessage(Convert.ToInt32(mUri));
                outfolder.AddMessage(MapiMessage.FromMailMessage(mail));
            }

            pst.Dispose();

            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.ContentType = "application/octet-stream";
            HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=" + outputFileName);
            HttpContext.Current.Response.WriteFile(HttpContext.Current.Server.MapPath(string.Format("~/output/{0}", outputFileName)));
            HttpContext.Current.Response.End();
        }

        # endregion PST Export methods


        # region PDF Export methods

        public override void ExportFolderToPDF(string folderName)
        {
            if (!Directory.Exists(HttpContext.Current.Server.MapPath("~/output")))
                Directory.CreateDirectory(HttpContext.Current.Server.MapPath("~/output"));

            string outputFileName = System.Guid.NewGuid().ToString() + ".pdf";

            Document document = new Document();

            int messageCount = client.GetMessageCount();
            for (int i = 1; i <= messageCount; i++)
            {
                // Retrieve the message in a MailMessage class
                MailMessage message = client.FetchMessage(i);

                Document msgDocument = Common.MailMessageToDocumentConverter(message, true);
                document.AppendDocument(msgDocument, ImportFormatMode.KeepSourceFormatting);
            }

            // save the document to Pdf file
            document.Save(HttpContext.Current.Server.MapPath(string.Format("~/output/{0}", outputFileName)), SaveFormat.Pdf);

            document.Save(HttpContext.Current.Response, outputFileName, ContentDisposition.Inline, null);
            HttpContext.Current.Response.End();
        }

        public override void ExportSelectedMessagesToAsposeWords(string folderName, List<string> messagesUrisList, ExportType exportType)
        {
            if (!Directory.Exists(HttpContext.Current.Server.MapPath("~/output")))
                Directory.CreateDirectory(HttpContext.Current.Server.MapPath("~/output"));

            string outputFileName = System.Guid.NewGuid().ToString() + "." + exportType.ToString();

            Document document = new Document();

            foreach (string mUri in messagesUrisList)
            {
                MailMessage message = client.FetchMessage(Convert.ToInt32(mUri));

                Document msgDocument = Common.MailMessageToDocumentConverter(message, true);
                document.AppendDocument(msgDocument, ImportFormatMode.KeepSourceFormatting);
            }

            //save the document to Html file
            document.Save(HttpContext.Current.Server.MapPath(string.Format("~/output/{0}", outputFileName)));

            document.Save(HttpContext.Current.Response, outputFileName, ContentDisposition.Inline, null);
            HttpContext.Current.Response.End();
        }

        # endregion PDF Export methods
    }
}