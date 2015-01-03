<%@ Page Language="C#" MasterPageFile="~/MainTemplate.Master" AutoEventWireup="true" CodeBehind="SchedulesPage.aspx.cs" Inherits="WebClient.SchedulesPage" Theme="GlobalSkin" %>

<asp:Content ID="ContentSchedule" ContentPlaceHolderID="MainContent" runat="server">
    <asp:GridView ID="ScheduleTable" runat="server">
    </asp:GridView>
    <div id="schedulestableblock">
        <center>
            <asp:Table ID="SchedulesTable" runat="server" GridLines="Both">
                <asp:TableHeaderRow>
                    <asp:TableHeaderCell>Имя расписания</asp:TableHeaderCell>
                    <asp:TableHeaderCell>Период обновления значений</asp:TableHeaderCell>
                </asp:TableHeaderRow>
            </asp:Table>
        </center>
    </div>
</asp:Content>
