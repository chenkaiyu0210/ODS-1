using fontWebCore.Models.ViewModels;
using Microsoft.AspNetCore.Authentication;
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
                System.Security.Claims.ClaimsIdentity identity = HttpContext.User.Identity as System.Security.Claims.ClaimsIdentity;
                if (identity == null)
                    return null;

                System.Security.Claims.Claim claim = identity.Claims.FirstOrDefault(c => c.Type == "memberInfo");
                
                //System.Security.Claims.Claim claim = HttpContext.User.Claims.Where(o => o.Type == "memberInfo").FirstOrDefault();
                return claim != null ? JsonConvert.DeserializeObject<viewModelMember>(claim.Value) : null;
            }
            set
            {
                System.Security.Claims.ClaimsIdentity identity = HttpContext.User.Identity as System.Security.Claims.ClaimsIdentity;
                if (identity == null)
                    return;

                // check for existing claim and remove it
                var existingClaim = identity.FindFirst("memberInfo");
                if (existingClaim != null)
                    identity.RemoveClaim(existingClaim);

                // add new claim
                identity.AddClaim(new System.Security.Claims.Claim("memberInfo", JsonConvert.SerializeObject(value)));
                System.Security.Claims.ClaimsPrincipal principal = new System.Security.Claims.ClaimsPrincipal(identity);
                HttpContext.SignInAsync(principal,
                            new AuthenticationProperties()
                            {
                                IsPersistent = false, //IsPersistent = false：瀏覽器關閉立馬登出；IsPersistent = true 就變成常見的Remember Me功能
                                //用戶頁面停留太久，逾期時間，在此設定的話會覆蓋Startup.cs裡的逾期設定
                                /* ExpiresUtc = DateTime.UtcNow.AddMinutes(loginExpireMinute) */
                            });               
            }
        }
        
        public string TransferControllerMsg
        {            
            set { TempData["Transfer"] = value; }
            get { return TempData["Transfer"] != null ? TempData["Transfer"].ToString() : ""; }
        }

        public System.DateTime TaiwanDateTime
        {
            get
            {
                return System.TimeZoneInfo.ConvertTimeBySystemTimeZoneId(System.DateTime.Now, "Taipei Standard Time");
            }
        }
        #endregion
    }
}
