using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace backendWeb.Models.ViewModel
{
    public class viewModelRequestSupplement
    {
        public string receive_id { get; set; }
        public string examine_no { get; set; }
        public IList<FileUpload> FileUploads { get; set; }
        public IList<string> supplement { get; set; }
    }
}