<%@ Page Language="C#" MasterPageFile="~/TreeTemplate.Master" AutoEventWireup="true" CodeBehind="Table.aspx.cs" Inherits="WebClient.Table" Theme="GlobalSkin" Culture="auto" UICulture="auto"%>

<%@ Register TagPrefix="uc" TagName="UpdateIntervalControl" Src="~/UpdateIntervalControl.ascx" %>

<asp:Content ID="Content" ContentPlaceHolderID="TreeContent" runat="server">
    
    <uc:UpdateIntervalControl ID="IntervalUpdateControl" runat="server" 
        OnOnPeriodChanged="UpdatePeriodChanged"/>
    
    <asp:Timer ID="UpdateTimer" runat="server" Interval="1000" 
        OnTick="UpdateValues"/>
    
    <asp:Panel runat="server">
        <asp:Table ID="MonitorTable" runat="server" EnableViewState="true"/>
    </asp:Panel>
</asp:Content>
