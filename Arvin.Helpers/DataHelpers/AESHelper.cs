using Arvin.Extensions;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Arvin.Helpers
{
    public class AESHelper
    {
        private readonly string EncryptionKey;

        public AESHelper(string key) : this(key, "") { }
        public AESHelper(string key, string EncryptionKey) : this(key, EncryptionKey, PaddingMode.PKCS7, CipherMode.ECB, null) { }

        public AESHelper(string key, string EncryptionKey, PaddingMode PaddingMode, CipherMode model, Byte[] iv)
        {
            RM = new RijndaelManaged()
            {
                Key = Encoding.UTF8.GetBytes(key),
                Mode = model,
                Padding = PaddingMode
            };
            if (iv != null)
                RM.IV = iv;
            this.EncryptionKey = EncryptionKey;
        }

        public AESHelper(RijndaelManaged rm, string EncryptionKey)
        {
            RM = rm;
            this.EncryptionKey = EncryptionKey;
        }

        public RijndaelManaged RM { get; set; }
        /// <summary>
        ///  AES 加密
        /// </summary>
        /// <param name="str">明文（待加密）</param>
        /// <returns></returns>
        public string AesEncrypt(string str)
        {
            if (str.IsNullOrEmpty())
                return str;
            Byte[] toEncryptArray = Encoding.UTF8.GetBytes(str);
            System.Security.Cryptography.ICryptoTransform cTransform = RM.CreateEncryptor();
            Byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            var encryptedValue = Convert.ToBase64String(resultArray, 0, resultArray.Length);
            if (EncryptionKey.IsNullOrWhiteSpace())
                return encryptedValue;
            else
                return EncryptionKey + encryptedValue + EncryptionKey;
        }

        /// <summary>
        ///  AES 解密
        /// </summary>
        /// <param name="str">明文（待解密）</param>
        /// <returns></returns>
        public string AesDecrypt(string str)
        {
            if (string.IsNullOrEmpty(str)) return "";
            if (!string.IsNullOrEmpty(str) && str.StartsWith(EncryptionKey) && str.EndsWith(EncryptionKey))
            {
                if (!EncryptionKey.IsNullOrWhiteSpace())
                    str = str.Substring(EncryptionKey.Length, str.Length - EncryptionKey.Length * 2);
                Byte[] toEncryptArray = Convert.FromBase64String(str);
                System.Security.Cryptography.ICryptoTransform cTransform = RM.CreateDecryptor();
                Byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
                return Encoding.UTF8.GetString(resultArray);
            }
            else
            {
                return str;
            }
        }
    }
}
