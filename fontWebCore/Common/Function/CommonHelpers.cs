using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;

namespace fontWebCore.Common.Function
{
    public static class CommonHelpers
    {
        public static string GeneratePassword(int length) //length of salt    
        {
            const string allowedChars = "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ0123456789";
            var randNum = new Random();
            var chars = new char[length];
            var allowedCharCount = allowedChars.Length - 1;
            for (var i = 0; i <= length - 1; i++)
            {
                chars[i] = allowedChars[Convert.ToInt32(allowedCharCount * randNum.NextDouble())];
            }
            return new string(chars);
        }
        /// <summary>
        /// 判斷身分證號及統一證號是否正確，並判斷性別及國籍
        ///
        /// 國籍
        /// 本署核發之外來人口統一證號編碼，共計10碼，前2碼使用英文字母，
        ///第1碼為區域碼（同國民身分證註1）
        ///第2碼為性別碼(註 2)、3至10碼為阿拉伯數字，其中第3至9碼為流水號、第10碼為檢查號碼。
        ///註1：英文字母代表直轄市、縣、市別：
        /// 台北市 A、台中市 B、基隆市 C、台南市 D、高雄市 E
        /// 新北市 F、宜蘭縣 G、桃園縣 H、嘉義市 I、新竹縣 J
        /// 苗栗縣 K、原台中縣 L、南投縣 M、彰化縣 N、新竹市 O
        /// 雲林縣 P、嘉義縣 Q、原台南縣 R、原高雄縣 S、屏東縣 T
        /// 花蓮縣 U、台東縣 V、金門 縣 W、澎湖 縣 X、連江縣 Z
        /// 註2：
        /// 臺灣地區無戶籍國民、大陸地區人民、港澳居民：
        /// 男性使用A、女性使用B
        ///外國人：
        /// 男性使用C、女性使用D
        /// </summary>
        /// <param name="str"></param>
        public static bool CheckPersonalID(string str)
        {
            string sex = "";
            string nationality = "";
            if (str == null || string.IsNullOrWhiteSpace(str) || str.Length != 10)
            {
                return false;
            }
            char[] pidCharArray = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
            str = str.ToUpper(); // 轉換大寫
            char[] strArr = str.ToCharArray(); // 字串轉成char陣列
            int verifyNum = 0;

            string pat = @"[A-Z]{1}[1-2]{1}[0-9]{8}";
            // Instantiate the regular expression object.
            Regex rTaiwan = new Regex(pat, RegexOptions.IgnoreCase);
            // Match the regular expression pattern against a text string.
            Match mTaiwan = rTaiwan.Match(str);
            // 檢查身分證字號
            if (mTaiwan.Success)
            {
                // 原身分證英文字應轉換為10~33，這裡直接作個位數*9+10
                int[] pidIDInt = { 1, 10, 19, 28, 37, 46, 55, 64, 39, 73, 82, 2, 11, 20, 48, 29, 38, 47, 56, 65, 74, 83, 21, 3, 12, 30 };
                // 第一碼
                verifyNum = verifyNum + pidIDInt[Array.BinarySearch(pidCharArray, strArr[0])];
                // 第二~九碼
                for (int i = 1, j = 8; i < 9; i++, j--)
                {
                    verifyNum += Convert.ToInt32(strArr[i].ToString(), 10) * j;
                }
                // 檢查碼
                verifyNum = (10 - (verifyNum % 10)) % 10;
                bool ok = verifyNum == Convert.ToInt32(strArr[9].ToString(), 10);
                if (ok)
                {
                    // 判斷性別 & 國籍
                    sex = "男";
                    if (strArr[1] == '2') sex = "女";
                    nationality = "本國籍";
                }
                return ok;
            }

            // 檢查統一證號(居留證)
            verifyNum = 0;
            pat = @"[A-Z]{1}[A-D]{1}[0-9]{8}";
            // Instantiate the regular expression object.
            Regex rForeign = new Regex(pat, RegexOptions.IgnoreCase);
            // Match the regular expression pattern against a text string.
            Match mForeign = rForeign.Match(str);
            if (mForeign.Success)
            {
                // 原居留證第一碼英文字應轉換為10~33，十位數*1，個位數*9，這裡直接作[(十位數*1) mod 10] + [(個位數*9) mod 10]
                int[] pidResidentFirstInt = { 1, 10, 9, 8, 7, 6, 5, 4, 9, 3, 2, 2, 11, 10, 8, 9, 8, 7, 6, 5, 4, 3, 11, 3, 12, 10 };
                // 第一碼
                verifyNum += pidResidentFirstInt[Array.BinarySearch(pidCharArray, strArr[0])];
                // 原居留證第二碼英文字應轉換為10~33，並僅取個位數*6，這裡直接取[(個位數*6) mod 10]
                int[] pidResidentSecondInt = { 0, 8, 6, 4, 2, 0, 8, 6, 2, 4, 2, 0, 8, 6, 0, 4, 2, 0, 8, 6, 4, 2, 6, 0, 8, 4 };
                // 第二碼
                verifyNum += pidResidentSecondInt[Array.BinarySearch(pidCharArray, strArr[1])];
                // 第三~八碼
                for (int i = 2, j = 7; i < 9; i++, j--)
                {
                    verifyNum += Convert.ToInt32(strArr[i].ToString(), 10) * j;
                }
                // 檢查碼
                verifyNum = (10 - (verifyNum % 10)) % 10;
                bool ok = verifyNum == Convert.ToInt32(strArr[9].ToString(), 10);
                if (ok)
                {
                    // 判斷性別 & 國籍
                    sex = "男";
                    if (strArr[1] == 'B' || strArr[1] == 'D') sex = "女";
                    nationality = "外籍人士";
                    if (strArr[1] == 'A' || strArr[1] == 'B') nationality += "(臺灣地區無戶籍國民、大陸地區人民、港澳居民)";
                }
                return ok;
            }
            return false;
        }

