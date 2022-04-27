using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace backendWeb.Models.ApiModel.responseModel
{
    public class apiResponseReceive
    {
        /// <summary>
        /// 回覆代碼
        /// </summary>
        public string code { get; set; }
        /// <summary>
        /// 回覆訊息
        /// </summary>
        public string msg { get; set; }
        /// <summary>
        /// 回覆審件編號
        /// </summary>
        public string examineNo { get; set; }
        /// <summary>
        /// 回覆審件狀態(預留)
        /// </summary>
        public string examStatus { get; set; }
        /// <summary>
        /// 檔案補件路徑(預留)
        /// </summary>
        public string fileUrl { get; set; }
    }
}