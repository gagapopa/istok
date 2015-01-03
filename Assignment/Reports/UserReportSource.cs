using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using COTES.ISTOK.ASC;
using System.Data;

namespace COTES.ISTOK.Assignment
{
    /// <summary>
    /// Источник данных отчета с Информацией о пользователях
    /// </summary>
    class UserReportSource : IReportSource
    {
        IUnitManager unitManager;

        IUserManager userManager;

        ReportSourceInfo info;
        static readonly Guid SettingsReportSourceID = new Guid("{E5C2034F-2E48-4919-91AC-DB35704D7BBB}");

        public UserReportSource(IUnitManager umanager, IUserManager smanager)
        {
            info = new ReportSourceInfo(SettingsReportSourceID, true, "Информация о пользователях системы");

            this.unitManager = umanager;
            this.userManager = smanager;
        }

        #region IReportSource Members

        public ReportSourceInfo Info
        {
            get { return info; }
        }

        public Guid[] References
        {
            get { return null; }
        }

        public void SetReference(IReportSource source)
        {
            throw new NotImplementedException();
        }

        public ReportSourceSettings CreateSettingsObject()
        {
            return new SimpleReportSourceSettings(info);
        }

        const String nameName = "name";
        const String fullnameName = "fullname";
        const String positionName = "position";
        const String roleName = "role";

        public void GenerateData(OperationState state, DataSet dataSet, ReportSourceSettings settings, params ReportParameter[] reportParameters)
        {
            DataTable table = CreateTable();

            dataSet.Tables.Add(table);

            UserNode[] users = userManager.GetUserNodes(state);

            foreach (var item in users)
            {
                DataRow row = table.Rows.Add();

                row[nameName] = item.Text;
                row[fullnameName] = item.UserFullName;
                row[positionName] = item.Position;
                row[roleName] = GetUserRow(item);
            }
        }

        public void GenerateEmptyData(OperationState state, DataSet dataSet, ReportSourceSettings settings)
        {
            const int testCount = 5;

            DataTable table = CreateTable();

            dataSet.Tables.Add(table);

            for (int i = 0; i < testCount; i++)
            {
                DataRow row = table.Rows.Add();

                row[nameName] = String.Format("user{0}", i);
                row[fullnameName] = String.Format("User Number {0}", i);
                row[positionName] = "Engineer";
                row[roleName] = i == 0 ? "Администратор" : "Пользователь";
            }
        }

        private DataTable CreateTable()
        {
            DataTable table = new DataTable(info.Caption);

            table.Columns.Add(nameName, typeof(String));
            table.Columns.Add(fullnameName, typeof(String));
            table.Columns.Add(positionName, typeof(String));
            table.Columns.Add(roleName, typeof(String));
            return table;
        }

        private String GetUserRow(UserNode item)
        {
            if (item.IsAdmin)
            {
                return "Администратор";
            }
            else
                return "Пользователь";
        }

        #endregion

    }
}
