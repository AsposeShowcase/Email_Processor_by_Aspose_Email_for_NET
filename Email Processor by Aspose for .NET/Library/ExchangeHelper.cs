using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Aspose.Email.Exchange;
using Aspose.Email.Mail;
using System.Web.UI.WebControls;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Aspose.Email.Outlook.Pst;
using Aspose.Email.Outlook;
using Aspose.Words;


namespace Aspose.EmailProcessing.Library
{
    public class ExchangeHelper : MailHelper
    {
        public string Domain { get; set; }

        public ExchangeHelper(string su, string dom, string u, string p, MailTypeEnum mte)
        {
            ServerURL = su; Domain = dom; Username = u; Password = p; MailType = mte;
        }

        private IEWSClient client
        {
            get { return (IEWSClient)MailClient; }
        }

        public override bool VerfiyCredentials()
        {
            try
            {
                NetworkCredential credentials = new NetworkCredential(Username, Password, Domain);
                IEWSClient client = EWSClient.GetEWSClient(ServerURL, credentials);
                MailClient = client;
            }
            catch (Exception)
            {
                HttpContext.Current.Session.Remove(Constants.MailHelperSession);
                return false;
            }
            return true;
        }

        private void AddFolderToList(ref List<MailFolder> foldersList, ref ExchangeFolderInfoCollection folderInfoColl, string folderName)
        {
            try
            {
                var folList = (from obj in folderInfoColl where obj.DisplayName.ToLower().Contains(folderName) select obj);
                if (folList != null)
                {
                    var folder = folList.FirstOrDefault();
                    if (folder != null)
                    {
                        foldersList.Add(new MailFolder(folder.DisplayName, folder.Uri));
                        folderInfoColl.Remove(folder);
                    }
                }
            }
            catch (Exception) { }
        }

        public override MailMessage GetMailMessage(string folderName, string UniqueUri)
        {
            return client.FetchMessage(UniqueUri);
        }

