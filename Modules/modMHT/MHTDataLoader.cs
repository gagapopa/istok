using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using HtmlAgilityPack;
using NLog;

namespace COTES.ISTOK.Modules.modMHT
{
    public class MHTDataLoader : IDataLoader
    {
        Logger log = LogManager.GetCurrentClassLogger();
        String channelLogPrefix = String.Empty;

        private string m_host;
        private string m_path;
        private string m_username;
        private string m_password;

        private ChannelInfo channel;
        private List<ParameterItem> parameters;

        //WNetConnection.NetworkConnection networkConnection;
        CommonFunctions.UNCAccess access = null;

        public bool KeepConnected { get; set; }

        public MHTDataLoader()
        {
            KeepConnected = true;
        }

        private void Connect()
        {
            //if (context != null) Disconnect();
            //if (!string.IsNullOrEmpty(m_username))
            //    context = UserManager.Login(m_username, m_password);
            //if (networkConnection == null)
            //{
            //    System.Net.NetworkCredential cr = new System.Net.NetworkCredential(m_username, m_password);
            //    networkConnection = new WNetConnection.NetworkConnection(m_path, cr);
            //}

            log.Trace(channelLogPrefix + "Подключение к серверу {0}.", m_host);

            access = new CommonFunctions.UNCAccess();
            if (!access.login(m_path, m_username, m_host, m_password)) // "192.168.0.9"
                Console.WriteLine("error: {0}", access.LastError);
        }
        private void Disconnect()
        {
            log.Trace(channelLogPrefix + "Отключение от сервера {0}.", m_host);
            
            //if (context != null)
            //    UserManager.Logout(context);
            //if (networkConnection != null)
            //{
            //    networkConnection.Dispose();
            //    networkConnection = null;
            //}
            if (access != null)
                access.NetUseDelete();
        }

        Regex regex = new Regex(@"(\d{1,2}\.\d{1,2}\.\d{2,4})\.mht");


//        private DataSet GetData(string filename)
//        {
//            using (FileStream s = new FileStream(filename, FileMode.Open))
//            {
//                return GetData(s);
//            }
//        }
        private DataSet GetData(Stream s)
        {
            StreamReader reader = new StreamReader(s);
            DataSet ds = new DataSet();
            MailMessage m = MailMessageMimeParser.ParseMessage(new StringReader(reader.ReadToEnd()));
            foreach (var item in m.AlternateViews)
            {
                if (item.ContentType.MediaType == "text/html")
                {
                    HtmlDocument doc = new HtmlDocument();
                    //string content;
                    //item.ContentStream.Position = 0;
                    //StreamReader sr = new StreamReader(item.ContentStream);
                    //content = sr.ReadToEnd();
                    item.ContentStream.Position = 0;

                    try
                    {
                        doc.Load(item.ContentStream);
                        IEnumerable<HtmlNode> tables = doc.DocumentNode.ChildNodes.Descendants("table");
                        if (tables != null)
                        {
                            DataTable table;
                            foreach (var nodeTable in tables)
                            {
                                table = new DataTable();
                                string tmp;
                                int[] rspan = new int[0];
                                int[] new_rspan = new int[0];

                                foreach (var nodeTr in nodeTable.ChildNodes)
                                {
                                    if (nodeTr.Name.ToLower() == "tr")
                                    {
                                        List<string> lstCol = new List<string>();
                                        List<int> lstHiddenColumns = new List<int>();
                                        int i = 0;
                                        foreach (var nodeTd in nodeTr.ChildNodes)
                                        {
                                            if (nodeTd.Name.ToLower() == "td")
                                            {
                                                tmp = GetText(nodeTd.InnerText);

                                                int rs = 1;
                                                if (new_rspan.Length < i + 1)
                                                {
                                                    int[] nrspan = new int[i + 1];
                                                    rspan.CopyTo(nrspan, 0);
                                                    rspan = nrspan;
                                                    nrspan = new int[i + 1];
                                                    new_rspan.CopyTo(nrspan, 0);
                                                    new_rspan = nrspan;
                                                }
                                                if (nodeTd.Attributes.Contains("rowspan"))
                                                    int.TryParse(nodeTd.Attributes["rowspan"].Value, out rs);
                                                int shift = 0;
                                                for (int r = 0; r < rspan.Length; r++)
                                                {
                                                    if (rspan[r] > 1)
                                                    {
                                                        if (r + 1 > lstCol.Count) lstCol.Add("");
                                                        continue;
                                                    }
                                                    if (shift == i)
                                                    {
                                                        new_rspan[r] = rs;
                                                        break;
                                                    }
                                                    shift++;
                                                }
                                                lstCol.Add(tmp);
                                                if (nodeTd.Attributes.Contains("width"))
                                                {
                                                    int width;
                                                    if (int.TryParse(nodeTd.Attributes["width"].Value, out width) && width == 0)
                                                        lstHiddenColumns.Add(lstCol.Count - 1);
                                                }

                                                i++;
                                            }
                                        }

                                        while (table.Columns.Count < lstCol.Count)
                                            table.Columns.Add();

                                        foreach (var hide in lstHiddenColumns)
                                            if (table.Columns.Count > hide) table.Columns[hide].ExtendedProperties["hidden"] = true;

                                        string[] arrCol = new string[table.Columns.Count];
                                        if (lstCol.Count > table.Columns.Count)
                                            //не должно, вроде, выполняться
                                            lstCol.CopyTo(0, arrCol, 0, table.Columns.Count);
                                        else
                                            arrCol = lstCol.ToArray();
                                        table.Rows.Add(arrCol);

                                        for (int r = 0; r < rspan.Length; r++)
                                            if (rspan[r] - 1 > 1) new_rspan[r] += rspan[r] - 1;
                                        rspan = new_rspan;
                                        new_rspan = new int[rspan.Length];
                                    }
                                }

                                for (int i = 0; i < table.Columns.Count; i++)
                                {
                                    if (table.Columns[i].ExtendedProperties.ContainsKey("hidden"))
                                        if ((bool)table.Columns[i].ExtendedProperties["hidden"])
                                        {
                                            table.Columns.RemoveAt(i);
                                            i--;
                                        }
                                }

                                CleanTable(table);
                                if (table.Columns.Count > 0 && table.Rows.Count > 0)
                                    ds.Tables.Add(table);
                            }
                        }
                    }
                    catch (XmlException exc)
                    {
                        Console.WriteLine("{0}. StackTrace: {1}", exc.Message, exc.StackTrace);
                        //
                    }
                }
            }
            return ds;
        }

