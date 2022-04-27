using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace backendWeb.Models.ViewModel
{
    public class viewModelBackendBank : baseModel
    {
        #region Search
        public string search_bank_code { get; set; }
        
        #endregion
        public string bank_code { get; set; }
        public string bank_name { get; set; }
        public bool is_enable { get; set; }
        public Nullable<System.DateTime> create_time { get; set; }
        public Nullable<System.DateTime> modify_time { get; set; }
    }
}