using SimpleCaptcha.Generator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fontWebCore.Common.Function
{
    public class MyCaptchaCodeGenerator : ICaptchaCodeGenerator
    {
        public string Generate(int length)
        {
            //throw new NotImplementedException();
            string str = string.Empty; //定義文字
            while (str.Length < length)//如果長度小於length定義就執行壘加
            {
                Random r = new Random();
                int i = r.Next(0, 10);
                //if (!str.Contains(i.ToString()))
                str += i.ToString();
            }

            return str;
        }
    }
}
