using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using COTES.ISTOK.ASC;
using COTES.ISTOK.ClientCore;
using FastReport.Design;
using FastReport.Design.StandardDesigner;
using FastReport.DevComponents.DotNetBar;

namespace COTES.ISTOK.Client
{
    /// <summary>
    /// Окно редактирования отчёта FasrReport
    /// </summary>
    partial class ReportEditForm : ButtonEditForm
    {
        static ReportEditForm()
        {
            FastReport.Design.DesignerPlugins.Add(typeof(IstokInjectionFastReportTooltip));
        }

        public ReportEditForm(StructureProvider structureProvider, UnitNode unitNode)
            : base(structureProvider, unitNode)
        {
            InitializeComponent();

            InitilateIstokInjection(strucProvider, UnitNode as ReportNode);
        }

        /// <summary>
        /// Initialize injection tool form in FastReport designer
        /// </summary>
        /// <param name="strucProvider"></param>
        /// <param name="reportNode"></param>
        private void InitilateIstokInjection(StructureProvider strucProvider, ReportNode reportNode)
        {
            var istokInjection = designerControl1.Plugins.FindType(typeof(IstokInjectionFastReportTooltip).ToString()) as IstokInjectionFastReportTooltip;

            DockInjectionToolTip(istokInjection);

            istokInjection.ReportNode = UnitNode as ReportNode;
            istokInjection.StructureProvider = strucProvider;
        }

        /// <summary>
        /// Dock Injection to the left
        /// </summary>
        /// <param name="istokInjection"></param>
        private void DockInjectionToolTip(IstokInjectionFastReportTooltip istokInjection)
        {
            var dockSite = (from s in designerControl1.Controls.OfType<DockSite>()
                            where s.Dock == DockStyle.Left && s.Name == "tbLeftDockSite"
                            select s).FirstOrDefault();

            if (dockSite != null)
            {
                var bar = istokInjection.ContainerControl as Bar;
                var manager = dockSite.GetDocumentUIManager();
                if (manager.RootDocumentDockContainer == null)
                    manager.RootDocumentDockContainer = new DocumentDockContainer();
                manager.Dock(bar);
            }
        }

        private void designerControl1_Load(object sender, EventArgs e)
        {
            ToolbarBase standardToolbar =
                designerControl1.Plugins.Find("StandardToolbar") as ToolbarBase;
            //standardToolbar.Items["btnStdOpen"].Visible = false;
            standardToolbar.Items["btnStdSave"].Visible = false;
            standardToolbar.Items["btnStdSaveAll"].Visible = false;

            designerControl1.MainMenu.miFileSave.Visible = false;
            designerControl1.MainMenu.miFileSaveAll.Visible = false;

            // load report
            designerControl1.Report = new FastReport.Report();
            ReportNode report = UnitNode as ReportNode;
            if (report.ReportBody != null)
            {
                if (report.ReportBody != null && report.ReportBody.Length > 0)
                {
                    using (MemoryStream ms = new MemoryStream(report.ReportBody))
                    {
                        designerControl1.Report.Load(ms);
                    }
                }
            }
            designerControl1.InitReport();

            designerControl1.UpdatePlugins(this);
            designerControl1.Update();
            designerControl1.RefreshLayout();
        }

        private void environmentSettings1_CustomSaveReport(object sender, OpenSaveReportEventArgs e)
        {
            SaveUnit();
        }

        protected override void SaveUnit()
        {
            ReportNode reportNode = UnitNode as ReportNode;

            if (reportNode != null)
            {
                // save report body
                if (designerControl1.Report != null)
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        designerControl1.Report.Save(ms);
                        reportNode.ReportBody = ms.ToArray();
                    }
                }

            }
            base.SaveUnit();
        }
    }
}
