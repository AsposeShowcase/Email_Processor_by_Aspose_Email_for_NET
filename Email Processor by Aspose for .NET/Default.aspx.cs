using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Aspose.EmailProcessing.Library;
using System.Net;
using Aspose.Email.Exchange;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using Aspose.Email.Imap;

namespace Aspose.EmailProcessing
{
    public partial class Default1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (HttpContext.Current.Session[Constants.MailHelperSession] != null)
            {
                HttpContext.Current.Session.Remove(Constants.MailHelperSession);
            }

            if (IsPostBack == false)
            {
                TestTreeview();
            }
            
        }

        private void TestTreeview()
        {
            //TestExchangeClient();
            //TestImapClient();
        }

        private void TestImapClient()
        {
            // Authentication information
            string ServerURL = "exchange.aspose.com";
            bool SSLEnabled = true;
            int SSLPort = 993;
            string Username = "saqib.razzaq@aspose.com";
            string Password = "mrooscancer";

            // Connect to the Imap server
            ImapClient client = new ImapClient(ServerURL, (SSLEnabled ? SSLPort : 143), Username, Password);
            if (SSLEnabled)
            {
                client.EnableSsl = true;
                client.SecurityMode = Aspose.Email.Imap.ImapSslSecurityMode.Implicit; // set security mode
            }

            try
            {
                ImapFolderInfoCollection folderInfoColl = client.ListFolders();
                PopulateImapFolders(client, folderInfoColl, tvFolders.Nodes);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void PopulateImapFolders(ImapClient client, ImapFolderInfoCollection folderInfoColl, TreeNodeCollection nodes)
        {
            // Add the current collection to the node
            foreach (ImapFolderInfo folderInfo in folderInfoColl)
            {
                int msgCount = 0;
                if (folderInfo.Name.ToUpper().Contains("AIB"))
                {
                    client.SelectFolder(folderInfo.Name);
                    msgCount = client.ListMessages(10).Count;
                }
                TreeNode node = new TreeNode(Common.formatImapFolderName(folderInfo.Name) + "(" + msgCount + ")", folderInfo.Name);
                nodes.Add(node);
                // If this folder has children, add them as well
                if (folderInfo.Selectable == true)
                {
                    PopulateImapFolders(client, client.ListFolders(folderInfo.Name), node.ChildNodes);
                }
            }
        }

        private void TestExchangeClient()
        {
            // Authentication information
            string ServerURL = "https://exchange.aspose.com/ews/exchange.asmx";
            string Username = "saqib.razzaq@aspose.com";
            string Password = "mrooscancer";
            string Domain = "";

            // Connection to Exchange server
            NetworkCredential credentials = new NetworkCredential(Username, Password, Domain);
            IEWSClient client = EWSClient.GetEWSClient(ServerURL, credentials);
            // Register callback method for SSL validation event
            ServicePointManager.ServerCertificateValidationCallback += RemoteCertificateValidationHandler;

            try
            {
                ExchangeMailboxInfo mailboxInfo = client.GetMailboxInfo();
                string rootUri = client.GetMailboxInfo().RootUri;
                // List all the folders from Exchange server
                PopulateFoldersExchangeClient(client, rootUri, tvFolders.Nodes);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        private void PopulateFoldersExchangeClient(IEWSClient client, string folderUri, TreeNodeCollection nodes)
        {
            // Get subfolders of the current URI
            ExchangeFolderInfoCollection folderInfoCollection = client.ListSubFolders(folderUri);

            // Add the subfolders to the current tree node
            foreach (ExchangeFolderInfo folderInfo in folderInfoCollection)
            {
                TreeNode node = new TreeNode(folderInfo.DisplayName + "(" + folderInfo.ChildFolderCount + ")", folderInfo.Uri);
                nodes.Add(node);
                // If there are subfolders, call function recursively to add its subfolders
                if (folderInfo.ChildFolderCount > 0)
                {
                    PopulateFoldersExchangeClient(client, folderInfo.Uri, node.ChildNodes);
                }
            }
        }

        private static bool RemoteCertificateValidationHandler(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true; //ignore the checks and go ahead
        }
    }
}