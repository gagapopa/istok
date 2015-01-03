using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using COTES.ISTOK.ASC;
using COTES.ISTOK.ClientCore.UnitProviders;
using COTES.ISTOK.ClientCore;

namespace COTES.ISTOK.Client
{
    partial class BaseUnitControl : UserControl, IDisposable
    {
        protected UnitProvider unitProvider = null;
        protected Stack<AsyncOperationWatcher> operations =
                        new Stack<AsyncOperationWatcher>();

        protected bool initiated = false;

        public BaseUnitControl() { InitializeComponent(); }
        public BaseUnitControl(UnitProvider unitProvider)
        {
            UnitProvider = unitProvider;
            InitializeComponent();
            this.Text = unitProvider.UnitNode.Text;
        }

        public BaseAsyncWorkForm UniForm { get; set; }

        protected virtual void CatchDieSilently()
        {
            //nothing
        }

        public virtual void InitiateProcess()
        {
            initiated = true;
        }

        public UnitProvider UnitProvider
        {
            get { return unitProvider; }
            set
            {
                try
                {
                    //InitiateProcess(value);
                }
                finally
                {
                    unitProvider = value;
                    //initiated = false;
                }
            }
        }

        protected UnitNode UnitNode { get { return unitProvider.UnitNode; } }

        public override string Text { get; set; }

        //public UnitTypeId Typ { get { if (UnitNode != null)  return UnitNode.Typ; else return UnitTypeId.Root; } }
        public UnitTypeId Typ { get; set; }

        /// <summary>
        /// Проверить годится ли данный контрол для отображение конкретного 
        /// </summary>
        /// <param name="node">Проверяемый узел</param>
        /// <returns></returns>
        public virtual bool ForShow(UnitNode node)
        {
            return false;
        }

        /// <summary>
        /// Выделить узел на контроле
        /// </summary>
        /// <param name="node">Узел</param>
        public virtual void SelectNode(UnitNode node)
        {
            throw new NotImplementedException();
        }
        
        protected virtual void SaveUnitData()
        {
            //
        }

        protected virtual void LockControls()
        {
            //
        }
        protected virtual void UnlockControls()
        {
            //
        }
        
        /// <summary>
        /// Сохраняет содержимое DataGridView в CSV-файл.
        /// Пропускает невидимые колонки (если они не указанны в исключениях)
        /// </summary>
        /// <param name="s">Поток, куда будет записанны данные</param>
        /// <param name="dataGridView">DataGridView с сохраняемыми данными</param>
        /// <param name="includeUnvisible">
        /// Исключения. 
        /// Имена колонок, которые необходимо включать, даже если они не видимы.
        /// null, если исключений нет.
        /// </param>
        /// <param name="excludeVisible">
        /// Исключения. 
        /// Имена колонок, которые не надо включать, даже если они видимы.
        /// null, если исключений нет.
        /// </param>
        /// <remarks>
        /// Если у строки или колонки в свойстве Tag будет хрониться ParameterNode,
        /// то будут применены свойства для форматирования значения (такие как количество знаков после запятой).
        /// </remarks>
        protected void GridToCSV(
            Stream s, 
            DataGridView dataGridView, 
            String[] includeUnvisible, 
            String[] excludeVisible)
        {
            if (includeUnvisible == null)
                includeUnvisible = new String[0];
            if (excludeVisible == null)
                excludeVisible = new String[0];

            using (StreamWriter w = new StreamWriter(s, Encoding.GetEncoding("windows-1251")))
            {
                w.Write(unitProvider.UnitNode.Text);
                w.WriteLine();

                for (int i = 0; i < dataGridView.Columns.Count; i++)
                    if ((dataGridView.Columns[i].Visible
                            || includeUnvisible.Contains(dataGridView.Columns[i].Name))
                            && !excludeVisible.Contains(dataGridView.Columns[i].Name))
                        w.Write(dataGridView.Columns[i].HeaderText + ";");
                w.WriteLine();
                String dbFormat;
                ParameterNode param;
                foreach (DataGridViewRow row in dataGridView.Rows)
                {
                    for (int i = 0; i < dataGridView.Columns.Count; i++)
                    {
                        DataGridViewColumn column = dataGridView.Columns[i];
                        if ((column.Visible
                            || includeUnvisible.Contains(column.Name))
                            && !excludeVisible.Contains(column.Name))
                            if (row.Cells[i].Value == null) w.Write(" " + ";");
                            else
                            {
                                Object value = row.Cells[i].Value;
                                if ((value is double && double.IsNaN(Convert.ToDouble(value)))
                                    || (value is DateTime && value.Equals(DateTime.MinValue))) value = "--";

                                if (value is double&&((param = row.Tag as ParameterNode) != null
                                    || (param = column.Tag as ParameterNode) != null ))
                                    dbFormat = "\"{0:" + ValuesControlSettings.Instance.DoubleFormat(param) + "}\";";
                                else dbFormat = "\"{0}\";";

                                w.Write(String.Format(dbFormat, value));
                            }
                    }
                    w.WriteLine();
                } 
            }
        }

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void DisposeControl()
        {
            //
        }
        //protected virtual void Dispose(bool disposing)
        //{
        //    SaveUnitData();
        //    DisposeControl();
        //}

        #endregion
    }
}
