using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace backendWeb.Models.ViewModel
{
    public class viewModelBackendSetConfig : baseModel
    {
        #region Search
        public string search_config_tag { get; set; }
        public string search_config_code { get; set; }
        public bool? search_is_enable { get; set; }
        #endregion
        public string config_tag { get; set; }
        public string config_tag_name { get; set; }
        public string parent_code { get; set; }
        public string config_code { get; set; }
        public string config_value { get; set; }
        public int status_sort { get; set; }
        public bool is_enable { get; set; }
        public Nullable<System.DateTime> create_time { get; set; }
        public Nullable<System.DateTime> modify_time { get; set; }
    }
}