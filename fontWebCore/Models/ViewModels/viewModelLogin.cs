using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fontWebCore.Models.ViewModels
{
    public class viewModelLogin
    {
        public string captcha_id { get; set; }
        public string captcha_code { get; set; }
        public string idcard_no { get; set; }
        public string password { get; set; }
        public string password_check { get; set; }
        public string sms_verify_code { get; set; }
    }
}