        private string GetText(string innerText)
        {
            StringBuilder sb = new StringBuilder();
            StringBuilder buf = new StringBuilder();
            bool spec = false;

            foreach (var item in innerText)
            {
                switch (item)
                {
                    case '&':
                        if (spec)
                            sb.Append("&" + buf.ToString());
                        else
                            spec = true;
                        break;
                    case ';':
                        if (spec)
                        {
                            sb.Append(ProcSpec(buf.ToString()));
                            spec = false;
                            buf = new StringBuilder();
                        }
                        else
                            sb.Append(item);
                        break;
                    default:
                        if (spec)
                            buf.Append(item);
                        else
                            sb.Append(item);
                        break;
                }
            }
            if (buf.Length > 0) sb.Append("&" + buf.ToString());
            return sb.ToString();
        }

        private string ProcSpec(string buf)
        {
            if (string.IsNullOrEmpty(buf)) return "";
            switch (buf)
            {
                case "nbsp": return " ";
                case "quot": return "\"";
            }
            if (buf[0] == '#')
            {
                uint c;
                if (uint.TryParse(buf.Substring(1), out c))
                    return ((char)c).ToString();
            }

            return "&" + buf + ";";
        }

        private void CleanTable(DataTable table)
        {
            List<DataRow> lstRowsToDelete = new List<DataRow>();
            List<DataColumn> lstColsToDelete = new List<DataColumn>();
            int state = 0;
            string cell;
            const string strUnit = "unit";
            bool skip = true;

            if (table == null || table.Columns.Count == 0) return;

            foreach (DataRow row in table.Rows)
            {
                cell = row[0].ToString();
                switch (state)
                {
                    case 0:// первая ячейка для названия таблички
                        table.TableName = cell.Replace('\n', ' ').Replace("\r", "").Trim();
                        lstRowsToDelete.Add(row);
                        state = 1;
                        break;
                    case 1:// ждем первую ячейку с текстом "Дата"
                        if (cell.ToLower() == "дата")
                        {
                            foreach (DataColumn col in table.Columns)
                            {
                                cell = row[col].ToString().Replace('\n', ' ').Replace("\r", "");
                                if (!string.IsNullOrEmpty(cell) && !table.Columns.Contains(cell))
                                    col.ColumnName = cell;
                            }
                            state = 2;
                            //break;
                        }
                        lstRowsToDelete.Add(row);
                        break;
                    case 2:// удаляем строки, пока не появится дата (вытаскивая ед. измерения),
                        // потом удаляем все строки, в которых нет даты в первой ячейке
                        DateTime date;
                        if (!DateTime.TryParse(cell, out date))
                        {
                            if (skip)
                            {
                                foreach (DataColumn col in table.Columns)
                                {
                                    cell = row[col].ToString().Trim();
                                    if (!string.IsNullOrEmpty(cell))
                                    {
                                        if (col.ExtendedProperties.ContainsKey(strUnit))
                                            col.ExtendedProperties[strUnit] = col.ExtendedProperties[strUnit].ToString() + "\n" + cell;
                                        else
                                            col.ExtendedProperties[strUnit] = cell;
                                    }
                                }
                            }
                            lstRowsToDelete.Add(row);
                        }
                        else
                            skip = false;
                        break;
                }
            }

            foreach (var item in lstRowsToDelete)
                table.Rows.Remove(item);

            //удаляем пустые колонки
            foreach (DataColumn col in table.Columns)
            {
                skip = false;
                foreach (DataRow row in table.Rows)
                {
                    if (!string.IsNullOrEmpty(row[col].ToString().Trim()))
                    {
                        skip = true;
                        break;
                    }
                }
                if (!skip) lstColsToDelete.Add(col);
            }
            foreach (var item in lstColsToDelete)
                table.Columns.Remove(item);
        }
    //}

