using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace backendWeb.Models.ApiModel
{
    public class apiModelReceive
    {
        public string customer_name { get; set; }
        public string customer_idcard_no { get; set; }
        public string customer_id_number_status { get; set; }
        public Nullable<System.DateTime> customer_id_issue_date { get; set; }
        public string customer_id_number_areacode { get; set; }
        public Nullable<System.DateTime> customer_birthday { get; set; }
        public string customer_resident_tel_code { get; set; }
        public string customer_resident_tel_num { get; set; }
        public string customer_resident_tel_ext { get; set; }
        public string customer_mail_tel_code { get; set; }
        public string customer_mail_tel_num { get; set; }
        public string customer_mail_tel_ext { get; set; }
        public string customer_mobile_phone { get; set; }
        public string customer_edutation_status { get; set; }
        public string customer_resident_postalcode { get; set; }
        public string customer_resident_addcity { get; set; }
        public string customer_resident_addregion { get; set; }
        public string customer_resident_address { get; set; }
        public string customer_mail_identical { get; set; }
        public string customer_mail_postalcode { get; set; }
        public string customer_mail_addcity { get; set; }
        public string customer_mail_addregion { get; set; }
        public string customer_mail_address { get; set; }
        public Nullable<byte> customer_dwell_year { get; set; }
        public Nullable<byte> customer_dwell_month { get; set; }
        public string customer_dwell_status { get; set; }
        public string customer_check_identical { get; set; }
        public string customer_check_postalcode { get; set; }
        public string customer_check_addcity { get; set; }
        public string customer_check_addregion { get; set; }
        public string customer_check_address { get; set; }
        public string customer_email { get; set; }
        public string customer_profession_status { get; set; }
        public string customer_company_name { get; set; }
        public string customer_company_tel_code { get; set; }
        public string customer_company_tel_num { get; set; }
        public string customer_company_tel_ext { get; set; }
        public string customer_company_postalcode { get; set; }
        public string customer_company_addcity { get; set; }
        public string customer_company_addregion { get; set; }
        public string customer_company_address { get; set; }
        public string customer_job_type { get; set; }
        public Nullable<byte> customer_work_year { get; set; }
        public Nullable<byte> customer_work_month { get; set; }
        public Nullable<int> customer_month_salary { get; set; }
        public string customer_creditcard_status { get; set; }
        public string customer_creditcard_status_remark { get; set; }
        public string customer_creditcard_bank { get; set; }
        public Nullable<byte> customer_creditcard_validdate_year { get; set; }
        public Nullable<byte> customer_creditcard_validdate_month { get; set; }
        public string payee_account_name { get; set; }//
        public string payee_account_idno { get; set; }//
        public string payee_bank_code { get; set; }//
        public string payee_bank_detail_code { get; set; }//
        public string payee_account_num { get; set; }//
        public string payment_mode { get; set; }//
        public string guarantor_option { get; set; }
        public string guarantor_name { get; set; }
        public string guarantor_relation { get; set; }
        public string guarantor_idcard_no { get; set; }
        public Nullable<System.DateTime> guarantor_birthday { get; set; }
        public string guarantor_resident_tel_code { get; set; }
        public string guarantor_resident_tel_num { get; set; }
        public string guarantor_resident_tel_ext { get; set; }
        public string guarantor_mobile_phone { get; set; }
        public string guarantor_company_name { get; set; }
        public string guarantor_job_type { get; set; }
        public string guarantor_company_tel_code { get; set; }
        public string guarantor_company_tel_num { get; set; }
        public string guarantor_company_tel_ext { get; set; }
        public string guarantor_postalcode { get; set; }//
        public string guarantor_addcity { get; set; }//
        public string guarantor_addregion { get; set; }//
        public string guarantor_address { get; set; }//
        public string contact_person_name_i { get; set; }
        public string contact_person_relation_i { get; set; }
        public string contact_person_mobile_phone_i { get; set; }
        public string contact_person_areacode_i { get; set; }
        public string contact_person_tel_i { get; set; }
        public string contact_person_tel_ext_i { get; set; }
        public string contact_person_company_areacode_i { get; set; }
        public string contact_person_company_tel_i { get; set; }
        public string contact_person_company_tel_ext_i { get; set; }
        public string contact_person_name_ii { get; set; }
        public string contact_person_relation_ii { get; set; }
        public string contact_person_mobile_phone_ii { get; set; }
        public string contact_person_areacode_ii { get; set; }
        public string contact_person_tel_ii { get; set; }
        public string contact_person_tel_ext_ii { get; set; }
        public string contact_person_company_areacode_ii { get; set; }
        public string contact_person_company_tel_ii { get; set; }
        public string contact_person_company_tel_ext_ii { get; set; }
        public string product_rate_option { get; set; }
        public string product_case_option { get; set; }
        public string product_category_id { get; set; }
        public string product_id { get; set; }
        public Nullable<int> deposit { get; set; }
        public Nullable<int> staging_amount { get; set; }
        public string promotion_no { get; set; }
        public string periods_num { get; set; }
        public string payment { get; set; }
        public Nullable<int> staging_total_price { get; set; }
        public string dealer_no { get; set; }//
        public string dealer_id_no { get; set; }
        public string dealer_name { get; set; }
        public string dealer_tel { get; set; }
        public string dealer_fax { get; set; }
        public string contact_id_no { get; set; }
        public string contact_name { get; set; }
        public string dealer_branch_no { get; set; }
        public string dealer_branch_name_identical { get; set; }
        public string dealer_branch_name { get; set; }
        public string dealer_branch_tel { get; set; }
        public string contact_phone { get; set; }
        public string dealer_note_code { get; set; }
        public Nullable<System.DateTime> dealer_note_date { get; set; }
        public string dealer_note { get; set; }
        public string bus_type { get; set; }
        public string bus_type_name { get; set; }
        public string company_principal_id { get; set; }
        public string company_principal_name { get; set; }
        public string customer_quality { get; set; }//
        public string commission_target { get; set; }//
        public string is_movable { get; set; }//
        public string transaction_id { get; set; }//
        public string recevie_source { get; set; }//
        public List<attachmentfileYrc> Attachmentfile { get; set; } = new List<attachmentfileYrc>();
    }
    public class attachmentfileYrc
    {
        public string file_index { get; set; }
        public string file_body_encode { get; set; }
        public string file_size { get; set; }
        public string content_type { get; set; }
    }
}