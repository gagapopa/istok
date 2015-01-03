<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="WebClient.Login" EnableTheming="true" Theme="GlobalSkin"%>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Авторизация.</title>
</head>
<body>
    <form id="LoginForm" runat="server">
        <center>
            <div style="border-style:solid; border-width:1px; height:200px; width:500px;">
                <br />
                <br />
                Вход в систему разрешен только авторизованным пользователям.
                <br />
                Авторизуйтесь, чтобы продолжить.
                <br />
                <br />
                <asp:Panel ID="LoginPanel" runat="server">
                    <table>
                        <tr>
                            <td style="text-align: right;">
                                Имя пользователя:
                            </td>
                            <td>
                                <asp:TextBox ID="UserName"
                                             runat="server"/>
                            </td>
                        </tr>
                        <tr>
                            <td style="text-align: right;">
                                Пароль:
                            </td>
                            <td>
                                <asp:TextBox ID="Password"
                                             runat="server" 
                                             TextMode="Password" />
                            </td>
                        </tr>
                        <tr>
                            <td style="text-align: right;">
                                <asp:Label ID="Status" 
                                           runat="server" 
                                           Text=" " />
                            </td>
                            <td style="text-align: right;">
                                <asp:Button ID="PerfomLogin"
                                            runat="server" 
                                            Text="Войти" 
                                            onclick="PerfomLogin_Click" />
                            </td>
                        </tr>
                    </table>        
                </asp:Panel>
            </div>
        </center>
    </form>
</body>
</html>