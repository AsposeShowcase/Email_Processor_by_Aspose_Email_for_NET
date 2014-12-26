using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Aspose.Email.Exchange;
using Aspose.Email.Mail;
using System.Web.UI.WebControls;
using System.IO;

namespace Aspose.EmailProcessing.Library
{
    public enum MailTypeEnum
    {
        ExchangeServer = 1,
        IMAP = 2,
        POP3 = 3
    }

    public class MailFolder
    {
        public MailFolder() { }
        public MailFolder(string fName, string fUri) { FolderName = fName; FolderUri = fUri; }

        public string FolderName { get; set; }
        public string FolderUri { get; set; }
    }

    public class MessageAttachment
    {
        public MessageAttachment() { }
        public MessageAttachment(string attachmentName)
        {
            AttachmentName = attachmentName;
        }

        public string AttachmentName { get; set; }
    }

    public class Message
    {
        public Message() { }
        public Message(string u, string d, string s, string f) { UniqueUri = u; Date = d; Subject = s; From = f; }

        public string UniqueUri { get; set; }
        public string Date { get; set; }
        public string Subject { get; set; }
        public string From { get; set; }
    }

    public abstract class MailHelper
    {
        public MailHelper() { }

        public object MailClient { get; set; }
        public string ServerURL { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public MailTypeEnum MailType { get; set; }

        public static MailHelper Current
        {
            get
            {
                string licenseFile = HttpContext.Current.Server.MapPath("~/App_Data/Aspose.Total.lic");
                if (File.Exists(licenseFile))
                {
                    Aspose.Email.License license = new Aspose.Email.License();
                    license.SetLicense(licenseFile);

                    Aspose.Words.License licenseWords = new Aspose.Words.License();
                    licenseWords.SetLicense(licenseFile);
                }

                if (HttpContext.Current.Session[Constants.MailHelperSession] != null)
                {
                    return (MailHelper)HttpContext.Current.Session[Constants.MailHelperSession];
                }
                else
                {
                    HttpContext.Current.Response.Redirect("~/Default.aspx");
                }

                return null;
            }
        }

        public abstract bool VerfiyCredentials();

        public abstract void PopulateFoldersList(ref Repeater treeNode);

        public abstract void PopulateFoldersList(ref TreeView treeNode);

        public abstract int ListMessagesInFolder(ref GridView gridView, string folderUri, string searchText);

        public abstract Aspose.Email.Mail.MailMessage GetMailMessage(string folderName, string UniqueUri);

        public abstract void DownloadAttachmentFromMessage(string folderName, string messageUniqueUri, string attachmentName);

        # region PST Export methods

        public abstract void ExportFolderToPST(string folderUri);

        public abstract void ExportSelectedMessagesToPST(string folderName, List<string> messagesUris);

        # endregion PST Export methods
        
        # region PDF Export methods

        public abstract void ExportFolderToPDF(string folderUri);

        public abstract void ExportSelectedMessagesToAsposeWords(string folderName, List<string> messagesUris, ExportType exportType);

        # endregion PDF Export methods
    }
}