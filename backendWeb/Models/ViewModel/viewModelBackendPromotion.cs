using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace backendWeb.Models.ViewModel
{
    public class viewModelBackendPromotion : baseModel
    {
        #region Search
        public string search_bus_type { get; set; }
        public string search_promo_no { get; set; }
        #endregion
        public string bus_type { get; set; }
        public string promo_no { get; set; }
        public string promo_name { get; set; }
        public bool is_enable { get; set; }
        public Nullable<System.DateTime> create_time { get; set; }
        public Nullable<System.DateTime> modify_time { get; set; }
    }
}