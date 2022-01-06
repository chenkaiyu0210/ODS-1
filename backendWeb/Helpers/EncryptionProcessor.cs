using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace backendWeb.Helpers
{
    /// <summary>
    /// 加解密通用介面
    /// </summary>
    public abstract class EncryptionProcessor<T> where T : class
    {
        public abstract byte[] Decode(byte[] base64Bytes);
        public abstract byte[] Encode(byte[] data);
    }
    /// <summary>
    /// Base64加解密
    /// </summary>
    public class Base64Processor : EncryptionProcessor<Base64Processor>
    {
        public override byte[] Decode(byte[] base64Bytes)
        {
            var bytes = Convert.FromBase64String(Encoding.UTF8.GetString(base64Bytes));
            return bytes;
        }

        public override byte[] Encode(byte[] data)
        {
            return Encoding.UTF8.GetBytes(Convert.ToBase64String(data));
        }
    }
    /// <summary>
    /// DES加解密
    /// </summary>
    public class DESCryptoProcessor : EncryptionProcessor<DESCryptoProcessor>
    {
        private byte[] key;
        private byte[] iv;
        private DESCryptoServiceProvider des;
        public DESCryptoProcessor()
        {
            //key = new byte[] { 0x01, 0xFF, 0x02, 0xAA, 0x55, 0xBB, 0x19, 0x20 };
            //iv = new byte[] { 0x11, 0xF3, 0x43, 0x0A, 0x35, 0xE9, 0x82, 0x80 };
            byte[] salt = new byte[] { 0x0A, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0xF1 };
            Rfc2898DeriveBytes rfcKey = new Rfc2898DeriveBytes("D823CADC-5C99-47A6-A865-239272C59F39", salt, 8);
            Rfc2898DeriveBytes rfcIv = new Rfc2898DeriveBytes("F2C07BCF-78A9-45CE-9E20-0F444141BCA7", salt, 8);
            key = rfcKey.GetBytes(8);
            iv = rfcIv.GetBytes(8);
            des = new DESCryptoServiceProvider();
            des.Key = key;
            des.IV = iv;
        }

        public override byte[] Decode(byte[] encryptBytes)
        {
            byte[] outputBytes = null;
            using (MemoryStream memoryStream = new MemoryStream(encryptBytes))
            {
                using (CryptoStream decryptStream = new CryptoStream(memoryStream, des.CreateDecryptor(), CryptoStreamMode.Read))
                {
                    MemoryStream outputStream = new MemoryStream();
                    decryptStream.CopyTo(outputStream);
                    outputBytes = outputStream.ToArray();
                }
            }
            return outputBytes;
        }

        public override byte[] Encode(byte[] data)
        {
            byte[] outputBytes = null;
            using (MemoryStream memoryStream = new MemoryStream())
            {

                using (CryptoStream encryptStream = new CryptoStream(memoryStream, des.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    MemoryStream inputStream = new MemoryStream(data);
                    inputStream.CopyTo(encryptStream);
                    encryptStream.FlushFinalBlock();
                    outputBytes = memoryStream.ToArray();
                }
            }

            return outputBytes;
        }
    }
    /// <summary>
    /// AES加解密
    /// </summary>
    public class AESCryptoProcessor : EncryptionProcessor<AESCryptoProcessor>
    {
        private byte[] key;
        private byte[] iv;
        private AesCryptoServiceProvider aes;
        public AESCryptoProcessor()
        {
            ////key = new byte[] { 0x01, 0xFF, 0x02, 0xAA, 0x55, 0xBB, 0x19, 0x20 };
            ////iv = new byte[] { 0x11, 0xF3, 0x43, 0x0A, 0x35, 0xE9, 0x82, 0x80 };
            ////key = Convert.FromBase64String(ConfigurationManager.AppSettings["EncryptionKey"].ToString());
            ////iv = Convert.FromBase64String(ConfigurationManager.AppSettings["IvKey"].ToString());
            //byte[] salt = new byte[] { 0x0A, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0xF1 };
            //Rfc2898DeriveBytes rfcKey = new Rfc2898DeriveBytes("D823CADC-5C99-47A6-A865-239272C59F39", salt, 8);
            //Rfc2898DeriveBytes rfcIv = new Rfc2898DeriveBytes("F2C07BCF-78A9-45CE-9E20-0F444141BCA7", salt, 8);
            //key = rfcKey.GetBytes(8);
            //iv = rfcIv.GetBytes(8);
            //aes = new AesCryptoServiceProvider();
            //aes.Key = key;
            //aes.IV = iv;

            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            SHA256CryptoServiceProvider sha256 = new SHA256CryptoServiceProvider();
            key = sha256.ComputeHash(Encoding.UTF8.GetBytes("D823CADC-5C99-47A6-A865-239272C59F39"));
            iv = md5.ComputeHash(Encoding.UTF8.GetBytes("F2C07BCF-78A9-45CE-9E20-0F444141BCA7"));
            aes = new AesCryptoServiceProvider();
            aes.KeySize = 256;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            aes.Key = key;
            aes.IV = iv;
        }

        public override byte[] Decode(byte[] encryptBytes)
        {
            byte[] outputBytes = null;
            using (MemoryStream memoryStream = new MemoryStream(encryptBytes))
            {
                using (CryptoStream decryptStream = new CryptoStream(memoryStream, aes.CreateDecryptor(), CryptoStreamMode.Read))
                {
                    MemoryStream outputStream = new MemoryStream();
                    decryptStream.CopyTo(outputStream);
                    outputBytes = outputStream.ToArray();
                }
            }
            return outputBytes;
        }

        public override byte[] Encode(byte[] data)
        {
            byte[] outputBytes = null;
            //using (MemoryStream ms = new MemoryStream())
            //{
            //    using (CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write))
            //    {
            //        cs.Write(data, 0, data.Length);
            //        cs.FlushFinalBlock();
            //        outputBytes = ms.ToArray();
            //    }
            //}
            using (MemoryStream memoryStream = new MemoryStream())
            {

                using (CryptoStream encryptStream = new CryptoStream(memoryStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    MemoryStream inputStream = new MemoryStream(data);
                    inputStream.CopyTo(encryptStream);
                    encryptStream.FlushFinalBlock();
                    outputBytes = memoryStream.ToArray();
                }

            }

            return outputBytes;
        }
    }
    /// <summary>
    /// Rijndael加解密
    /// </summary>
    public class RijndaelProcessor : EncryptionProcessor<RijndaelProcessor>
    {
        private RijndaelManaged rijndael;
        /// <summary>
        ///  一般設定
        /// </summary>
        /// <param name="keyValue">key 值</param>
        /// <param name="ivValue">iv 值</param>
        public RijndaelProcessor(string keyValue, string ivValue)
        {
            rijndael = new RijndaelManaged();
            rijndael.KeySize = 256;
            rijndael.BlockSize = 128;
            rijndael.Key = Encoding.UTF8.GetBytes(keyValue);
            rijndael.IV = Encoding.UTF8.GetBytes(ivValue);
            rijndael.Mode = CipherMode.CBC;
            rijndael.Padding = PaddingMode.PKCS7;
        }
        /// <summary>
        /// 進階設定
        /// </summary>
        /// <param name="keyValue">key 值</param>
        /// <param name="ivValue">iv 值</param>
        /// <param name="keySize">key鍵入值大小 </param>
        /// <param name="blockSize">密碼編譯區塊大小</param>
        /// <param name="cipher">演算法作業模式</param>
        /// <param name="padding">演算法使用填補模式</param>
        public RijndaelProcessor(string keyValue, string ivValue, int keySize, int blockSize, CipherMode cipher, PaddingMode padding)
        {
            rijndael = new RijndaelManaged();
            rijndael.KeySize = keySize;
            rijndael.BlockSize = blockSize;
            rijndael.Key = Encoding.UTF8.GetBytes(keyValue);
            rijndael.IV = Encoding.UTF8.GetBytes(ivValue);
            rijndael.Mode = cipher;
            rijndael.Padding = padding;
        }

        public override byte[] Decode(byte[] encryptBytes)
        {
            byte[] outputBytes = null;
            using (MemoryStream memoryStream = new MemoryStream(encryptBytes))
            {
                using (CryptoStream decryptStream = new CryptoStream(memoryStream, rijndael.CreateDecryptor(), CryptoStreamMode.Read))
                {
                    MemoryStream outputStream = new MemoryStream();
                    decryptStream.CopyTo(outputStream);
                    outputBytes = outputStream.ToArray();
                }
            }
            return outputBytes;
        }

        public override byte[] Encode(byte[] data)
        {
            byte[] outputBytes = null;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (CryptoStream encryptStream = new CryptoStream(memoryStream, rijndael.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    MemoryStream inputStream = new MemoryStream(data);
                    inputStream.CopyTo(encryptStream);
                    encryptStream.FlushFinalBlock();
                    outputBytes = memoryStream.ToArray();
                }
            }
            return outputBytes;
        }
    }
    /// <summary>
    /// MD5加密
    /// </summary>
    public class MD5Processor : EncryptionProcessor<MD5Processor>
    {
        public override byte[] Decode(byte[] base64Bytes)
        {
            return null;
        }

        public override byte[] Encode(byte[] data)
        {
            string pwd = "";
            MD5 md5 = MD5.Create();//實體化一個md5類別
            //加密後是一個byte[]，這裡要注意編碼UTF8/Unicode的選擇
            byte[] s = md5.ComputeHash(data);
            //通过迴圈，將byte[]的內容轉換為字符串，此字符串是常规字符格式化所得
            for (int i = 0; i < s.Length; i++)
            {
                //將得到的字符串使用十六進制類型格式。格式後的字符是小寫的字母，如果使用大寫（X）則格式後的字符是大寫字符                
                pwd = pwd + s[i].ToString("x2"); //x:十六進制 2:每次都是兩位數
            }
            return Encoding.UTF8.GetBytes(pwd);
        }
    }
    /// <summary>
    /// SHA256加密
    /// </summary>
    public class SHA256Processor : EncryptionProcessor<SHA256Processor>
    {
        private string salt { get; set; }
        public SHA256Processor()
        {

        }
        public SHA256Processor(string salt)
        {
            this.salt = salt;
        }

        public override byte[] Decode(byte[] base64Bytes)
        {
            return null;
        }

        public override byte[] Encode(byte[] data)
        {
            string str = Encoding.UTF8.GetString(data);
            SHA256 sha256 = new SHA256CryptoServiceProvider();//建立一個SHA256    
            string result = string.IsNullOrWhiteSpace(salt) ? str : salt + str;
            byte[] source = Encoding.Default.GetBytes(result);//將字串轉為Byte[]
            byte[] crypto = sha256.ComputeHash(source);//進行SHA256加密
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < crypto.Length; i++)
            {
                builder.Append(crypto[i].ToString("x2"));
            }
            //return Convert.ToBase64String(crypto);//輸出結果，把加密後的字串從Byte[]轉為字串   
            return Encoding.UTF8.GetBytes(builder.ToString());
        }
    }
}