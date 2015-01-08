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