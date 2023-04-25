using System;
using System.Collections.Generic;

namespace backendWeb.Models.ViewModel
{
    public class viewModelBackendUser : baseModel
    {
        /// <summary>
        /// 帳號
        /// </summary>
        public string account { get; set; }
        /// <summary>
        /// 名稱
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 密碼
        /// </summary>
        public string password { get; set; }
        /// <summary>
        /// salt_key
        /// </summary>
        public string salt_key { get; set; }
        /// <summary>
        /// 註冊人員
        /// </summary>
        public string register_user { get; set; }
        /// <summary>
        /// 註冊時間
        /// </summary>
        public Nullable<System.DateTime> register_time { get; set; }
        /// <summary>
        /// 是否啟用
        /// </summary>
        public bool is_enable { get; set; }
        /// <summary>
        /// 停用時間
        /// </summary>
        public Nullable<System.DateTime> disable_time { get; set; }
        /// <summary>
        /// 停用人員
        /// </summary>
        public string disable_user { get; set; }
        /// <summary>
        /// 角色群組碼(多筆[,]串接)
        /// </summary>
        public string role_group_codes { get; set; }

        #region Search
        /// <summary>
        /// 帳號(查詢用)
        /// </summary>
        public string search_account { get; set; }
        /// <summary>
        /// 名稱(查詢用)
        /// </summary>
        public string search_name { get; set; }
        /// <summary>
        /// 是否啟用(查詢用)
        /// </summary>
        public bool? search_enable { get; set; }
        #endregion
        #region Other
        /// <summary>
        /// 角色群組(撈全部)
        /// </summary>
        public IList<viewModelBackendRoleGroup> listRoleGroup { get; set; }
        /// <summary>
        /// 角色群組(View給值用)
        /// </summary>
        public string[] listRoleGroupCodes { get; set; }
        /// <summary>
        /// 角色群組(Post接收值用)
        /// </summary>
        public string[] dlRoleCodes { get; set; }
        /// <summary>
        /// 密碼(輸入用)
        /// </summary>
        public string inputPassword { get; set; }
        #endregion
    }
}