
using System.Web;
using System.Web.Mvc;

namespace backendWeb.Filter
{
    public class LoginFilterAttibute : ActionFilterAttribute, IActionFilter, IResultFilter
    {
        /// <summary>
        /// 在執行 Action 之前執行
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ViewDataDictionary keyValues = new ViewDataDictionary();
            if (HttpContext.Current.Request.Cookies["login"] != null)
            {
                #region 拆解info
                string[] login_info = HttpContext.Current.Request.Cookies["login"].Value.Split('&');
                keyValues.Add("account", login_info[0].Split('=')[1]);
                keyValues.Add("password", login_info[1].Split('=')[1]);
                #endregion
                filterContext.Controller.ViewData = keyValues;
            }
        }
        /// <summary>
        /// 在執行 Action 之後執行
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {

        }
        /// <summary>
        /// 在執行 Action Result 之前執行
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {

        }
        /// <summary>
        /// 在執行 Action Result 之後執行
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {

        }
    }
}