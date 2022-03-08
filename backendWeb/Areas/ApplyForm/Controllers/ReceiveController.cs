using backendWeb.Controllers;
using backendWeb.Models.ViewModel;
using backendWeb.Service.InterFace;
using backendWeb.Service.ServiceClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace backendWeb.Areas.ApplyForm.Controllers
{
    public class ReceiveController : BaseController
    {
        // GET: ApplyForm/Receive
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Table()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Table(int draw, int start, int length)
        {
            IBaseCrudService<viewModelReceiveCases> crudService = new receiveCasesService();
            IList<viewModelReceiveCases> list = crudService.GetList(new viewModelReceiveCases { draw = draw, start = start, length = length });
            var returnObj =
                  new
                  {
                      draw = draw,
                      recordsTotal = 4,
                      recordsFiltered = 4,
                      data = list == null ? new List<viewModelReceiveCases>() : list//分頁後的資料 
                  };
            return Json(returnObj);
        }
        public ActionResult Edit(string id)
        {
            IBaseCrudService<viewModelReceiveCases> crudService = new receiveCasesService();
            viewModelReceiveCases item = crudService.GetOnly(new viewModelReceiveCases { search_receive_id = id });
            reBindModel(ref item);
            return View(item);
        }

        public void reBindModel(ref viewModelReceiveCases model)
        {
            IBaseCrudService<viewModelPostfile> crudService = new postfileService();
            IList<viewModelPostfile> list = crudService.GetList(new viewModelPostfile());
            model.city_list = (from d in list
                               orderby d.zipcode
                               select new viewModelPostfile
                               {
                                   zipcode = d.zipcode.Substring(0, 1),
                                   city_name = d.city_name,
                               }).GroupBy(o => new
                               {
                                   o.zipcode,
                                   o.city_name
                               }).Select(o => new viewModelPostfile { zipcode = o.Key.zipcode, city_name = o.Key.city_name }).OrderBy(o => o.zipcode).ToList();
        }
    }
}