using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GateWay.Models
{//申覆
    public class ReqRE
    {
        //
        // Summary:
        //     通路商編號
        public string dealerNo { get; set; }
        //
        // Summary:
        //     審件編號
        public string examineNo { get; set; }
        //
        // Summary:
        //     進件來源
        public string source { get; set; }
        //
        // Summary:
        //     據點編號
        public string branchNo { get; set; }
        //
        // Summary:
        //     業務人員ID
        public string salesNo { get; set; }
        //
        // Summary:
        //     申覆內容
        public string comment { get; set; }
        //
        // Summary:
        //     強制爭取
        public string forceTryForExam { get; set; }
        
        public List<ReqREFile> attachmentFile { get; set; }
    }
    public class ReqREFile
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
    public class RespRE
    {
        /// <summary>
        /// S001:成功 F001:執行失敗 E003:查無該筆審件資料
        /// </summary>
        //
        // Summary:
        //     回覆代碼
        public string code { get; set; }
        //
        // Summary:
        //     回覆訊息
        public string msg { get; set; }
        //
        // Summary:
        //     申覆次數
        public string negotiateTimes { get; set; }
    }
}