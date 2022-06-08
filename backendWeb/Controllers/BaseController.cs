using backendWeb.Models;
using backendWeb.Models.ViewModel;
using backendWeb.Service.ServiceClass;
using Newtonsoft.Json;
using System.Configuration;
using System.Text;
using System.Web.Mvc;

namespace backendWeb.Controllers
{
    public class BaseController : Controller
    {
        #region 參數
        /// <summary>
        /// Config File
        /// </summary>
        public settingConifgModel configSetting
        {
            get
            {
                return JsonConvert.DeserializeObject<settingConifgModel>(ConfigurationManager.AppSettings["SettingConfig"].ToString());
            }
        }
        public string userInfo
        {
            get
            {
                return Session["userInfo"] == null ? "" : Session["userInfo"].ToString();
            }
            set
            {
                Session["userInfo"] = value;
            }
        }
        public viewModelBackendUser userInfoMdoel
        {
            get
            {
                return Session["userInfo"] == null ? null : JsonConvert.DeserializeObject<viewModelBackendUser>(this.userInfo);
            }
        }
        //public string roleInfo
        //{
        //    get
        //    {
        //        return Session["roleInfo"] == null ? "" : Session["roleInfo"].ToString();
        //    }
        //    set
        //    {
        //        Session["roleInfo"] = value;
        //    }
        //}
        public string errMsg
        {
            set
            {
                ViewData["errMsg"] = value;
            }
        }
        public string infoMsg
        {
            set
            {
                ViewData["infoMsg"] = value;
            }
        }
        public string warnMsg
        {
            set
            {
                ViewData["warnMsg"] = value;
            }
        }
        public string successMsg
        {
            set
            {
                ViewData["successMsg"] = value;
            }
        }
        public System.DateTime TaiwanDateTime
        {
            get
            {
                return System.TimeZoneInfo.ConvertTimeBySystemTimeZoneId(System.DateTime.Now, "Taipei Standard Time");
            }
        }

        LogUtil logUtil = new LogUtil();
        #endregion

        /// <summary>
        /// 錯誤事件
        /// </summary>
        /// <param name="filterContext"></param>
        protected override void OnException(ExceptionContext filterContext)
        {
            //儲存錯誤訊息
            StringBuilder buff = new StringBuilder();
            buff.Append(string.Concat(new object[] { "Exception.Type : ", filterContext.Exception.GetType().Name, "\r\nException.Message : ", filterContext.Exception.Message, "\r\nException.TargetSite: ", filterContext.Exception.TargetSite, "\r\nException.StackTrace: \r\n", filterContext.Exception.StackTrace }));
            this.logUtil.OutputLog("系統發生錯誤", buff.ToString());

            base.OnException(filterContext);
        }
    }
}