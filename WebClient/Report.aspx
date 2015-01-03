<%@ Page Language="C#" MasterPageFile="~/TreeTemplate.Master" AutoEventWireup="true" CodeBehind="Report.aspx.cs" Inherits="WebClient.ReportPage" Theme="GlobalSkin" Culture="auto" UICulture="auto"%>

<%@ Register Assembly="FastReport.Web" Namespace="FastReport.Web" TagPrefix="frw" %>

<%@ Register src="UltimatePropertyGrid.ascx" tagname="UltimatePropertyGrid" tagprefix="uc1" %>

<asp:Content ID="Content" ContentPlaceHolderID="TreeContent" runat="server">
    <div>
        <p>Формирование отчета&nbsp;"<asp:Label ID="lblReport" runat="server" Text=""></asp:Label>"
    </p>
    <uc1:UltimatePropertyGrid ID="UltimatePropertyGrid1" runat="server" />
    <asp:Button ID="btnMake" runat="server" Text="Сформировать" onclick="btnMake_Click" />
    <asp:HyperLink ID="hl" runat="server" Text="Загрузить" Visible="false" />

    <br/>
    <br/>
    <frw:WebReport ID="frw" runat="server" Width="100%" Height="100%" OnStartReport="frw_StartReport" Visible="false" ShowRefreshButton="False" />
    </div>
</asp:Content>



