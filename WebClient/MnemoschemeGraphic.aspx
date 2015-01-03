<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MnemoschemeGraphic.aspx.cs" Inherits="WebClient.MnemoschemeGraphic" MasterPageFile="~/MainTemplate.Master" Theme="GlobalSkin" Culture="auto" UICulture="auto"%>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<%@ Register Assembly="ZedGraph.Web" Namespace="ZedGraph.Web" TagPrefix="zgw" %>

<asp:Content ID="ContentSchedule" ContentPlaceHolderID="MainContent" runat="server">
                        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <div id="datatab">
            <div id="datetimeblock">
                <table>
                <caption class="tablecaption">Период</caption>
                    <tr>
                        <td class="labelcell">
                            От&nbsp;
                        </td>
                        <td>&nbsp;</td>
                        <td>&nbsp;</td>
                        <td>&nbsp;</td>
                        <td>&nbsp;</td>
                        <td>&nbsp;</td>
                    </tr>
                       
                    <tr class="datetimerow">
                        <td>&nbsp;</td>
                        <td class="labelcell">
                            <label>Дата:</label>
                        </td>
                        <td>
                            <asp:TextBox ID="FromDate" CssClass="dateBox" runat="server" 
                                style="margin-bottom: 0px" />
                        </td>
                        <td>
                            <asp:ImageButton ID="FromCalendarButton" runat="server" ImageUrl="~/Images/calendarbutton.png" CausesValidation="false" />    
                        </td>
                        <td class="labelcell">
                            <label>Время:</label>
                        </td>
                        <td>
                            <asp:TextBox ID="FromTime" runat="server" />
                        </td>
                        <td class="validate_cell">
                            <asp:MaskedEditExtender ID="FromDateMasketEdit" runat="server" 
                                ErrorTooltipEnabled="true" Mask="99/99/9999" MaskType="Date"
                                MessageValidatorTip="true" OnFocusCssClass="timeeditfocus" 
                                OnInvalidCssClass=".timeeditinvalid" TargetControlID="FromDate" />
                            <asp:MaskedEditValidator ID="FromDateMasketValidator" runat="server" 
                                ControlExtender="FromDateMasketEdit" ControlToValidate="FromDate" 
                                Display="Static" EmptyValueBlurredText="*" 
                                EmptyValueMessage="Дата не может быть не определена" InvalidValueBlurredMessage="*" 
                                InvalidValueMessage="Вы указали не корруктную дату" IsValidEmpty="false" 
                                TooltipMessage="Необходимо указать дату"/>
                            <asp:CalendarExtender ID="FromCalendar" runat="server" ClearTime="true"
                                TargetControlID="FromDate" PopupButtonID="FromCalendarButton" Format="dd.MM.yyyy" />
                        
                            <asp:MaskedEditExtender ID="FromTimeMaskedEdit" runat="server"
                                ErrorTooltipEnabled="true" AcceptAMPM="false" Mask="99:99:99" 
                                MaskType="Time" MessageValidatorTip="true" 
                                OnFocusCssClass="timeeditfocus" OnInvalidCssClass=".timeeditinvalid" 
                                TargetControlID="FromTime" />
                            <asp:MaskedEditValidator ID="FromTimeMaskedValidator" runat="server" 
                                ControlExtender="FromTimeMaskedEdit" ControlToValidate="FromTime" 
                                Display="Dynamic" EmptyValueBlurredText="*" 
                                EmptyValueMessage="Время не может быть не определено" InvalidValueBlurredMessage="*" 
                                InvalidValueMessage="Вы указали не корректное время" IsValidEmpty="false" 
                                TooltipMessage="Необходимо указать время" /> 
                        </td>
                    </tr>
                        
                    <!--  -->
                        
                    <tr>
                        <td class="labelcell">
                            <asp:Label ID="ToLabel" runat="server">До</asp:Label>
                        </td>
                        <td>&nbsp;</td>
                        <td>&nbsp;</td>
                        <td>&nbsp;</td>
                        <td>&nbsp;</td>
                        <td>&nbsp;</td>
                    </tr>
                    <tr class="datetimerow">
                        <td>&nbsp;</td>
                        <td class="labelcell">
                            <asp:Label ID="ToDateLabel" runat="server">Дата:</asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="ToDate" CssClass="dateBox" runat="server" />
                        </td>
                        <td>
                            <asp:ImageButton ID="ToCalendarButton" runat="server" CausesValidation="false" ImageUrl="~/Images/calendarbutton.png" />
                        </td>
                        <td class="labelcell">
                            <asp:Label ID="ToTimeLabel" runat="server">Время:</asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="ToTime" runat="server" />
                        </td>
                        <td class="validate_cell">
                            <asp:MaskedEditExtender ID="ToDateMasketEdit" runat="server" 
                                ErrorTooltipEnabled="true" Mask="99/99/9999" MaskType="Date" 
                                MessageValidatorTip="true" OnFocusCssClass="timeeditfocus" 
                                OnInvalidCssClass=".timeeditinvalid" TargetControlID="ToDate" />
                            <asp:MaskedEditValidator ID="ToDateMasketValidator" runat="server" 
                                ControlExtender="ToDateMasketEdit" ControlToValidate="ToDate" Display="Dynamic" 
                                EmptyValueBlurredText="*" EmptyValueMessage="Дата не может быть не определена" 
                                InvalidValueBlurredMessage="*" 
                                InvalidValueMessage="Вы указали не корруктную дату" IsValidEmpty="false" 
                                TooltipMessage="Необходимо указать дату" />
                            <asp:CalendarExtender ID="ToCalendar" runat="server" ClearTime="True" 
                                TargetControlID="ToDate" PopupButtonID="ToCalendarButton"  Format="dd.MM.yyyy"/>
                                    
                            <asp:MaskedEditExtender ID="ToTimeMaskedEdit" runat="server" AcceptAMPM="false" 
                                Mask="99:99:99" MaskType="Time" MessageValidatorTip="true" 
                                OnFocusCssClass="timeeditfocus" OnInvalidCssClass=".timeeditinvalid" 
                                TargetControlID="ToTime" ErrorTooltipEnabled = "true" />
                            <asp:MaskedEditValidator ID="ToTimeMaskedEditValidator" runat="server" 
                                ControlExtender="ToTimeMaskedEdit" ControlToValidate="ToTime" Display="Dynamic" 
                                EmptyValueBlurredText="*" EmptyValueMessage="Время не может быть не определено" 
                                InvalidValueBlurredMessage="*" InvalidValueMessage="Вы указали не корректное время" 
                                IsValidEmpty="false" TooltipMessage="Необходимо указать время" />     
                        </td>
                    </tr>
                    
                    <!-- -->
                      
                    <tr>
                        <td></td>
                        <td>
                            <asp:ImageButton ID="ImageButtonSubDate" runat="server" 
                                ImageUrl="~/Images/arrowleft.png" onclick="ImageButtonSubDate_Click" 
                                ImageAlign="Right" />
                            </td>
                        <td>
                            <asp:DropDownList ID="DropDownListInterval" runat="server" 
                                CssClass="intervaldrowdown"
                                onselectedindexchanged="DropDownListInterval_SelectedIndexChanged" 
                                AutoPostBack="True" EnableViewState="true">
                                <asp:ListItem>30 минут</asp:ListItem>
                                <asp:ListItem Selected="True">1 час</asp:ListItem>
                                <asp:ListItem>4 часа</asp:ListItem>
                                <asp:ListItem>8 часов</asp:ListItem>
                                <asp:ListItem>сутки</asp:ListItem>
                                <asp:ListItem>30 суток</asp:ListItem>
                                <asp:ListItem>на выбор</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButtonAddDate" runat="server" 
                                ImageUrl="~/Images/arrowright.png" onclick="ImageButtonAddDate_Click" />
                        </td>
                        <td>
                            &nbsp;</td>
                        <td>
                            <asp:Button ID="DrawButton" runat="server" onclick="DrawButton_Click" 
                                Text="Показать" CausesValidation="true"/>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="5" />
                        <td>
                            <asp:Button ID="ClearParametersButton" runat="server" onclick="ClearButton_Click" 
                                Text="Очистить" CausesValidation="true"/>
                    </tr>
                    <tr>
                        <td></td>
                        <td></td>
                        <td></td>
                        <td></td>
                        <td></td>
                        <td>
                            <asp:CheckBox ID="chbxUseBlock" runat="server" Text="Удаленный сервер"/>
                            <br/>
                            <asp:CheckBox ID="chbxShowMarkers" runat="server" Text="Отображать маркеры" AutoPostBack="true" EnableViewState="true"/>
                        </td>
                    </tr>
                </table>
                <br />
            </div>

    
            <br />
    <asp:UpdatePanel ID="GraphPanel" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div id="graphblock">
                <zgw:ZedGraphWeb ID="Graph" runat="server" rendermode="ImageTag" onrendergraph="Graph_RenderGraph" 
                    IsImageMap="True" Width="600" Height="400">
                    <XAxis AxisColor="Black" Cross="0" CrossAuto="True" IsOmitMag="False" 
                        IsPreventLabelOverlap="True" IsShowTitle="True" IsTicsBetweenLabels="True" 
                        IsUseTenPower="False" IsVisible="True" IsZeroLine="False" MinSpace="0" Title="" 
                        Type="Linear">
                        <FontSpec Angle="0" Family="Arial" FontColor="Black" IsBold="True" 
                            IsItalic="False" IsUnderline="False" Size="14" StringAlignment="Center">
                            <Border Color="Black" InflateFactor="0" IsVisible="False" Width="1" />
                            <Fill AlignH="Center" AlignV="Center" Color="White" ColorOpacity="100" 
                                IsScaled="True" IsVisible="True" RangeMax="0" RangeMin="0" Type="None" />
                        </FontSpec>
                        <MinorGrid Color="Black" DashOff="5" DashOn="1" IsVisible="False" 
                            PenWidth="1" />
                        <MajorGrid Color="Black" DashOff="5" DashOn="1" IsVisible="False" 
                            PenWidth="1" />
                        <MinorTic Color="Black" IsInside="True" IsOpposite="True" IsOutside="True" 
                            PenWidth="1" Size="5" />
                        <MajorTic Color="Black" IsInside="True" IsOpposite="True" IsOutside="True" 
                            PenWidth="1" Size="5" />
                        <Scale Align="Center" Format="g" FormatAuto="True" IsReverse="False" Mag="0" 
                            MagAuto="True" MajorStep="1" MajorStepAuto="True" MajorUnit="Day" Max="0" 
                            MaxAuto="True" MaxGrace="0.1" Min="0" MinAuto="True" MinGrace="0.1" 
                            MinorStep="1" MinorStepAuto="True" MinorUnit="Day">
                            <FontSpec Angle="0" Family="Arial" FontColor="Black" IsBold="False" 
                                IsItalic="False" IsUnderline="False" Size="14" StringAlignment="Center">
                                <Border Color="Black" InflateFactor="0" IsVisible="False" Width="1" />
                                <Fill AlignH="Center" AlignV="Center" Color="White" ColorOpacity="100" 
                                    IsScaled="True" IsVisible="True" RangeMax="0" RangeMin="0" Type="None" />
                            </FontSpec>
                        </Scale>
                    </XAxis>
                    <Y2Axis AxisColor="Black" Cross="0" CrossAuto="True" IsOmitMag="False" 
                        IsPreventLabelOverlap="True" IsShowTitle="True" IsTicsBetweenLabels="True" 
                        IsUseTenPower="False" IsVisible="False" IsZeroLine="True" MinSpace="0" Title="" 
                        Type="Linear">
                        <FontSpec Angle="0" Family="Arial" FontColor="Black" IsBold="True" 
                            IsItalic="False" IsUnderline="False" Size="14" StringAlignment="Center">
                            <Border Color="Black" InflateFactor="0" IsVisible="False" Width="1" />
                            <Fill AlignH="Center" AlignV="Center" Color="White" ColorOpacity="100" 
                                IsScaled="True" IsVisible="True" RangeMax="0" RangeMin="0" Type="None" />
                        </FontSpec>
                        <MinorGrid Color="Black" DashOff="5" DashOn="1" IsVisible="False" 
                            PenWidth="1" />
                        <MajorGrid Color="Black" DashOff="5" DashOn="1" IsVisible="False" 
                            PenWidth="1" />
                        <MinorTic Color="Black" IsInside="True" IsOpposite="True" IsOutside="True" 
                            PenWidth="1" Size="5" />
                        <MajorTic Color="Black" IsInside="True" IsOpposite="True" IsOutside="True" 
                            PenWidth="1" Size="5" />
                        <Scale Align="Center" Format="g" FormatAuto="True" IsReverse="False" Mag="0" 
                            MagAuto="True" MajorStep="1" MajorStepAuto="True" MajorUnit="Day" Max="0" 
                            MaxAuto="True" MaxGrace="0.1" Min="0" MinAuto="True" MinGrace="0.1" 
                            MinorStep="1" MinorStepAuto="True" MinorUnit="Day">
                            <FontSpec Angle="-90" Family="Arial" FontColor="Black" IsBold="False" 
                                IsItalic="False" IsUnderline="False" Size="14" StringAlignment="Center">
                                <Border Color="Black" InflateFactor="0" IsVisible="False" Width="1" />
                                <Fill AlignH="Center" AlignV="Center" Color="White" ColorOpacity="100" 
                                    IsScaled="True" IsVisible="True" RangeMax="0" RangeMin="0" Type="None" />
                            </FontSpec>
                        </Scale>
                    </Y2Axis>
                    <FontSpec Angle="0" Family="Arial" FontColor="Black" IsBold="True" 
                        IsItalic="False" IsUnderline="False" Size="16" StringAlignment="Center">
                        <Border Color="Black" InflateFactor="0" IsVisible="False" Width="1" />
                        <Fill AlignH="Center" AlignV="Center" Color="White" ColorOpacity="100" 
                            IsScaled="True" IsVisible="True" RangeMax="0" RangeMin="0" Type="None" />
                    </FontSpec>
                    <MasterPaneFill AlignH="Center" AlignV="Center" Color="White" 
                        ColorOpacity="100" IsScaled="True" IsVisible="True" RangeMax="0" RangeMin="0" 
                        Type="Solid" />
                    <YAxis AxisColor="Black" Cross="0" CrossAuto="True" IsOmitMag="False" 
                        IsPreventLabelOverlap="True" IsShowTitle="True" IsTicsBetweenLabels="True" 
                        IsUseTenPower="False" IsVisible="True" IsZeroLine="True" MinSpace="0" Title="" 
                        Type="Linear">
                        <FontSpec Angle="-180" Family="Arial" FontColor="Black" IsBold="True" 
                           IsItalic="False" IsUnderline="False" Size="14" StringAlignment="Center">
                            <Border Color="Black" InflateFactor="0" IsVisible="False" Width="1" />
                            <Fill AlignH="Center" AlignV="Center" Color="White" ColorOpacity="100" 
                                IsScaled="True" IsVisible="True" RangeMax="0" RangeMin="0" Type="None" />
                        </FontSpec>
                        <MinorGrid Color="Black" DashOff="5" DashOn="1" IsVisible="False" 
                            PenWidth="1" />
                        <MajorGrid Color="Black" DashOff="5" DashOn="1" IsVisible="False" 
                            PenWidth="1" />
                        <MinorTic Color="Black" IsInside="True" IsOpposite="True" IsOutside="True" 
                            PenWidth="1" Size="5" />
                        <MajorTic Color="Black" IsInside="True" IsOpposite="True" IsOutside="True" 
                            PenWidth="1" Size="5" />
                        <Scale Align="Center" Format="g" FormatAuto="True" IsReverse="False" Mag="0" 
                            MagAuto="True" MajorStep="1" MajorStepAuto="True" MajorUnit="Day" Max="0" 
                            MaxAuto="True" MaxGrace="0.1" Min="0" MinAuto="True" MinGrace="0.1" 
                            MinorStep="1" MinorStepAuto="True" MinorUnit="Day">
                            <FontSpec Angle="90" Family="Arial" FontColor="Black" IsBold="False" 
                                IsItalic="False" IsUnderline="False" Size="14" StringAlignment="Center">
                                <Border Color="Black" InflateFactor="0" IsVisible="False" Width="1" />
                                <Fill AlignH="Center" AlignV="Center" Color="White" ColorOpacity="100" 
                                    IsScaled="True" IsVisible="True" RangeMax="0" RangeMin="0" Type="None" />
                            </FontSpec>
                        </Scale>
                    </YAxis>
                    <Legend IsHStack="True" IsReverse="False" IsVisible="True" Position="Top">
                        <Location AlignH="Left" AlignV="Center" CoordinateFrame="ChartFraction" 
                            Height="0" Width="0" X="0" Y="0">
                            <TopLeft X="0" Y="0" />
                            <BottomRight X="0" Y="0" />
                        </Location>
                        <FontSpec Angle="0" Family="Arial" FontColor="Black" IsBold="False" 
                            IsItalic="False" IsUnderline="False" Size="12" StringAlignment="Center">
                            <Border Color="Black" InflateFactor="0" IsVisible="False" Width="1" />
                            <Fill AlignH="Center" AlignV="Center" Color="White" ColorOpacity="100" 
                                IsScaled="True" IsVisible="True" RangeMax="0" RangeMin="0" Type="Solid" />
                        </FontSpec>
                        <Fill AlignH="Center" AlignV="Center" Color="White" ColorOpacity="100" 
                            IsScaled="True" IsVisible="True" RangeMax="0" RangeMin="0" Type="Brush" />
                        <Border Color="Black" InflateFactor="0" IsVisible="True" Width="1" />
                    </Legend>
                    <PaneFill AlignH="Center" AlignV="Center" Color="White" ColorOpacity="100" 
                        IsScaled="True" IsVisible="True" RangeMax="0" RangeMin="0" Type="Solid" />
                    <ChartFill AlignH="Center" AlignV="Center" Color="White" ColorOpacity="100" 
                        IsScaled="True" IsVisible="True" RangeMax="0" RangeMin="0" Type="Brush" />
                    <ChartBorder Color="Black" InflateFactor="0" IsVisible="True" Width="1" />
                    <MasterPaneBorder Color="Black" InflateFactor="0" IsVisible="True" Width="1" />
                    <Margins Bottom="10" Left="10" Right="10" Top="10" />
                    <PaneBorder Color="Black" InflateFactor="0" IsVisible="True" Width="1" />
                </zgw:ZedGraphWeb>
            </div>
    
            <br />
    
            <div id="graphvaluesblock">
                <asp:Panel ID="CollapseHeaderPanel" runat="server" CssClass="collapsepanelheader">
                    <asp:ImageButton ID="CollapseImageButton" runat="server" ImageUrl="~/Images/expand.jpg"/>
                    <asp:Label ID="CollapseHeaderLabel" runat="server">Показать данные графика.</asp:Label>
                </asp:Panel>
                <asp:Panel ID="CollapseContentPanel" runat="server" CssClass="collapsecontentpanel" Height="0">
                    <br />
                    <asp:GridView ID="GridViewGraphData" runat="server" CssClass="graphdatatable" />
                    <br />
                </asp:Panel>
                <asp:CollapsiblePanelExtender ID="CollapseController" runat="server"
                     TargetControlID="CollapseContentPanel"
                     ExpandControlID="CollapseHeaderPanel"
                     CollapseControlID="CollapseHeaderPanel"
                     ImageControlID="CollapseImageButton"
                     TextLabelID="CollapseHeaderLabel"
                     AutoCollapse="false"
                     AutoExpand="false"
                     Collapsed="true"
                     ScrollContents="false"
                     CollapsedText="Показать данные графика."
                     CollapsedImage="~/Images/expand.jpg"
                     ExpandedText="Скрыть данные графика."
                     ExpandedImage="~/Images/collapse.jpg"
                     ExpandDirection="Vertical" 
                     SuppressPostBack="true" 
                     Enabled="True" />
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
</asp:Content>
