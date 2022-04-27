using fontWebCore.Common.Context;
using fontWebCore.Common.Function;
using fontWebCore.Models.Repositories;
using fontWebCore.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fontWebCore.Controllers
{
    public class ReceiveController : BaseController
    {
        private readonly ILogger<ReceiveController> _logger;
        private readonly ODSContext _context;
        public ReceiveController(IServiceProvider provider)
        {
            _logger = (ILogger<ReceiveController>)provider.GetService(typeof(ILogger<ReceiveController>));
            _context = (ODSContext)provider.GetService(typeof(ODSContext));
        }
        public IActionResult Apply()
        {
            #region 進件限制
            //viewModelReceiveCase chk = _context.receiveCases.FromSqlRaw("select receive_date from receiveCases WHERE customer_idcard_no = @customer_idcard_no", parameters).OrderByDescending(o => o.receive_date).Select(
            //        o => new viewModelReceiveCase
            //        {
            //            receive_date = o.receive_date
            //        }).FirstOrDefault();
            //if (chk != null)
            //{
            //    int reciprocal = 30 - (this.TaiwanDateTime - chk.receive_date.Value).Days;
            //    if (reciprocal > 0)
            //    {
            //        ViewData["errMsg"] = $"提醒您：尚有{reciprocal}天可再次提出申辦";
            //        return RedirectToAction("MemberCenter");
            //    }
            //}
            #endregion


            viewModelReceiveCase item = new viewModelReceiveCase();
            viewModelMember info = this.MemberInfo;
            if (info != null)
            {
                item.customer_idcard_no = info.customer_idcard_no;
                item.customer_mobile_phone = info.customer_mobile_phone;
                item.customer_name = info.customer_name;
                if (info.customer_birthday.HasValue)
                {
                    item.customer_birthdayYY = (info.customer_birthday.Value.Year - 1911).ToString();
                    item.customer_birthdayMM = info.customer_birthday.Value.Month.ToString();
                    item.customer_birthdayDD = info.customer_birthday.Value.Day.ToString();
                }
            }
            reBindModel(ref item);
            return View(item);
        }
        [HttpPost]
        public IActionResult Apply(viewModelReceiveCase model)
        {
            try
            {
                //檢查資料
                StringBuilder strChk = chkData(model);
                if (!string.IsNullOrWhiteSpace(strChk.ToString()))
                {
                    ViewData["errMsg"] = strChk.ToString();
                    reBindModel(ref model);
                    return View(model);
                }
                #region 合併資料
                int.TryParse(model.customer_birthdayYY, out int yy);
                int.TryParse(model.customer_birthdayMM, out int mm);
                int.TryParse(model.customer_birthdayDD, out int dd);
                if (!DateTime.TryParse((yy + 1911).ToString() + "/" + mm.ToString("00") + "/" + dd.ToString("00"), out DateTime BD))
                {
                    ViewData["errMsg"] = "出生日期格式錯誤!";
                    return View(model);
                }
                model.customer_birthday = BD;
                if (!string.IsNullOrWhiteSpace(model.guarantor_birthdayYY)
                    && !string.IsNullOrWhiteSpace(model.guarantor_birthdayMM)
                    && !string.IsNullOrWhiteSpace(model.guarantor_birthdayDD))
                {
                    int.TryParse(model.guarantor_birthdayYY, out int gy);
                    int.TryParse(model.guarantor_birthdayMM, out int gm);
                    int.TryParse(model.guarantor_birthdayDD, out int gd);
                    if (!DateTime.TryParse((gy + 1911).ToString() + "/" + gm.ToString("00") + "/" + gd.ToString("00"), out DateTime GD))
                    {
                        ViewData["errMsg"] = "保人出生日期格式錯誤!";
                        return View(model);
                    }
                    model.guarantor_birthday = GD;
                }
                if (model.customer_mail_identical.HasValue)
                {
                    model.customer_mail_postalcode = model.customer_resident_postalcode;
                    model.customer_mail_addcity = model.customer_resident_addcity;
                    model.customer_mail_addregion = model.customer_resident_addregion;
                    model.customer_mail_address = model.customer_resident_address;
                }
                foreach (payment p in model.paymentInput)
                {
                    if (model.paymentInput.IndexOf(p) + 1 == model.paymentInput.Count)
                    {
                        model.num += p.num;
                        model.num_amount += p.num_amount;
                    }
                    else
                    {
                        model.num += p.num + ";";
                        model.num_amount += p.num_amount + ";";
                    }
                }
                #endregion
                model.receive_id = Guid.NewGuid();
                model.receive_status = "前端收件";
                model.receive_date = this.TaiwanDateTime;
                
                members m = _context.members.FromSqlRaw($"select * from members WHERE customer_idcard_no = '{model.customer_idcard_no}' AND ISNULL(customer_name,'') = '' AND ISNULL(customer_birthday,'') = ''").FirstOrDefault();
                if (m != null)
                {
                    if (string.IsNullOrWhiteSpace(m.customer_name) || !m.customer_birthday.HasValue)
                    {
                        m.customer_name = model.customer_name;
                        m.customer_birthday = BD;
                        _context.members.Update(m);
                    }              
                }
                //儲存資料                
                _context.receiveCases.Add(CommonHelpers.Migration<viewModelReceiveCase, receiveCases>(model));
                _context.SaveChanges();
                //更新登入資訊
                //this.MemberInfo = CommonHelpers.Migration<members, viewModelMember>(m);
                return RedirectToAction("Finish");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IActionResult Finish()
        {
            return View();
        }
        public IActionResult MemberCenter()
        {
            try
            {
                object[] parameters = new object[] {
                        new SqlParameter { ParameterName = "customer_idcard_no", Value = this.MemberInfo.customer_idcard_no },
                };
                //viewModelReceiveCase item = _context.receiveCases.FromSqlRaw("select receive_date from receiveCases WHERE ISNULL(receive_status,0) <> '0' AND customer_idcard_no = @customer_idcard_no", parameters).OrderByDescending(o=>o.receive_date).Select(
                //    o => new viewModelReceiveCase
                //    {
                //        receive_date = o.receive_date
                //    }).FirstOrDefault();
                viewModelReceiveCase item = _context.receiveCases.FromSqlRaw("select receive_date from receiveCases WHERE customer_idcard_no = @customer_idcard_no", parameters).OrderByDescending(o => o.receive_date).Select(
                    o => new viewModelReceiveCase
                    {
                        receive_date = o.receive_date
                    }).FirstOrDefault();
                return View(item);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public JsonResult search_post_code(string query_type, string city_name, string town_name)
        {
            viewModelReceiveCase model = new viewModelReceiveCase();
            if (query_type == "town")
            {
                object[] parameters = new object[] {
                        new SqlParameter { ParameterName = "city_name", Value = city_name },
                };
                List<postfile> list = _context.postfile.FromSqlRaw("select * from postfile where city_name = @city_name", parameters).ToList();
                model.town_list = list.Select(o => new viewModelPostfile() { town_name = o.town_name }).ToList();
                return Json(model);
            }
            else
            {
                object[] parameters = new object[] {
                        new SqlParameter { ParameterName = "city_name", Value = city_name },
                        new SqlParameter { ParameterName = "town_name", Value = town_name },
                };
                List<postfile> list = _context.postfile.FromSqlRaw("select * from postfile where city_name = @city_name And town_name = @town_name", parameters).ToList();

                if (list.Count > 0)
                {
                    viewModelPostfile model_ics = CommonHelpers.Migration<postfile, viewModelPostfile>(list.FirstOrDefault());
                    return Json(model_ics);
                }
                else
                {
                    return Json(new viewModelPostfile());
                }
            }
        }
        public void reBindModel(ref viewModelReceiveCase model)
        {
            List<postfile> list = _context.postfile.FromSqlRaw("select * from postfile").ToList();
            model.city_list = (from d in list
                               orderby d.zipcode
                               select new viewModelPostfile
                               {
                                   zipcode = d.zipcode.Substring(0, 1),
                                   city_name = d.city_name,
                               }).GroupBy(o => new
                               {
                                   o.zipcode,
                                   o.city_name
                               }).Select(o => new viewModelPostfile { zipcode = o.Key.zipcode, city_name = o.Key.city_name }).OrderBy(o => o.zipcode).ToList();
        }
        public StringBuilder chkData(viewModelReceiveCase model)
        {
            StringBuilder chk = new StringBuilder();
            if (!CommonHelpers.CheckPersonalID(model.customer_idcard_no)) { chk.Append("身分證格式錯誤!"); return chk; }
            if (string.IsNullOrWhiteSpace(model.customer_name)) { chk.Append("中文姓名欄位未填!"); return chk; }
            if (string.IsNullOrWhiteSpace(model.customer_idcard_no)) { chk.Append("身份證字號欄位未填!"); return chk; }
            if (string.IsNullOrWhiteSpace(model.customer_mobile_phone)) { chk.Append("行動電話欄位未填!"); return chk; }
            if (string.IsNullOrWhiteSpace(model.customer_birthdayYY) ||
                string.IsNullOrWhiteSpace(model.customer_birthdayMM) ||
                string.IsNullOrWhiteSpace(model.customer_birthdayDD)) { chk.Append("出生日期欄位未填!"); return chk; }
            if (string.IsNullOrWhiteSpace(model.customer_resident_postalcode) ||
                string.IsNullOrWhiteSpace(model.customer_resident_address)) { chk.Append("戶籍地址欄位未填!"); return chk; }
            if(!model.customer_mail_identical.HasValue &&
                (string.IsNullOrWhiteSpace(model.customer_mail_postalcode) || 
                string.IsNullOrWhiteSpace(model.customer_mail_addregion))) { chk.Append("通訊地址欄位未填!"); return chk; }
            if (string.IsNullOrWhiteSpace(model.customer_company_name)) { chk.Append("公司名稱欄位未填!"); return chk; }
            if (string.IsNullOrWhiteSpace(model.customer_job_type)) { chk.Append("工作職稱欄位未填!"); return chk; }
            if (!model.customer_month_salary.HasValue) { chk.Append("月薪欄位未填!"); return chk; }
            if (string.IsNullOrWhiteSpace(model.customer_company_tel_code) ||
                string.IsNullOrWhiteSpace(model.customer_company_tel_num)) { chk.Append("公司電話欄位未填!"); return chk; }
            if (string.IsNullOrWhiteSpace(model.contact_person_name_i)) { chk.Append("聯絡人姓名欄位未填!"); return chk; }
            if (string.IsNullOrWhiteSpace(model.contact_person_relation_i)) { chk.Append("聯絡人關係欄位未填!"); return chk; }
            if (string.IsNullOrWhiteSpace(model.contact_person_mobile_phone_i)) { chk.Append("聯絡人行動電話欄位未填!"); return chk; }
            if (!string.IsNullOrWhiteSpace(model.guarantor_idcard_no))
                if (!CommonHelpers.CheckPersonalID(model.guarantor_idcard_no)) { chk.Append("保證人身份證字號格式錯誤!"); return chk; }
            return chk;
        }
    }
}
