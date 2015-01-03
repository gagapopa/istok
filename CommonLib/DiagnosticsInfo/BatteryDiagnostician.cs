using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Windows.Forms;

namespace COTES.ISTOK.DiagnosticsInfo
{
    public class BatteryDiagnostician : ISummaryInfo
    {
        const string caption = "Состояние питания";

        public DataSet GetSummaryInfo()
        {
            DataSet ds = new DataSet();
            DataTable table = new DataTable(caption);
            PowerStatus power = SystemInformation.PowerStatus;

            table.Columns.Add("Внешнее питание");
            table.Columns.Add("Состояние батареи");
            table.Columns.Add("Полное время работы от батареи");
            table.Columns.Add("Оставшийся заряд батареи");
            table.Columns.Add("Оставшееся время работы от батареи");

            table.Rows.Add(FormatPowerStatus(power.PowerLineStatus),
                FormatBatteryStatus(power.BatteryChargeStatus),
                FormatLifetime(power.BatteryFullLifetime),
                FormatLifePercent(power.BatteryLifePercent),
                FormatLifetime(power.BatteryLifeRemaining));
            ds.Tables.Add(table);

            return ds;
        }

        public string GetSummaryCaption()
        {
            return caption;
        }

        private string FormatPowerStatus(PowerLineStatus status)
        {
            switch (status)
            {
                case PowerLineStatus.Offline:
                    return "Отключено";
                case PowerLineStatus.Online:
                    return "Подключено";
                default:
                    return "Неизвестно";
            }
        }
        private string FormatBatteryStatus(BatteryChargeStatus status)
        {
            switch (status)
            {
                case BatteryChargeStatus.Charging:
                    return "Заряжается";
                case BatteryChargeStatus.Critical:
                    return "Критический";
                case BatteryChargeStatus.High:
                    return "Высокий";
                case BatteryChargeStatus.Low:
                    return "Низкий";
                case BatteryChargeStatus.NoSystemBattery:
                    return "Отсутствует";
                default:
                    return "Неизвестно";
            }
        }
        private string FormatLifetime(int val)
        {
            int h = 0, m = 0, s = 0;

            if (val >= 3600)
            {
                int tmp;
                h = val / 3600;
                tmp = val - h * 3600;
                m = tmp / 60;
                s = tmp - m * 60;
            }
            else
                if (val >= 60)
                {
                    m = val / 60;
                    s = val - m * 60;
                }
                else
                    if (val >= 0)
                        s = val;
                    else
                        return "Неизвестно";

            return string.Format("{0}:{1}:{2}", h, m, s);
        }
        private string FormatLifePercent(float val)
        {
            return string.Format("{0}%", 100 * val);
        }
    }
}
