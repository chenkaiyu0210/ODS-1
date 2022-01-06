using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace backendWeb.Models.ViewModel
{
    public class baseModel
    {
        #region Table Search
        /// <summary>
        /// 
        /// </summary>
        public int? draw { get; set; }
        /// <summary>
        /// Sql從第幾筆撈取
        /// </summary>
        public int? start { get; set; }
        /// <summary>
        /// Sql要撈取的筆數
        /// </summary>
        public int? length { get; set; }
        /// <summary>
        /// Sql列表取的總筆數
        /// </summary>
        public int? numCount { get; set; }
        #endregion
        /// <summary>
        /// 存檔動作(Create/Motify)
        /// </summary>
        public string saveAction { get; set; }
        /// <summary>
        /// 回傳結果(True/False)
        /// </summary>
        public bool? replyResult { get; set; }
        /// <summary>
        /// 回傳訊息(True/False)
        /// </summary>
        public string replyMsg { get; set; }
    }
}