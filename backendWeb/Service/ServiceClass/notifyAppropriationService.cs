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
    public class notifyAppropriationService : IBaseCrudService<modelAppropriationNotifyReq>
    {
        public modelAppropriationNotifyReq Delete(modelAppropriationNotifyReq model)
        {
            throw new NotImplementedException();
        }

        public IList<modelAppropriationNotifyReq> GetList(modelAppropriationNotifyReq model)
        {
            try
            {
                StringBuilder builder = new StringBuilder();
                builder.Append(@"
SELECT *
FROM [dbo].[notifyAppropriation]
WHERE 1 = 1" + Environment.NewLine);
                List<SqlParameter> parameters = new List<SqlParameter>();

                if (model.examineNo != null)
                {
                    builder.Append(" AND examineNo = @examineNo" + Environment.NewLine);
                    parameters.Add(new SqlParameter { ParameterName = "examineNo", Value = model.examineNo });
                }

                return new baseRepository<modelAppropriationNotifyReq>(new List<string> { builder.ToString() }, new List<List<SqlParameter>> { parameters }).GetList().ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public modelAppropriationNotifyReq GetOnly(modelAppropriationNotifyReq model)
        {
            throw new NotImplementedException();
        }

        public modelAppropriationNotifyReq Save(modelAppropriationNotifyReq model)
        {
            try
            {
                LogUtil logUtil = new LogUtil();
                // 改用Entity，所以隨便傳入泛型
                int result = new baseRepository<object>(null)
                    .SaveEntity<modelAppropriationNotifyReq, notifyAppropriation>(model, "Create");
                if (result > 0)
                {
                    return new modelAppropriationNotifyReq { replyResult = true };
                }
                else
                {
                    return new modelAppropriationNotifyReq { replyResult = false, replyMsg = "存檔錯誤" };
                }
            }
            catch (Exception ex)
            {
                return new modelAppropriationNotifyReq { replyResult = false, replyMsg = ex.Message };
            }
        }
    }
}