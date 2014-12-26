using Aspose.Email.Mail;
using Aspose.Words;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Aspose.EmailProcessing.Library
{
    public class Common
    {
        public static int MaxNumberOfMessages = 100;
        /// <summary>
        /// Converts an instance of Aspose.Email.Mail.MailMessage to Aspose.Words.Document class
        /// </summary>
        /// <param name="message"></param>
        /// <param name="includeAttachments"></param>
        /// <returns></returns>
        public static Aspose.Words.Document MailMessageToDocumentConverter(Aspose.Email.Mail.MailMessage message, bool includeAttachments)
        {
            MemoryStream msgStream = new MemoryStream();
            message.Save(msgStream, MailMessageSaveType.MHtmlFormat);
            msgStream.Position = 0;

            // Load the MHTML stream using Aspose.Words for .NET
            Document msgDocument = new Document(msgStream);
            return msgDocument;
        }

        internal static string FormatSubject(string subject)
        {
            return (subject.Length <= 60) ? subject : subject.Substring(0, 60) + "...";
        }

        internal static string FormatSender(MailAddress mailAddress)
        {
            return (mailAddress.DisplayName.Trim().Length > 0) ? mailAddress.DisplayName : mailAddress.Address;
        }

        internal static string FormatDate(DateTime dateTime)
        {
            string result = dateTime.ToString();
            
            // If today, then just display the time
            if (dateTime.Date == DateTime.Now.Date)
                result = dateTime.ToString("t");
            // If this year, then display month name and day of month
            else if (dateTime.Year == DateTime.Now.Year)
                result = dateTime.ToString("MMM d");
            // For previous year and all other emails, display mm/dd/yy
            else
                result = dateTime.ToString("M/d/y");
            
            return result;
        }



        internal static Email.MailQuery GetMailQuery(string searchText)
        {
            Email.MailQuery mQuery = null;
            if (searchText.Trim().Length > 0)
            {
                Aspose.Email.MailQueryBuilder queryBuilder = new Aspose.Email.MailQueryBuilder();
                mQuery = queryBuilder.Or(queryBuilder.Body.Contains(searchText, true), queryBuilder.Cc.Contains(searchText, true));
                mQuery = queryBuilder.Or(mQuery, queryBuilder.From.Contains(searchText, true));
                mQuery = queryBuilder.Or(mQuery, queryBuilder.To.Contains(searchText, true));
                mQuery = queryBuilder.Or(mQuery, queryBuilder.Subject.Contains(searchText, true));
            }

            return mQuery;
        }

        internal static string formatImapFolderName(string folderName)
        {
            int lastIndex = folderName.LastIndexOf("/");

            string formattedFolderName = folderName;
            if (lastIndex != -1)
                formattedFolderName = folderName.Substring(lastIndex + 1);

            return formattedFolderName;
        }
    }
}