using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GateWay.Models
{
    public class ReqRCO
    {//
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
        //     重照時間
        public string calloutDate { get; set; }
        //
        // Summary:
        //     連絡電話
        public string tel { get; set; }
        //
        // Summary:
        //     備註描述
        public string descript { get; set; }
        //
        // Summary:
        //     現可照(Y/N)
        public string nowCallout { get; set; }
    }
}