<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProgressBar.ascx.cs" Inherits="WebClient.ProgressBar" %>
<asp:ScriptManager ID="ProgressBarScriptManager" runat="server" />
<center>
    <asp:UpdatePanel ID="ProgressBarUpdatePanel" runat="server">
        <ContentTemplate>
            <asp:Table runat="server">
                <asp:TableRow runat="server">
                    <asp:TableCell ID="ProgressIndicator" runat="server"/>
                    <asp:TableCell runat="server">
                        <asp:Label ID="PercentIndicator" runat="server" Text=" "/>
                    </asp:TableCell>
                </asp:TableRow>
            </asp:Table>
        </ContentTemplate>
    </asp:UpdatePanel>
</center>