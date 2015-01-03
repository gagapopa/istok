<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DownloadPage.aspx.cs" Inherits="WebClient.DownloadPage" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Загрузка</title>
</head>
<body>
    <form id="download_form" runat="server">
        <center>
            Вы действительное желаете загрузить файл на ваш компьютер?
            <br />
            Внимание! Загрузка может занять некоторое время.
            <br />
            <asp:Button ID="Download" runat="server" onclick="Download_Click" Text="Загрузить" />
        </center>
    </form>
</body>
</html>
