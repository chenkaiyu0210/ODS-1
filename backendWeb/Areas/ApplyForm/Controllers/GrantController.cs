using backendWeb.Controllers;
using backendWeb.Helpers;
using backendWeb.Models.ApiModel;
using backendWeb.Models.ApiModel.responseModel;
using backendWeb.Models.ViewModel;
using backendWeb.Service.InterFace;
using backendWeb.Service.ServiceClass;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;


namespace backendWeb.Areas.ApplyForm
{
    public class GrantController : BaseController
    {
        LogUtil logUtil = new LogUtil();
        // GET: ApplyForm/Grant
        public ActionResult Index()
        {
            IBaseCrudService<viewModelBackendUser> crudService = new backendUserService();

            IList<viewModelBackendUser> list = crudService.GetList(new viewModelBackendUser());
            List<SelectListItem> selectListItems = new SelectList(list, "account", "name").ToList();
            selectListItems.Insert(0, (new SelectListItem { Text = "全部", Value = "" }));

            TempData["UserList"] = selectListItems;
            return View();
        }
        public ActionResult Table()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Table(viewModelReceiveCases model) //int draw, int start, int length
        {
            IBaseCrudService<viewModelReceiveCases> crudService = new receiveCasesService();
            model.search_isAppropriation = true;
            IList<viewModelReceiveCases> list = crudService.GetList(model);
            var returnObj =
                  new
                  {
                      draw = model.draw,
                      recordsTotal = list == null || list.Count == 0 ? 0 : list.Count,
                      recordsFiltered = list == null || list.Count == 0 ? 0 : list.Count,
                      data = list == null ? new List<viewModelReceiveCases>() : list//分頁後的資料 
                  };
            return Json(returnObj);
        }

        public ActionResult Edit(string id)
        {
            viewModelReceiveCases item = new viewModelReceiveCases();
            if (!string.IsNullOrEmpty(id))
            {
                IBaseCrudService<viewModelReceiveCases> crudService = new receiveCasesService();
                item = crudService.GetOnly(new viewModelReceiveCases { search_receive_id = id });
                item.customer_id_number_areacode = item.customer_id_number_areacode.Trim();
            }

            reBindModel(ref item);
            return View(item);
        }
        public ActionResult UudateStatus(string examine_no)
        {
            modelAppropriationQueryRes modelAppropriationQueryRes = new modelAppropriationQueryRes();
            logUtil.OutputLog("查詢撥款狀態", examine_no);
            string returnString = string.Empty;
            string status = string.Empty;

            #region 取得案件狀態
            try
            {
                ReqQCS reqQCS = new ReqQCS
                {
                    dealerNo = "OD01",
                    branchNo = "0001",
                    salesNo = "88021796",
                    examineNo = examine_no,
                    source = "22"
                };
               
                EncryptionProcessor<RijndaelProcessor> encryption = new RijndaelProcessor(this.configSetting.apiSetting.apiKey.aesKey, this.configSetting.apiSetting.apiKey.aesIv, 256, 128, CipherMode.CBC, PaddingMode.PKCS7);
                apiModelEncryption modelEncryption = new apiModelEncryption
                {
                    encryptEnterCase = Convert.ToBase64String(encryption.Encode(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(reqQCS)))),
                    version = "2.0",
                    transactionId = Guid.NewGuid().ToString()
                };
                HttpResponseMessage responseMessage = HttpHelpers.PostHttpClient(modelEncryption, "https://egateway.tac.com.tw/production/api/yrc/agent/QueryAppropriation");
                if (responseMessage.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    string result = responseMessage.Content.ReadAsStringAsync().Result;
                    modelAppropriationQueryRes = JsonConvert.DeserializeObject<modelAppropriationQueryRes>(result);
                    if (modelAppropriationQueryRes != null)
                    {
                        if (modelAppropriationQueryRes.code == "S001")
                            status = modelAppropriationQueryRes.appropriations.First().status;
                        else
                            returnString = modelAppropriationQueryRes.msg;
                    }
                    else
                        returnString = "API回傳NULL";
                }
                else
                    returnString = "API回傳錯誤";
            }
            catch (Exception ex)
            {
                returnString = "查詢撥款狀態異常";
                logUtil.OutputLog("查詢撥款狀態異常", ex.Message);
            }
            #endregion

