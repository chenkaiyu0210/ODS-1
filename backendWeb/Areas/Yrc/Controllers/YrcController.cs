using backendWeb.Controllers;
using backendWeb.Helpers;
using backendWeb.Models.ApiModel;
using backendWeb.Models.ViewModel;
using backendWeb.Service.InterFace;
using backendWeb.Service.ServiceClass;
using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web.Http;

namespace backendWeb.Areas.Yrc.Controllers
{
    public class YrcController : BaseApiController
    {
        LogUtil logUtil = new LogUtil();
        /// <summary>
        /// 案件狀態通知
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, Route("api/Yrc/NotifyCaseStatus")]
        public object NotifyCaseStatus(modelNotifyStatusReq model)
        {
            Response response = new Response();
            try
            {
                #region 儲存
                IBaseCrudService<modelNotifyStatusReq> notifyStatusService = new notifyCaseStatusService();
                RespQCS respQCS = QueryCaseStatus(model.examineNo);
                model.QueryCaseStatus = $"{JsonConvert.SerializeObject(respQCS)}";
                notifyStatusService.Save(model);
                #endregion

                response.code = "S001";
                response.msg = "成功";
                logUtil.OutputLog("API NotifyCaseStatus Log", $"{JsonConvert.SerializeObject(model)}");

                IBaseCrudService<viewModelReceiveCases> crudService = new receiveCasesService();

                viewModelReceiveCases item = crudService.GetOnly(new viewModelReceiveCases { search_examine_no = model.examineNo });

                if (item != null)
                {
                    item.receive_status = model.examStatusExplain;
                    item.receive_status_update_time = model.ModifyTime;
                    item.saveAction = "Modify";

                    viewModelReceiveCases returnValue = crudService.Save(item);

                    if (returnValue.replyResult == null ||
                        !returnValue.replyResult.Value)
                    {
                        logUtil.OutputLog("API NotifyCaseStatus 錯誤", "儲存更新狀態錯誤");
                        response.code = "F001";
                        response.msg = "儲存更新狀態錯誤";
                    }
                }
                else
                {
                    logUtil.OutputLog("API NotifyCaseStatus 錯誤", "查無相對應審編的專案");
                    response.code = "F001";
                    response.msg = "查無相對應審編的專案";
                }
            }
            catch (System.Exception e)
            {
                response.code = "F001";
                response.msg = e.Message;

                logUtil.OutputLog("API NotifyCaseStatus 錯誤", e.Message);
                throw e;
            }

            return response;
        }
        /// <summary>
        /// 撥款通知
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, Route("api/Yrc/NotifyAppropriation")]
        public object NotifyAppropriation(modelAppropriationNotifyReq model)
        {
            Response response = new Response();
            try
            {
                #region 儲存
                IBaseCrudService<modelAppropriationNotifyReq> appropriationNotify = new notifyAppropriationService();
                RespQCS respQCS = QueryCaseStatus(model.examineNo);
                model.QueryCaseStatus = $"{JsonConvert.SerializeObject(respQCS)}";
                appropriationNotify.Save(model);
                #endregion

                response.code = "S001";
                response.msg = "成功";
                logUtil.OutputLog("API NotifyAppropriation Log", $"{JsonConvert.SerializeObject(model)}");

                IBaseCrudService<viewModelReceiveCases> crudService = new receiveCasesService();

                viewModelReceiveCases item = crudService.GetOnly(new viewModelReceiveCases { search_examine_no = model.examineNo });

                if (item != null)
                {
                    item.appropriationDate = model.appropriationDate;
                    item.appropriationAmt = model.appropriationAmt;
                    item.repayKindName = model.repayKindName;
                    item.appropriation_status = model.status;

                    item.saveAction = "Modify";

                    viewModelReceiveCases returnValue = crudService.Save(item);

                    if (returnValue.replyResult == null ||
                        !returnValue.replyResult.Value)
                    {
                        logUtil.OutputLog("API NotifyAppropriation 錯誤", "儲存更新狀態錯誤");
                        response.code = "F001";
                        response.msg = "儲存更新狀態錯誤";
                    }
                }
                else
                {
                    logUtil.OutputLog("API NotifyAppropriation 錯誤", "查無相對應審編的專案");
                    response.code = "F001";
                    response.msg = "查無相對應審編的專案";
                }
            }
            catch (System.Exception e)
            {
                response.code = "F001";
                response.msg = e.Message;

                logUtil.OutputLog("API NotifyAppropriation 錯誤", e.Message);
                throw e;
            }

            return response;
        }
        /// <summary>
        /// 取得案件狀態
        /// </summary>
        /// <param name="examine_no"></param>
        /// <returns></returns>
        //[HttpPost, Route("api/Yrc/QueryCaseStatus")]
        public RespQCS QueryCaseStatus(string examine_no)
        {
            RespQCS respQCS = new RespQCS();

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
                }
            }
            catch (Exception ex)
            {
            }

            return respQCS;
        }
    }

    //
    // 摘要:
    //     撥款通知(輸入參數)
    public class modelAppropriationNotifyReq
    {
        //
        // 摘要:
        //     通路商編號
        public string dealerNo { get; set; }

        //
        // 摘要:
        //     據點編號
        public string branchNo { get; set; }

        //
        // 摘要:
        //     業務人員ID
        public string salesNo { get; set; }

        //
        // 摘要:
        //     審件編號
        public string examineNo { get; set; }

        //
        // 摘要:
        //     撥款時間
        public string appropriationDate { get; set; }

        //
        // 摘要:
        //     撥款金額
        public int? appropriationAmt { get; set; }

        //
        // 摘要:
        //     繳款方式
        public string repayKindName { get; set; }

        //
        // 摘要:
        //     請款狀態
        public string status { get; set; }

        //
        // 摘要:
        //     來源
        public string souce { get; set; }
        /// <summary>
        /// 回傳結果(True/False)
        /// </summary>
        public bool? replyResult { get; set; }
        /// <summary>
        /// 回傳訊息(True/False)
        /// </summary>
        public string replyMsg { get; set; }

        public string QueryCaseStatus { get; set; }
    }

    //
    // 摘要:
    //     案件通知(輸入參數)
    public class modelNotifyStatusReq
    {
        //
        // 摘要:
        //     通路商編號
        public string dealerNo { get; set; }

        //
        // 摘要:
        //     據點編號
        public string branchNo { get; set; }

        //
        // 摘要:
        //     業務人員ID
        public string salesNo { get; set; }

        //
        // 摘要:
        //     審件編號
        public string examineNo { get; set; }

        //
        // 摘要:
        //     案件狀態
        public string examStatusExplain { get; set; }

        //
        // 摘要:
        //     異動時間
        public string ModifyTime { get; set; }
        /// <summary>
        /// 回傳結果(True/False)
        /// </summary>
        public bool? replyResult { get; set; }
        /// <summary>
        /// 回傳訊息(True/False)
        /// </summary>
        public string replyMsg { get; set; }
        public string QueryCaseStatus { get; set; }
    }

    public class Response
    {
        public string code { get; set; }
        public string msg { get; set; }
    }
}