    //static class UserManager
    //{
    //    #region Some DLL imports
    //    [DllImport("advapi32.dll", SetLastError = true)]
    //    public static extern bool LogonUser(string pszUsername, string pszDomain, string pszPassword,
    //          int dwLogonType, int dwLogonProvider, ref IntPtr phToken);

    //    [DllImport("kernel32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
    //    private unsafe static extern int FormatMessage(int dwFlags, ref IntPtr lpSource,
    //          int dwMessageId, int dwLanguageId, ref String lpBuffer, int nSize, IntPtr* Arguments);

    //    [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    //    public extern static bool DuplicateToken(IntPtr ExistingTokenHandle,
    //          int SECURITY_IMPERSONATION_LEVEL, ref IntPtr DuplicateTokenHandle);

    //    [DllImport("advapi32.dll", SetLastError = true)]
    //    public static extern int ImpersonateLoggedOnUser(
    //          IntPtr hToken
    //    );

    //    [DllImport("advapi32.dll", SetLastError = true)]
    //    static extern int RevertToSelf();


    //    // For CreateProcessAsUser
    //    enum LOGON_TYPE
    //    {
    //        LOGON32_LOGON_INTERACTIVE = 2,
    //        LOGON32_LOGON_NETWORK,
    //        LOGON32_LOGON_BATCH,
    //        LOGON32_LOGON_SERVICE,
    //        LOGON32_LOGON_UNLOCK = 7,
    //        LOGON32_LOGON_NETWORK_CLEARTEXT,
    //        LOGON32_LOGON_NEW_CREDENTIALS
    //    }

    //    enum LOGON_PROVIDER
    //    {
    //        LOGON32_PROVIDER_DEFAULT,
    //        LOGON32_PROVIDER_WINNT35,
    //        LOGON32_PROVIDER_WINNT40,
    //        LOGON32_PROVIDER_WINNT50
    //    }

    //    public enum SECURITY_IMPERSONATION_LEVEL
    //    {
    //        SecurityAnonymous = 0,
    //        SecurityIdentification = 1,
    //        SecurityImpersonation = 2,
    //        SecurityDelegation = 3
    //    }


