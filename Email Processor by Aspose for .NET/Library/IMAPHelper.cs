using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Aspose.Email.Exchange;
using Aspose.Email.Mail;
using System.Web.UI.WebControls;
using System.IO;
using System.Net;
using Aspose.Email.Imap;
using Aspose.Email.Outlook.Pst;
using Aspose.Email.Outlook;
using Aspose.Words;

namespace Aspose.EmailProcessing.Library
{
    public class IMAPHelper : MailHelper
    {
        public bool SSLEnabled { get; set; }
        public int SSLPort { get; set; }

        public IMAPHelper(string su, string u, string p, bool ssl, int port, MailTypeEnum mte)
        {
            ServerURL = su; Username = u; Password = p; SSLEnabled = ssl; SSLPort = port; MailType = mte;
        }

        private ImapClient client
        {
            get { return (ImapClient)MailClient; }
        }

        public override bool VerfiyCredentials()
        {
            try
            {
                ImapClient client = new ImapClient(ServerURL, (SSLEnabled ? SSLPort : 143), Username, Password);
                if (SSLEnabled)
                {
                    client.EnableSsl = true;
                    client.SecurityMode = Aspose.Email.Imap.ImapSslSecurityMode.Implicit; // set security mode
                }
                //ImapFolderInfoCollection folderInfoColl = client.ListFolders();
                MailClient = client;
            }
            catch (Exception)
            {
                HttpContext.Current.Session.Remove(Constants.MailHelperSession);
                return false;
            }
            return true;
        }

        private void AddFolderToList(ref List<MailFolder> foldersList, ref ImapFolderInfoCollection folderInfoColl, string folderName)
        {
            try
            {
                var folList = (from obj in folderInfoColl where obj.Name.ToLower().Contains(folderName) select obj);
                if (folList != null)
                {
                    var folder = folList.FirstOrDefault();
                    if (folder != null)
                    {
                        foldersList.Add(new MailFolder(folder.Name, folder.Name));
                        folderInfoColl.Remove(folder);
                    }
                }
            }
            catch (Exception) { }
        }

        public override void DownloadAttachmentFromMessage(string folderName, string messageUniqueUri, string attachmentName)
        {
            if (!Directory.Exists(HttpContext.Current.Server.MapPath("~/output")))
                Directory.CreateDirectory(HttpContext.Current.Server.MapPath("~/output"));

            string outputFileName = System.Guid.NewGuid().ToString();

            // Get the message
            client.SelectFolder(folderName);
            MailMessage message = client.FetchMessage(messageUniqueUri);
            // Get the attachment
            Attachment attachment = null;
            foreach(Attachment att in message.Attachments)
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
            HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment;filename=\"" + attachment.Name + "\"");
            // ^                   ^
            // Response.AddHeader("Content-Length", new FileInfo(sFilePath).Length.ToString()); // upon you 
            HttpContext.Current.Response.WriteFile(savepath);

            HttpContext.Current.Response.Flush();
            HttpContext.Current.Response.End();
        }

        public override void PopulateFoldersList(ref Repeater repater)
        {
            try
            {
                ImapFolderInfoCollection folderInfoColl = client.ListFolders();
                List<MailFolder> foldersList = new List<MailFolder>();
                List<MailFolder> foldersList2 = new List<MailFolder>();

                // inbox
                // drafts
                // sent items
                // deleted items

                AddFolderToList(ref foldersList, ref folderInfoColl, "inbox");
                AddFolderToList(ref foldersList, ref folderInfoColl, "drafts");
                AddFolderToList(ref foldersList, ref folderInfoColl, "sent");
                AddFolderToList(ref foldersList, ref folderInfoColl, "deleted");
                AddFolderToList(ref foldersList, ref folderInfoColl, "trash");

                foldersList2 = (from list in folderInfoColl
                                select new MailFolder()
                                {
                                    FolderName = list.Name,
                                    FolderUri = list.Name
                                }).ToList();

                foldersList.AddRange(foldersList2);

                repater.DataSource = foldersList;
                repater.DataBind();
            }
            catch (System.Exception) { }
        }

        public override void PopulateFoldersList(ref TreeView tvFolders)
        {
            try
            {
                //ImapFolderInfoCollection folderInfoColl = client.ListFolders();
                //List<MailFolder> foldersList = new List<MailFolder>();
                //List<MailFolder> foldersList2 = new List<MailFolder>();

                //// inbox
                //// drafts
                //// sent items
                //// deleted items

                //AddFolderToList(ref foldersList, ref folderInfoColl, "inbox");
                //AddFolderToList(ref foldersList, ref folderInfoColl, "drafts");
                //AddFolderToList(ref foldersList, ref folderInfoColl, "sent");
                //AddFolderToList(ref foldersList, ref folderInfoColl, "deleted");
                //AddFolderToList(ref foldersList, ref folderInfoColl, "trash");

                //foldersList2 = (from list in folderInfoColl
                //                select new MailFolder()
                //                {
                //                    FolderName = list.Name,
                //                    FolderUri = list.Name
                //                }).ToList();

                //foldersList.AddRange(foldersList2);

                //tvFolders.DataSource = foldersList;
                //tvFolders.DataBind();
                ImapFolderInfoCollection folderInfoColl = client.ListFolders();
                PopulateImapFolders(client, folderInfoColl, tvFolders.Nodes);
            }
            catch (System.Exception) { }
        }

