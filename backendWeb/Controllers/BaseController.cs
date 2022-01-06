using backendWeb.Models;
using backendWeb.Models.ViewModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
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
        #endregion
    }
}