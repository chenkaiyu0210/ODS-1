using backendWeb.Models.ViewModel;
using backendWeb.Service.InterFace;
using backendWeb.Service.ServiceClass;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Web.Http;

namespace backendWeb.Areas.Yrc.Controllers
{
    public class YrcController : ApiController
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
    }

    public class Response
    {
        public string code { get; set; }
        public string msg { get; set; }
    }
}