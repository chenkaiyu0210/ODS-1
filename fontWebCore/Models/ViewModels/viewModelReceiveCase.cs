using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fontWebCore.Models.ViewModels
{
    public class viewModelReceiveCase
    {
        public System.Guid recevie_id { get; set; }
        public string bus_type { get; set; }
        public string promotion { get; set; }
        public Nullable<System.DateTime> recevie_date { get; set; }
        public string recevie_status { get; set; }
        public string customer_name { get; set; }
        public string customer_idcard_no { get; set; }
        public string customer_mobile_phone { get; set; }
        public System.DateTime customer_birthday { get; set; }
        public string customer_id_number_status { get; set; }
        public Nullable<System.DateTime> customer_id_issue_date { get; set; }
        public string customer_id_number_areacode { get; set; }
        public string customer_edutation_status { get; set; }
        public string customer_resident_tel_code { get; set; }
        public string customer_resident_tel_num { get; set; }
        public string customer_mail_tel_code { get; set; }
        public string customer_mail_tel_num { get; set; }
        public string customer_resident_postalcode { get; set; }
        public string customer_resident_addcity { get; set; }
        public string customer_resident_addregion { get; set; }
        public string customer_resident_address { get; set; }
        public string customer_mail_postalcode { get; set; }
        public string customer_mail_addcity { get; set; }
        public string customer_mail_addregion { get; set; }
        public string customer_mail_address { get; set; }
        public Nullable<byte> customer_dwell_year { get; set; }
        public Nullable<byte> customer_dwell_month { get; set; }
        public string customer_dwell_status { get; set; }
        public string customer_company_name { get; set; }
        public string customer_job_type { get; set; }
        public string customer_work_year { get; set; }
        public string customer_work_month { get; set; }
        public Nullable<int> customer_month_salary { get; set; }
        public string customer_company_tel_code { get; set; }
        public string customer_company_tel_num { get; set; }
        public string customer_company_tel_ext { get; set; }
        public string customer_company_postalcode { get; set; }
        public string customer_company_addcity { get; set; }
        public string customer_company_addregion { get; set; }
        public string customer_company_address { get; set; }
        public string customer_creditcard_status { get; set; }
        public string customer_creditcard_status_remark { get; set; }
        public string customer_creditcard_bank { get; set; }
        public string customer_creditcard_validdate_month { get; set; }
        public string customer_creditcard_validdate_year { get; set; }
        public string customer_email { get; set; }
        public string card_num_01 { get; set; }
        public string card_num_02 { get; set; }
        public string card_num_03 { get; set; }
        public string card_num_04 { get; set; }
        public string bank_code { get; set; }
        public string bank_detail_code { get; set; }
        public string account_num { get; set; }
        public Nullable<byte> payment_mode { get; set; }
        public string contact_person_name_i { get; set; }
        public string contact_person_relation_i { get; set; }
        public string contact_person_mobile_phone_i { get; set; }
        public string contact_person_areacode_i { get; set; }
        public string contact_person_tel_i { get; set; }
        public string contact_person_company_areacode_i { get; set; }
        public string contact_person_company_tel_i { get; set; }
        public string contact_person_company_tel_ext_i { get; set; }
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
        public string note_time { get; set; }
        public string dealer_branch_name { get; set; }
        public string product_name { get; set; }
        public string product_brand { get; set; }
        public string product_kind { get; set; }
        public Nullable<int> staging_amount { get; set; }
        public string num { get; set; }
        public string num_amount { get; set; }
        public string note_remark { get; set; }
        public Nullable<bool> is_secret { get; set; }
        public string commission_target { get; set; }

        #region Other        
        public string customer_birthdayYY { get; set; }
        public string customer_birthdayMM { get; set; }
        public string customer_birthdayDD { get; set; }
        public bool? customer_mail_identical { get; set; }
        public string guarantor_option { get; set; }
        public string guarantor_birthdayYY { get; set; }
        public string guarantor_birthdayMM { get; set; }
        public string guarantor_birthdayDD { get; set; }
        public List<payment> paymentInput { get; set; } = new List<payment>();
        public List<viewModelPostfile> city_list { get; set; }
        public List<viewModelPostfile> town_list { get; set; }
        #endregion
    }
    public class payment
    {
        public string num { get; set; }
        public string num_amount { get; set; }
    }
}
