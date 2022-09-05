using backendWeb.Areas.Yrc.Controllers;
using backendWeb.Controllers;
using backendWeb.Helpers;
using backendWeb.Models.ApiModel;
using backendWeb.Models.ApiModel.responseModel;
using backendWeb.Models.ViewModel;
using backendWeb.Service.InterFace;
using backendWeb.Service.ServiceClass;
using GateWay.Models;
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
    public class ResendController : BaseController
    {
        LogUtil logUtil = new LogUtil();
        // GET: ApplyForm/Resend
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
            model.isResend = true;
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
        public ActionResult History(string examine_no)
        {
            //IBaseCrudService<modelAppropriationNotifyReq> appropriationNotify = new notifyAppropriationService();
            //List<modelAppropriationNotifyReq> listAppropriationNotify = appropriationNotify.GetList(new modelAppropriationNotifyReq() { examineNo = examine_no }).ToList();
            IBaseCrudService<modelNotifyStatusReq> notifyStatus = new notifyCaseStatusService();
            List<modelNotifyStatusReq> listNotifyStatus = notifyStatus.GetList(new modelNotifyStatusReq() { examineNo = examine_no }).ToList();

            //List<RespQCS> respQCs = new List<RespQCS>();

            //foreach (modelAppropriationNotifyReq a in listAppropriationNotify)
            //{
            //    respQCs.Add(JsonConvert.DeserializeObject<RespQCS>(a.QueryCaseStatus));
            //}
            //listNotifyStatus = listNotifyStatus.OrderByDescending(x => x.ModifyTime).ToList();

            //foreach (modelNotifyStatusReq s in listNotifyStatus)
            //{
            //    respQCs.Add(JsonConvert.DeserializeObject<RespQCS>(s.QueryCaseStatus));
            //}

            return View(listNotifyStatus);
        }

        /// <summary>
        /// 申覆 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult RequestforExam(string id)
        {
            viewModelRequestforExam item = new viewModelRequestforExam();

            if (!string.IsNullOrEmpty(id))
            {
                item.receive_id = id;
                IBaseCrudService<viewModelReceiveCases> crudService = new receiveCasesService();
                item.examine_no = crudService.GetOnly(new viewModelReceiveCases { search_receive_id = id }).examine_no;
            }
            return View(item);
        }

        [HttpPost]
        public ActionResult RequestforExam(viewModelRequestforExam item)
        {
            try
            {
                #region 初始值
                ReqRE reqRE = new ReqRE
                {
                    dealerNo = "OD01",
                    branchNo = "0001",
                    salesNo = "88021796",
                    examineNo = item.examine_no,
                    source = "22",
                    comment = item.comment,
                    forceTryForExam = item.forceTryForExam ? "Y" : null
                };
                #endregion

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
                reqRE.attachmentFile = new List<ReqREFile> {
                    new ReqREFile {
                        file_index = "0",
                        content_type = "zip",
                        file_body_encode = Convert.ToBase64String(compress.Compression(dicFiles))
                    }
                };
                #endregion

                #region 呼叫API
                EncryptionProcessor<RijndaelProcessor> encryption = new RijndaelProcessor(this.configSetting.apiSetting.apiKey.aesKey, this.configSetting.apiSetting.apiKey.aesIv, 256, 128, CipherMode.CBC, PaddingMode.PKCS7);
                apiModelEncryption modelEncryption = new apiModelEncryption
                {
                    encryptEnterCase = Convert.ToBase64String(encryption.Encode(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(reqRE)))),
                    version = "2.0",
                    transactionId = Guid.NewGuid().ToString()
                };
                HttpResponseMessage responseMessage = HttpHelpers.PostHttpClient(modelEncryption, ConfigurationManager.AppSettings["API"].ToString() + "RequestforExam");
                #endregion

                if (responseMessage.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    string result = responseMessage.Content.ReadAsStringAsync().Result;
                    RespRE respRE = JsonConvert.DeserializeObject<RespRE>(result);
                    if (respRE != null)
                    {
                        if (respRE.code == "S001")
                            ViewData["successMsg"] = "成功, 申覆次數" + respRE.negotiateTimes;
                        else
                            ViewData["errMsg"] = respRE.msg;
                    }
                    else
                        ViewData["errMsg"] = result;
                }
            }
            catch (Exception ex)
            {
                ViewData["errMsg"] = ex.Message;
            }

            return View(item);
        }

        /// <summary>
        /// 補件
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult RequestSupplement(string id)
        {
            viewModelRequestSupplement item = new viewModelRequestSupplement();
            if (!string.IsNullOrEmpty(id))
            {
                item.receive_id = id;
                IBaseCrudService<viewModelReceiveCases> crudService = new receiveCasesService();
                item.examine_no = crudService.GetOnly(new viewModelReceiveCases { search_receive_id = id }).examine_no;
            }
            return View(item);
        }
        [HttpPost]
        public ActionResult RequestSupplement(viewModelRequestSupplement item)
        {
            try
            {
                #region 初始值
                ReqRS reqRE = new ReqRS
                {
                    dealerNo = "OD01",
                    branchNo = "0001",
                    salesNo = "88021796",
                    examineNo = item.examine_no,
                    source = "22"
                };
                #endregion

                #region 補件說明陣列
                List<ReqRSItem> reqRSItems = new List<ReqRSItem>();
                foreach (string s in item.supplement)
                {
                    reqRSItems.Add(new ReqRSItem() { item = "00", comment = s });
                }
                reqRE.supplement = reqRSItems;
                #endregion

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
                reqRE.attachmentFile = new List<ReqRSFile> {
                    new ReqRSFile {
                        file_index = "0",
                        content_type = "zip",
                        file_body_encode = Convert.ToBase64String(compress.Compression(dicFiles))
                    }
                };
                #endregion

                #region 呼叫API
                EncryptionProcessor<RijndaelProcessor> encryption = new RijndaelProcessor(this.configSetting.apiSetting.apiKey.aesKey, this.configSetting.apiSetting.apiKey.aesIv, 256, 128, CipherMode.CBC, PaddingMode.PKCS7);
                apiModelEncryption modelEncryption = new apiModelEncryption
                {
                    encryptEnterCase = Convert.ToBase64String(encryption.Encode(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(reqRE)))),
                    version = "2.0",
                    transactionId = Guid.NewGuid().ToString()
                };
                HttpResponseMessage responseMessage = HttpHelpers.PostHttpClient(modelEncryption, ConfigurationManager.AppSettings["API"].ToString() + "RequestSupplement");
                #endregion

                if (responseMessage.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    string result = responseMessage.Content.ReadAsStringAsync().Result;
                    RespRS respRS = JsonConvert.DeserializeObject<RespRS>(result);
                    if (respRS != null)
                    {
                        if (respRS.code == "S001")
                            ViewData["successMsg"] = respRS.msg;
                        else
                            ViewData["errMsg"] = respRS.msg;

                    }
                    else
                        ViewData["errMsg"] = result;
                }
            }
            catch (Exception ex)
            {
                ViewData["errMsg"] = ex.Message;
            }

            return View(item);
        }

        /// <summary>
        /// 重照
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult ReCallout(string id)
        {
            viewModelReCallout item = new viewModelReCallout();
            if (!string.IsNullOrEmpty(id))
            {
                item.receive_id = id;
                IBaseCrudService<viewModelReceiveCases> crudService = new receiveCasesService();
                viewModelReceiveCases viewModelReceiveCases = crudService.GetOnly(new viewModelReceiveCases { search_receive_id = id });
                item.examine_no = viewModelReceiveCases.examine_no;
                item.tel = viewModelReceiveCases.customer_mobile_phone;
            }
            return View(item);
        }
        [HttpPost]
        public ActionResult ReCallout(viewModelReCallout item)
        {
            #region 初始值
            ReqRCO ReqRCO = new ReqRCO
            {
                dealerNo = "OD01",
                branchNo = "0001",
                salesNo = "88021796",
                examineNo = item.examine_no,
                source = "22",
                calloutDate = item.calloutDate.ToString("yyyyMMddHHmmss"),
                tel = item.tel,
                descript = item.descript,
                nowCallout = item.nowCallout ? "Y" : "N"
            };
            #endregion

            #region 呼叫API
            EncryptionProcessor<RijndaelProcessor> encryption = new RijndaelProcessor(this.configSetting.apiSetting.apiKey.aesKey, this.configSetting.apiSetting.apiKey.aesIv, 256, 128, CipherMode.CBC, PaddingMode.PKCS7);
            apiModelEncryption modelEncryption = new apiModelEncryption
            {
                encryptEnterCase = Convert.ToBase64String(encryption.Encode(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(ReqRCO)))),
                version = "2.0",
                transactionId = Guid.NewGuid().ToString()
            };
            HttpResponseMessage responseMessage = HttpHelpers.PostHttpClient(modelEncryption, ConfigurationManager.AppSettings["API"].ToString() + "reCallout");
            #endregion
            if (responseMessage.StatusCode == System.Net.HttpStatusCode.OK)
            {
                string result = responseMessage.Content.ReadAsStringAsync().Result;
                RespRS respRS = JsonConvert.DeserializeObject<RespRS>(result);
                if (respRS != null)
                {
                    if (respRS.code == "S001")
                        ViewData["successMsg"] = respRS.msg;
                    else
                        ViewData["errMsg"] = respRS.msg;
                }
                else
                    ViewData["errMsg"] = result;
            }

            return View(item);
        }
    }
}