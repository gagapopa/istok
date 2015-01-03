<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UpdateIntervalControl.ascx.cs" Inherits="WebClient.UpdateIntervalSlider" %>

<asp:UpdatePanel runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <table>
            <tr>
                <td>
                    <label>Период обновления значений</label>
                </td>
                <td>
                    <asp:TextBox ID="UpdatePeriodTextBound" runat="server" Width="25"
                        ontextchanged="UpdatePeriodTextBound_TextChanged"/>
                    <asp:ImageButton ID="ButtonDown" runat="server" AlternateText="-" ImageUrl="~/Images/down.png" Width="15" Height="15" />
                    <asp:ImageButton ID="ButtonUp" runat="server" AlternateText="+" ImageUrl="~/Images/up.png" Width="15" Height="15" />
                    <asp:NumericUpDownExtender ID="NumericUpDown" runat="server"
                        TargetControlID="UpdatePeriodTextBound"
                        TargetButtonDownID="ButtonDown"
                        TargetButtonUpID="ButtonUp"
                        ServiceDownMethod=""
                        ServiceUpMethod=""
                        Minimum="1"
                        Maximum="60" />
                </td>
                <td>
                    <label>сек</label>
                </td>
            </tr>
        </table>
    </ContentTemplate>
</asp:UpdatePanel>