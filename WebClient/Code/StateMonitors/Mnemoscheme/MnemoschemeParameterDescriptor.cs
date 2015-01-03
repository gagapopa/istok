using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Drawing;
using COTES.ISTOK.ASC;

namespace WebClient
{
    public class MnemoschemeParameterDescriptor : ParameterDescriptor
    {
        public Rectangle AreaOnMnemoscheme { get; private set; }
        //public double? MinAlertValue { get; private set; }
        //public double? MaxAlertValue { get; private set; }
        //public double? MinWarningValue { get; private set; }
        //public double? MaxWarningValue { get; private set; }
        //public readonly Color AlertColor = Color.Red;
        //public readonly Color WarningColor = Color.Yellow;
        //public readonly Color NormalColor = Color.Green;
        private Double? fieldValue = null;
        private Double? oldFieldValue = null;
        private Color DefaultBackColor = Color.White;
        
        SchemaParamNode dataNode;
        int Relevance;


        public MnemoschemeParameterDescriptor(SchemaNode node, SchemaParamNode item)
            : base(item.Idnum, item.ParameterId, item.Text)
        {
            dataNode = item;
            Relevance = node.Relevance;
            AreaOnMnemoscheme = new Rectangle(item.Left, item.Top, item.Width, item.Height);
            //MinAlertValue = min_alert;
            //MaxAlertValue = max_alert;
            //MinWarningValue = min_warning;
            //MaxWarningValue = max_warning;
        }

        public Color GetColorState( COTES.ISTOK.ParamValueItem p)//double value)
        {
            //if ((MaxAlertValue != null && value > MaxAlertValue)
            //    || (MinAlertValue != null && value < MinAlertValue))
            //    return AlertColor;
            //if ((MaxWarningValue != null && value > MaxWarningValue)
            //    || (MinWarningValue != null && value < MinWarningValue))
            //    return WarningColor;
            //else
            //    return NormalColor;
            Color backColor = Color.Empty;// base.BackColor;

            //if (isSprDesign || fieldValue == null)
            //{ if (dataNode.NominalColor.IsEmpty) backColor = DefaultBackColor; else backColor = dataNode.NominalColor; }
            //else
            //{
            int Interval = 0;
            if (dataNode.Attributes.ContainsKey(COTES.ISTOK.CommonData.IntervalProperty))
            {
                int.TryParse(dataNode.Attributes[COTES.ISTOK.CommonData.IntervalProperty], out Interval);
            }

            if (p.Quality == 0)
            {
                backColor = Color.Gray;
                // return;
            }
            else
            {
                if (p.Time != null && Interval > 0 && Relevance > 0
                    && DateTime.Now.Subtract(p.Time).TotalSeconds > Interval + Relevance)
                {
                    backColor = Color.LightGray;
                    //return;
        }
                else
                {
                    double hyst;

                    if (dataNode.Hysteresis != null)
                        hyst = Math.Abs((double)dataNode.Hysteresis) / 100;
                    else
                        hyst = 0;

                    if (oldFieldValue == null)
                        hyst = 0f;

                    double hystPlus = 1f + hyst;
                    double hystMinus = 1f - hyst;

                    // в нижней зоне аварии
                    if (dataNode.MinAlertValue != null && fieldValue < hystMinus * dataNode.MinAlertValue)
                    {
                        if (dataNode.MinAlertColor.IsEmpty)
                            backColor = DefaultBackColor;
                        else backColor = dataNode.MinAlertColor;
    }
                    // в нижней зоне предупреждения
                    else if (dataNode.MinWarningValue != null && dataNode.MinAlertValue != null
                        && (fieldValue < hystMinus * dataNode.MinWarningValue)
                        && (fieldValue > hystPlus * dataNode.MinAlertValue))
                    {
                        if (dataNode.MinWarningColor.IsEmpty)
                            backColor = DefaultBackColor;
                        else backColor = dataNode.MinWarningColor;
                    }

                    // в верхней зоне аварии
                    else if (dataNode.MaxAlertValue != null && fieldValue > hystPlus * dataNode.MaxAlertValue)
                    {
                        if (dataNode.MaxAlertColor.IsEmpty)
                            backColor = DefaultBackColor;
                        else backColor = dataNode.MaxAlertColor;
                    }
                    // в верхней зоне предупреждения
                    else if (dataNode.MaxWarningValue != null && dataNode.MaxAlertValue != null
                        && (fieldValue > hystPlus * dataNode.MaxWarningValue)
                        && (fieldValue < hystMinus * dataNode.MaxAlertValue))
                    {
                        if (dataNode.MaxWarningColor.IsEmpty)
                            backColor = DefaultBackColor;
                        else backColor = dataNode.MaxWarningColor;
                    }

                    // в норме
                    else/* if ((dataNode.MinAlertValue == null || fieldValue >= hystPlus * dataNode.MinAlertValue)
                          && (dataNode.MinWarningValue == null || fieldValue >= hystPlus * dataNode.MinWarningValue)
                          && (dataNode.MaxAlertValue == null || fieldValue <= hystMinus * dataNode.MaxAlertValue)
                          && (dataNode.MaxWarningValue == null || fieldValue <= hystMinus * dataNode.MaxWarningValue)
                          )*/
                    {
                        if (dataNode.NominalColor.IsEmpty)
                            backColor = DefaultBackColor;
                        else backColor = dataNode.NominalColor;
                    }
                }
            }
            //}
            return backColor;
            //if (backColor != base.BackColor)
            //{
            //    base.BackColor = backColor;
            //    this.Invalidate();
            //}
        }
    }
}
