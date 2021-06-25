using System;
using System.Security.Cryptography;
using System.Text;

namespace Bootstrap.Components.Cryptography
{
    public class CryptographyUtils
    {
        public static string Md5(string str)
        {
            return BitConverter.ToString(MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(str))).Replace("-", "");
        }

        public static byte[] Crc16(byte[] data)
        {
            var len = data.Length;
            if (len > 0)
            {
                ushort crc = 0xFFFF;
                for (var i = 0; i < len; i++)
                {
                    crc = (ushort) (crc ^ (data[i]));
                    for (var j = 0; j < 8; j++)
                    {
                        crc = (crc & 1) != 0 ? (ushort) ((crc >> 1) ^ 0xA001) : (ushort) (crc >> 1);
                    }
                }

                var hi = (byte) ((crc & 0xFF00) >> 8); //High
                var lo = (byte) (crc & 0x00FF); //Low

                return new[] {lo, hi};
            }

            return new byte[] {0, 0};
        }

        public static byte[] AesEncrypt(byte[] data, byte[] key)
        {
            var rm = new RijndaelManaged
            {
                Key = key,
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };

            var cTransform = rm.CreateEncryptor();
            return cTransform.TransformFinalBlock(data, 0, data.Length);
        }

        public static string AesEncryptAndBase64Encoding(byte[] data, byte[] key)
        {
            var result = AesEncrypt(data, key);
            return Convert.ToBase64String(result, 0, result.Length);
        }

        public static string AesEncryptAndBase64Encoding(string data, string key)
        {
            return AesEncryptAndBase64Encoding(Encoding.UTF8.GetBytes(data), Encoding.UTF8.GetBytes(key));
        }

        /// <summary>
        ///  AES 解密
        /// </summary>
        /// <param name="str">明文（待解密）</param>
        /// <param name="key">密文</param>
        /// <returns></returns>
        public static string AesDecrypt(string str, string key)
        {
            if (string.IsNullOrEmpty(str)) return null;
            var toEncryptArray = Convert.FromBase64String(str);

            var rm = new RijndaelManaged
            {
                Key = Encoding.UTF8.GetBytes(key),
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };

            var cTransform = rm.CreateDecryptor();
            var resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            return Encoding.UTF8.GetString(resultArray);
        }
    }
}