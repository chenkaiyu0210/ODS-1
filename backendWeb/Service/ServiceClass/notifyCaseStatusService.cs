using backendWeb.Areas.Yrc.Controllers;
using backendWeb.Models.Repositories;
using backendWeb.Service.InterFace;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;


namespace backendWeb.Service.ServiceClass
{
    public class notifyCaseStatusService : IBaseCrudService<modelNotifyStatusReq>
    {
        public modelNotifyStatusReq Delete(modelNotifyStatusReq model)
        {
            throw new NotImplementedException();
        }

        public IList<modelNotifyStatusReq> GetList(modelNotifyStatusReq model)
        {
            try
            {
                StringBuilder builder = new StringBuilder();
                builder.Append(@"
/****** SSMS 中 SelectTopNRows 命令的指令碼  ******/
SELECT [dealerNo]
	,[branchNo]
	,[salesNo]
	,[examineNo]
	,[examStatusExplain]
	,[ModifyTime]
	,[QueryCaseStatus]
FROM [dbo].[notifyCaseStatus]
WHERE 1 = 1" + Environment.NewLine);
                List<SqlParameter> parameters = new List<SqlParameter>();

                if (model.examineNo != null)
                {
                    builder.Append(" AND examineNo = @examineNo" + Environment.NewLine);
                    parameters.Add(new SqlParameter { ParameterName = "examineNo", Value = model.examineNo });
                }

                builder.Append(@"
GROUP BY [dealerNo]
    ,[branchNo]
    ,[salesNo]
    ,[examineNo]
    ,[examStatusExplain]
    ,[ModifyTime]
    ,[QueryCaseStatus]
ORDER BY ModifyTime DESC" + Environment.NewLine);

                return new baseRepository<modelNotifyStatusReq>(new List<string> { builder.ToString() }, new List<List<SqlParameter>> { parameters }).GetList().ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public modelNotifyStatusReq GetOnly(modelNotifyStatusReq model)
        {
            throw new NotImplementedException();
        }

        public modelNotifyStatusReq Save(modelNotifyStatusReq model)
        {
            try
            {
                LogUtil logUtil = new LogUtil();
                // 改用Entity，所以隨便傳入泛型
                int result = new baseRepository<object>(null).SaveEntity<modelNotifyStatusReq, notifyCaseStatus>(model, "Create");
                if (result > 0)
                {
                    return new modelNotifyStatusReq { replyResult = true };
                }
                else
                {
                    return new modelNotifyStatusReq { replyResult = false, replyMsg = "存檔錯誤" };
                }
            }
            catch (Exception ex)
            {
                return new modelNotifyStatusReq { replyResult = false, replyMsg = ex.Message };
            }
        }
    }
}