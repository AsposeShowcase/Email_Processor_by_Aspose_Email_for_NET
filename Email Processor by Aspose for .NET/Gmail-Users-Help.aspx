<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Gmail-Users-Help.aspx.cs" Inherits="Aspose.EmailProcessing.Gmail_Users_Help" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <br />
    <div class="loginDiv">
                    <div class="panel panel-default">
                        <div class="panel-heading"><strong>Help for Gmail Users</strong></div>
                        <div class="panel-body">
                            <p>
                                By default, Gmail <b>blocks</b> access to <b>IMAP</b> and <b>POP3</b>. 
                                You need to <b>enable</b> it from the <b>Gmail Settings</b> in order to access your emails.
                            </p>
                        </div>

                        <div class="panel-heading"><strong>Step 1 - Enable POP/IMAP</strong></div>
                        <div class="panel-body">
                            <p>
                                Open the Gmail Settings from the menu
                            </p>
                            <p>
                                <img style="border:dotted" src="Images/01-settings.png" />
                            </p>
                            <p>
                                On the settings page, click on "Forwarding and POP/IMAP".
                            </p>
                            <p>
                                <img style="border:dotted" src="Images/02-enable-imap.png" />
                            </p>
                            <p>
                                Enable IMAP, if you want to access Gmail using IMAP method.<br />
                                Enable POP3, if you want to use POP3 method. Save settings.
                            </p>
                        </div>

                        <div class="panel-heading"><strong>Step 2 - Allow access to less secure apps</strong></div>
                        <div class="panel-body">
                            <p>
                                Open <a href="https://www.google.com/settings/security/lesssecureapps" target="_blank">https://www.google.com/settings/security/lesssecureapps</a>
                                in your browser and choose <b>Enable</b>.
                            </p>
                            <p>
                                <img style="border:dotted" src="Images/04-enable-sign-in-access.png" />
                            </p>
                        </div>
                    </div>
                </div>
</asp:Content>
