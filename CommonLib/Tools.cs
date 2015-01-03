using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Windows.Forms;
using System.Data;
using System.IO;
using System.Globalization;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using System.ComponentModel;
using System.Net;
using System.Security.Cryptography;
using System.Security;
using System.Runtime.InteropServices;

namespace COTES.ISTOK
{
    // общие для всех данные
    public partial class CommonData
    {
        public const String IstokDataReadMutexName = "ISTOK-A7AAB2A7-CCCA-4ca1-A2FF-B13A4D24A0D4";

        public static String RegisterOrganization;
        public static DateTime RegisterDate;
        /// <summary>
        /// Максимальное количество подключений блочных
        /// </summary>
        public static int MaxBlockCount;

        // проверить регистрацию по файлу с ключом
        public static bool CheckRegister(String licenseFileName)
        {
            bool reg = false;

            byte[] buf = new byte[256], key;
            FileInfo finfo = new FileInfo(licenseFileName);
            if (finfo.Exists)
            {
                FileStream file = new FileStream(licenseFileName, FileMode.Open, FileAccess.Read);
                int cnt = file.Read(buf, 0, 256);
                key = new byte[cnt];

                Array.Copy(buf, 0, key, 0, cnt);
                reg = CheckRegisterKey(key);

                file.Close();
            }
            else throw new Exception("Файл не найден.");
            return reg;
        }
        // Проверить регистрацию по ключу
        public static bool CheckRegisterKey(byte[] key)
        {
            bool reg;
            LicenseKey license = new LicenseKey();
            reg = license.Active(key, MachineIdentification.GetMachineID());
            if (reg)
            {
                RegisterOrganization = license.Organization;
                RegisterDate = license.RegisterDate;
                MaxBlockCount = license.MaxBlockCount;
            }
            return reg;
        }

        #region Константы

        #region Названия свойств
        public const string IntervalProperty = "interval";
        public const string RelevanceProperty = "relevance";
        public const string ApertureProperty = "aperture";
        public const string MaxValueProperty = "MaxValue";
        public const string MinValueProperty = "MinValue";
        public const string StoreDBProperty = "store_db";

        public const string BlockUIDProperty = "block_uid";

        #endregion

        #region Информация для справочников
        public const string CanalTableName = "canals";
        public const string ParameterTableName = "parameters";
        public const string PropertyTableName = "properties";
        #endregion
        
        #region Названия служб
        public const string GlobalServiceName = "ISTOKStation";
        public const string BlockServiceName = "ISTOKLoader";
        #endregion

        #region Названия ремоутинг объектов
        public const string BlockDiagnosticsURI = "BlockDiagnostics.rem";
        public const string GlobalDiagnosticsURI = "GlobalDiagnostics.rem";
        public const string QueryManagerURI = "QueryManager.rem";
        public const string TunmanInfoURI = "TunmanInfo.rem";
        #endregion      
        #endregion
        // настройки
        public static string applicationName = null;


        public enum Approx
        {
            Linear,
            Quadratic,
            None
        }

        public static ulong GetMachineCode()
        {
            return MachineIdentification.GetMachineID();
        }

        private static byte[] GetBytes(int pos, int array)
        {
            byte[] bytes = new byte[array];

            for (int i = 0; i < array; i++)
                bytes[i] = (byte)(Math.E * pos * (pos + i) * (pos + array) / Math.PI);

            return bytes;
        }

        public static string SecureStringToString(SecureString sstring)
        {
            IntPtr ptr = Marshal.SecureStringToBSTR(sstring);
            string res = Marshal.PtrToStringBSTR(ptr);
            Marshal.FreeBSTR(ptr);

            return res;
        }
        public static SecureString StringToSecureString(string str)
        {
            SecureString res = new SecureString();

            foreach (var item in str) res.AppendChar(item);

            return res;
        }

        public static string Base64ToString(string base64)
        {
            if (base64 == null)
                return String.Empty;
            return Encoding.Default.GetString(Convert.FromBase64String(base64));
        }
        public static string StringToBase64(string str)
        {
            return Convert.ToBase64String(Encoding.Default.GetBytes(str));
        }

        public static SecureString DecryptText(string cryptedText)
        {
            SecureString res = new SecureString();
            MemoryStream stream = new MemoryStream();
            CryptoStream cstream = null;
            StreamReader sreader = null;
            Rijndael alg = Rijndael.Create();

            if (cryptedText == null) return null;
            if (cryptedText == "") return res;

            try
            {
                byte[] arr = Encoding.Default.GetBytes(cryptedText);
                stream.Write(arr, 0, arr.Length);
                stream.Flush();
                stream.Seek(0, SeekOrigin.Begin);
                cstream = new CryptoStream(stream,
                    alg.CreateDecryptor(GetBytes(315, 32),
                            GetBytes(731, 16)),
                    CryptoStreamMode.Read);
                sreader = new StreamReader(cstream);
                string str = sreader.ReadToEnd();
                foreach (char item in str) res.AppendChar(item);
            }
            finally
            {
                if (sreader != null) sreader.Close();
                if (cstream != null) cstream.Close();
                if (stream != null) stream.Close();
            }

            return res;
        }
        public static string EncryptText(SecureString sstring)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter swriter = null;
            CryptoStream cstream = null;
            Rijndael alg = Rijndael.Create();
            string res = null;

            try
            {
                cstream = new CryptoStream(stream,
                    alg.CreateEncryptor(GetBytes(315, 32),
                            GetBytes(731, 16)),
                    CryptoStreamMode.Write);
                swriter = new StreamWriter(cstream);
                swriter.Write(SecureStringToString(sstring));
                swriter.Flush();
                cstream.FlushFinalBlock();

                byte[] arr = stream.ToArray();
                res = Encoding.Default.GetString(arr);
            }
            finally
            {
                if (swriter != null) swriter.Close();
                if (cstream != null) cstream.Close();
                if (stream != null) stream.Close();
            }

            return res;
        }
    }

    public enum ServerType
    {
        MSSQL,
        MSSQLOLE,
        Oracle,
        MySQL,
        MSJet,
        Files
    }

    public static class ServerTypeFormatter
    {
        public static ServerType Format(string serverType)
        {
            switch (serverType.ToLower())
            {
                case "mssql": return ServerType.MSSQL;
                case "oracle": return ServerType.Oracle;
                case "mysql": return ServerType.MySQL;
                case "files": return ServerType.Files;
                default:
                    throw new ArgumentException("Wrong server type");
            }
        }

        public static string Format(ServerType serverType)
        {
            switch (serverType)
            {
                case ServerType.MSSQL: return "MSSQL";
                case ServerType.MySQL: return "MySQL";
                case ServerType.Oracle: return "Oracle";
                case ServerType.Files: return "Files";
                default:
                    throw new ArgumentException("Wrong server type");
            }
        }
    }
}
