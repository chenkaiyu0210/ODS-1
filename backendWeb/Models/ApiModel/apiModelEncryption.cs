using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace backendWeb.Models.ApiModel
{
    public class apiModelEncryption
    {
        public string dealerNo { get; set; } = "OO02";
        public string source { get; set; } = "80659759";
        public string encryptEnterCase { get; set; }
        public string version { get; set; }
        public string transactionId { get; set; }
    }
}