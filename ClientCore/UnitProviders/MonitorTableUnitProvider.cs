using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using COTES.ISTOK.ASC;
using System.Data;

namespace COTES.ISTOK.ClientCore.UnitProviders
{
    public class MonitorTableUnitProvider : OnlineUnitProvider
    {
        public MonitorTableUnitProvider(StructureProvider strucProvider, MonitorTableNode unitNode)
            : base(strucProvider, unitNode)
        {
            //
        }

        //protected override BaseUnitControl GetUnitControl(bool multitab)
        //{
        //    return new MonitorTableUnitControl(this);
        //}

        #region Настройки компонента
        //
        #endregion

        public override IEnumerable<ActionArgs> Save()
        {
            throw new NotImplementedException("Save");
            //метод один фиг не вызывается никогда
            MonitorTableNode node = (MonitorTableNode)unitNode;
            bool found;

            //удаление параметров, которые не используются в таблице
            if (node.Parameters != null)
            {
                foreach (var item in node.Parameters)
                {
                    found = false;

                    if (node.Table != null)
                    {
                        Cell cell = null;

                        for (int row = 0; row < node.Table.Rows.Count; row++)
                        {
                            for (int col = 0; col < node.Table.Columns.Count; col++)
                            {
                                cell = node.Table.Rows[row][col] as Cell;
                                if (cell != null && cell.ParameterId == item.ParameterId)
                                {
                                    found = true;
                                    break;
                                }
                            }
                            if (found) break;
                        }
                    }

                    if (!found) node.RemoveChildParam(item);
                }
            }
            //base.Save();
        }
    }
}
