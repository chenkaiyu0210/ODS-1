using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GateWay.Models
{//審件補件
    public class ReqRS
    {
        public string dealerNo { get; set; }
        public string branchNo { get; set; }
        public string salesNo { get; set; }
        public string examineNo { get; set; }
        public string source { get; set; }
        public List<ReqRSItem> supplement { get; set; }
        public List<ReqRSFile> attachmentFile { get; set; }
    }
    public class ReqRSItem
    {
        public string item { get; set; }
        public string comment { get; set; }
    }
    public class ReqRSFile
    {
        //
        // Summary:
        //     檔案編碼索引
        public string file_index { get; set; }
        //
        // Summary:
        //     檔案主體
        public string file_body_encode { get; set; }
        //
        // Summary:
        //     檔案大小
        public string file_size { get; set; }
        //
        // Summary:
        //     檔案格式
        public string content_type { get; set; }
    }
    public class RespRS
    {
        /// <summary>
        /// S001:成功 F001:執行失敗 E003:查無該筆審件資料
        /// </summary>
        public string code { get; set; }
        public string msg { get; set; }
    }
}