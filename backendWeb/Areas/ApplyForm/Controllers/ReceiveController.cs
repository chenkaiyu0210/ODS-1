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
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;


namespace backendWeb.Areas.ApplyForm.Controllers
{
    public class ReceiveController : BaseController
    {
        LogUtil logUtil = new LogUtil();

        // GET: ApplyForm/Receive
        public ActionResult Index()
        {
            IBaseCrudService<viewModelBackendUser> crudService = new backendUserService();


            IList<viewModelBackendUser> list = crudService.GetList(new viewModelBackendUser());

            if (!this.userInfoMdoel.role_group_codes.Contains("system"))
                list = list.Where(x => x.account == this.userInfoMdoel.account).ToList();

            List<SelectListItem> selectListItems = new SelectList(list, "account", "name").ToList();

            if (this.userInfoMdoel.role_group_codes.Contains("system"))
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
            if (!this.userInfoMdoel.role_group_codes.Contains("system"))
                model.receive_staff = this.userInfoMdoel.account;

            IBaseCrudService<viewModelReceiveCases> crudService = new receiveCasesService();
            IList<viewModelReceiveCases> list = crudService.GetList(model).OrderByDescending(x => x.receive_date).ToList();
            var returnObj =
                  new
                  {
                      //draw = model.draw,
                      //recordsTotal = list == null || list.Count == 0 ? 0 : list.Count,
                      //recordsFiltered = list == null || list.Count == 0 ? 0 : list.Count,
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
                //item.customer_id_number_areacode = item.customer_id_number_areacode.Trim();
            }

            reBindModel(ref item);
            return View(item);
        }

        public ActionResult UudateStatus(string examine_no)
        {
            logUtil.OutputLog("查詢案件狀態", examine_no);
            string returnString = string.Empty;
            string status = string.Empty;

            #region 取得案件狀態
            try
            {
                ReqQCS reqQCS = new ReqQCS
                {
                    dealerNo = "OO02",
                    branchNo = null,
                    salesNo = null,
                    examineNo = examine_no,
                    source = "22"
                };
                RespQCS respQCS = new RespQCS();


                EncryptionProcessor<RijndaelProcessor> encryption = new RijndaelProcessor(this.configSetting.apiSetting.apiKey.aesKey, this.configSetting.apiSetting.apiKey.aesIv, 256, 128, CipherMode.CBC, PaddingMode.PKCS7);
                apiModelEncryption modelEncryption = new apiModelEncryption
                {
                    encryptEnterCase = Convert.ToBase64String(encryption.Encode(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(reqQCS)))),
                    version = "2.0",
                    transactionId = Guid.NewGuid().ToString()
                };
                HttpResponseMessage responseMessage = HttpHelpers.PostHttpClient(modelEncryption, ConfigurationManager.AppSettings["API"].ToString() + "QueryCaseStatus");
                if (responseMessage.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    string result = responseMessage.Content.ReadAsStringAsync().Result;
                    respQCS = JsonConvert.DeserializeObject<RespQCS>(result);
                    if (respQCS != null)
                    {
                        if (respQCS.code == "S001")
                            status = respQCS.examStatusExplain;
                        else
                            returnString = respQCS.msg;
                    }
                    else
                        returnString = "API回傳NULL";
                }
                else
                    returnString = "API回傳錯誤";
            }
            catch (Exception ex)
            {
                returnString = "查詢案件狀態異常";
                logUtil.OutputLog("查詢案件狀態異常", ex.Message);
            }
            #endregion

