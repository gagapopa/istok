using System;
using System.Collections.Generic;
using System.Management;
using System.Text;

namespace COTES.ISTOK
{
    /// <summary>
    /// Класс, идентифицирующий текущую машину
    /// </summary>
    internal static class MachineIdentification
    {
        /// <summary>
        /// Генерирует 8ми байтный код, на основе данных о оборудовании машины
        /// </summary>
        /// <returns></returns>
        public static UInt64 GetMachineID()
        {
            String srcCode = "";
            int[] code;
            int codeLength = 16, srcLength, bs;

            List<String> features = new List<String>();
            // запрос PnP и MAC-адреса сетевой карты и серийного кода материнской платы
            //try
            //{
            String[] queries ={ "SELECT PNPDeviceID, MACAddress FROM Win32_NetworkAdapter where PNPDeviceID like 'PCI%'",
                    "SELECT SerialNumber FROM Win32_BaseBoard" };

            ManagementObjectSearcher s =
            new ManagementObjectSearcher(
               "root\\cimv2",
                "",//"SELECT * FROM Win32_NetworkAdapter where not PNPDeviceID like 'root%'",
            new EnumerationOptions(
            null, System.TimeSpan.MaxValue,
            1, true, false, true,
            true, false, true, true));

            foreach (String query in queries)
            {
                s.Query.QueryString = query;
                foreach (ManagementObject service in s.Get())
                {
                    foreach (PropertyData property in service.Properties)
                    {
                        if (property.Name == "Tag" || property.Name == "DeviceID") continue;
                        if (property.Value != null)
                        {
                            features.Add(property.Value.ToString());
                        }
                    }
                }
            }
            //}
            //catch (Exception exc) { CommonData.Error(exc.Message); }

            // полученный код преобразуем в 8ми байтный код.
            foreach (String str in features) srcCode += str;

            code = new int[codeLength];
            srcLength = srcCode.Length;

            int i, j;
            bs = srcLength / codeLength;
            for (j = 0; j < bs; j++)
                for (i = 0; i < codeLength; i++) code[i] += srcCode[j * codeLength + i];
            for (j = 0; j < srcLength % codeLength; j++)
                for (i = 0; i < codeLength; i++) code[i] += srcCode[bs * codeLength + j];

            ulong nCode = 0;
            for (i = 0; i < codeLength; i++)
            {
                code[i] %= 16;
                nCode += ((uint)code[i] * (ulong)1 << (4 * i));
            }
            return nCode;
        }
    }
}
