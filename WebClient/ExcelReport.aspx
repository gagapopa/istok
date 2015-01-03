<%@ Page Language="C#" MasterPageFile="~/TreeTemplate.Master" AutoEventWireup="true" CodeBehind="ExcelReport.aspx.cs" Inherits="WebClient.ExcelReport" Theme="GlobalSkin" Culture="auto" UICulture="auto"%>

<asp:Content ID="Content" ContentPlaceHolderID="TreeContent" runat="server">
    <p>Формирование отчета&nbsp;"<asp:Label ID="lblReport" runat="server" Text=""></asp:Label>"
    </p>
    <table>
    <caption class="tablecaption">Период</caption>
        <tr>
            <td>С</td>
            <td>
                <asp:TextBox ID="FromDate" runat="server"></asp:TextBox>
            </td>
            <td>
                <asp:ImageButton ID="FromCalendarButton" runat="server" ImageUrl="~/Images/calendarbutton.png" CausesValidation="false" />    
            </td>
            <td></td>
            <td>
                <asp:MaskedEditExtender ID="FromDatTimeMaskedEdit" runat="server" 
                                ErrorTooltipEnabled="true" Mask="99/99/9999 99:99:99" MaskType="DateTime"
                                MessageValidatorTip="true" OnFocusCssClass="timeeditfocus" 
                                OnInvalidCssClass=".timeeditinvalid" TargetControlID="FromDate" />
                <asp:CalendarExtender ID="FromCalendar" runat="server" ClearTime="False" FirstDayOfWeek="Monday" 
                                TargetControlID="FromDate" PopupButtonID="FromCalendarButton"  Format="dd.MM.yyyy HH:mm:ss"/>
            </td>
        </tr>
        <tr>
            <td>По</td>
            <td>
                <asp:TextBox ID="ToDate" runat="server"></asp:TextBox>
            </td>
            <td>
                <asp:ImageButton ID="ToCalendarButton" runat="server" ImageUrl="~/Images/calendarbutton.png" CausesValidation="false" />    
            </td>
            <td></td>
            <td>
                <asp:MaskedEditExtender ID="ToDatTimeMaskedEdit" runat="server" 
                                ErrorTooltipEnabled="true" Mask="99/99/9999 99:99:99" MaskType="DateTime"
                                MessageValidatorTip="true" OnFocusCssClass="timeeditfocus" 
                                OnInvalidCssClass=".timeeditinvalid" TargetControlID="ToDate" />
                <asp:CalendarExtender ID="ToCalendar" runat="server" ClearTime="False" FirstDayOfWeek="Monday" 
                                TargetControlID="ToDate" PopupButtonID="ToCalendarButton" Format="dd.MM.yyyy HH:mm:ss"/>
            </td>
        </tr>
    </table>
    <asp:Button ID="btnMake" runat="server" Text="Сформировать" onclick="btnMake_Click" />
    <asp:HyperLink ID="hl" runat="server" Text="Загрузить" Visible="false" />
</asp:Content>
