using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fontWebCore.Models.ViewModels
{
    public class viewModelMember
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

        #region Other
        /// <summary>
        /// 註冊會員密碼確認
        /// </summary>
        public string password_check { get; set; }
        public string customer_birthdayYY { get; set; }
        public string customer_birthdayMM { get; set; }
        public string customer_birthdayDD { get; set; }
        public string last_mobile_phone { get; set; }
        /// <summary>
        /// 修改密碼用
        /// </summary>
        public string new_password { get; set; }
        /// <summary>
        /// 修改密碼確認用
        /// </summary>
        public string confirm_password { get; set; }
        #endregion
    }
}