            if (!string.IsNullOrWhiteSpace(status))
            {
                IBaseCrudService<viewModelReceiveCases> crudService = new receiveCasesService();
                viewModelReceiveCases item = crudService.GetOnly(new viewModelReceiveCases { search_examine_no = examine_no });

                if (status == item.receive_status)
                {
                    returnString = "案件狀態與分期系統一致";
                }
                else
                {
                    item.receive_status = status;
                    item.receive_status_update_time = DateTime.Now.ToString("yyyyMMddhhmm");

                    viewModelReceiveCases returnValue = crudService.Save(item);

                    if (returnValue.replyResult == null ||
                        !returnValue.replyResult.Value)
                    {
                        logUtil.OutputLog("查詢案件狀態異常", "儲存更新狀態錯誤");
                        returnString = "儲存更新狀態錯誤";
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(returnString))
                logUtil.OutputLog("查詢案件狀態異常", returnString);

            return Json(returnString, JsonRequestBehavior.DenyGet);
        }

        public ActionResult Create()
        {
            viewModelReceiveCases item = new viewModelReceiveCases();
            reBindModel(ref item);

            return View("Edit", item);
        }

        [HttpPost]
        public ActionResult Edit(string action, viewModelReceiveCases model)
        {
            IBaseCrudService<viewModelReceiveCases> crudService = new receiveCasesService();

            try
            {
                #region 合併資料
                int.TryParse(model.customer_birthdayYY, out int yy);
                int.TryParse(model.customer_birthdayMM, out int mm);
                int.TryParse(model.customer_birthdayDD, out int dd);
                if (!DateTime.TryParse((yy + 1911).ToString() + "/" + mm.ToString("00") + "/" + dd.ToString("00"), out DateTime BD))
                {
                    ViewData["errMsg"] = "出生日期格式錯誤!";
                    reBindModel(ref model);
                    return View(model);
                }
                model.customer_birthday = BD;
                if (!string.IsNullOrWhiteSpace(model.customer_id_issue_dateYY)
                    && !string.IsNullOrWhiteSpace(model.customer_id_issue_dateMM)
                    && !string.IsNullOrWhiteSpace(model.customer_id_issue_dateDD))
                {
                    int.TryParse(model.customer_id_issue_dateYY, out int isy);
                    int.TryParse(model.customer_id_issue_dateMM, out int ism);
                    int.TryParse(model.customer_id_issue_dateDD, out int isd);
                    if (!DateTime.TryParse((isy + 1911).ToString() + "/" + ism.ToString("00") + "/" + isd.ToString("00"), out DateTime ISD))
                    {
                        ViewData["errMsg"] = "初補換日期格式錯誤!";
                        reBindModel(ref model);
                        return View(model);
                    }
                    model.customer_id_issue_date = ISD;
                }
                if (!string.IsNullOrWhiteSpace(model.guarantor_birthdayYY)
                    && !string.IsNullOrWhiteSpace(model.guarantor_birthdayMM)
                    && !string.IsNullOrWhiteSpace(model.guarantor_birthdayDD))
                {
                    int.TryParse(model.guarantor_birthdayYY, out int gy);
                    int.TryParse(model.guarantor_birthdayMM, out int gm);
                    int.TryParse(model.guarantor_birthdayDD, out int gd);
                    if (!DateTime.TryParse((gy + 1911).ToString() + "/" + gm.ToString("00") + "/" + gd.ToString("00"), out DateTime GD))
                    {
                        ViewData["errMsg"] = "保人出生日期格式錯誤!";
                        reBindModel(ref model);
                        return View(model);
                    }
                    model.guarantor_birthday = GD;
                }
                if (model.customer_mail_identical.HasValue)
                {
                    model.customer_mail_postalcode = model.customer_resident_postalcode;
                    model.customer_mail_addcity = model.customer_resident_addcity;
                    model.customer_mail_addregion = model.customer_resident_addregion;
                    model.customer_mail_address = model.customer_resident_address;
                }
                foreach (payment p in model.paymentInput)
                {
                    if (model.paymentInput.IndexOf(p) + 1 == model.paymentInput.Count)
                    {
                        model.num += p.num;
                        model.num_amount += p.num_amount;
                    }
                    else
                    {
                        model.num += p.num + ";";
                        model.num_amount += p.num_amount + ";";
                    }
                }
                if (!string.IsNullOrEmpty(model.bank_code))
                    model.bank_code = model.bank_code.Split('-')[0];
                if (!string.IsNullOrEmpty(model.bank_detail_code))
                    model.bank_detail_code = model.bank_detail_code.Split('-')[0];
                #endregion
                #region 儲存資料
                model.receive_staff = this.userInfoMdoel.account;

                if (model.receive_id == null || model.receive_id == Guid.Empty)
                {
                    model.saveAction = "Create";
                    model.receive_id = Guid.NewGuid();
                }
                else
                    model.saveAction = "Modify";

                model.receive_status = "案件存檔";
                model.error_message = "";
                model.receive_date = this.TaiwanDateTime;
                viewModelReceiveCases returnValue = crudService.Save(model);
                #endregion
                if (action == "temp") //暫存模式不需要打API
                {
                    if (returnValue.replyResult != null &&
                        returnValue.replyResult.Value)
                        ViewData["successMsg"] = "存檔完成";
                    else
                    {
                        ViewData["errMsg"] = returnValue.replyMsg;
                        logUtil.OutputLog("進件存檔失敗", returnValue.replyMsg);
                    }
                }
                else
                {
                    #region 呼叫YRC
                    apiModelReceive apiModel = BindApiModel(model);
                    EncryptionProcessor<RijndaelProcessor> encryption = new RijndaelProcessor(this.configSetting.apiSetting.apiKey.aesKey, this.configSetting.apiSetting.apiKey.aesIv, 256, 128, CipherMode.CBC, PaddingMode.PKCS7);
                    apiModelEncryption modelEncryption = new apiModelEncryption
                    {
                        encryptEnterCase = Convert.ToBase64String(encryption.Encode(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(apiModel)))),
                        version = "2.0",
                        transactionId = Guid.NewGuid().ToString()
                    };
                    HttpResponseMessage responseMessage = HttpHelpers.PostHttpClient(modelEncryption, this.configSetting.apiSetting.apiUrls.Where(o => o.func == "receive").FirstOrDefault().url);
                    if (responseMessage.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        string result = responseMessage.Content.ReadAsStringAsync().Result;
                        apiResponseReceive apiResponse = JsonConvert.DeserializeObject<apiResponseReceive>(result);
                        if (apiResponse != null)
                        {
                            if (apiResponse.code == "1000" || apiResponse.code == "1001")
                            {
                                model.saveAction = "Modify";
                                model.receive_status = "案件送出";
                                model.error_message = "";
                                model.examine_no = apiResponse.examineNo;
                                crudService.Save(model);
                                ViewData["successMsg"] = "案件送出完成";
                            }
                            else
                            {
                                model.saveAction = "Modify";
                                model.receive_status = "案件送出失敗";
                                model.error_message = apiResponse.msg;
                                crudService.Save(model);
                                ViewData["errMsg"] = apiResponse.msg;
                                logUtil.OutputLog("案件送出失敗", apiResponse.msg);
                            }
                        }
                        else
                        {
                            model.saveAction = "Modify";
                            model.receive_status = "案件送出失敗";
                            model.error_message = "案件送出失敗 API回傳NULL";
                            crudService.Save(model);
                            ViewData["errMsg"] = result;
                            logUtil.OutputLog("案件送出失敗", "API回傳NULL");
                        }
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                model.saveAction = "Modify";
                model.receive_status = "案件送出異常";
                model.error_message = "案件送出異常";
                crudService.Save(model);
                ViewData["errMsg"] = ex.Message;
                logUtil.OutputLog("案件送出異常", ex.Message);
            }
            finally
            {
                reBindModel(ref model);
            }
            return View(model);
        }
        [HttpPost]
        public ActionResult Delete(string receive_id)
        {
            IBaseCrudService<viewModelReceiveCases> crudService = new receiveCasesService();
            viewModelReceiveCases item = new viewModelReceiveCases();
            item = crudService.GetOnly(new viewModelReceiveCases { search_receive_id = receive_id });
            item.is_delete = true;

            viewModelReceiveCases returnValue = crudService.Save(item);

            if (returnValue.replyResult != null &&
                returnValue.replyResult.Value)
                ViewData["successMsg"] = "存檔完成";
            else
                ViewData["errMsg"] = returnValue.replyMsg;

            return RedirectToAction("index", "Receive");
        }
        [HttpPost]
        public JsonResult searchPostfileCode(string query_type, string city_name, string town_name)
        {
            viewModelReceiveCases model = new viewModelReceiveCases();
            IBaseCrudService<viewModelPostfile> crudService = new postfileService();
            if (query_type == "town")
            {
                IList<viewModelPostfile> list = crudService.GetList(new viewModelPostfile { search_city_name = city_name });
                model.town_list = list.Select(o => new viewModelPostfile() { town_name = o.town_name }).ToList();
                return Json(model);
            }
            else
            {
                viewModelPostfile model_ics = crudService.GetOnly(new viewModelPostfile { search_city_name = city_name, search_town_name = town_name });
                return Json(model_ics);
            }
        }
        [HttpPost]
        public JsonResult searchProductKind(string query_type, string search_bus_type, string search_product_brand)
        {
            IBaseCrudService<viewModelBackendProduct> productService = new backendProductService();
            if (query_type == "productBrand")
            {
                IList<viewModelBackendProduct> list = productService.GetList(new viewModelBackendProduct { search_bus_type = search_bus_type }).GroupBy(o => new { o.product_brand, o.product_brand_name }).Select(o => new viewModelBackendProduct { product_brand = o.Key.product_brand, product_brand_name = o.Key.product_brand_name }).ToList();
                if (list.Count > 0)
                    return Json(list);
                else
                    return Json(null);
            }
            else
            {
                IList<viewModelBackendProduct> list = productService.GetList(new viewModelBackendProduct { search_bus_type = search_bus_type, search_product_brand = search_product_brand });
                if (list.Count > 0)
                    return Json(list);
                else
                    return Json(null);
            }
        }
        [HttpPost]
        public JsonResult searchBankDetailCode(string search_bank_code)
        {
            IBaseCrudService<viewModelBackendBankDetail> bankService = new backendBankDetailService();
            IList<viewModelBackendBankDetail> list = bankService.GetList(new viewModelBackendBankDetail { search_bank_code = search_bank_code });
            if (list.Count > 0)
                return Json(list);
            else
                return Json(null);
        }
        [HttpPost]
        public JsonResult searchPromotion(string search_bus_type)
        {

            IBaseCrudService<viewModelBackendPromotion> promotionService = new backendPromotionService();
            IList<viewModelBackendPromotion> list = promotionService.GetList(new viewModelBackendPromotion { search_bus_type = search_bus_type });
            if (list.Count > 0)
                return Json(list);
            else
                return Json(null);
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
        public apiModelReceive BindApiModel(viewModelReceiveCases item)
        {
            apiModelReceive apiModel = CommonHelpers.Migration<viewModelReceiveCases, apiModelReceive>(item);
            apiModel.customer_birthday = item.customer_birthday;
            apiModel.customer_check_identical = item.customer_mail_identical.HasValue ? item.customer_mail_identical.Value ? "Y" : "N" : "N";
            apiModel.payee_account_name = item.customer_name;
            apiModel.payee_account_idno = item.customer_idcard_no;
            apiModel.payee_bank_code = item.bank_code;
            apiModel.payee_bank_detail_code = item.bank_detail_code;
            apiModel.payee_account_num = item.account_num;
            apiModel.promotion_no = item.promotion;

            apiModel.periods_num = item.num;
            apiModel.payment = item.num_amount;
            apiModel.payment_mode = item.payment_mode.ToString();

            int staging_total_price = 0;
            foreach (payment p in item.paymentInput)
            {
                int.TryParse(p.num, out int _num);
                int.TryParse(p.num_amount, out int _numAmount);
                staging_total_price += _num * _numAmount;
            }
            apiModel.staging_amount = item.staging_amount;
            apiModel.staging_total_price = staging_total_price;
            apiModel.product_category_id = item.product_brand;
            apiModel.product_id = item.product_kind;
            apiModel.dealer_no = "OO02";
            apiModel.dealer_name = "OOS中";
            apiModel.dealer_id_no = "80659759";
            apiModel.dealer_branch_no = "0001";
            apiModel.dealer_branch_name = "陳家蓁";
            //apiModel.dealer_tel = "OD01";
            //apiModel.contact_id_no = "OD01";
            apiModel.contact_name = "陳家蓁";
            apiModel.contact_phone = "0903113735";
            apiModel.dealer_note = item.note_remark;

            #region 壓縮檔案
            CompressHelpers compress = new DotNetZipHelpers();
            Dictionary<string, System.IO.Stream> dicFiles = new Dictionary<string, System.IO.Stream>();
            foreach (FileUpload fu in item.FileUploads)
            {
                if (fu.File != null)
                {
                    if (fu.File.ContentLength > 0)
                    {
                        dicFiles.Add(fu.File.FileName, fu.File.InputStream);
                    }
                }
            }
            apiModel.Attachmentfile = new List<attachmentfileYrc> {
                new attachmentfileYrc { file_index = "0",
                                        content_type = "zip",
                                        file_body_encode = Convert.ToBase64String(compress.Compression(dicFiles))
                                    } };
            #endregion


            return apiModel;
        }
    }
}