    //    [StructLayout(LayoutKind.Sequential)]
    //    public struct STARTUPINFO
    //    {
    //        public int cb;
    //        public String lpReserved;
    //        public String lpDesktop;
    //        public String lpTitle;
    //        public uint dwX;
    //        public uint dwY;
    //        public uint dwXSize;
    //        public uint dwYSize;
    //        public uint dwXCountChars;
    //        public uint dwYCountChars;
    //        public uint dwFillAttribute;
    //        public uint dwFlags;
    //        public short wShowWindow;
    //        public short cbReserved2;
    //        public IntPtr lpReserved2;
    //        public IntPtr hStdInput;
    //        public IntPtr hStdOutput;
    //        public IntPtr hStdError;
    //    }

    //    [StructLayout(LayoutKind.Sequential)]
    //    public struct PROCESS_INFORMATION
    //    {
    //        public IntPtr hProcess;
    //        public IntPtr hThread;
    //        public uint dwProcessId;
    //        public uint dwThreadId;
    //    }

    //    [StructLayout(LayoutKind.Sequential)]
    //    public struct SECURITY_ATTRIBUTES
    //    {
    //        public int Length;
    //        public IntPtr lpSecurityDescriptor;
    //        public bool bInheritHandle;
    //    }

    //    [DllImport("kernel32.dll", EntryPoint = "CloseHandle", SetLastError = true, CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
    //    public extern static bool CloseHandle(IntPtr handle);

    //    [DllImport("advapi32.dll", EntryPoint = "CreateProcessAsUser", SetLastError = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
    //    public extern static bool CreateProcessAsUser(IntPtr hToken, String lpApplicationName, String lpCommandLine, ref SECURITY_ATTRIBUTES lpProcessAttributes,
    //          ref SECURITY_ATTRIBUTES lpThreadAttributes, bool bInheritHandle, int dwCreationFlags, IntPtr lpEnvironment,
    //          String lpCurrentDirectory, ref STARTUPINFO lpStartupInfo, out PROCESS_INFORMATION lpProcessInformation);

    //    [DllImport("advapi32.dll", EntryPoint = "DuplicateTokenEx")]
    //    public extern static bool DuplicateTokenEx(IntPtr ExistingTokenHandle, uint dwDesiredAccess,
    //          ref SECURITY_ATTRIBUTES lpThreadAttributes, int TokenType,
    //          int ImpersonationLevel, ref IntPtr DuplicateTokenHandle);

    //    [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    //    private static extern bool CreateProcessWithLogonW(String lpszUsername, String lpszDomain, String lpszPassword, int dwLogonFlags, string applicationName, StringBuilder commandLine, uint creationFlags, IntPtr environment, string currentDirectory, ref STARTUPINFO sui, out PROCESS_INFORMATION processInfo);
    //    #endregion

    //    public static WindowsImpersonationContext Login(string user, string password)
    //    {
    //        WindowsIdentity identity;
    //        WindowsImpersonationContext context = null;
    //        string domain = "192.168.0.9";

    //        try
    //        {
    //            context = Impersonate(domain, user, password, out identity);
    //        }
    //        //catch (Exception ex)
    //        //{
    //        //    MessageBox.Show(ex.Message);
    //        //}
    //        finally
    //        {
    //            //
    //        }

    //        return context;
    //    }

    //    public static void Logout(WindowsImpersonationContext context)
    //    {
    //        if (context != null) context.Undo();
    //    }

    //    #region GetErrorMessage
    //    // GetErrorMessage formats and returns an error message
    //    // corresponding to the input errorCode.
    //    private unsafe static string GetErrorMessage(int errorCode)
    //    {
    //        int FORMAT_MESSAGE_ALLOCATE_BUFFER = 0x00000100;
    //        int FORMAT_MESSAGE_IGNORE_INSERTS = 0x00000200;
    //        int FORMAT_MESSAGE_FROM_SYSTEM = 0x00001000;

    //        int messageSize = 255;
    //        String lpMsgBuf = "";
    //        int dwFlags = FORMAT_MESSAGE_ALLOCATE_BUFFER | FORMAT_MESSAGE_FROM_SYSTEM | FORMAT_MESSAGE_IGNORE_INSERTS;

    //        IntPtr ptrlpSource = IntPtr.Zero;
    //        IntPtr prtArguments = IntPtr.Zero;

    //        int retVal = FormatMessage(dwFlags, ref ptrlpSource, errorCode, 0, ref lpMsgBuf, messageSize, &prtArguments);
    //        if (0 == retVal)
    //        {
    //            throw new Exception("Failed to format message for error code " + errorCode + ". ");
    //        }

