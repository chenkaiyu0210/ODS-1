using fontWebCore.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Linq;

namespace fontWebCore.Controllers
{
    public class BaseController : Controller
    {
        #region 參數
        public viewModelMember MemberInfo
        {
            get
            {
                System.Security.Claims.Claim claim = HttpContext.User.Claims.Where(o => o.Type == "memberInfo").FirstOrDefault();
                return claim != null ? JsonConvert.DeserializeObject<viewModelMember>(claim.Value) : null;
            }
        }
        
        public string TransferControllerMsg
        {            
            set { TempData["Transfer"] = value; }
            get { return TempData["Transfer"] != null ? TempData["Transfer"].ToString() : ""; }
        }
        #endregion
    }
}