        public override void DownloadAttachmentFromMessage(string folderName, string messageUniqueUri, string attachmentName)
        {
            if (!Directory.Exists(HttpContext.Current.Server.MapPath("~/output")))
                Directory.CreateDirectory(HttpContext.Current.Server.MapPath("~/output"));

            string outputFileName = System.Guid.NewGuid().ToString();

            // Get the message
            MailMessage message = client.FetchMessage(messageUniqueUri);
            
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

        public override void PopulateFoldersList(ref Repeater repater)
        {
            // Register callback method for SSL validation event
            ServicePointManager.ServerCertificateValidationCallback += RemoteCertificateValidationHandler;

            try
            {
                ExchangeMailboxInfo mailboxInfo = client.GetMailboxInfo();
                string rootUri = client.GetMailboxInfo().RootUri;
                // List all the folders from Exchange server
                ExchangeFolderInfoCollection folderInfoCollection = client.ListSubFolders(rootUri);

                List<MailFolder> foldersList = new List<MailFolder>();
                List<MailFolder> foldersList2 = new List<MailFolder>();

                AddFolderToList(ref foldersList, ref folderInfoCollection, "inbox");
                AddFolderToList(ref foldersList, ref folderInfoCollection, "drafts");
                AddFolderToList(ref foldersList, ref folderInfoCollection, "sent");
                AddFolderToList(ref foldersList, ref folderInfoCollection, "deleted");
                AddFolderToList(ref foldersList, ref folderInfoCollection, "trash");

                foreach(ExchangeFolderInfo folderInfo in folderInfoCollection)
                {
                    if (folderInfo.DisplayName.Equals("Conversation Action Settings", StringComparison.InvariantCultureIgnoreCase) ||
                        folderInfo.DisplayName.Equals("Quick Step Settings", StringComparison.InvariantCultureIgnoreCase) ||
                        folderInfo.DisplayName.Equals("Contacts", StringComparison.InvariantCultureIgnoreCase) ||
                        folderInfo.DisplayName.Equals("News Feed", StringComparison.InvariantCultureIgnoreCase) ||
                        folderInfo.DisplayName.Equals("Notes", StringComparison.InvariantCultureIgnoreCase) ||
                        folderInfo.DisplayName.Equals("Outbox", StringComparison.InvariantCultureIgnoreCase) ||
                        folderInfo.DisplayName.Equals("Suggested Contacts", StringComparison.InvariantCultureIgnoreCase) ||
                        folderInfo.DisplayName.Equals("Sync Issues", StringComparison.InvariantCultureIgnoreCase) ||
                        folderInfo.DisplayName.Equals("Tasks", StringComparison.InvariantCultureIgnoreCase) ||
                        folderInfo.DisplayName.Equals("Calendar", StringComparison.InvariantCultureIgnoreCase)
                    )
                    {
                        // do not add any of these folder
                    }
                    else
                    {
                        foldersList2.Add(new MailFolder(folderInfo.DisplayName, folderInfo.Uri));
                    }
                        
                }

                foldersList.AddRange(foldersList2);

                repater.DataSource = foldersList;
                repater.DataBind();
            }
            catch (System.Exception) { }
        }

        public override void PopulateFoldersList(ref TreeView tvFolders)
        {
            // Register callback method for SSL validation event
            ServicePointManager.ServerCertificateValidationCallback += RemoteCertificateValidationHandler;

            try
            {
                ExchangeMailboxInfo mailboxInfo = client.GetMailboxInfo();
                string rootUri = client.GetMailboxInfo().RootUri;
                // List all the folders from Exchange server
                PopulateFoldersExchangeClient(client, rootUri, tvFolders.Nodes);
                
            }
            catch (System.Exception) { }
        }

        private void PopulateFoldersExchangeClient(IEWSClient client, string folderUri, TreeNodeCollection nodes)
        {
            // Get subfolders of the current URI
            ExchangeFolderInfoCollection folderInfoCollection = client.ListSubFolders(folderUri);

            // Add the subfolders to the current tree node
            foreach (ExchangeFolderInfo folderInfo in folderInfoCollection)
            {
                if (folderInfo.DisplayName.Equals("Conversation Action Settings", StringComparison.InvariantCultureIgnoreCase) ||
                        folderInfo.DisplayName.Equals("Quick Step Settings", StringComparison.InvariantCultureIgnoreCase) ||
                        folderInfo.DisplayName.Equals("Contacts", StringComparison.InvariantCultureIgnoreCase) ||
                        folderInfo.DisplayName.Equals("News Feed", StringComparison.InvariantCultureIgnoreCase) ||
                        folderInfo.DisplayName.Equals("Notes", StringComparison.InvariantCultureIgnoreCase) ||
                        folderInfo.DisplayName.Equals("Outbox", StringComparison.InvariantCultureIgnoreCase) ||
                        folderInfo.DisplayName.Equals("Suggested Contacts", StringComparison.InvariantCultureIgnoreCase) ||
                        folderInfo.DisplayName.Equals("Sync Issues", StringComparison.InvariantCultureIgnoreCase) ||
                        folderInfo.DisplayName.Equals("Tasks", StringComparison.InvariantCultureIgnoreCase) ||
                        folderInfo.DisplayName.Equals("Calendar", StringComparison.InvariantCultureIgnoreCase)
                    )
                {
                    // do not add any of these folder
                }
                else
                {
                    TreeNode node = new TreeNode(folderInfo.DisplayName, folderInfo.Uri);
                    nodes.Add(node);
                    // If there are subfolders, call function recursively to add its subfolders
                    if (folderInfo.ChildFolderCount > 0)
                    {
                        PopulateFoldersExchangeClient(client, folderInfo.Uri, node.ChildNodes);
                    }
                }
                    
            }
        }

        private static bool RemoteCertificateValidationHandler(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true; //ignore the checks and go ahead
        }

        public override int ListMessagesInFolder(ref GridView gridView, string folderUri, string searchText)
        {
            try
            {
                Email.MailQuery mQuery = Common.GetMailQuery(searchText);
                ExchangeMessageInfoCollection msgCollection = null;
                if (mQuery == null)
                    msgCollection = client.ListMessages(folderUri, Common.MaxNumberOfMessages);
                else
                    msgCollection = client.ListMessages(folderUri, Common.MaxNumberOfMessages, mQuery);
                List<Message> messagesList = new List<Message>();

                messagesList = (from msg in msgCollection
                                orderby msg.Date descending
                                select new Message()
                                {
                                   UniqueUri = msg.UniqueUri,
                                   Date = Common.FormatDate(msg.Date),
                                   Subject = Common.FormatSubject(msg.Subject),
                                   From = Common.FormatSender(msg.From)
                                }
                               ).ToList();

                gridView.DataSource = messagesList;
                gridView.DataBind();
                gridView.Visible = true;

                return msgCollection.Count;
            }
            catch (Exception) { }
            return 0;
        }
        
        
        # region PST Export methods

        public override void ExportFolderToPST(string folderUri)
        {
            if (!Directory.Exists(HttpContext.Current.Server.MapPath("~/output")))
                Directory.CreateDirectory(HttpContext.Current.Server.MapPath("~/output"));

            ExchangeFolderInfo info = client.GetFolderInfo(folderUri);
            ExchangeFolderInfoCollection fc = new ExchangeFolderInfoCollection();

            string outputFileName = System.Guid.NewGuid().ToString() + ".pst";

            fc.Add(info);
            client.Backup(fc, HttpContext.Current.Server.MapPath(string.Format("~/output/{0}", outputFileName)), Aspose.Email.Outlook.Pst.BackupOptions.Recursive);

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

            ExchangeFolderInfo info = client.GetFolderInfo(folderUri);
            int messageCount = info.TotalCount;
            ExchangeMessageInfoCollection msgCollection = client.ListMessages(folderUri, Common.MaxNumberOfMessages);
            foreach(ExchangeMessageInfo msgInfo in msgCollection)
            {
                //Retrieve the message in a MailMessage class
                MailMessage message = client.FetchMessage(msgInfo.UniqueUri);

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