        public static string CallMessage(string url, string postData)
        {
            string result = "";
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri(url));
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(ValidateServerCertificate);
                byte[] bs = System.Text.Encoding.UTF8.GetBytes(postData);
                request.ContentLength = bs.Length;
                request.GetRequestStream().Write(bs, 0, bs.Length);
                //取得 WebResponse 的物件 然後把回傳的資料讀出
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                StreamReader sr = new StreamReader(response.GetResponseStream());
                result = sr.ReadToEnd();
            }
            catch (Exception ex)
            {
                return "false";
            }
            return result;
        }
        public static bool ValidateServerCertificate(Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        public static TTarget Migration<TSource, TTarget>(TSource sourceInstance) where TSource : class, new()
        {
            Type type = sourceInstance.GetType();
            Type typeFromHandle = typeof(TTarget);
            PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            PropertyInfo[] properties2 = typeFromHandle.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            TTarget tTarget = Activator.CreateInstance<TTarget>();
            Type typeFromHandle2 = typeof(ObjectMappingAttribute);
            PropertyInfo[] array = properties;
            for (int i = 0; i < array.Length; i++)
            {
                PropertyInfo propertyInfo = array[i];
                string name = propertyInfo.Name;
                object value = propertyInfo.GetValue(sourceInstance);
                PropertyInfo[] array2 = properties2;
                for (int j = 0; j < array2.Length; j++)
                {
                    PropertyInfo propertyInfo2 = array2[j];
                    string name2 = propertyInfo2.Name;
                    bool flag = name == name2;
                    if (flag)
                    {
                        bool flag2 = propertyInfo.PropertyType == propertyInfo2.PropertyType;
                        if (flag2)
                        {
                            propertyInfo2.SetValue(tTarget, value);
                            break;
                        }
                    }
                    object[] customAttributes = propertyInfo2.GetCustomAttributes(typeFromHandle2, false);
                    bool flag3 = customAttributes.Any<object>();
                    if (flag3)
                    {
                        string propertyName = ((ObjectMappingAttribute)customAttributes[0]).PropertyName;
                        bool flag4 = propertyName == name;
                        if (flag4)
                        {
                            bool flag5 = propertyInfo.PropertyType == propertyInfo2.PropertyType;
                            if (flag5)
                            {
                                propertyInfo2.SetValue(tTarget, value);
                                break;
                            }
                        }
                    }
                }
            }
            return tTarget;
        }

    }
    [AttributeUsage(AttributeTargets.Property)]
    public class ObjectMappingAttribute : Attribute
    {
        public string PropertyName
        {
            get;
            set;
        }

        public ObjectMappingAttribute(string propertyName)
        {
            this.PropertyName = propertyName;
        }
    }
}
