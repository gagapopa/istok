<%@ Page Language="C#" MasterPageFile="~/TreeTemplate.Master" AutoEventWireup="true" CodeBehind="ParamView.aspx.cs" Inherits="WebClient.ParamView" Theme="GlobalSkin" Culture="auto" UICulture="auto"%>

<asp:Content ID="Content" ContentPlaceHolderID="TreeContent" runat="server">
    <table ID="tbl" runat="server">
        <tr>
            <td>
                <asp:TextBox ID="FromDate" runat="server" ></asp:TextBox>
                
                
            </td>
            <td><asp:ImageButton ID="FromCalendarButton" runat="server" ImageUrl="~/Images/calendarbutton.png" CausesValidation="false"/></td>
            <td><asp:ImageButton ID="LeftImageButton" runat="server" AlternateText="Назад" ImageUrl="~/Images/arrowleft.png" CausesValidation="false" OnClick="LeftIB_Click"/></td>
            <td><asp:ImageButton ID="RightImageButton" runat="server" AlternateText="Вперед" ImageUrl="~/Images/arrowright.png" CausesValidation="false" OnClick="RightIB_Click"/></td>
            <td><asp:ImageButton ID="GetDataButton" AlternateText="Запросить данные" runat="server" ImageUrl="~/Images/dock.png" CausesValidation="false"/></td>
            <td>
                <asp:CalendarExtender ID="FromCalendar" runat="server" ClearTime="False" FirstDayOfWeek="Monday" 
                                TargetControlID="FromDate" PopupButtonID="FromCalendarButton"  Format="dd.MM.yyyy HH:mm:ss"/>
            </td>
        </tr>
    </table>
    <asp:Table ID="ParamTable" Width="100%" runat="server" BorderStyle="Solid" BorderColor="Black" BorderWidth="1px" CellSpacing="0"/>
</asp:Content>
