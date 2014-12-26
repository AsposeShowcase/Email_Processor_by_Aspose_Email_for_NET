# **Introduction**

**Email Processor by Aspose for .NET** lets you view your emails from standard Exchange, IMAP and POP3 servers. You can export or convert the emails to PDF, DOCX, PST, TIFF or XPS. It is developed in ASP.NET, HTML5 and jQuery and supports the latest versions of major browsers like Firefox, Chrome, Internet Explorer.

# Features

*   Connect to Exchange, IMAP or POP3 mail servers like hosted Exchange, Gmail etc
*   List folders and sub folders
*   List email messages from any folder
*   Search emails
*   View the email message with HTML formatting
*   View and download the attachments in email
*   Export a folder or selected messages to PST
*   Convert selected messages to variety of formats like PDF, XPS, DOCX and TIFF.

## Login

You can connect to any Exchange, IMAP or POP3 server.

### Exchange Server

For connection to Exchange server, following details are required.

1.  **Server URL:** Usually, this is in the format of https://exchange.**domain.com**/ews/exchange.asmx and you can replace domain.com with your company's domain name
2.  **Username:** Your Exchange server email address
3.  **Password:** Your password
4.  **Domain:** This is optional, you can leave it empty if you provided email address in username

### IMAP Server

For connection to an IMAP server, it requires the following details

1.  **Server URL:** Your IMAP server URL.
2.  **Username:** Your email address
3.  **Password:** Your password
4.  **SSL Enabled:** If your server requires SSL, then enable it and specify the port number. Typically, this is 993

### POP3 Server

POP3 also requires the similar information as the IMAP server like URL, username, password and SSL port (optional).

**Note:** _POP3 servers are very rarely used these days, as it is based on older protocol. If you download a message, it gets deleted on the server by default. IMAP or Exchange servers are commonly used, as they provide better email sync and email management features._

![Login to Exchange, IMAP or POP3 mail server](http://download-codeplex.sec.s-msft.com/Download?ProjectName=emailprocessor&DownloadId=1265792)

## Instructions for Gmail Users

You can also access your Gmail account using the IMAP or POP3 option. But Gmail does not allow IMAP or POP3 access by default, you need to enable it in **Settings**. Moreover, it does not allow external apps or websites to access the emails, to solve this, you also need to allow access using [ this link](https://www.google.com/settings/security/lesssecureapps).

### Step 1: Enable IMAP/POP3

Open the Gmail settings.

![Gmail settings](https://www.codeplex.com/Download?ProjectName=emailprocessor&amp;DownloadId=1279408)

On the Settings page, click on Forwarding and POP/IMAP. Enable IMAP, if you want to access Gmail using IMAP method (recommended). Enable POP3, if you want to access with this method.

![Enable IMAP and POP](https://www.codeplex.com/Download?ProjectName=emailprocessor&amp;DownloadId=1279471)

### Step 2: Allow access to less secure apps

Open&nbsp;[https://www.google.com/settings/security/lesssecureapps](https://www.google.com/settings/security/lesssecureapps "Enable less secure apps")&nbsp;and choose **Enable**.

![Enable less secure apps](https://www.codeplex.com/Download?ProjectName=emailprocessor&amp;DownloadId=1279528)

## List Folders

Once you are connected to a mail server, it will show you the list of folders and sub folders (if any).

![Get folders list from mail server](https://www.codeplex.com/Download?ProjectName=emailprocessor&amp;DownloadId=1266110)

## Get Emails

To get the latest emails, click on any folder e.g. Inbox, Sent Items etc. it will show you the list of emails.

![Read emails from Inbox or other folder](https://www.codeplex.com/Download?ProjectName=emailprocessor&amp;DownloadId=1266224)

## Export to PST

You can export the emails to a PST file. The PST will be downloaded to your local drive and you can open it in Microsoft Outlook. You can either add the whole folder or select specific emails, to be included in the PST file.

![Export emails to PST](https://www.codeplex.com/Download?ProjectName=emailprocessor&amp;DownloadId=1268685)

## Convert to PDF, DOCX, XPS and TIFF

You can also convert the selected messages to PDF, DOCX, XPS and TIFF. After conversion, the file will be downloaded to your local drive.

If you select multiple emails, they will be concatenated in one output document. And in case of TIFF conversion, it will have multiple pages, if you select large or multiple emails.

![Convert email to PDF, DOCX, TIFF or XPS](https://www.codeplex.com/Download?ProjectName=emailprocessor&amp;DownloadId=1268863)

## View Email

Click on any email from the list to view it. The message will retain the HTML formatting, CSS styles and embedded images.

![View email message](https://www.codeplex.com/Download?ProjectName=emailprocessor&amp;DownloadId=1269344)

## Download Email Attachments

Attachments (if any) will be listed on top of the message. Click on an attachment to download it to your local drive.

![Download attachments from email](https://www.codeplex.com/Download?ProjectName=emailprocessor&amp;DownloadId=1269660)

## Search Emails

You can search the emails by entering the text in the top textbox. It matches the search text in subject, to, from and body of the email.

![Search emails](https://www.codeplex.com/Download?ProjectName=emailprocessor&amp;DownloadId=1270017)

# Support and Feedback

Please feel free to ask any questions or share feedback here.

If you have any query related to the [ Aspose components](http://www.aspose.com/.net/total-component.aspx "Aspose components"), please use our [ forums](http://www.aspose.com/community/forums/default.aspx "Aspose forums").