    //        return lpMsgBuf;
    //    }
    //    #endregion

    //    #region Impersonate
    //    [PermissionSetAttribute(SecurityAction.Demand, Name = "FullTrust")]
    //    private static WindowsImpersonationContext Impersonate(string domainName, string userName, string password, out WindowsIdentity identity)
    //    {
    //        System.Management.ManagementObject mo = new System.Management.ManagementObject(new System.Management.ManagementPath());
    //        mo.Scope.Options.EnablePrivileges = true;

    //        IntPtr tokenHandle = IntPtr.Zero;

    //        // Get the user token for the specified user, domain, and password using the 
    //        // unmanaged LogonUser method.
    //        const int LOGON32_PROVIDER_DEFAULT = 0;
    //        //This parameter causes LogonUser to create a primary token.
    //        const int LOGON32_LOGON_INTERACTIVE = 2;

    //        // Call LogonUser to obtain a handle to an access token.
    //        bool returnValue = LogonUser(userName, domainName, password,
    //              LOGON32_LOGON_INTERACTIVE, LOGON32_PROVIDER_DEFAULT,
    //              ref tokenHandle);

    //        if (!returnValue)
    //        {
    //            int ret = Marshal.GetLastWin32Error();
    //            throw new ApplicationException("Impersonation logon failed with code " + ret + "\nError: " + GetErrorMessage(ret));
    //        }

    //        // The token that is passed to the following constructor must 
    //        // be a primary token in order to use it for impersonation.
    //        identity = new WindowsIdentity(tokenHandle);
    //        WindowsImpersonationContext impersonatedUser = identity.Impersonate();

    //        // Free the tokens.
    //        if (tokenHandle != IntPtr.Zero)
    //            CloseHandle(tokenHandle);

    //        return impersonatedUser;
    //    }
    //    #endregion
        #region IDataLoader Members

        public void Init(ChannelInfo channelInfo)
        {
            channelLogPrefix = CommonProperty.ChannelMessagePrefix(channelInfo);

            log.Trace(channelLogPrefix + "Инициализация канала.");

            m_host = channelInfo[MHTDataLoaderFactory.HostProperty];
            m_path = channelInfo[MHTDataLoaderFactory.PathProperty];
            m_username = channelInfo[MHTDataLoaderFactory.UsernameProperty];
            m_password = channelInfo[MHTDataLoaderFactory.PasswordProperty];

            channel = channelInfo;
            parameters = new List<ParameterItem>(channelInfo.Parameters);

            log.Debug(channelLogPrefix + "Канал инициирован. Зарегистрировано {0} параметров.", parameters.Count);
        }

