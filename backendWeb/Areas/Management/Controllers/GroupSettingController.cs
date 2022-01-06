using backendWeb.Controllers;
using backendWeb.Models.ViewModel;
using backendWeb.Service.InterFace;
using backendWeb.Service.ServiceClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace backendWeb.Areas.Management.Controllers
{
    public class GroupSettingController : BaseController
    {
        #region 群組設定
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Table()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Table(viewModelBackendRoleGroup model)//(int draw, int start, int length , string search_account)
        {
            IBaseCrudService<viewModelBackendRoleGroup> crudService = new backendRoleGroupService();
            //IList<viewModelBackendRoleGroup> list = crudService.GetList(new viewModelBackendRoleGroup { search_draw = draw, search_start = start, search_length = length });
            IList<viewModelBackendRoleGroup> list = crudService.GetList(model);
            var returnObj =
                  new
                  {
                      draw = model.draw,
                      recordsTotal = list == null || list.Count == 0 ? 0 : list[0].numCount,
                      recordsFiltered = list == null || list.Count == 0 ? 0 : list[0].numCount,
                      data = list == null || list.Count == 0 ? new List<viewModelBackendRoleGroup>() : list//分頁後的資料 
                  };
            return Json(returnObj);
        }
        public ActionResult Add()
        {
            IBaseCrudService<viewModelBackendMenu> crudServiceMenu = new backendMenuService();
            viewModelBackendRoleGroup model = new viewModelBackendRoleGroup();
            model.listAuthorize = crudServiceMenu.GetList(new viewModelBackendMenu { search_hasAuthorize = true });
            return View("Data", model);
        }
        [HttpPost]
        public ActionResult Add(viewModelBackendRoleGroup model)
        {
            model.saveAction = "Create";            
            #region Check
            StringBuilder strChk = chkData(model);
            if (!string.IsNullOrWhiteSpace(strChk.ToString()))
            {
                this.errMsg = strChk.ToString();
                IBaseCrudService<viewModelBackendMenu> crudServiceMenu = new backendMenuService();
                model.listAuthorize = crudServiceMenu.GetList(new viewModelBackendMenu { search_hasAuthorize = true });
                return View("Data", model);
            }
            #endregion
            IBaseCrudService<viewModelBackendRoleGroup> crudService = new backendRoleGroupService();            
            model.authorize_codes = String.Join(",", model.dlAuthorizeCodes);
            viewModelBackendRoleGroup item = crudService.Save(model);
            if (!item.replyResult.Value)
            {
                this.errMsg = item.replyMsg;
                IBaseCrudService<viewModelBackendMenu> crudServiceMenu = new backendMenuService();
                model.listAuthorize = crudServiceMenu.GetList(new viewModelBackendMenu { search_hasAuthorize = true });
                model.listAuthorizeCodes = model.dlAuthorizeCodes;                
                return View("Data", model);
            }
            else
                return RedirectToAction("Index");
        }
        public ActionResult Edit(string search_role_group_code)
        {
            IBaseCrudService<viewModelBackendRoleGroup> crudService = new backendRoleGroupService();
            IBaseCrudService<viewModelBackendMenu> crudServiceMenu = new backendMenuService();
            viewModelBackendRoleGroup model = crudService.GetOnly(new viewModelBackendRoleGroup { search_role_group_code = search_role_group_code });
            model.listAuthorizeCodes = model.authorize_codes.Split(',');
            model.listAuthorize = crudServiceMenu.GetList(new viewModelBackendMenu { search_hasAuthorize = true });
            return View("Data", model);
        }
        [HttpPost]
        public ActionResult Edit(viewModelBackendRoleGroup model)
        {
            model.saveAction = "Motify";
            #region Check
            StringBuilder strChk = chkData(model);
            if (!string.IsNullOrWhiteSpace(strChk.ToString()))
            {
                this.errMsg = strChk.ToString();
                IBaseCrudService<viewModelBackendMenu> crudServiceMenu = new backendMenuService();
                model.listAuthorizeCodes = model.dlAuthorizeCodes;
                model.listAuthorize = crudServiceMenu.GetList(new viewModelBackendMenu { search_hasAuthorize = true });
                return View("Data", model);
            }
            #endregion
            IBaseCrudService<viewModelBackendRoleGroup> crudService = new backendRoleGroupService();
            model.authorize_codes = String.Join(",", model.dlAuthorizeCodes);
            viewModelBackendRoleGroup item = crudService.Save(model);
            if (!item.replyResult.Value)
            {
                this.errMsg = item.replyMsg;
                IBaseCrudService<viewModelBackendMenu> crudServiceMenu = new backendMenuService();
                model.listAuthorizeCodes = model.dlAuthorizeCodes;
                model.listAuthorize = crudServiceMenu.GetList(new viewModelBackendMenu { search_hasAuthorize = true });
                return View("Data", model);
            }
            else
                return RedirectToAction("Index");
        }
        #endregion        
        public StringBuilder chkData(viewModelBackendRoleGroup model)
        {
            StringBuilder chk = new StringBuilder();
            if (string.IsNullOrWhiteSpace(model.role_group_code)) { chk.Append("帳號欄位未填!"); return chk; }
            if (string.IsNullOrWhiteSpace(model.role_group_name)) { chk.Append("名稱欄位未填!"); return chk; }            
            if (model.dlAuthorizeCodes.Length == 0) { chk.Append("角色群組碼欄位未填!"); return chk; }
            return chk;
        }
    }
}