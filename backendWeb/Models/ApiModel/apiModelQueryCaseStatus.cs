using System.Collections.Generic;

namespace backendWeb.Models.ApiModel
{
    public class ReqQCS
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
        //     來源
        public string source { get; set; }
        //
        // Summary:
        //     據點編號
        public string branchNo { get; set; }
        //
        // Summary:
        //     業務人員ID
        public string salesNo { get; set; }
    }
    public class RespQCS
    {
        //
        // Summary:
        //     回傳結果
        public string code { get; set; }
        //
        // Summary:
        //     回傳訊息
        public string msg { get; set; }
        //
        // Summary:
        //     審件狀態(中文)
        public string examStatusExplain { get; set; }
        //
        // Summary:
        //     專案代碼
        public string promoNo { get; set; }
        //
        // Summary:
        //     專案名稱
        public string promoName { get; set; }
        public List<contentCustomer> customer { get; set; }
        //
        // Summary:
        //     申貸本金
        public string instCap { get; set; }
        //
        // Summary:
        //     期付資訊
        public List<paymentInfo> payment { get; set; }
        //
        // Summary:
        //     備註說明
        public string examineComment { get; set; }
        //
        // Summary:
        //     原因/建議資訊
        public List<reasonSuggestionDetailInfo> reasonSuggestionDetail { get; set; }
        //
        // Summary:
        //     核准日期(yyyyMMdd)
        public string approveDate { get; set; }
        //
        // Summary:
        //    撥款資訊
        public contentPayee payee { get; set; }
        //
        // Summary:
        //    撥款資訊
        public List<capitalApplyInfo> capitalApply { get; set; }
        /// <summary>
        /// 舊約編號
        /// </summary>
        public string oldLoanNo { get; set; }
        /// <summary>
        /// 佣金資訊
        /// </summary>
        public string brokeragePersonal { get; set; }
        /// <summary>
        /// 扣除借新還舊預計撥付金額(預計金額)
        /// </summary>
        public int? netBorrowToPayBackAmt { get; set; }
        /// <summary>
        /// 借新還舊預計結清金額
        /// </summary>
        public int? borrowToPayBackAmt { get; set; }
        /// <summary>
        /// 產品號碼/車牌號碼
        /// </summary>
        public string carNo { get; set; }
        /// <summary>
        /// 產品品牌
        /// </summary>
        public string carBrand { get; set; }
        /// <summary>
        /// 產品名稱
        /// </summary>
        public string carName { get; set; }

    }

    //
    // 摘要:
    //     明細資訊
    public class reasonSuggestionDetailInfo
    {
        //
        // 摘要:
        //     原因/建議
        public string kind { get; set; }

        //
        // 摘要:
        //     原因/建議 項目說明
        public string explain { get; set; }

        //
        // 摘要:
        //     原因/建議 備註說明
        public string comment { get; set; }
    }
    //
    // 摘要:
    //     客戶資訊(案件明細用)
    public class contentCustomer : customerInfo
    {
        //
        // 摘要:
        //     客戶行動電話
        public List<mobilePhoneInfo> mobilePhone { get; set; }

        //
        // 摘要:
        //     備註
        public new calloutResult calloutResult { get; set; }
    }

    //
    // 摘要:
    //     照會資訊
    public class calloutResult
    {
        //
        // 摘要:
        //     照會備註
        public string comment { get; set; }
    }

    //
    // 摘要:
    //     電話號碼資訊
    public class mobilePhoneInfo
    {
        //
        // 摘要:
        //     客戶行動電話
        public string number { get; set; }
    }

    //
    // 摘要:
    //     期付資訊
    public class paymentInfo
    {
        //
        // 摘要:
        //     序號
        public int? seqNo { get; set; }

        //
        // 摘要:
        //     期數
        public int? instNo { get; set; }

        //
        // 摘要:
        //     期付金額
        public int? instAmt { get; set; }
    }

    //
    // 摘要:
    //     客戶資訊(列表用)
    public class customerInfo
    {
        //
        // 摘要:
        //     客戶
        public string idno { get; set; }

        //
        // 摘要:
        //     通路商代碼
        public string name { get; set; }

        //
        // 摘要:
        //     通路商代碼
        public string birthday { get; set; }

        //
        // 摘要:
        //     客戶序列(0為申請人，依序為保人或負責人)
        public int index { get; set; }

        //
        // 摘要:
        //     照會資訊
        public calloutResultInfo calloutResult { get; set; }
    }

    //
    // 摘要:
    //     照會資訊(物件)
    public class calloutResultInfo
    {
        //
        // 摘要:
        //     照會備註
        public string comment { get; set; }

        //
        // 摘要:
        //     客戶行動電話
        public mobilePhoneInfo mobilePhone { get; set; }
    }

    //
    // 摘要:
    //     撥款資訊
    public class capitalApplyInfo
    {
        //
        // 摘要:
        //     撥款金額
        public int? remitAmount { get; set; }

        //
        // 摘要:
        //     撥款日期
        public string appropriateDate { get; set; }

        //
        // 摘要:
        //     撥款對象說明
        public string payeeTypeName { get; set; }
    }

    public class contentPayee
    {
        //
        // 摘要:
        //     撥款對象
        public string payeeType { get; set; }
        //
        // 摘要:
        //     撥款銀行總行代碼
        public string bankCode { get; set; }
        //
        // 摘要:
        //     撥款銀行分行代碼
        public string bankDetailCode { get; set; }
        //
        // 摘要:
        //     撥款帳戶帳號
        public string accountNo { get; set; }
    }
}