using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using COTES.ISTOK.ASC;
using ZedGraph;
using COTES.ISTOK.ClientCore.Utils;

namespace COTES.ISTOK.ClientCore.UnitProviders
{
    public class NormFuncUnitProvider : UnitProvider
    {
        public NormFuncUnitProvider(StructureProvider strucProvider, NormFuncNode funcNode)
            : base(strucProvider, funcNode)
        {

        }

        public override IEnumerable<ActionArgs> Save()
        {
            //throw new NotImplementedException();
            NewUnitNode.AcceptChanges();
            return base.Save();
        }

        #region Настройки компонента
        #endregion

        public void FillPane(GraphPane pane)
        {
            NormFuncGraphParam param;
            NormFuncDrawer drawer;
            NormFuncNode nnode = UnitNode as NormFuncNode;
            MultiDimensionalTable mdt = nnode.GetMDTable(this.strucProvider.CurrentRevision);

            param = new NormFuncGraphParam();

            param.mdtable = mdt;
            if (mdt != null && mdt.DimensionInfo.Length > 1)
                param.strAxeZ = mdt.DimensionInfo[1].Name;
            param.delta_x = 1;//double.Parse(txtStepX.Text);
            param.delta_y = 1;//double.Parse(txtStepY.Text);
            param.coordinates = new double[0];

            pane.Title.Text = UnitNode.Text;
            if (mdt.DimensionInfo.Length > 0)
            {
                string txt;

                txt = mdt.DimensionInfo[0].Name;
                if (!string.IsNullOrEmpty(mdt.DimensionInfo[0].Measure))
                    txt += ", " + mdt.DimensionInfo[0].Measure;
                pane.XAxis.Title.Text = txt;
            }
            pane.YAxis.Title.Text = nnode.ResultUnit;

            drawer = new NormFuncDrawer(param);
            NormFuncDrawer.Clear(pane);
            if (mdt != null && param.coordinates != null && param.coordinates.Length >= mdt.DimensionInfo.Length - 2)
                drawer.Draw(pane);
        }
    }
}
