using backendWeb.Models;
using backendWeb.Models.ViewModel;
using backendWeb.Service.ServiceClass;
using Newtonsoft.Json;
using System.Configuration;
using System.Text;
using System.Web.Http;
using System.Web.Mvc;

namespace backendWeb.Controllers
{
    public class BaseApiController : ApiController
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
        #endregion
    }
}