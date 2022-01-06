using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace backendWeb.Models.ViewModel
{
    public class viewModelBackendMenu
    {
        /// <summary>
        /// 應用系統代碼
        /// </summary>
        public string app_id { get; set; }
        /// <summary>
        /// 功能代碼
        /// </summary>
        public string func_id { get; set; }
        /// <summary>
        /// controller代碼
        /// </summary>
        public string controller_id { get; set; }
        /// <summary>
        /// view代碼
        /// </summary>
        public string view_id { get; set; }
        /// <summary>
        /// 階層
        /// </summary>
        public Nullable<int> func_layer { get; set; }
        /// <summary>
        /// 功能名稱
        /// </summary>
        public string func_name { get; set; }
        /// <summary>
        /// 功能網址
        /// </summary>
        public string func_url { get; set; }
        /// <summary>
        /// 是否顯示(Y/N)
        /// </summary>
        public Nullable<bool> is_display { get; set; }
        /// <summary>
        /// 排序順序
        /// </summary>
        public string sort_order { get; set; }
        /// <summary>
        /// 上層功能代碼
        /// </summary>
        public string parent_func_id { get; set; }
        /// <summary>
        /// icon圖示
        /// </summary>
        public string icon { get; set; }
        /// <summary>
        /// 權限碼
        /// </summary>
        public string authorize_code { get; set; }

        #region Search
        /// <summary>
        /// 權限碼(查詢IN用)
        /// </summary>
        public string searchIn_authorize_code { get; set; }
        /// <summary>
        /// 角色碼(查詢IN用)
        /// </summary>
        public string searchIn_role_group_code { get; set; }
        /// <summary>
        /// 是否有權限碼(查詢用)
        /// </summary>
        public bool? search_hasAuthorize { get; set; }
        #endregion
    }
}