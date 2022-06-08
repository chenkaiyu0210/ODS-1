using System.Collections.Generic;

namespace backendWeb.Models.ApiModel
{
    //
    // 摘要:
    //     撥款狀態查詢(回傳結果)
    public class modelAppropriationQueryRes
    {
        //
        // 摘要:
        //     回覆代碼
        public string code { get; set; }

        //
        // 摘要:
        //     回覆訊息
        public string msg { get; set; }

        //
        // 摘要:
        //     撥款列表
        public List<Appropriation> appropriations { get; set; } = new List<Appropriation>();

    }
    public class Appropriation
    {
        //
        // 摘要:
        //     審件編號
        public string examineNo { get; set; }

        //
        // 摘要:
        //     撥款時間
        public string appropriationDate { get; set; }

        //
        // 摘要:
        //     撥款金額
        public int? appropriationAmt { get; set; }

        //
        // 摘要:
        //     繳款方式
        public string repayKindName { get; set; }

        //
        // 摘要:
        //     通路商編號
        public string dealerNo { get; set; }

        //
        // 摘要:
        //     據點編號
        public string branchNo { get; set; }

        //
        // 摘要:
        //     業務人員ID
        public string salesNo { get; set; }

        //
        // 摘要:
        //     請款狀態
        public string status { get; set; }
    }
}