            if (!string.IsNullOrWhiteSpace(status))
            {
                IBaseCrudService<viewModelReceiveCases> crudService = new receiveCasesService();
                viewModelReceiveCases item = crudService.GetOnly(new viewModelReceiveCases { search_examine_no = examine_no });

                if (status == item.appropriation_status)
                {
                    returnString = "撥款狀態與分期系統一致";
                }
                else
                {
                    item.appropriationDate = modelAppropriationQueryRes.appropriations.First().appropriationDate;
                    item.appropriationAmt = modelAppropriationQueryRes.appropriations.First().appropriationAmt;
                    item.repayKindName = modelAppropriationQueryRes.appropriations.First().repayKindName;
                    item.appropriation_status = modelAppropriationQueryRes.appropriations.First().status;
                    item.appropriation_update_time = DateTime.Now.ToString("yyyyMMddhhmm");

                    viewModelReceiveCases returnValue = crudService.Save(item);

                    if (returnValue.replyResult == null ||
                        !returnValue.replyResult.Value)
                    {
                        logUtil.OutputLog("查詢撥款狀態異常", "儲存更新狀態錯誤");
                        returnString = "儲存更新狀態錯誤";
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(returnString))
                logUtil.OutputLog("查詢案件狀態異常", returnString);

            return Json(returnString, JsonRequestBehavior.DenyGet);
        }
        public void reBindModel(ref viewModelReceiveCases model)
        {
            #region DateTime西元轉民國
            if (model.customer_birthday != null)
            {
                model.customer_birthdayYY = (model.customer_birthday.Year - 1911).ToString();
                model.customer_birthdayMM = model.customer_birthday.Month.ToString();
                model.customer_birthdayDD = model.customer_birthday.Day.ToString();
            }
            if (model.guarantor_birthday.HasValue)
            {
                model.customer_birthdayYY = (model.guarantor_birthday.Value.Year - 1911).ToString();
                model.customer_birthdayMM = model.guarantor_birthday.Value.Month.ToString();
                model.customer_birthdayDD = model.guarantor_birthday.Value.Day.ToString();
            }
            if (model.customer_id_issue_date.HasValue)
            {
                model.customer_id_issue_dateYY = (model.customer_id_issue_date.Value.Year - 1911).ToString();
                model.customer_id_issue_dateMM = model.customer_id_issue_date.Value.Month.ToString();
                model.customer_id_issue_dateDD = model.customer_id_issue_date.Value.Day.ToString();
            }
            #endregion
            IBaseCrudService<viewModelPostfile> postfileService = new postfileService();
            IBaseCrudService<viewModelBackendSetConfig> setConfigService = new backendSetConfigService();
            IBaseCrudService<viewModelBackendBusType> busTypeService = new backendBusTypeService();
            IBaseCrudService<viewModelBackendProduct> productService = new backendProductService();
            IBaseCrudService<viewModelBackendBank> bankService = new backendBankService();
            #region 郵遞地址
            IList<viewModelPostfile> list = postfileService.GetList(new viewModelPostfile());
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
            #endregion
            #region 業務別&促銷專案
            model.busType_list = busTypeService.GetList(new viewModelBackendBusType()).ToList();
            if (!string.IsNullOrEmpty(model.bus_type))
            {
                IBaseCrudService<viewModelBackendPromotion> promotionService = new backendPromotionService();
                model.promotion_list = promotionService.GetList(new viewModelBackendPromotion { search_bus_type = model.bus_type }).ToList();
            }
            else if (model.busType_list != null && model.busType_list.Count > 0 &&
                model.busType_list[0] != null && model.busType_list[0].bus_type != null)
            {
                IBaseCrudService<viewModelBackendPromotion> promotionService = new backendPromotionService();
                model.promotion_list = promotionService.GetList(new viewModelBackendPromotion { search_bus_type = model.busType_list[0].bus_type }).ToList();
            }
            else
                model.promotion_list = new List<viewModelBackendPromotion>();
            #endregion
            #region 參數代碼
            IList<viewModelBackendSetConfig> setConfigList = setConfigService.GetList(new viewModelBackendSetConfig { search_is_enable = true });
            model.edutation_list = setConfigList.Where(o => o.config_tag == "edutation").ToList();
            model.idIssue_list = setConfigList.Where(o => o.config_tag == "idIssue").ToList();
            model.idRcv_list = setConfigList.Where(o => o.config_tag == "idRcv").ToList();
            model.dwellStatus_list = setConfigList.Where(o => o.config_tag == "dwellStatus").ToList();
            #endregion
            #region 商品品牌&類別
            model.productBrand_list = productService.GetList(new viewModelBackendProduct { search_bus_type = model.bus_type }).GroupBy(o => new { o.product_brand, o.product_brand_name }).Select(o => new viewModelBackendProduct { product_brand = o.Key.product_brand, product_brand_name = o.Key.product_brand_name }).ToList();
            if (!string.IsNullOrEmpty(model.product_brand))
            {
                model.productKind_list = productService.GetList(new viewModelBackendProduct { search_bus_type = model.bus_type, search_product_brand = model.product_brand }).ToList();
            }
            else
                model.productKind_list = new List<viewModelBackendProduct>();
            #endregion
            #region 銀行代碼
            model.bank_list = bankService.GetList(new viewModelBackendBank()).ToList();
            if (!string.IsNullOrEmpty(model.bank_code))
            {
                IBaseCrudService<viewModelBackendBankDetail> bankDetailService = new backendBankDetailService();
                model.bankDetail_list = bankDetailService.GetList(new viewModelBackendBankDetail { search_bank_code = model.bank_code }).ToList();
            }
            else
                model.bankDetail_list = new List<viewModelBackendBankDetail>();

            if (!string.IsNullOrEmpty(model.bank_code))
                model.bank_code = model.bank_code.Split('-')[0];
            if (!string.IsNullOrEmpty(model.bank_detail_code))
                model.bank_detail_code = model.bank_detail_code.Split('-')[0];
            #endregion
            #region 是否有保人
            if (!string.IsNullOrEmpty(model.guarantor_idcard_no))
            {
                model.guarantor_option = "1";
            }
            #endregion
            if (!string.IsNullOrEmpty(model.num) && model.paymentInput.Count == 0)
            {
                model.paymentInput = new List<payment>();
                string[] numArr = model.num.Split(';');
                string[] numAmountArr = model.num_amount.Split(';');
                for (int i = 0; i < numArr.Length; i++)
                {
                    model.paymentInput.Add(new payment { num = numArr[i], num_amount = numAmountArr[i] });
                }
            }
        }
    }
}