using backendWeb.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace backendWeb.Areas.Management.Controllers
{
    public class MenuSettingController : BaseController
    {
        // GET: Management/MenuSetting
        public ActionResult Index()
        {
            return View();
        }
    }
}