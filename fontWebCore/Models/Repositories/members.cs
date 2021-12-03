using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fontWebCore.Models.Repositories
{
    public class members
    {
        public System.Guid customer_id { get; set; }
        public string customer_name { get; set; }
        public string customer_idcard_no { get; set; }
        public string customer_email { get; set; }
        public Nullable<System.DateTime> customer_birthday { get; set; }
        public string customer_mobile_phone { get; set; }
        public string password { get; set; }
        public string salt_key { get; set; }
        public Nullable<System.DateTime> register_time { get; set; }
        public string sms_verify_code { get; set; }
        public Nullable<bool> sms_is_verify { get; set; }
        public bool is_enable { get; set; }
    }
}
