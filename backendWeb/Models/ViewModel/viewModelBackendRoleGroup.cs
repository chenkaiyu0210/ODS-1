using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace backendWeb.Models.ViewModel
{
    public class viewModelBackendRoleGroup : baseModel
    {
        /// <summary>
        /// 角色群組代碼
        /// </summary>
        public string role_group_code { get; set; }
        /// <summary>
        /// 角色群組名稱
        /// </summary>
        public string role_group_name { get; set; }
        /// <summary>
        /// 是否啟用
        /// </summary>
        public Nullable<bool> is_enable { get; set; }
        /// <summary>
        /// 選單權限碼(多筆[,]串接)
        /// </summary>
        public string authorize_codes { get; set; }
        /// <summary>
        /// 建立時間
        /// </summary>
        public System.DateTime create_time { get; set; }
        #region Search
        /// <summary>
        /// 角色群組代碼(查詢IN用)
        /// </summary>
        public string searchIn_role_group_code { get; set; }
        /// <summary>
        /// 角色群組代碼(查詢用)
        /// </summary>
        public string search_role_group_code { get; set; }
        /// <summary>
        /// 角色群組名稱(查詢用)
        /// </summary>
        public string search_role_group_name { get; set; }        
        #endregion
        #region Other
        /// <summary>
        /// 選單權限碼(撈全部)
        /// </summary>
        public IList<viewModelBackendMenu> listAuthorize { get; set; }
        /// <summary>
        /// 角色群組(View給值用)
        /// </summary>
        public string[] listAuthorizeCodes { get; set; }
        /// <summary>
        /// 權限碼(Post接收值用)
        /// </summary>
        public string[] dlAuthorizeCodes { get; set; }
        #endregion
    }
}