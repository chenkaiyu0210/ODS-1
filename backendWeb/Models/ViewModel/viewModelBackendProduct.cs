using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace backendWeb.Models.ViewModel
{
    public class viewModelBackendProduct : baseModel
    {
        #region Sreach
        public string search_bus_type { get; set; }
        public string search_product_brand { get; set; }
        #endregion
        public string bus_type { get; set; }
        public string product_brand { get; set; }
        public string product_brand_name { get; set; }
        public string product_kind { get; set; }
        public string product_kind_name { get; set; }
        public Nullable<bool> is_enable { get; set; }
        public Nullable<System.DateTime> create_time { get; set; }
        public Nullable<System.DateTime> modify_time { get; set; }
    }
}