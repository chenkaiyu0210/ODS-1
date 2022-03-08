using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace backendWeb.Models.ViewModel
{
    public class viewModelPostfile
    {
        #region Search
        public string search_city_name { get; set; }
        public string search_town_name { get; set; }
        #endregion
        public string zipcode { get; set; }
        public string city_name { get; set; }
        public string town_name { get; set; }
        public System.DateTime import_time { get; set; }

    }
}