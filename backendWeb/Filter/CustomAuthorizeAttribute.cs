using backendWeb.Models.ViewModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace backendWeb.Filter
{
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        //private readonly string[] allowedroles;
        //public CustomAuthorizeAttribute(params string[] roles)
        //{
        //    this.allowedroles = roles;
        //}
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            bool authorize = false;
            string _userInfo = Convert.ToString(httpContext.Session["userInfo"]);
           
            if (!string.IsNullOrEmpty(_userInfo))
            {                
                viewModelBackendUser user = JsonConvert.DeserializeObject<viewModelBackendUser>(_userInfo);
                if (user.role_group_codes == "adminstrator" || string.IsNullOrWhiteSpace(this.Roles)) authorize = true;
                else
                {
                    string[] userData = user.role_group_codes.Split(new char[] { ',' });
                    authorize = userData.Any(o => o == this.Roles);
                }                
            }
            return authorize;
        }
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                filterContext.Result = new HttpUnauthorizedResult();
            }
            else
            {
                filterContext.Result = new RedirectToRouteResult(new
                    RouteValueDictionary(new RouteValueDictionary(new
                    {
                        action = "Error",
                        controller = "Home",
                        area = ""
                    })));
            }
        }
    }
}