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
using SimpleCaptcha;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace fontWebCore.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ODSContext _context;
        private readonly ICaptcha _captcha;
        private readonly settingConifgModel _setting;

        public HomeController(IServiceProvider provider)
        {
            _logger = (ILogger<HomeController>)provider.GetService(typeof(ILogger<HomeController>));
            _context = (ODSContext)provider.GetService(typeof(ODSContext));
            _captcha = (ICaptcha)provider.GetService(typeof(ICaptcha));
            _setting = (settingConifgModel)provider.GetService(typeof(settingConifgModel));
        }
        /// <summary>
        /// 首頁
        /// </summary>
        [AllowAnonymous]
        public IActionResult Index() => View();
        /// <summary>
        /// 登入
        /// </summary>
        [AllowAnonymous]
        public IActionResult Login() => View();
        [AllowAnonymous, HttpPost]
        public async Task<IActionResult> Login(viewModelLogin model, string ReturnUrl)
        {
            try
            {
                if (!_captcha.Validate(model.captcha_id, model.captcha_code))
                {
                    ViewData["errMsg"] = "驗證碼錯誤";
                    return View();
                }
                
                members m = _context.members.FromSqlRaw($"select * from members where customer_idcard_no = @customer_idcard_no", new object[] {
                        new SqlParameter { ParameterName = "customer_idcard_no", Value = model.idcard_no }
                }).FirstOrDefault();

                if (m != null)
                {
                    SHA256Processor sHA256Processor = new SHA256Processor(m.salt_key);
                    string _pwd = Encoding.UTF8.GetString(sHA256Processor.Encode(Encoding.UTF8.GetBytes(model.password)));
                    if (_pwd != m.password)
                    {
                        return View();
                    }
                    else
                    {
                        //viewModelMember member = new viewModelMember
                        //{
                        //    customer_id = m.customer_id.ToString(),
                        //    idcard_no = m.customer_idcard_no,
                        //    mobile = m.customer_mobile_phone,
                        //    sms_verify_code = m.sms_is_verify.Value ? "1" : "0"
                        //};

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
                        //加上 Url.IsLocalUrl 防止Open Redirect漏洞
                        if (!string.IsNullOrEmpty(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
                        {
                            return Redirect(ReturnUrl);//導到原始要求網址
                        }
                        else
                        {
                            return RedirectToAction("MemberCenter", "Recevie"); //到登入後的第一頁，自行決定
                        }
                    }
                }
                else
                {
                    return View();
                }
            }
            catch (Exception)
            {
                return RedirectToAction("Error");
            }
        }
        /// <summary>
        /// 登出
        /// </summary>
        [AllowAnonymous]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();

            return RedirectToAction("Index");
        }
        /// <summary>
        /// 聯絡我們
        /// </summary>
        [AllowAnonymous,HttpPost]
        public IActionResult conactUs (viewModelConact model)
        {
            try
            {
                string _subject = $"聯絡我們:{model.name}";
                string _body = model.message;

                if (!string.IsNullOrWhiteSpace(model.lineID))
                {
                    _subject += "，LINE ID:" + model.lineID;
                }

                CommonHelpers.SendMail(_setting.setMail.mailAccount, model.email, _subject, _body, _setting.setMail);               
                ViewData["errMsg"] = "信件已發送。";
                return View("Index");
            }
            catch (Exception ex)
            {
                ViewData["errMsg"] = "信件發送錯誤，請洽專人。";
                return View("Index");
            }            
        }
        /// <summary>
        /// 產生驗證碼
        /// </summary>        
        /// <returns></returns>
        [AllowAnonymous]
        public IActionResult Captcha(string id)
        {
            var info = _captcha.Generate(id);
            var stream = new MemoryStream(info.CaptchaByteData);
            return File(stream, "image/png");
        }
        /// <summary>
        /// 錯誤頁
        /// </summary>
        [AllowAnonymous, ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