        public ParameterItem[] GetParameters()
        {
            List<ParameterItem> lstParams = new List<ParameterItem>();
            //CommonFunctions.UNCAccess access = null;
            try
            {
                ////usage...
                //NetUse.WinNet.UseRecord(m_path, m_username, m_password, String.Empty);
                //access = new CommonFunctions.UNCAccess();
                //if (!access.login(m_path, m_username, "192.168.0.9", m_password))
                //    Console.WriteLine("error: {0}", access.LastError);


                //System.Net.NetworkCredential cr = new System.Net.NetworkCredential(m_username, m_password);
                //lock (syncObject)
                //{
                //    using (NetworkConnection networkConnection = new WNetConnection.NetworkConnection(m_path, cr))
                //    {

                log.Trace(channelLogPrefix + "Запрос параметров.");
                
                Connect();

                DirectoryInfo di = new DirectoryInfo(m_path);
                System.Security.AccessControl.DirectorySecurity security = di.GetAccessControl();
                log.Trace(di);
                //security.
                FileInfo[] files = di.GetFiles("*.mht");
                DateTime max = DateTime.MinValue;
                FileInfo filemax = null;
                foreach (var item in files)
                {
                    if (regex.IsMatch(item.Name))
                    {
                        log.Trace("{0}({1})", item, item.CreationTime);

                        if (item.CreationTime > max)
                        {
                            max = item.CreationTime;
                            filemax = item;
                        }
                    }
                }
                if (filemax != null)
                {
                    DataSet ds = GetData(filemax.OpenRead()); //filemax.FullName);
                    if (ds != null)
                    {
                        Dictionary<string, ParameterItem> dicParams = new Dictionary<string, ParameterItem>();
                        ParameterItem p;
                        string pname;
                        foreach (DataTable table in ds.Tables)
                        {
                            foreach (DataColumn col in table.Columns)
                            {
                                switch (col.ColumnName.ToLower())
                                {
                                    case "дата":
                                    case "время":
                                        continue;
                                    default:
                                        if (!dicParams.ContainsKey(col.ColumnName))
                                        {
                                            p = new ParameterItem();
                                            pname = string.Format(@"{0}/{1}", table.TableName, col.ColumnName);
                                            p.Name = pname;
                                            p[CommonProperty.ParameterCodeProperty] = pname;
                                            if (col.ExtendedProperties.ContainsKey("unit"))
                                                p.SetPropertyValue(Consts.ParameterUnit, col.ExtendedProperties["unit"].ToString());

                                            dicParams[pname] = p;
                                        }
                                        break;
                                }
                            }
                        }
                        lstParams = dicParams.Values.ToList<ParameterItem>();
                    }
                    //if (xmler.IsOk)
                    //{
                    //    table.AddRange(xmler.Parameters);
                    //}
                }

                //table = new List<ParamSendItem>();
                //while (reader.Read())
                //{
                //    if (!reader.IsDBNull(0))
                //    {
                //        ParamSendItem p = new ParamSendItem();
                //        p.propertylist.Add(Consts.ParameterCode, reader[0].ToString());
                //        if (!reader.IsDBNull(1))
                //            p.name = reader[1].ToString();
                //        if (!reader.IsDBNull(2))
                //            p.propertylist.Add(Consts.ParameterUnit, reader[2].ToString());
                //        table.Add(p);
                //    }
                //} 
                //    } 
                //}
            }
//            catch (Exception exc)
//            {
//#if TRACE
//                Console.WriteLine("{0}. TargetSite: {1}. StackTrace: {2}", exc.Message, exc.TargetSite, exc.StackTrace);
//#endif
//                //
//            }
            finally
            {
                //if (reader != null) reader.Close();
                if (!KeepConnected)
                {
                    Disconnect();
                }
                //if (access != null)
                //    access.NetUseDelete();
            }

            log.Debug(channelLogPrefix + "Получен список из {0} параметров.", lstParams.Count);

            return lstParams.ToArray();
        }

        public IDataListener DataListener { get; set; }

        public DataLoadMethod LoadMethod
        {
            get { return DataLoadMethod.Archive; }
        }

        public void RegisterSubscribe()
        {
            throw new NotSupportedException();
        }

        public void UnregisterSubscribe()
        {
            throw new NotSupportedException();
        }

        public void GetCurrent()
        {
            throw new NotSupportedException();
        }

