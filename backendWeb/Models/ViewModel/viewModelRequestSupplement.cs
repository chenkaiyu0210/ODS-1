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
        public List<viewModelPostfile> city_list { get; set; }
        public IList<string> supplement { get; set; }

        public string guarantor_relation { get; set; }
        public string guarantor_name { get; set; }
        public string guarantor_idcard_no { get; set; }
        public Nullable<System.DateTime> guarantor_birthday { get; set; }
        public string guarantor_mobile_phone { get; set; }
        public string guarantor_postalcode { get; set; }
        public string guarantor_addcity { get; set; }
        public string guarantor_addregion { get; set; }
        public string guarantor_address { get; set; }
        public string guarantor_resident_tel_code { get; set; }
        public string guarantor_resident_tel_num { get; set; }
        public string guarantor_company_name { get; set; }
        public string guarantor_job_type { get; set; }
        public string guarantor_company_tel_code { get; set; }
        public string guarantor_company_tel_num { get; set; }
        public string guarantor_company_tel_ext { get; set; }
        public string guarantor_option { get; set; }
        public string guarantor_birthdayYY { get; set; }
        public string guarantor_birthdayMM { get; set; }
        public string guarantor_birthdayDD { get; set; }
    }
}