        private void PopulateImapFolders(ImapClient client, ImapFolderInfoCollection folderInfoColl, TreeNodeCollection nodes)
        {
            // Add the current collection to the node
            foreach (ImapFolderInfo folderInfo in folderInfoColl)
            {
                TreeNode node = new TreeNode(Common.formatImapFolderName(folderInfo.Name), folderInfo.Name);
                nodes.Add(node);
                // If this folder has children, add them as well
                if (folderInfo.Selectable == true)
                {
                    PopulateImapFolders(client, client.ListFolders(folderInfo.Name), node.ChildNodes);
                }
            }
        }

        public override int ListMessagesInFolder(ref GridView gridView, string folderUri, string searchText)
        {
            try
            {
                Email.MailQuery mQuery = Common.GetMailQuery(searchText);
                ImapFolderInfo folderInfoStatus = client.ListFolder(folderUri);
                client.SelectFolder(folderUri);
                ImapMessageInfoCollection msgInfoColl = null;
                if (mQuery == null)
                    msgInfoColl = client.ListMessages(Common.MaxNumberOfMessages);
                else
                    msgInfoColl = client.ListMessages(mQuery, Common.MaxNumberOfMessages);

                List<Message> messagesList = new List<Message>();

                messagesList = (from msg in msgInfoColl
                                orderby msg.Date descending
                                select new Message()
                                {
                                    UniqueUri = msg.UniqueId,
                                    Date = Common.FormatDate(msg.Date),
                                    Subject = Common.FormatSubject(msg.Subject),
                                    From = Common.FormatSender(msg.From)
                                }).ToList();

                gridView.DataSource = messagesList;
                gridView.DataBind();

                return msgInfoColl.Count;
            }
            catch (Exception) { }

            return 0;
        }

        public override MailMessage GetMailMessage(string folderName, string UniqueUri)
        {
            client.SelectFolder(folderName);
            return client.FetchMessage(UniqueUri);
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

            client.SelectFolder(folderName);
            ImapMessageInfoCollection messageInfoCollection = client.ListMessages(Common.MaxNumberOfMessages);

            foreach (ImapMessageInfo msg in messageInfoCollection)
            {
                MailMessage mail = client.FetchMessage(msg.UniqueId);
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

            client.SelectFolder(folderName);

            foreach (string mUri in messagesUrisList)
            {
                MailMessage mail = client.FetchMessage(mUri);
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

        public override void ExportFolderToPDF(string folderUri) 
        {
            if (!Directory.Exists(HttpContext.Current.Server.MapPath("~/output")))
                Directory.CreateDirectory(HttpContext.Current.Server.MapPath("~/output"));

            string outputFileName = System.Guid.NewGuid().ToString() + ".pdf";

            Document document = new Document();

            client.SelectFolder(folderUri);
            ImapMessageInfoCollection messageInfoCollection = client.ListMessages(Common.MaxNumberOfMessages);

            foreach (ImapMessageInfo msg in messageInfoCollection)
            {
                MailMessage message = client.FetchMessage(msg.UniqueId);
                Document msgDocument = Common.MailMessageToDocumentConverter(message, true);
                document.AppendDocument(msgDocument, ImportFormatMode.KeepSourceFormatting);
            }

            //save the document to Pdf file
            document.Save(HttpContext.Current.Server.MapPath(string.Format("~/output/{0}", outputFileName)), SaveFormat.Pdf);

            document.Save(HttpContext.Current.Response, outputFileName, ContentDisposition.Inline, null);
            HttpContext.Current.Response.End();
        }

        public override void ExportSelectedMessagesToAsposeWords(string folderName, List<string> messagesUris, ExportType exportType) 
        {
            if (!Directory.Exists(HttpContext.Current.Server.MapPath("~/output")))
                Directory.CreateDirectory(HttpContext.Current.Server.MapPath("~/output"));

            string outputFileName = System.Guid.NewGuid().ToString() + "." + exportType.ToString();

            Document document = new Document();

            client.SelectFolder(folderName);

            foreach (string mUri in messagesUris)
            {
                MailMessage message = client.FetchMessage(mUri);
                Document msgDocument = Common.MailMessageToDocumentConverter(message, true);
                document.AppendDocument(msgDocument, ImportFormatMode.KeepSourceFormatting);
            }

            //save the document to Pdf file
            document.Save(HttpContext.Current.Server.MapPath(string.Format("~/output/{0}", outputFileName)));

            document.Save(HttpContext.Current.Response, outputFileName, ContentDisposition.Inline, null);
            HttpContext.Current.Response.End();
        }

        # endregion PDF Export methods
    }
}