        public void GetArchive(DateTime startTime, DateTime endTime)
        {
            try
            {
                //System.Net.NetworkCredential cr = new System.Net.NetworkCredential(m_username, m_password);
                //lock (syncObject)
                //{
                //    using (NetworkConnection networkConnection = new WNetConnection.NetworkConnection(m_path, cr))
                //    {
                int valuesCount = 0;

                log.Trace(channelLogPrefix + "Запрос архивных данных за [{0}; {1}].", startTime, endTime);
                
                Connect();

                DirectoryInfo di = new DirectoryInfo(m_path);
                FileInfo[] files = di.GetFiles("*.mht");
                //Regex regex = new Regex(@"(\d{1,2}\.\d{1,2}\.\d{2,4})");
                Match m;
                DateTime date1;
                DateTime TimeStartCorr = startTime.AddDays(-1);
                DateTime TimeEndCorr = endTime.AddDays(1);

                Dictionary<ParameterItem, List<ParamValueItem>> valuesDictionary = new Dictionary<ParameterItem, List<ParamValueItem>>();
                List<ParamValueItem> valueList;

                foreach (var item in files)
                {
                    m = regex.Match(item.Name);
                    if (m.Success)
                    {
                        if (DateTime.TryParse(m.Groups[1].Value, out date1))
                        {
                            if ((TimeStartCorr <= date1 && date1 <= TimeEndCorr))
                            {
                                try
                                {
                                    //ParamValueItemWithID[] arrParams;
                                    DataSet ds = GetData(item.OpenRead());//item.FullName);
                                    if (ds != null)
                                    {
                                        Dictionary<string, ParameterItem> dicParams = new Dictionary<string, ParameterItem>();
                                        List<DataColumn> lstColumnsToRemove;
                                        string pname;
                                        bool found;
                                        foreach (DataTable table in ds.Tables)
                                        {
                                            DataColumn clmDate = null, clmTime = null;
                                            lstColumnsToRemove = new List<DataColumn>();
                                            foreach (DataColumn col in table.Columns)
                                            {
                                                found = false;
                                                switch (col.ColumnName.ToLower())
                                                {
                                                    case "дата":
                                                        clmDate = col;
                                                        break;
                                                    case "время":
                                                        clmTime = col;
                                                        break;
                                                    default:
                                                        pname = string.Format(@"{0}/{1}", table.TableName, col.ColumnName);
                                                        foreach (var par in parameters)
                                                            if (par[CommonProperty.ParameterCodeProperty] == pname)
                                                            {
                                                                dicParams[pname] = par;
                                                                found = true;
                                                                break;
                                                            }
                                                        if (!found)
                                                            lstColumnsToRemove.Add(col);
                                                        break;
                                                }
                                            }

                                            if (clmDate == null || clmTime == null) continue;

                                            foreach (var col in lstColumnsToRemove)
                                                table.Columns.Remove(col);

                                            DateTime lastdate = DateTime.MinValue;
                                            foreach (DataRow row in table.Rows)
                                            {
                                                DateTime date;
                                                TimeSpan time;
                                                bool isBad = false;

                                                if (!DateTime.TryParse(row[clmDate].ToString(), out date)) continue;
                                                if (!TimeSpan.TryParse(row[clmTime].ToString(), out time)) continue;
                                                date = date.Subtract(-time);
                                                if (date < startTime || date > endTime) continue;
                                                if (date == lastdate) isBad = true;
                                                else lastdate = date;
                                                foreach (DataColumn col in table.Columns)
                                                {
                                                    double val;
                                                    if (col == clmDate || col == clmTime) continue;

                                                    pname = string.Format(@"{0}/{1}", table.TableName, col.ColumnName);
                                                    if (dicParams.ContainsKey(pname))
                                                    {
                                                        var parameter=dicParams[pname];

                                                        if (!valuesDictionary.TryGetValue(parameter, out valueList))
                                                        {
                                                            valuesDictionary[parameter] = valueList = new List<ParamValueItem>();
                                                        }
                                                        var value = new ParamValueItem();

                                                        value.Time = date;
                                                        //try
                                                        //{
                                                        val = Convert.ToDouble(row[col].ToString(), CultureInfo.InvariantCulture);
                                                        //    parameter = new ParamValueItemWithID();
                                                        //    parameter.Time = date;
                                                        if (isBad)
                                                            value.Quality = Quality.Bad;
                                                        else
                                                            value.Quality = Quality.Good;
                                                        value.Value = val;
                                                        valueList.Add(value);
                                                        //    parameter.ParameterID = dicParams[pname].param;
                                                        //    lstResult.Add(parameter);
                                                        //}
                                                        //catch (InvalidCastException) { }
                                                        //catch (FormatException) { }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                catch (Exception exc)
                                {
                                    log.ErrorException(String.Format("Ошибка чтения файла {0}", item), exc);
                                }
                            }
                        }
                    }
                }

                foreach (var parameter in valuesDictionary.Keys)
                {
                    DataListener.NotifyValues(null, parameter, valuesDictionary[parameter]);
                    
                    if (log.IsDebugEnabled)
                    {
                        valuesCount += valuesDictionary[parameter].Count;
                    }
                }

                log.Debug(channelLogPrefix + "Получено архивных данных за период [{0}; {1}]: {2}", startTime, endTime, valuesCount);
                //    } 
                //}
            }
            finally
            {
                //if (dataReader != null) dataReader.Close();
                if (!KeepConnected)
                {
                    Disconnect();
                }
            }
        }

        public void SetArchiveParameterTime(ParameterItem parameter, DateTime startTime, DateTime endTime)
        {
            throw new NotSupportedException();
        }

        public void GetArchive()
        {
            throw new NotSupportedException();
        }

        #endregion
    }
}
