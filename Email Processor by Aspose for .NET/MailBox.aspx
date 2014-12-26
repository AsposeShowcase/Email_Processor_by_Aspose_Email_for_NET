<%@ Page EnableEventValidation="false" Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master" CodeBehind="MailBox.aspx.cs" Inherits="Aspose.EmailProcessing.MailBox" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        function resizeIframe(obj) {
            var iframe = document.getElementsByTagName("iframe")[0];
            //var ifrm = window.frames[0].height;
            //alert("Resize method: " + iframe.style.height);
            iframe.style.height = window.frames[0].document.body.scrollHeight + 'px';
            iframe.style.width = '800px';
            //alert(document.getElementsByTagName("iframe")[0].contentWindow.document.body.innerHTML);
            var tc = document.getElementsByTagName("iframe")[0].contentWindow.document.body;
            var ary = tc ? tc.getElementsByTagName("a") : [];
            for (var i = 0; i < ary.length; i++) {
                ary[i].setAttribute('target', '_blank');
                //Do something
            }

            
        }
    </script>
    <script type="text/javascript">
        $(document).ready(function () {
            $('.selectAllCheckBox input[type="checkbox"]').click(function (event) {  //on click
                if (this.checked) { // check select status
                    $('.selectableCheckBox input[type="checkbox"]').each(function () { //loop through each checkbox
                        this.checked = true;  //select all checkboxes with class "checkbox1"              
                    });
                } else {
                    $('.selectableCheckBox input[type="checkbox"]').each(function () { //loop through each checkbox
                        this.checked = false; //deselect all checkboxes with class "checkbox1"                      
                    });
                }
            });

            $('.tablesorterPager img').click(function (event) {  //on click
                $('.selectAllCheckBox input[type="checkbox"]').each(function () {  //on click
                    this.checked = false;
                });
            });

            $('#MessagesGridView')
            .tablesorter({
                widthFixed: true, widgets: ['zebra'], headers:
                    {
                        0: { sorter: false },
                        1: { sorter: false },
                        2: { sorter: false },
                        3: { sorter: false }
                    }
            })
            .tablesorterPager({ container: $('#pager'), size: 20, positionFixed: false });
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <br />
    <div class="container">
        <table style="width:100%;">
            <tr>
                <td class="coloredRow" colspan="2">
                    <table class="mailBoxTable">
                        <tr>
                            <td>
                                    <div class="floatLeft">
                                        <h4>Welcome</h4>
                                    </div>
                                    <div>

                                    </div>
                                    <div id="ExportButtonsDiv" runat="server" visible="true">
                                        <div class="btn-group floatRight">
                                            <button data-toggle="dropdown" type="button" class="btn btn-default btn-sm dropdown-toggle">
                                                Export to Other Formats ... &nbsp;&nbsp;&nbsp;<span class="caret"></span>
                                            </button>
                                            <ul role="menu" class="dropdown-menu">
                                                <li>
                                                    <asp:LinkButton ID="LinkButton1" OnClick="PDF_ExportSelectedLinkButton_Click" runat="server"><i class="pdf-export-icon"></i> PDF</asp:LinkButton>
                                                </li>
                                                <li>
                                                    <asp:LinkButton ID="LinkButton2" OnClick="PDF_ExportAllLinkButton_Click" runat="server"><i class="docx-export-icon"></i> DOCX</asp:LinkButton>
                                                </li>
                                                <li>
                                                    <asp:LinkButton ID="LinkButton4" OnClick="LinkButton4_Click" runat="server"><i class="jpeg-export-icon"></i> TIFF</asp:LinkButton>
                                                </li>
                                                <li>
                                                    <asp:LinkButton ID="LinkButton3" OnClick="LinkButton3_Click" runat="server"><i class="xps-export-icon"></i> XPS</asp:LinkButton>
                                                </li>
                                            </ul>
                                        </div>
                        
                                        <div class="btn-spacer floatRight"></div>

                                        <div class="btn-group floatRight">
                                            <button data-toggle="dropdown" type="button" class="btn btn-default btn-sm dropdown-toggle">
                                                <i class="pst-export-icon"></i>
                                                Export to PST &nbsp;&nbsp;&nbsp;<span class="caret"></span>
                                            </button>
                                            <ul role="menu" class="dropdown-menu">
                                                <li>
                                    
                                                    <asp:LinkButton ID="PST_ExportSelectedLinkButton" OnClick="PST_ExportSelectedLinkButton_Click" runat="server">Selected Messages</asp:LinkButton>
                                                </li>
                                                <li>
                                                    <asp:LinkButton ID="PST_ExportAllLinkButton" OnClick="PST_ExportAllLinkButton_Click" runat="server">Selected Folder</asp:LinkButton>
                                                </li>
                                            </ul>
                                        </div>
                        
                        
                                    </div>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td style="vertical-align:top">
                    <%--<asp:Repeater ID="MailFoldersRepeater" runat="server" OnItemCommand="MailFoldersRepeater_ItemCommand">
                        <HeaderTemplate>
                            <ul style="max-width: 260px;" class="nav nav-pills nav-stacked">
                        </HeaderTemplate>
                        <ItemTemplate>
                            <li id="folderRowLi" runat="server">
                                <asp:LinkButton ID="FolderNameLinkButton" runat="server" Text='<%# Eval("FolderName")%>' CommandName="ShowFolderDetails" />
                                <asp:Label ID="FolderUriLabel" runat="server" Text='<%# Eval("FolderUri")%>' Visible="false" />
                            </li>
                        </ItemTemplate>
                        <FooterTemplate>
                            </ul>
                        </FooterTemplate>
                    </asp:Repeater>--%>
                        <asp:TreeView runat="server" ID="tvFolders" CollapseImageUrl="~/Images/GlyphDown.png" 
                            ExpandImageUrl="~/Images/GlyphRight.png" NoExpandImageUrl="~/Images/blank.png" 
                            OnSelectedNodeChanged="tvFolders_SelectedNodeChanged" SelectedNodeStyle-Font-Bold="true"
                            SelectedNodeStyle-Font-Underline="true"
                            >
                        </asp:TreeView>
                    
                </td>
                <td class="rowBorderTop width80">
                    <div class="alert alert-warning" id="ErrorsDiv" runat="server" visible="false">
                        <strong>Oops!</strong>
                        <asp:Literal ID="ErrorLiteral" Text="Please select one or more messages to export" runat="server"></asp:Literal>
                    </div>
                    <div id="InfoDiv" runat="server">
                        Please select folder from the left side to view messages.
                        <br />
                        <br />
                        This sample demostrates some of very cool features provided by Aspose.Email for .NET. The features in this version includes
                        <ul>
                            <li>Connect to Exchange and IMAP servers</li>
                            <li>Fetch folders list and messages in those folders</li>
                            <li>Export selected/all messages in a folder to PST</li>
                            <li>The generated PST is can be download immediately</li>
                        </ul>
                    </div>
                    
                    <div runat="server" id="EmailViewDiv" visible="false">
                        <ul class="pager">
                            <li class="previous">
                                <asp:LinkButton ID="BackButton" OnClick="BackButton_Click" runat="server"><i class="back-arrow"></i> Back</asp:LinkButton>
                            </li>
                        </ul>
                        
                        
                        <h4><asp:Label ID="EmailView_Subject" Text="Subject of the email" runat="server"></asp:Label></h4>
                        
                        

                        <strong>
                            <asp:Label ID="EmailView_FromName" Text="From" runat="server"></asp:Label>
                        </strong>
                        <asp:Label ID="EmailView_FromAddress" Text="From address" runat="server"></asp:Label>
                        <br />

                        <asp:Label ID="EmailView_To" Text="To address" runat="server"></asp:Label>
                        <br />
                        sent at <asp:Label ID="EmailView_SentDate" Text="Today" runat="server"></asp:Label>
                        
                        <div runat="server" id="EmailAttachmentsDiv">
                            Attachments: 
                            <asp:Repeater ID="RepeaterAttachments" runat="server" OnItemCommand="RepeaterAttachments_ItemCommand">
                                <ItemTemplate>
                                    <asp:LinkButton ID="AttachmentLinkButton" runat="server" Text='<%# Eval("AttachmentName")%>' CommandName="DownloadAttachment" />
                                </ItemTemplate>
                                <SeparatorTemplate>, </SeparatorTemplate>
                            </asp:Repeater>
                        </div>

                        <iframe id="EmailBodyIframe" runat="server" frameborder="0" style="overflow-x: hidden; overflow-y: scroll"   ></iframe>

                        
                        <br />
                    </div>

                    <div id="EmailsListDiv" runat="server">
                        <div id="SearchDiv" runat="server" visible="false" class="row">
                          <div class="col-lg-6">
                            <div class="input-group">
                                
                                <asp:TextBox runat="server" ID="txtSearch" class="form-control"></asp:TextBox>
                              <span class="input-group-btn">
                                
                                <asp:Button runat="server" ID="btnSearch" class="btn btn-default" Text="Search" OnClick="btnSearch_Click" />
                              </span>
                            </div><!-- /input-group -->
                          </div><!-- /.col-lg-6 -->
                        </div><!-- /.row -->
                        <asp:GridView ID="MessagesGridView" AllowSorting="false" EmptyDataText="There are no messages in this folder." EmptyDataRowStyle-CssClass="emptyClass" 
                            GridLines="None" BorderWidth="0px" AutoGenerateColumns="False" CssClass="table table-striped" DataKeyNames="UniqueUri" 
                            ClientIDMode="Static" runat="server" OnSelectedIndexChanged="Gridview_OnSelectedIndexChanged" OnRowDataBound="Gridview_RowDataBound" >
                            <Columns>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <asp:CheckBox ID="SelectAllCheckBox" CssClass="selectAllCheckBox" runat="server" />
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:CheckBox ID="SelectedCheckBox" CssClass="selectableCheckBox" runat="server" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="From" HeaderText="From" />
                                <asp:TemplateField HeaderText="Subject">
                                    <ItemTemplate>
                                        <asp:LinkButton CommandName="Select" ID="Label1" runat="server" Text='<%# Bind("Subject") %>'></asp:LinkButton>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="Date" HeaderText="Date" />
                                
                            </Columns>

<EmptyDataRowStyle CssClass="emptyClass"></EmptyDataRowStyle>
                        </asp:GridView>

                        <div id="PagingDiv" runat="server" class="pagingOuterDiv " visible="false">
                            <asp:Panel Width="550px" ID="pager" ClientIDMode="Static" CssClass="tablesorterPager" runat="server">
                                <img src="/images/first.png" class="first" />
                                <img src="/images/prev.png" class="prev" />
                                <input type="text" class="pagedisplay" />
                                <img src="/images/next.png" class="next" />
                                <img src="/images/last.png" class="last" />
                                <div class="recordsToDisplay">
                                    <label>Records to Display</label>
                                    <select class="pagesize">
                                        <option value="5">5</option>
                                        <option value="10">10</option>
                                        <option selected="selected" value="30">20</option>
                                        <option value="40">30</option>
                                    </select>
                                </div>
                            </asp:Panel>
                        </div>
                    </div>

                </td>
            </tr>
        </table>
    </div>
</asp:Content>
