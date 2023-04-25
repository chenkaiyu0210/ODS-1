using backendWeb.Areas.Yrc.Controllers;
using backendWeb.Controllers;
using backendWeb.Helpers;
using backendWeb.Models.ApiModel;
using backendWeb.Models.ViewModel;
using backendWeb.Service.InterFace;
using backendWeb.Service.ServiceClass;
using GateWay.Models;
using Newtonsoft.Json;
using PrinterKit;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
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
            if (!this.userInfoMdoel.role_group_codes.Contains("system"))
                model.receive_staff = this.userInfoMdoel.account;

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
                    dealerNo = "OO02",
                    branchNo = "0001",
                    salesNo = "80659759",
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

            #region 郵遞地址
            IBaseCrudService<viewModelPostfile> postfileService = new postfileService();
            IList<viewModelPostfile> list = postfileService.GetList(new viewModelPostfile());
            item.city_list = (from d in list
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
                    dealerNo = "OO02",
                    branchNo = "0001",
                    salesNo = "80659759",
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

                Dictionary<string, string> dct = new Dictionary<string, string>();
                dct.Add("examineNo", item.examine_no);
                dct.Add("guarantor_name", item.guarantor_name);
                dct.Add("guarantor_relation", item.guarantor_relation);
                dct.Add("guarantor_idcard_no", item.guarantor_idcard_no);
                dct.Add("guarantor_birthday", item.guarantor_birthdayYY + item.guarantor_birthdayMM + item.guarantor_birthdayDD);
                dct.Add("guarantor_mobile_phone", item.guarantor_mobile_phone);
                dct.Add("guarantor_resident_tel_num", item.guarantor_mobile_phone + item.guarantor_resident_tel_num);
                dct.Add("guarantor_address", item.guarantor_postalcode + " " + item.guarantor_addcity + item.guarantor_addregion + item.guarantor_addregion + item.guarantor_address);
                dct.Add("guarantor_company_name", item.guarantor_company_name);
                dct.Add("guarantor_job_type", item.guarantor_job_type);
                dct.Add("guarantor_company_tel_num", item.guarantor_company_tel_code + " " + item.guarantor_company_tel_num + " " + item.guarantor_company_tel_ext);
                byte[] member_case = printpdf(dct, "guarantor.pdf");

                dicFiles.Add("guarantor_file.pdf", new MemoryStream(member_case));

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

            #region 郵遞地址
            IBaseCrudService<viewModelPostfile> postfileService = new postfileService();
            IList<viewModelPostfile> list = postfileService.GetList(new viewModelPostfile());
            item.city_list = (from d in list
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

            return View(item);
        }

        [HttpPost]
        public JsonResult GetGuarantor(string guarantor_idcard_no)
        {
            viewModelReceiveCases viewModelReceiveCases = new viewModelReceiveCases();
            IBaseCrudService<viewModelReceiveCases> crudService = new receiveCasesService();
            viewModelReceiveCases.guarantor_idcard_no = guarantor_idcard_no;
            List<viewModelReceiveCases> list = crudService.GetList(viewModelReceiveCases)
                 .OrderByDescending(x => x.receive_date).ToList();

            viewModelReceiveCases g = list.FirstOrDefault();

            viewModelReceiveCases returnValue = new viewModelReceiveCases();

            if (g != null)
            {
                returnValue.guarantor_name = g.guarantor_name;
                returnValue.guarantor_relation = g.guarantor_relation;
                returnValue.guarantor_idcard_no = g.guarantor_idcard_no;
                returnValue.guarantor_birthdayYY = g.guarantor_birthdayYY;
                returnValue.guarantor_birthdayMM = g.guarantor_birthdayMM;
                returnValue.guarantor_birthdayDD = g.guarantor_birthdayDD;
                returnValue.guarantor_mobile_phone = g.guarantor_mobile_phone;
                returnValue.guarantor_resident_tel_code = g.guarantor_resident_tel_code;
                returnValue.guarantor_resident_tel_num = g.guarantor_resident_tel_num;
                returnValue.guarantor_addcity = g.guarantor_addcity;
                returnValue.guarantor_addregion = g.guarantor_addregion;
                returnValue.guarantor_address = g.guarantor_address;
                returnValue.guarantor_postalcode = g.guarantor_postalcode;
                returnValue.guarantor_company_name = g.guarantor_company_name;
                returnValue.guarantor_job_type = g.guarantor_job_type;
                returnValue.guarantor_company_tel_code = g.guarantor_company_tel_code;
                returnValue.guarantor_company_tel_num = g.guarantor_company_tel_num;
                returnValue.guarantor_company_tel_ext = g.guarantor_company_tel_ext;
            }

            return g != null ? Json(returnValue) : Json(null);
        }

        private byte[] printpdf(Dictionary<string, string> dct, string docx_filename)
        {
            List<PdfItem> pdfI = new List<PdfItem>();
            foreach (var d in dct)
            {
                pdfI.Add(new PdfItem() { Key = d.Key, Value = d.Value, Type = PdfItemType.Text });
            }
            PdfTemplate template = new PdfTemplate()
            {
                Pages = new List<PdfPage>() { new PdfPage() { Items = pdfI } },
                FontPaths = new string[] { Server.MapPath("~/Service/PrinterKit/FileTemplate/msjh.ttc,1") },
                FileName = docx_filename,
                SourceFilePath = Server.MapPath("~/Service/PrinterKit/FileTemplate/"),
                TargetFilePath = Server.MapPath("~/Service/PrinterKit/FileConvertTemp/")
            };
            byte[] pdf_byte = template.TemplateToPdf();


            //System.IO.File.Delete(tempFilePath);
            return pdf_byte;
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
                dealerNo = "OO02",
                branchNo = "0001",
                salesNo = "80659759",
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