using backendWeb.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace backendWeb.Controllers
{
    public class HomeController : Controller
    {
        #region 登入
        [LoginFilterAttibute, AllowAnonymous]
        public ActionResult Login() => View();
        [HttpPost, AllowAnonymous]
        public ActionResult Login(string txtAccount, string txtPassword, string ckRemeber)
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
            return RedirectToAction("Index");
        }
        #endregion
        #region 登出
        public ActionResult Logout()
        {
            Session["account"] = null;
            Session["CN"] = null;
            Session["OU"] = null;
            Session.RemoveAll();
            Response.Cookies.Clear();
            FormsAuthentication.SignOut();
            ViewData["MessageFormat"] = "success,已成功登出!";
            return RedirectToAction("Login");
        }
        #endregion
        #region 首頁        
        public ActionResult Index() => View();
        #endregion
        #region 選單
        public ContentResult BuildMenu()
        {
            StringBuilder result = new StringBuilder();
            //IICS0007Service _IICS0007Service = new ICS0007Service();
            //ICS0007ViewModel model = _IICS0007Service.getmenu(Session["account"] == null ? "" : Session["account"].ToString(), Session["CN"] == null ? "" : Session["CN"].ToString(), "yrs");
            #region 抓第一層
            //foreach (ICS0007ResultModel item in model.list.Where(o => o.func_layer == 1).OrderBy(o => o.sort_order).ToList())
            //{
            //    result.Append("<li>");
            //    string arrow = (model.list.Count(o => o.parent_func_id == item.func_id) > 0 ? "<span class='fa arrow'></span>" : "");
            //    if (!string.IsNullOrWhiteSpace(arrow))
            //        result.Append("<a href='#'><i class='fa fa-fw " + item.icon + "'></i>" + item.func_name + arrow + "</a>");
            //    else
            //        result.Append("<a href='" + Url.Content("~" + item.func_url) + "'><i class='fa fa-fw " + item.icon + "'></i>" + item.func_name + arrow + "</a>");

            //    ///判斷有下層在進入組下層Menu
            //    if (!string.IsNullOrWhiteSpace(arrow))
            //        result = (menu_loop(item.func_id, model.list, result, item.func_id));
            //    result.Append("</li>");
            //}
            #endregion
            return Content(result.ToString());
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
            cookie.Expires = DateTime.Now.AddDays(15);

            return cookie;
        }
        private HttpCookie GetCookieLoginInfoClear()
        {
            HttpCookie cookie = new HttpCookie("login");
            cookie.Values.Add("user_id", "");
            cookie.Values.Add("user_pwd", "");
            cookie.Expires = DateTime.Now.AddDays(-1);

            return cookie;
        }
        #endregion
    }
}