using backendWeb.Controllers;
using backendWeb.Helpers;
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
    public class UserSettingController : BaseController
    {
        #region 人員設定
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Table()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Table(viewModelBackendUser model)//(int draw, int start, int length , string search_account)
        {
            IBaseCrudService<viewModelBackendUser> crudService = new backendUserService();
            //IList<viewModelBackendUser> list = crudService.GetList(new viewModelBackendUser { search_draw = draw, search_start = start, search_length = length });
            IList<viewModelBackendUser> list = crudService.GetList(model);
            var returnObj =
                  new
                  {
                      draw = model.draw,
                      recordsTotal = list == null || list.Count == 0 ? 0 : list[0].numCount,
                      recordsFiltered = list == null || list.Count == 0 ? 0 : list[0].numCount,
                      data = list == null || list.Count == 0 ? new List<viewModelBackendUser>() : list//分頁後的資料 
                  };
            return Json(returnObj);
        }
        public ActionResult Add()
        {
            IBaseCrudService<viewModelBackendRoleGroup> crudServiceRole = new backendRoleGroupService();
            viewModelBackendUser model = new viewModelBackendUser();
            model.listRoleGroup = crudServiceRole.GetList(new viewModelBackendRoleGroup());
            return View("Data", model);
        }
        [HttpPost]
        public ActionResult Add(viewModelBackendUser model)
        {
            model.saveAction = "Create";
            model.register_user = this.userInfoMdoel.account;
            #region Check
            StringBuilder strChk = chkData(model);
            if (!string.IsNullOrWhiteSpace(strChk.ToString()))
            {
                this.errMsg = strChk.ToString();
                IBaseCrudService<viewModelBackendRoleGroup> crudServiceRole = new backendRoleGroupService();
                model.listRoleGroup = crudServiceRole.GetList(new viewModelBackendRoleGroup());
                return View("Data", model);
            }
            #endregion
            IBaseCrudService<viewModelBackendUser> crudService = new backendUserService();
            #region Password
            string _salt = CommonHelpers.GeneratePassword(6);
            SHA256Processor sHA256Processor = new SHA256Processor(_salt);
            model.password = Encoding.UTF8.GetString(sHA256Processor.Encode(Encoding.UTF8.GetBytes(model.inputPassword)));
            model.salt_key = _salt;
            #endregion
            model.role_group_codes = String.Join(",", model.dlRoleCodes);
            viewModelBackendUser item = crudService.Save(model);
            if (!item.replyResult.Value)
            {
                this.errMsg = item.replyMsg;
                IBaseCrudService<viewModelBackendRoleGroup> crudServiceRole = new backendRoleGroupService();
                model.listRoleGroupCodes = model.dlRoleCodes;
                model.listRoleGroup = crudServiceRole.GetList(new viewModelBackendRoleGroup());
                return View("Data", model);
            }
            else
                return RedirectToAction("Index");
        }
        public ActionResult Edit(string search_account)
        {
            IBaseCrudService<viewModelBackendUser> crudService = new backendUserService();
            IBaseCrudService<viewModelBackendRoleGroup> crudServiceRole = new backendRoleGroupService();
            viewModelBackendUser model = crudService.GetOnly(new viewModelBackendUser { search_account = search_account });
            model.listRoleGroupCodes = model.role_group_codes.Split(',');
            model.listRoleGroup = crudServiceRole.GetList(new viewModelBackendRoleGroup());
            return View("Data", model);
        }
        [HttpPost]
        public ActionResult Edit(viewModelBackendUser model)
        {
            model.saveAction = "Motify";
            #region Check
            StringBuilder strChk = chkData(model);
            if (!string.IsNullOrWhiteSpace(strChk.ToString()))
            {
                this.errMsg = strChk.ToString();
                IBaseCrudService<viewModelBackendRoleGroup> crudServiceRole = new backendRoleGroupService();
                model.listRoleGroupCodes = model.dlRoleCodes;
                model.listRoleGroup = crudServiceRole.GetList(new viewModelBackendRoleGroup());
                return View("Data", model);
            }
            #endregion
            IBaseCrudService<viewModelBackendUser> crudService = new backendUserService();
            #region Password
            if (!string.IsNullOrWhiteSpace(model.inputPassword))
            {
                string _salt = CommonHelpers.GeneratePassword(6);
                SHA256Processor sHA256Processor = new SHA256Processor(_salt);
                model.password = Encoding.UTF8.GetString(sHA256Processor.Encode(Encoding.UTF8.GetBytes(model.inputPassword)));
                model.salt_key = _salt;
            }
            if (!model.is_enable.Value)
            {
                model.disable_user = this.userInfoMdoel.account;
            }
            #endregion
            model.role_group_codes = String.Join(",", model.dlRoleCodes);
            viewModelBackendUser item = crudService.Save(model);
            if (!item.replyResult.Value)
            {
                this.errMsg = item.replyMsg;
                IBaseCrudService<viewModelBackendRoleGroup> crudServiceRole = new backendRoleGroupService();
                model.listRoleGroupCodes = model.dlRoleCodes;
                model.listRoleGroup = crudServiceRole.GetList(new viewModelBackendRoleGroup());
                return View("Data", model);
            }
            else
                return RedirectToAction("Index");
        }
        #endregion
        public StringBuilder chkData(viewModelBackendUser model)
        {
            StringBuilder chk = new StringBuilder();            
            if (string.IsNullOrWhiteSpace(model.account)) { chk.Append("帳號欄位未填!"); return chk; }
            if (string.IsNullOrWhiteSpace(model.name)) { chk.Append("名稱欄位未填!"); return chk; }
            if (string.IsNullOrWhiteSpace(model.inputPassword) && model.saveAction == "Add") { chk.Append("密碼欄位未填!"); return chk; }
            //if (!model.is_enable.HasValue) { chk.Append("帳號啟用欄位未填!"); return chk; }
            if (model.dlRoleCodes.Length == 0) { chk.Append("角色群組碼欄位未填!"); return chk; }           
            return chk;
        }
    }
}