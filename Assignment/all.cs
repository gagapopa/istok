using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;

namespace COTES.ISTOK.Assignment
{
    // общие данные
    public static class GNSI
    {
        public static String CommonAppDataPath
        {
            get
            {
                return
                    Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) +
                    Path.DirectorySeparatorChar + System.Windows.Forms.Application.CompanyName +
                    Path.DirectorySeparatorChar + System.Windows.Forms.Application.ProductName;
            }
        }

        public static String LicenseFile { get { return Path.Combine(CommonAppDataPath, "global.lic"); } }
    }
}
