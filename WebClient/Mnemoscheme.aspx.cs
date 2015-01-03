using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using COTES.ISTOK;
using COTES.ISTOK.ASC;

namespace WebClient
{
    public partial class Mnemoscheme : StateMonitorPage
    {

        List<int> parmeterList;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                SetSpecificTab(true);
                var id = Int32.Parse(Parameters[Configuration.Get(Setting.IdObjectMarker)]);

                MnemoschemesDescriptor mnemoscheme = 
                    DataService.GetMnemoschemeDescriptor(id);

                MnemoSchemePanel.Height = mnemoscheme.ContentSize.Height;
                MnemoSchemePanel.Width = mnemoscheme.ContentSize.Width;
                FileResource fres = DataService.GetResource(mnemoscheme.Content);
                if (fres != null) MnemoSchemePanel.BackImageUrl = fres.Link;

                DivBuilder div = new DivBuilder();
                TextBox temp = null;
                foreach (var it in mnemoscheme)
                {
                    temp = new TextBox()
                    {
                        ID = it.ID.ToString(),
                        Text = "",
                        ToolTip = it.Name,
                        Visible = true,
                        ReadOnly = true
                    };

                    div.Top = it.AreaOnMnemoscheme.Top;
                    div.Left = it.AreaOnMnemoscheme.Left;

                    //var args = new PostBackOptions(this);
                    //args.ActionUrl = "MnemoschemeGraphic.aspx";
                    //args.AutoPostBack = false;
                    //args.RequiresJavaScriptProtocol = true;
                    //args.PerformValidation = true;                    
                    HyperLink link = new HyperLink();
                    //link.Attributes["onclick"]=ClientScript.GetCallbackEventReference(this, null, null, 

                    //link.NavigateUrl = String.Format("MnemoschemeGraphic.aspx?params={0}&start={1}&end={2}", it.ID.ToString(), null, null); //ClientScript.GetPostBackEventReference(args);
                    link.Target = id.ToString();
                    link.Controls.Add(temp);

                    //ClientScript.GetPostBackEventReference(new PostBackOptions(new Graphic())
                    temp.Style.Add(HtmlTextWriterStyle.Width,
                                   it.AreaOnMnemoscheme.Width.ToString() + "px");
                    temp.Style.Add(HtmlTextWriterStyle.Height,
                                   it.AreaOnMnemoscheme.Height.ToString() + "px");
                    temp.Style.Add(HtmlTextWriterStyle.Margin,
                                   "0 0 0 0");
                    temp.Style.Add(HtmlTextWriterStyle.BorderStyle,
                                   "none");

                    MnemoSchemePanel.Controls.Add(div.BeginTag);
                    //MnemoSchemePanel.Controls.Add(temp);
                    MnemoSchemePanel.Controls.Add(link);
                    MnemoSchemePanel.Controls.Add(div.EndTag);
                }
            }
            catch (BaseWebClientException exp)
            {
                exp.Accept(Handler.Instance);
            }
            catch (Exception exp)
            {
                Handler.Instance.ProcessUndefined(exp);
            }
        }

        private void CreateGraph(object sender)
        {
            //throw new NotImplementedException();
        }

        protected void UpdateTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                MnemoschemesDescriptor mnemoscheme =
                    DataService.GetMnemoschemeDescriptor(Int32.Parse(Parameters[Configuration.Get(Setting.IdObjectMarker)]));

                ParamValueItemWithID[] param_value =
                    DataService.GetUpdatedValues(mnemoscheme.UpdateTransactionID);
                TextBox temp = null;
                foreach (var p in param_value)
                    foreach (var pd in mnemoscheme[p.ParameterID])
                    {
                        temp = MnemoSchemePanel.FindControl(pd.ID.ToString()) as TextBox;
                        temp.Text = p.Value.ToString();
                        temp.BackColor = pd.GetColorState(p);//.Value);
                        temp.ToolTip = FormatTooltip(pd.Name,
                                                     p.Time);
                        HyperLink link = temp.Parent as HyperLink;
                        if (link != null)
                        {
                            link.NavigateUrl = GenerateGraphLink(mnemoscheme, pd, p);
                        }
                    }
            }
            catch (BaseWebClientException exp)
            {
                exp.Accept(Handler.Instance);
            }
            catch (Exception exp)
            {
                Handler.Instance.ProcessUndefined(exp);
            }
        }

        private string GenerateGraphLink(MnemoschemesDescriptor mnemoscheme, MnemoschemeParameterDescriptor pd, ParamValueItem p)
        {
            //var args = new PostBackOptions(this);
            //args.ActionUrl = "MnemoschemeGraphic.aspx";
            //args.AutoPostBack = false;
            //args.RequiresJavaScriptProtocol = true;
            //args.PerformValidation = true;                    
            //ClientScript.GetPostBackEventReference(args);
            //String startTime = Server.UrlEncode(p.Time.AddDays(-1).ToString());
            //String endTime = Server.UrlEncode(p.Time.ToString());
            //Interval interval = DataService.GetParameterInterval(DataService.GetUnitNode(pd.ParametrID) as ParameterNode);
            //WebClient.Graphic.IntervalMarker marker = GetBaseInterval(interval);
            ////int baseInterval = (int)GetParentInterval(interval);
            TimeSpan timeSpan;
            //switch (marker)
            //{
            //    case Graphic.IntervalMarker.HalfAnHour:
            timeSpan = TimeSpan.FromMinutes(30);
            //        break;
            //    case Graphic.IntervalMarker.OneHour:
            //    case Graphic.IntervalMarker.Custom:
            //    default:
            //        timeSpan = TimeSpan.FromHours(1);
            //        break;
            //    case Graphic.IntervalMarker.FourHours:
            //        timeSpan = TimeSpan.FromHours(4);
            //        break;
            //    case Graphic.IntervalMarker.EightHours:
            //        timeSpan = TimeSpan.FromHours(8);
            //        break;
            //    case Graphic.IntervalMarker.Day:
            //        timeSpan = TimeSpan.FromDays(1);
            //        break;
            //    case Graphic.IntervalMarker.Month:
            //        timeSpan = TimeSpan.FromDays(30);
            //        break;
            //}
            String startTime = Server.UrlEncode(p.Time.Add(-timeSpan).ToString(System.Globalization.DateTimeFormatInfo.InvariantInfo));

            System.Text.StringBuilder linkBuilder = new System.Text.StringBuilder();

            linkBuilder.Append("MnemoschemeGraphic.aspx?");
            linkBuilder.AppendFormat("schemaid={0}", mnemoscheme.ID);
            linkBuilder.AppendFormat("&params={0}", pd.ParametrID);
            linkBuilder.AppendFormat("&start={0}", startTime);
            //linkBuilder.AppendFormat("&interval={0}", (int)marker);

            //return String.Format("MnemoschemeGraphic.aspx?params={0}&start={1}&interval={2}", pd.ParametrID.ToString(), startTime, (int)marker); 
            return linkBuilder.ToString();
        }

        //private Graphic.IntervalMarker GetBaseInterval(Interval interval)
        //{
        //    if (interval < new Interval(30 * 60))
        //        return Graphic.IntervalMarker.HalfAnHour;
        //    if (interval < new Interval(60 * 60))
        //        return Graphic.IntervalMarker.OneHour;
        //    if (interval < new Interval(4 * 60 * 60))
        //        return Graphic.IntervalMarker.FourHours;
        //    if (interval < new Interval(8 * 60 * 60))
        //        return Graphic.IntervalMarker.EightHours;
        //    if (interval < new Interval(24 * 60 * 60))
        //        return Graphic.IntervalMarker.Day;
        //    if (interval < new Interval(30 * 24 * 60 * 60))
        //        return Graphic.IntervalMarker.Month;
        //    return Graphic.IntervalMarker.Custom;
        //}

        protected void UpdatePeriodChanged(object sender, EventArgs e)
        {
            try
            {
                UpdateTimer.Interval = IntervalControl.Period;
            }
            catch (BaseWebClientException exp)
            {
                exp.Accept(Handler.Instance);
            }
            catch (Exception exp)
            {
                Handler.Instance.ProcessUndefined(exp);
            }
        }
    }
}
