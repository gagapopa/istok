using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace COTES.ISTOK
{
    internal class LicenseKey
    {
        private const String passKey = "Gypaetus";
        private const char keySeparator = '\t';
        private const String organizationKey = "dr";
        private const String registerDateKey = "or";
        private const String blockLimiterKey = "lr";

        private String organization = null;
        private DateTime registerDate = DateTime.MaxValue;
        private int maxBlockCount = 0;

        private RC2 rc2Cryptor;
        private byte[] rc2IV = { 36, 215, 159, 114, 59, 8, 44, 210 };
        private byte[] rc2Key = { 188, 107, 213, 169, 142, 63, 168, 254, 196, 255, 131, 141, 138, 73, 242, 217 };

        public LicenseKey()
        { rc2Cryptor = RC2.Create(); }

        /// <summary>
        /// Название организации
        /// </summary>
        public String Organization
        {
            get
            {
                if (organization == null) return String.Empty;
                else return organization;
            }
#if _COTES_KEY_Generator
            set { organization = value; }
#endif
        }

        /// <summary>
        /// Дата выдачи лицензии
        /// </summary>
        public DateTime RegisterDate
        {
            get { return registerDate; }
#if _COTES_KEY_Generator
            set { registerDate = value; }
#endif
        }

        /// <summary>
        /// Количество подключений блочных
        /// </summary>
        public int MaxBlockCount
        {
            get { return maxBlockCount; }
#if _COTES_KEY_Generator
            set { maxBlockCount = value; }
#endif
        }
#if _COTES_KEY_Generator
        /// <summary>
        /// Сгенерировать ключ, на основании запоненых полей
        /// </summary>
        /// <param name="machineID">8ми байтный код оборудования</param>
        /// <returns></returns>
        public byte[] GenerateKey(UInt64 machineID)
        {
            MemoryStream encryptingStream = new MemoryStream();
            StreamWriter writer;
            ICryptoTransform encryptor;
            char[] source;
            byte[] dest;
            String sourceKey = passKey, keyFormat = keySeparator + "{0}={1}";
            String machineString = machineID.ToString("X");
            int i, machineStringLength = machineString.Length;

            // формирование лицензионной строки
            sourceKey += String.Format(keyFormat, registerDateKey, registerDate.ToString("yyyy-MM-dd"));
            sourceKey += String.Format(keyFormat, organizationKey, organization);

            // к сформированной строки прибавляем 8ми байтный код оборудования
            source = new char[sourceKey.Length];
            for (i = 0; i < sourceKey.Length; i++) source[i] = (char)(sourceKey[i] + machineString[i % machineStringLength]);

            // получинную строку шифруем стандартной функцией
            encryptor = rc2Cryptor.CreateEncryptor(rc2Key, rc2IV);
            using (CryptoStream cryptoStream = new CryptoStream(encryptingStream, encryptor, CryptoStreamMode.Write))
            {
                writer = new StreamWriter(cryptoStream);
                writer.Write(new String(source));
                writer.Flush();
                cryptoStream.FlushFinalBlock();

                dest = encryptingStream.ToArray();
                cryptoStream.Clear();
            }

            return dest;
        }
#else
        /// <summary>
        /// Заполнить поля на основании ключа
        /// </summary>
        /// <param name="key">лицензионный ключ</param>
        /// <param name="machineID">8ми байтный код оборудования</param>
        /// <returns>true, если сгенерирован для этого оборудования,
        /// false - в противном случае</returns>
        public bool Active(byte[] key, ulong machineID)
        {
            MemoryStream srcStream = new MemoryStream(key);
            StreamReader reader = null;
            String machineString = machineID.ToString("X");
            int i, machineStringLength = machineString.Length;
            String plaintext;

            // расшифровываем входную строку и вычитаем из нее код оборудования
            using (CryptoStream cryptoStream = new CryptoStream(srcStream, rc2Cryptor.CreateDecryptor(rc2Key, rc2IV), CryptoStreamMode.Read))
            {
                reader = new StreamReader(cryptoStream);
                plaintext = reader.ReadToEnd();
                cryptoStream.Clear();
            }
            char[] source = new char[plaintext.Length];
            for (i = 0; i < plaintext.Length; i++) source[i] = (char)(plaintext[i] - machineString[i % machineStringLength]);

            String src = new String(source);

            String[] strs = src.Split(keySeparator); // разбираем полученную строку
            if (strs[0].Equals(passKey))
            {
                for (i = 1; i < strs.Length; i++)
                {
                    int ind;
                    String parKey, value;
                    ind = strs[i].IndexOf('=');
                    parKey = strs[i].Substring(0, ind);
                    value = strs[i].Substring(ind + 1);
                    switch (parKey)
                    {
                        case organizationKey:
                            organization = value;
                            break;
                        case registerDateKey:
                            try
                            {
                                registerDate = DateTime.Parse(value);
                            }
                            catch (FormatException) { return false; }
                            break;
                        case blockLimiterKey:
                            if (!int.TryParse(value, out maxBlockCount)) return false;
                            break;
                    }
                }
                return true;
            }
            return false;
        }
#endif
    }
}
