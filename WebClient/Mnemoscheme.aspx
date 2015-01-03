<%@ Page Language="C#" MasterPageFile="~/TreeTemplate.Master" AutoEventWireup="true" CodeBehind="Mnemoscheme.aspx.cs" Inherits="WebClient.Mnemoscheme" Theme="GlobalSkin"%>

<%@ Register TagPrefix="uc" TagName="UpdateIntervalControl" Src="~/UpdateIntervalControl.ascx" %>

<asp:Content ID="MnemoschemeContext" ContentPlaceHolderID="TreeContent" runat="server">
    
    <uc:UpdateIntervalControl ID="IntervalControl" runat="server" OnOnPeriodChanged="UpdatePeriodChanged"/>
    
    <asp:UpdatePanel ID="MnemoschemeUpdatePanel" runat="server" UpdateMode="Conditional">
        <ContentTemplate>    
            <asp:Timer ID="UpdateTimer" runat="server" Interval="1000" 
                ontick="UpdateTimer_Tick">
            </asp:Timer>
            <asp:Panel ID="MnemoSchemePanel" runat="server" CssClass="mnemoschemepanel">
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
