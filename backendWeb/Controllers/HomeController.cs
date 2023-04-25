using backendWeb.Filter;
using backendWeb.Helpers;
using backendWeb.Models.ViewModel;
using backendWeb.Service.InterFace;
using backendWeb.Service.ServiceClass;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace backendWeb.Controllers
{
    public class HomeController : BaseController
    {
        #region 登入
        [LoginFilterAttibute, AllowAnonymous]
        public ActionResult Login() => View();
        [HttpPost, AllowAnonymous]
        public ActionResult Login(string txtAccount, string txtPassword, string ckRemeber, string ReturnUrl)
        {
            #region 記住我
            if (!string.IsNullOrWhiteSpace(ckRemeber))
            {
                Response.Cookies.Add(GetCookieLoginInfo(txtAccount, txtPassword));
            }
            else
            {
                Response.Cookies.Add(GetCookieLoginInfoClear());
            }
            #endregion

            IBaseCrudService<viewModelBackendUser> service = new backendUserService();

            viewModelBackendUser viewModel = service.GetOnly(new viewModelBackendUser { search_account = txtAccount });
            if (viewModel != null)
            {
                //if (!viewModel.is_enable.Value) { this.errMsg = "無此帳號"; return View(); }
                #region 驗證
                SHA256Processor sHA256Processor = new SHA256Processor(viewModel.salt_key);
                string _pwd = Encoding.UTF8.GetString(sHA256Processor.Encode(Encoding.UTF8.GetBytes(txtPassword)));
                if (_pwd != viewModel.password)
                {
                    this.errMsg = "帳號密碼不符";
                    return View();
                }
                else
                {
                    #region FormsAuthentication
                    ////建立FormsAuthentication ticket
                    //FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1,
                    //        viewModel.account,//使用者帳號
                    //        DateTime.Now, //核發日期
                    //        DateTime.Now.AddMinutes(configSetting.loginExpireMinute), //到期時間 30分鐘
                    //        true, // 是否記住我
                    //        viewModel.role_group_codes, //使用者身份
                    //        FormsAuthentication.FormsCookiePath);
                    //string encTicket = FormsAuthentication.Encrypt(ticket);
                    //System.Web.HttpContext.Current.Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, encTicket));
                    #endregion
                    //string roleGroup = new backendRoleGroupService().GetRoleGroupCodes(new viewModelBackendRoleGroup { searchIn_role_group_code = viewModel.role_group_codes });
                    //紀錄登入資訊
                    this.userInfo = JsonConvert.SerializeObject(viewModel);
                    //this.roleInfo = roleGroup;

                    if (string.IsNullOrWhiteSpace(ReturnUrl))
                        return RedirectToAction("Index");
                    else
                        return Redirect(ReturnUrl);
                }
                #endregion
            }
            else
            {
                this.errMsg = "無此帳號";
                return View();
            }
        }
        #endregion
        #region 登出
        public ActionResult Logout()
        {
            //Session["account"] = null;
            //Session["CN"] = null;
            //Session["OU"] = null;
            //Session.Abandon();
            Session.Clear();
            Session.RemoveAll();
            //Response.Cookies.Clear();
            //FormsAuthentication.SignOut();
            this.infoMsg = "已成功登出!";
            return RedirectToAction("Login");
        }
        #endregion
        #region 首頁
        [CustomAuthorize]
        public ActionResult Index()
        {
            if (string.IsNullOrEmpty(this.userInfo))
                return RedirectToAction("Login");
            else
                return View();
        }

        #endregion
        #region 選單
        public ContentResult BuildMenu()
        {
            if (this.userInfo == null)
            {
                return Content("");
            }
            else
            {
                StringBuilder result = new StringBuilder();
                viewModelBackendUser item = JsonConvert.DeserializeObject<viewModelBackendUser>(this.userInfo);
                IList<viewModelBackendMenu> itemList = new backendMenuService().GetMenu(new viewModelBackendMenu { searchIn_role_group_code = item.role_group_codes });
                #region 產生選單
                foreach (viewModelBackendMenu menuItem in itemList.Where(o => o.func_layer == 1))
                {

                    result.Append($"<div class=\"sidebar-heading\">{menuItem.func_name}</div>");
                    SubMenu(menuItem.app_id, menuItem.func_id, itemList, result);
                    result.Append("<hr class=\"sidebar-divider\">");
                }
                #endregion
                return Content(result.ToString());
            }
        }
        #endregion
        #region 錯誤
        [AllowAnonymous]
        public ActionResult Error() => View();
        #endregion
        #region Method
        private HttpCookie GetCookieLoginInfo(string user_id, string user_pwd)
        {
            HttpCookie cookie = new HttpCookie("login");
            cookie.Values.Add("user_id", user_id);
            cookie.Values.Add("user_pwd", user_pwd);
            cookie.Expires = this.TaiwanDateTime.AddDays(15);

            return cookie;
        }
        private HttpCookie GetCookieLoginInfoClear()
        {
            HttpCookie cookie = new HttpCookie("login");
            cookie.Values.Add("user_id", "");
            cookie.Values.Add("user_pwd", "");
            cookie.Expires = this.TaiwanDateTime.AddDays(-1);

            return cookie;
        }
        /// <summary>
        /// 組合子選單功能
        /// </summary>
        /// <param name="app_id">傳入第一層app_id</param>
        /// <param name="parent_func_id">傳入第一層func_id</param>
        /// <param name="itemList">傳入查詢出來的選單列表</param>
        /// <param name="result">傳入要產生的選單文字</param>
        /// <returns></returns>
        private StringBuilder SubMenu(string app_id, string parent_func_id, IList<viewModelBackendMenu> itemList, StringBuilder result)
        {
            List<viewModelBackendMenu> secondMenu = itemList.Where(o => o.app_id == app_id && o.parent_func_id == parent_func_id && o.func_layer == 2).OrderBy(o => o.sort_order).ToList();
            ///第二層
            foreach (viewModelBackendMenu sec in secondMenu)
            {
                result.Append("<li class=\"nav-item\">" + Environment.NewLine);
                if (string.IsNullOrWhiteSpace(sec.func_url))
                {
                    string secondId = sec.app_id + sec.func_id;
                    ///第二層HTML RAW

                    result.Append("<a class=\"nav-link collapsed\" href=\"#\" data-toggle=\"collapse\" data-target=\"#" + secondId + "\"aria-expanded=\"true\" aria-controls=\"" + secondId + "\">" + Environment.NewLine);
                    result.Append("<i class=\"fas fa-fw " + sec.icon + "\"></i><span>" + sec.func_name + "</span></a>" + Environment.NewLine);
                    List<viewModelBackendMenu> thirdMenu = itemList.Where(o => o.app_id == sec.app_id && o.parent_func_id == sec.func_id && o.func_layer == 3).OrderBy(o => o.sort_order).ToList();
                    ///第三層
                    if (thirdMenu.Count > 0)
                    {
                        ///第三層 HTML RAW
                        result.Append("<div id=\"" + secondId + "\" class=\"collapse\" aria-labelledby=\"heading" + secondId + "\" data-parent=\"#accordionSidebar\">" + Environment.NewLine);
                        result.Append("<div class=\"bg-white py-2 collapse-inner rounded\">" + Environment.NewLine);
                        //result.Append("<h6 class=\"collapse-header\">功能:</h6>" + Environment.NewLine);
                        foreach (viewModelBackendMenu thr in thirdMenu)
                        {
                            result.Append("<a class=\"collapse-item\" href=\"" + thr.func_url + "\">" + thr.func_name + "</a>");
                        }
                        result.Append("</div></div>");
                    }
                }
                else
                {
                    result.Append("<a class=\"nav-link\" href=\"" + sec.func_url + "\"><i class=\"fas fa-fw " + sec.icon + "\"></i><span>" + sec.func_name + "</span></a>" + Environment.NewLine);
                }
                result.Append("</li>");
            }
            return result;
        }
        #endregion
    }
}