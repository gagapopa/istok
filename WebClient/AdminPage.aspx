<%@ Page Language="C#" MasterPageFile="~/MainTemplate.Master" AutoEventWireup="true" CodeBehind="AdminPage.aspx.cs" Inherits="WebClient.AdminPage" Title="Администрирование." Theme="GlobalSkin" %>

<asp:Content ID="ContentAdmin" ContentPlaceHolderID="MainContent" runat="server">
        <br />
        <br />
        <table style="border-bottom-style:solid; border-top-style: solid;">
            <tr>
                <td>
                    <asp:Button ID="LoadIconsBtn" runat="server" Text="Обновить иконки" 
                        onclick="LoadIconsBtn_Click" Width="245px"/>
                </td>
                <td class="separator">
                    В случае добалвения новых типов на сервер или изменения пиктограмм уже имеющихся, 
                    изображения не будут обновлены, что негативно скажется на информативности. Для 
                    обновления пиктограм типов используйте эту кнопку.</td>
            </tr>
            <tr>
                <td>
                    <asp:Button ID="ClearTempGraphImageBtn" runat="server" 
                        Text="Удалить временные файлы" onclick="ClearTempGraphImageBtn_Click" 
                        Width="245px" />
                </td>
                <td class="separator">
                    В случае аварийного завершения работы веб-сервера, некоторые временные данные 
                    могут быть не удалены. Для удаления воспользуйтесь этой кнопкой. Внимание, 
                    настоятельное рекомендуется производит удаление при отсутсвии открытых 
                    подключений к веб-серверу.</td>
            </tr>
        </table>
</asp:Content>
