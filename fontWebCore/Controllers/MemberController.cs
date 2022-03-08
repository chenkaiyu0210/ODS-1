using fontWebCore.Common.Context;
using fontWebCore.Common.Function;
using fontWebCore.Models;
using fontWebCore.Models.Repositories;
using fontWebCore.Models.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace fontWebCore.Controllers
{
    /// <summary>
    /// 會員資料
    /// </summary>   
    public class MemberController : BaseController
    {
        private readonly ILogger<MemberController> _logger;
        private readonly ODSContext _context;
        private readonly settingConifgModel _setting;
        public MemberController(IServiceProvider provider)
        {
            _logger = (ILogger<MemberController>)provider.GetService(typeof(ILogger<MemberController>));
            _context = (ODSContext)provider.GetService(typeof(ODSContext));
            _setting = (settingConifgModel)provider.GetService(typeof(settingConifgModel));
        }
        /// <summary>
        /// 會員修改
        /// </summary>        
        public IActionResult MemberSetting()
        {
            members m = _context.members.FromSqlRaw($"select * from members where customer_id = @customer_id", new object[] {
                        new SqlParameter { ParameterName = "customer_id", Value =  this.MemberInfo.customer_id }
                }).FirstOrDefault();
            viewModelMember item = CommonHelpers.Migration<members, viewModelMember>(m);
            if (item != null)
            {               
                if (item.customer_birthday.HasValue)
                {
                    item.customer_birthdayYY = (item.customer_birthday.Value.Year - 1911).ToString();
                    item.customer_birthdayMM = item.customer_birthday.Value.Month.ToString();
                    item.customer_birthdayDD = item.customer_birthday.Value.Day.ToString();
                }
            }
            return View(item);
        }
        [HttpPost]
        public IActionResult MemberSetting(viewModelMember model)
        {
            try
            {
                members chk = _context.members.FromSqlRaw($"select * from members where customer_id = @customer_id", new object[] {
                        new SqlParameter { ParameterName = "customer_id", Value =  model.customer_id , DbType = System.Data.DbType.Guid }
                }).AsNoTracking().FirstOrDefault();
                viewModelMember insertModel = CommonHelpers.Migration<members, viewModelMember>(chk);
                SHA256Processor sHA256Processor;
                string _pwd = string.Empty;
                #region 檢核
                if (model.customer_mobile_phone != model.last_mobile_phone)
                {
                    if (model.sms_verify_code != chk.sms_verify_code)
                    {
                        ViewData["errMsg"] = "簡訊驗證碼不符";
                        model.sms_verify_code = string.Empty;
                        return View(model);
                    }
                }
                if (!string.IsNullOrWhiteSpace(model.new_password))
                {
                    sHA256Processor = new SHA256Processor(chk.salt_key);
                    _pwd = Encoding.UTF8.GetString(sHA256Processor.Encode(Encoding.UTF8.GetBytes(model.password)));

                    if (chk.password != _pwd)
                    {
                        ViewData["errMsg"] = "舊密碼不符";
                        return View(model);
                    }
                    if (model.new_password != model.confirm_password)
                    {
                        ViewData["errMsg"] = "新密碼與確認密碼不符";
                        return View(model);
                    }
                    string _salt = CommonHelpers.GeneratePassword(6);
                    sHA256Processor = new SHA256Processor(_salt);
                    _pwd = Encoding.UTF8.GetString(sHA256Processor.Encode(Encoding.UTF8.GetBytes(model.new_password)));
                    insertModel.salt_key = _salt;
                    insertModel.password = _pwd;
                }               
                
                int.TryParse(model.customer_birthdayYY, out int yy);
                int.TryParse(model.customer_birthdayMM, out int mm);
                int.TryParse(model.customer_birthdayDD, out int dd);
                if (!DateTime.TryParse((yy + 1911).ToString() + "/" + mm.ToString("00") + "/" + dd.ToString("00"), out DateTime BD))
                {
                    ViewData["errMsg"] = "出生日期錯誤!";
                    return View(model);
                }
                insertModel.customer_birthday = BD;
                #endregion
                insertModel.customer_email = model.customer_email;
                insertModel.customer_name = model.customer_name;


                _context.members.Update(CommonHelpers.Migration<viewModelMember, members>(insertModel));
                _context.SaveChanges();
                ViewData["errMsg"] = "修改完成";
                this.MemberInfo = insertModel;
                return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 會員註冊
        /// </summary>
        [AllowAnonymous]
        public IActionResult Register(string customer_idcard_no)
        {
            ViewData["errMsg"] = this.TransferControllerMsg;
            return View(new viewModelMember { customer_idcard_no = customer_idcard_no });
        }
        [AllowAnonymous, HttpPost]
        public async Task<IActionResult> Register(viewModelMember model)
        {
            try
            {
                object[] parameters = new object[] {
                        new SqlParameter { ParameterName = "customer_id", Value = model.customer_id },
                };
                members m = _context.members.FromSqlRaw($"select * from members where customer_id = @customer_id", parameters).FirstOrDefault();
                if (model.sms_verify_code == m.sms_verify_code)
                {
                    if (model.password != model.password_check)
                    {
                        ViewData["errMsg"] = "密碼與確認密碼不符";
                        model.password = string.Empty;
                        model.password_check = string.Empty;
                        model.sms_verify_code = string.Empty;
                        return View(model);
                    }
                    string _salt = CommonHelpers.GeneratePassword(6);
                    SHA256Processor sHA256Processor = new SHA256Processor(_salt);
                    string _pwd = Encoding.UTF8.GetString(sHA256Processor.Encode(Encoding.UTF8.GetBytes(model.password)));

                    m.customer_idcard_no = model.customer_idcard_no;
                    m.customer_mobile_phone = model.customer_mobile_phone;                    
                    m.password = _pwd;
                    m.salt_key = _salt;
                    m.register_time = this.TaiwanDateTime;
                    m.sms_is_verify = true;
                    m.is_enable = true;

                    _context.members.Update(m);
                    _context.SaveChanges();


                    //帳密都輸入正確，ASP.net Core要多寫三行程式碼 
                    Claim[] claims = new[] { new Claim("memberInfo", JsonConvert.SerializeObject(m)) }; //Key取名"Account"，在登入後的頁面，讀取登入者的帳號會用得到，自己先記在大腦
                    ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);//Scheme必填
                    ClaimsPrincipal principal = new ClaimsPrincipal(claimsIdentity);


                    //從組態讀取登入逾時設定
                    //double loginExpireMinute = this.config.GetValue<double>("LoginExpireMinute");
                    //執行登入，相當於以前的FormsAuthentication.SetAuthCookie()
                    await HttpContext.SignInAsync(principal,
                         new AuthenticationProperties()
                         {
                             IsPersistent = false, //IsPersistent = false：瀏覽器關閉立馬登出；IsPersistent = true 就變成常見的Remember Me功能
                                                   //用戶頁面停留太久，逾期時間，在此設定的話會覆蓋Startup.cs裡的逾期設定
                            /* ExpiresUtc = DateTime.UtcNow.AddMinutes(loginExpireMinute) */
                         });
                    return RedirectToAction("MemberCenter", "Receive");                    

                }
                else
                {
                    ViewData["errMsg"] = "簡訊驗證碼不符";
                    model.sms_verify_code = string.Empty;
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home");
            }
        }        
        /// <summary>
        /// 忘記密碼
        /// </summary>
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View(new viewModelMember());
        }
        [AllowAnonymous , HttpPost]
        public IActionResult ForgotPassword(viewModelMember model)
        {
            try
            {
                if(string.IsNullOrWhiteSpace(model.customer_mobile_phone) || string.IsNullOrWhiteSpace(model.customer_idcard_no))
                {
                    ViewData["errMsg"] = "輸入內容有誤";
                    model.sms_verify_code = string.Empty;
                    return View(model);
                }
                members chk = _context.members.FromSqlRaw($"select * from members WHERE customer_mobile_phone = @customer_mobile_phone AND customer_idcard_no = @customer_idcard_no",
                   new object[] {
                        new SqlParameter { ParameterName = "customer_mobile_phone", Value = model.customer_mobile_phone },
                        new SqlParameter { ParameterName = "customer_idcard_no", Value = model.customer_idcard_no }
                    }).FirstOrDefault();
                if (chk == null)
                {
                    ViewData["errMsg"] = "無此會員資料";
                    model.sms_verify_code = string.Empty;
                    return View(model);
                }
                #region 檢核
                if (model.sms_verify_code != chk.sms_verify_code)
                {
                    ViewData["errMsg"] = "簡訊驗證碼不符";
                    model.sms_verify_code = string.Empty;
                    return View(model);
                }
                if (model.new_password != model.confirm_password)
                {
                    ViewData["errMsg"] = "新密碼與確認密碼不符";
                    return View(model);
                }
                #endregion

                string _salt = CommonHelpers.GeneratePassword(6);
                SHA256Processor sHA256Processor = new SHA256Processor(_salt);
                string _pwd = Encoding.UTF8.GetString(sHA256Processor.Encode(Encoding.UTF8.GetBytes(model.new_password)));

                chk.password = _pwd;
                chk.salt_key = _salt;
                chk.sms_is_verify = true;

                _context.members.Update(chk);
                _context.SaveChanges();

                return RedirectToAction("Login", "Home");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }        
        /// <summary>
        /// 發送驗證碼簡訊
        /// </summary>
        [AllowAnonymous]
        public IActionResult SendSms(string mobile, string customer_idcard_no, string customer_id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(mobile) && string.IsNullOrWhiteSpace(customer_idcard_no) && string.IsNullOrWhiteSpace(customer_id))
                {
                    return Json(new { code = "0", msg = "輸入錯誤" });
                }
                string strSql = "select * from members where 1 = 1";
                object[] parameters;
                if(!string.IsNullOrWhiteSpace(mobile) && !string.IsNullOrWhiteSpace(customer_idcard_no))
                {
                    strSql += "AND customer_mobile_phone = @customer_mobile_phone AND customer_idcard_no = @customer_idcard_no";
                    parameters = new object[] {
                        new SqlParameter { ParameterName = "customer_mobile_phone", Value = mobile },
                        new SqlParameter { ParameterName = "customer_idcard_no", Value = customer_idcard_no }
                    };
                }
                else if (!string.IsNullOrWhiteSpace(customer_id))
                {                    
                    strSql += "AND customer_id = @customer_id";
                    parameters = new object[] {
                        new SqlParameter { ParameterName = "customer_id", Value = customer_id }
                    };
                }
                else
                {
                    strSql += "AND customer_mobile_phone = @customer_mobile_phone";
                    parameters = new object[] {
                        new SqlParameter { ParameterName = "customer_mobile_phone", Value = mobile }
                    };
                }
                members m = _context.members.FromSqlRaw(strSql, parameters).FirstOrDefault();
                if (m != null)
                {
                    string VerificateCode = string.Format("{0:000000}", (new Random()).Next(1000000));
                    StringBuilder postDataSb = new StringBuilder();
                    postDataSb.Append("UID=").Append("UM188888");
                    postDataSb.Append("&PWD=").Append("Omued283393");
                    postDataSb.Append("&SB=").Append("簡訊認證碼");
                    postDataSb.Append("&MSG=歡迎加入岳沐，您的驗證碼為").Append(VerificateCode);
                    postDataSb.Append("&DEST=").Append(mobile);
                    postDataSb.Append("&ST=").Append("");
                    string resultString = CommonHelpers.CallMessage(_setting.every8dUrl, postDataSb.ToString());
                    //CreateLog(resultString);
                    m.sms_verify_code = VerificateCode;
                    //m.sms_is_verify = false;
                    _context.members.Update(m);

                    _context.SaveChanges();
                    return Json(new { code = "1", msg = m.customer_id });
                }
                else if (!string.IsNullOrWhiteSpace(customer_idcard_no))
                {
                    return Json(new { code = "0", msg = "查無此會員" });
                }
                else
                {
                    string VerificateCode = string.Format("{0:000000}", (new Random()).Next(1000000));
                    StringBuilder postDataSb = new StringBuilder();
                    postDataSb.Append("UID=").Append("UM188888");
                    postDataSb.Append("&PWD=").Append("Omued283393");
                    postDataSb.Append("&SB=").Append("簡訊認證碼");
                    postDataSb.Append("&MSG=歡迎加入岳沐，您的驗證碼為").Append(VerificateCode);
                    postDataSb.Append("&DEST=").Append(mobile);
                    postDataSb.Append("&ST=").Append("");
                    string resultString = CommonHelpers.CallMessage(_setting.every8dUrl, postDataSb.ToString());
                    //CreateLog(resultString);
                    string _id = Guid.NewGuid().ToString();
                    _context.members.Add(new Models.Repositories.members
                    {
                        customer_id = Guid.Parse(_id),
                        customer_mobile_phone = mobile,
                        sms_verify_code = VerificateCode,
                        is_enable = false
                    });

                    _context.SaveChanges();
                    return Json(new { code = "1", msg = _id });
                }


                //if (resultString != "false")
                //{
                //    /* 
                //     * 傳送成功 回傳字串內容格式為：CREDIT,SENDED,COST,UNSEND,BATCH_ID，各值中間以逗號分隔。
                //     * CREDIT：發送後剩餘點數。負值代表發送失敗，系統無法處理該命令
                //     * SENDED：發送通數。
                //     * COST：本次發送扣除點數
                //     * UNSEND：無額度時發送的通數，當該值大於0而剩餘點數等於0時表示有部份的簡訊因無額度而無法被發送。
                //     * BATCH_ID：批次識別代碼。為一唯一識別碼，可藉由本識別碼查詢發送狀態。格式範例：220478cc-8506-49b2-93b7-2505f651c12e
                //     */
                //    string[] split = resultString.Split(',');
                //    string batchID = split[4];
                //    Session["VerificateCode"] = VerificateCode;
                //    Session["VerifyMobile"] = mobile;
                //    TempData["IsGetMobileVCode"] = "true";
                //}
                //else
                //{
                //    TempData["message"] = "發送失敗";
                //}



            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home");
            }

        }
        [AllowAnonymous]
        public IActionResult checkIdcardNo(string idcardNo)
        {
            try
            {
                members m = _context.members.FromSqlRaw($"select * from members where customer_idcard_no = @idcardNo", new object[] {
                        new SqlParameter { ParameterName = "idcardNo", Value = idcardNo } }).FirstOrDefault();
                return Json(m != null ? new { code = "1", msg = "該身分證已有註冊" } : new { code = "0" });

            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home");
            }           
        }

        public void CreateLog(string msg)
        {
            _context.logJson.Add(new logJson
            {
                log_id = Guid.NewGuid(),
                source = "sendSms",
                request_json = msg,
                request_time = this.TaiwanDateTime
            });
        }
    }
}
