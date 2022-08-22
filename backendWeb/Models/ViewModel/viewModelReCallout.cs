using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace backendWeb.Models.ViewModel
{
    public class viewModelReCallout
    {
        public string examine_no { get; set; }
        public string receive_id { get; set; }
        //
        // Summary:
        //     重照時間
        public DateTime calloutDate { get; set; }
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
        public bool nowCallout { get; set; }
    }
}