<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LoadInform.aspx.cs" Inherits="WebClient.LoadInform" Theme="GlobalSkin" %>

<%@ Register Src="~/ProgressBar.ascx" TagName="ProgressBar" TagPrefix="uc" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Удаленный сервер загружается...</title>
</head>
<body>
    <form id="formprogressinform" runat="server">
    
        <center>
            <div id="blockinform">
                <asp:UpdatePanel ID="ProgressInformPanel" runat="server">
                    <ContentTemplate>
                        <asp:Image ID="LogoImage" runat="server" ImageUrl="~/Images/wait_logo.png" CssClass="waitlogo"/>
                        <br />
                        <br />
                        <br />
                        <uc:ProgressBar ID="Progress" runat="server" CssProgress="loadprogress" CssText="informtext"/>
                        <asp:Timer ID="RefreshTimer" runat="server" Interval="1000" 
                            ontick="RefreshTimer_Tick" />
                        <asp:Label ID="SatusString" runat="server" Text=" " CssClass="informtext" />
                        <br />
                        <asp:HyperLink ID="RedirectLink" runat="server" Enabled="true" Visible="false"
                            Text="Если ваш браузер не поддерживает автоматическую переадресацию, воспользуйтесь этой ссылкой." />
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
        </center>
    
    </form>
</body>
</html>
