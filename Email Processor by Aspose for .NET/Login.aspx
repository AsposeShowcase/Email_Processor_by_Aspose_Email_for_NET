<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="Aspose.EmailProcessing.Login" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <br />
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <div class="container">
                <div class="alert alert-warning" id="ConnectionError" runat="server" visible="false">
                    <strong>Oops!</strong> We are unable to connect to mail server using the information you have provided. Please check the information below and try again.
                </div>

                <div class="loginDiv">
                    <div class="panel panel-default">
                        <div class="panel-heading"><strong>Please enter details below to Login</strong></div>
                        <div class="panel-body">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <label for="inputAccountType" class="control-label col-xs-3">Account type</label>
                                    <div class="col-xs-8">
                                        <asp:DropDownList AutoPostBack="true" CssClass="form-control" ID="MailServerDropDownList" runat="server" OnSelectedIndexChanged="MailServerDropDownList_SelectedIndexChanged">
                                            <asp:ListItem Text="Exchange" Value="Exchange"></asp:ListItem>
                                            <asp:ListItem Text="IMAP" Value="IMAP"></asp:ListItem>
                                            <asp:ListItem Text="POP3" Value="POP3"></asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </div>
                                <div class="form-group">
                                    <label for="inputServerURL" class="control-label col-xs-3">Server URL</label>
                                    <div class="col-xs-8">
                                        <asp:TextBox ID="ServerURLTextBox" CssClass="form-control" runat="server" Text="https://exchange.domain.com/ews/exchange.asmx"></asp:TextBox>
                                    </div>
                                    <asp:RequiredFieldValidator ControlToValidate="ServerURLTextBox" ID="RequiredFieldValidator1" Display="Dynamic" SetFocusOnError="true" runat="server" ErrorMessage="*"></asp:RequiredFieldValidator>
                                </div>
                                <div class="form-group">
                                    <label for="inputUsername" class="control-label col-xs-3">Username</label>
                                    <div class="col-xs-8">
                                        <asp:TextBox ID="UsernameTextBox" runat="server" CssClass="form-control" Text="user@domain.com"></asp:TextBox>
                                    </div>
                                    <asp:RequiredFieldValidator ControlToValidate="UsernameTextBox" ID="RequiredFieldValidator2" Display="Dynamic" SetFocusOnError="true" runat="server" ErrorMessage="*"></asp:RequiredFieldValidator>
                                </div>

                                <div class="form-group">
                                    <label for="inputPassword" class="control-label col-xs-3">Password</label>
                                    <div class="col-xs-8">
                                        <asp:TextBox ID="PasswordTextBox" TextMode="Password" runat="server" CssClass="form-control"></asp:TextBox>
                                    </div>
                                    <asp:RequiredFieldValidator ControlToValidate="PasswordTextBox" ID="RequiredFieldValidator3" Display="Dynamic" SetFocusOnError="true" runat="server" ErrorMessage="*"></asp:RequiredFieldValidator>
                                </div>

                                <div class="form-group" id="DomainRow" runat="server">
                                    <label for="inputDomain" class="control-label col-xs-3">Domain</label>
                                    <div class="col-xs-8">
                                        <asp:TextBox ID="DomainTextBox" runat="server" CssClass="form-control"></asp:TextBox>
                                    </div>
                                    <asp:RequiredFieldValidator ValidationGroup="DoNotCheck" ControlToValidate="DomainTextBox" ID="RequiredFieldValidator4" Display="Dynamic" SetFocusOnError="true" runat="server" ErrorMessage="*"></asp:RequiredFieldValidator>
                                </div>
                                <div class="form-group" id="SSLEnabledRow" visible="false" runat="server">
                                    <label for="inputSSL" class="control-label col-xs-3">SSL Enabled</label>
                                    <div class="col-xs-8">
                                        <div class="checkbox">
                                            <asp:CheckBox Checked="true" ID="SSLEnabledCheckBox" runat="server" AutoPostBack="true" OnCheckedChanged="SSLEnabledCheckBox_CheckedChanged"></asp:CheckBox>
                                        </div>
                                    </div>
                                </div>
                                <div class="form-group" id="SSLPortDiv" runat="server" visible="false">
                                    <label for="inputDomain" class="control-label col-xs-3">SSL Port</label>
                                    <div class="col-xs-8">
                                        <asp:TextBox ID="SSLPortTextBox" Text="993" CssClass="form-control" runat="server"></asp:TextBox>
                                    </div>
                                    <asp:RequiredFieldValidator ControlToValidate="SSLPortTextBox" ID="RequiredFieldValidator5" Display="Dynamic" SetFocusOnError="true" runat="server" ErrorMessage="*"></asp:RequiredFieldValidator>
                                </div>
                                <hr />
                                <div class="form-group">
                                    <label for="inputlogin" class="control-label col-xs-3 visibilityHidden">login</label>
                                    <div class="col-xs-8">
                                        <asp:Button ID="LoginButton" runat="server" Text="Login" OnClick="LoginButton_Click"></asp:Button>
                                        &nbsp;&nbsp;&nbsp;
                                        <a href="Gmail-Users-Help.aspx" target="_blank">Sign-in Help for Gmail Users</a>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:UpdateProgress ID="UpdateProgress1" AssociatedUpdatePanelID="UpdatePanel1" runat="server">
        <ProgressTemplate>
            <div class="overlay">
                <div class="overlayContent">
                    <div class="contentInnerDiv">
                        <img src="Images/ajax-loader.gif" alt="Loading" class="floatLeft" />
                        <div class="pleaseWait">Please wait ...</div>
                    </div>
                </div>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
</asp:Content>
