using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace COTES.ISTOK.Assignment
{
    internal class Cryptor
    {
        private byte[] rc2IV ={ 234, 201, 57, 75, 227, 26, 242, 152 };
        private byte[] rc2Key ={ 191, 127, 167, 88, 52, 154, 22, 34, 204, 78, 32, 1, 131, 10, 218, 252 };

        private RC2 rc2Cryptor;

        public Cryptor()
        {
            rc2Cryptor = RC2.Create();
            rc2Cryptor.IV = rc2IV;
            rc2Cryptor.Key = rc2Key;
        }

        public String EncryptingString(String str)
        {
            MemoryStream encryptingStream = new MemoryStream();
            StreamWriter writer;
            //byte[] source = Encoding.UTF8.GetBytes(str);
            byte[] dest;
            ICryptoTransform encryptor = rc2Cryptor.CreateEncryptor(rc2Key, rc2IV);

            using (CryptoStream cryptoStream = new CryptoStream(encryptingStream, encryptor, CryptoStreamMode.Write))
            {
                writer = new StreamWriter(cryptoStream);
                writer.Write(str);
                writer.Flush();
                cryptoStream.FlushFinalBlock();

                dest = encryptingStream.ToArray();
                cryptoStream.Clear();
            }

            return Encoding.Default.GetString(dest);
        }

        public String DecryptingString(String str)
        {
            byte[] source = Encoding.Default.GetBytes(str);
            MemoryStream srcStream = new MemoryStream(source);
            StreamReader reader = null;
            //byte[] dest;
            String plaintext;

            using (CryptoStream cryptoStream = new CryptoStream(srcStream, rc2Cryptor.CreateDecryptor(rc2Key, rc2IV), CryptoStreamMode.Read))
            {
                reader = new StreamReader(cryptoStream);
                plaintext = reader.ReadToEnd();
                cryptoStream.Clear();
            }

            return plaintext;
        }
    }
}
