<%@ Page Language="C#" MasterPageFile="~/TreeTemplate.Master" AutoEventWireup="true" CodeBehind="NormFunc.aspx.cs" Inherits="WebClient.NormFunc" Theme="GlobalSkin" Culture="auto" UICulture="auto"%>
<%@ Register Assembly="ZedGraph.Web" Namespace="ZedGraph.Web" TagPrefix="zgw" %>

<asp:Content ID="Content" ContentPlaceHolderID="TreeContent" runat="server">
    <h2>Нормативная функция</h2>
    <div>
        <zgw:ZedGraphWeb ID="Graph" runat="server" onrendergraph="Graph_RenderGraph" 
                        IsImageMap="True" Width="600" Height="400">
            <XAxis AxisColor="Black" Cross="0" CrossAuto="True" IsOmitMag="False" 
                            IsPreventLabelOverlap="True" IsShowTitle="True" IsTicsBetweenLabels="True" 
                            IsUseTenPower="False" IsVisible="True" IsZeroLine="False" 
                MinSpace="0" Title="" 
                            Type="Linear">
                <FontSpec Angle="0" Family="Arial" FontColor="Black" IsBold="True" 
                                IsItalic="False" IsUnderline="False" Size="14" 
                    StringAlignment="Center">
                    <Border Color="Black" InflateFactor="0" IsVisible="False" Width="1" />
                    <Fill AlignH="Center" AlignV="Center" Color="White" ColorOpacity="100" 
                                    IsScaled="True" IsVisible="True" RangeMax="0" RangeMin="0" 
                        Type="None" />
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
                                MagAuto="True" MajorStep="1" MajorStepAuto="True" 
                    MajorUnit="Day" Max="0" 
                                MaxAuto="True" MaxGrace="0.1" Min="0" MinAuto="True" MinGrace="0.1" 
                                MinorStep="1" MinorStepAuto="True" MinorUnit="Day">
                    <FontSpec Angle="0" Family="Arial" FontColor="Black" IsBold="False" 
                                    IsItalic="False" IsUnderline="False" Size="14" 
                        StringAlignment="Center">
                        <Border Color="Black" InflateFactor="0" IsVisible="False" Width="1" />
                        <Fill AlignH="Center" AlignV="Center" Color="White" ColorOpacity="100" 
                                        IsScaled="True" IsVisible="True" RangeMax="0" RangeMin="0" 
                            Type="None" />
                    </FontSpec>
                </Scale>
            </XAxis>
            <Y2Axis AxisColor="Black" Cross="0" CrossAuto="True" IsOmitMag="False" 
                            IsPreventLabelOverlap="True" IsShowTitle="True" IsTicsBetweenLabels="True" 
                            IsUseTenPower="False" IsVisible="False" IsZeroLine="True" 
                MinSpace="0" Title="" 
                            Type="Linear">
                <FontSpec Angle="0" Family="Arial" FontColor="Black" IsBold="True" 
                                IsItalic="False" IsUnderline="False" Size="14" 
                    StringAlignment="Center">
                    <Border Color="Black" InflateFactor="0" IsVisible="False" Width="1" />
                    <Fill AlignH="Center" AlignV="Center" Color="White" ColorOpacity="100" 
                                    IsScaled="True" IsVisible="True" RangeMax="0" RangeMin="0" 
                        Type="None" />
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
                                MagAuto="True" MajorStep="1" MajorStepAuto="True" 
                    MajorUnit="Day" Max="0" 
                                MaxAuto="True" MaxGrace="0.1" Min="0" MinAuto="True" MinGrace="0.1" 
                                MinorStep="1" MinorStepAuto="True" MinorUnit="Day">
                    <FontSpec Angle="-90" Family="Arial" FontColor="Black" IsBold="False" 
                                    IsItalic="False" IsUnderline="False" Size="14" 
                        StringAlignment="Center">
                        <Border Color="Black" InflateFactor="0" IsVisible="False" Width="1" />
                        <Fill AlignH="Center" AlignV="Center" Color="White" ColorOpacity="100" 
                                        IsScaled="True" IsVisible="True" RangeMax="0" RangeMin="0" 
                            Type="None" />
                    </FontSpec>
                </Scale>
            </Y2Axis>
            <FontSpec Angle="0" Family="Arial" FontColor="Black" IsBold="True" 
                            IsItalic="False" IsUnderline="False" Size="16" 
                StringAlignment="Center">
                <Border Color="Black" InflateFactor="0" IsVisible="False" Width="1" />
                <Fill AlignH="Center" AlignV="Center" Color="White" ColorOpacity="100" 
                                IsScaled="True" IsVisible="True" RangeMax="0" RangeMin="0" 
                    Type="None" />
            </FontSpec>
            <MasterPaneFill AlignH="Center" AlignV="Center" Color="White" 
                            ColorOpacity="100" IsScaled="True" IsVisible="True" 
                RangeMax="0" RangeMin="0" 
                            Type="Solid" />
            <YAxis AxisColor="Black" Cross="0" CrossAuto="True" IsOmitMag="False" 
                            IsPreventLabelOverlap="True" IsShowTitle="True" IsTicsBetweenLabels="True" 
                            IsUseTenPower="False" IsVisible="True" IsZeroLine="True" 
                MinSpace="0" Title="" 
                            Type="Linear">
                <FontSpec Angle="-180" Family="Arial" FontColor="Black" IsBold="True" 
                               IsItalic="False" IsUnderline="False" Size="14" 
                    StringAlignment="Center">
                    <Border Color="Black" InflateFactor="0" IsVisible="False" Width="1" />
                    <Fill AlignH="Center" AlignV="Center" Color="White" ColorOpacity="100" 
                                    IsScaled="True" IsVisible="True" RangeMax="0" RangeMin="0" 
                        Type="None" />
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
                                MagAuto="True" MajorStep="1" MajorStepAuto="True" 
                    MajorUnit="Day" Max="0" 
                                MaxAuto="True" MaxGrace="0.1" Min="0" MinAuto="True" MinGrace="0.1" 
                                MinorStep="1" MinorStepAuto="True" MinorUnit="Day">
                    <FontSpec Angle="90" Family="Arial" FontColor="Black" IsBold="False" 
                                    IsItalic="False" IsUnderline="False" Size="14" 
                        StringAlignment="Center">
                        <Border Color="Black" InflateFactor="0" IsVisible="False" Width="1" />
                        <Fill AlignH="Center" AlignV="Center" Color="White" ColorOpacity="100" 
                                        IsScaled="True" IsVisible="True" RangeMax="0" RangeMin="0" 
                            Type="None" />
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
                                IsItalic="False" IsUnderline="False" Size="12" 
                    StringAlignment="Center">
                    <Border Color="Black" InflateFactor="0" IsVisible="False" Width="1" />
                    <Fill AlignH="Center" AlignV="Center" Color="White" ColorOpacity="100" 
                                    IsScaled="True" IsVisible="True" RangeMax="0" RangeMin="0" 
                        Type="Solid" />
                </FontSpec>
                <Fill AlignH="Center" AlignV="Center" Color="White" ColorOpacity="100" 
                                IsScaled="True" IsVisible="True" RangeMax="0" RangeMin="0" 
                    Type="Brush" />
                <Border Color="Black" InflateFactor="0" IsVisible="True" Width="1" />
            </Legend>
            <PaneFill AlignH="Center" AlignV="Center" Color="White" ColorOpacity="100" 
                            IsScaled="True" IsVisible="True" RangeMax="0" RangeMin="0" 
                Type="Solid" />
            <ChartFill AlignH="Center" AlignV="Center" Color="White" ColorOpacity="100" 
                            IsScaled="True" IsVisible="True" RangeMax="0" RangeMin="0" 
                Type="Brush" />
            <ChartBorder Color="Black" InflateFactor="0" IsVisible="True" Width="1" />
            <MasterPaneBorder Color="Black" InflateFactor="0" IsVisible="True" Width="1" />
            <Margins Bottom="10" Left="10" Right="10" Top="10" />
            <PaneBorder Color="Black" InflateFactor="0" IsVisible="True" Width="1" />
        </zgw:ZedGraphWeb>
        <br/>
        <asp:CheckBox ID="chbxShowMarkers" runat="server" Text="Отображать маркеры" AutoPostBack="true" EnableViewState="true"/>
    </div>
    <div>
        <asp:Panel ID="CollapseHeaderPanel" runat="server" CssClass="collapsepanelheader">
            <asp:ImageButton ID="CollapseImageButton" runat="server" ImageUrl="~/Images/expand.jpg"/>
            <asp:Label ID="CollapseHeaderLabel" runat="server">Показать значения</asp:Label>
        </asp:Panel>
        <asp:Panel ID="CollapseContentPanel" runat="server" CssClass="collapsecontentpanel" Height="0">
            <br />
            <asp:GridView ID="GridViewData" runat="server" CssClass="normfuncdatatable" />
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
             CollapsedText="Показать значения"
             CollapsedImage="~/Images/expand.jpg"
             ExpandedText="Скрыть значения"
             ExpandedImage="~/Images/collapse.jpg"
             ExpandDirection="Vertical" 
             SuppressPostBack="true" 
             Enabled="True" />
    </div>
</asp:Content>
