using backendWeb.Models.Repositories;
using backendWeb.Models.ViewModel;
using backendWeb.Service.InterFace;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace backendWeb.Service.ServiceClass
{
    public class receiveCasesService : IBaseCrudService<viewModelReceiveCases>
    {
        public viewModelReceiveCases Delete(viewModelReceiveCases model)
        {
            throw new NotImplementedException();
        }

        public IList<viewModelReceiveCases> GetList(viewModelReceiveCases model)
        {
            try
            {
                StringBuilder builder = new StringBuilder();
                builder.Append(@"
SELECT r.*
	,isnull(u.name, '') AS receive_staff_name
FROM receiveCases r
LEFT JOIN backendUser u ON r.receive_staff = u.account
WHERE 1 = 1 and is_delete = 0" + Environment.NewLine);
                List<SqlParameter> parameters = new List<SqlParameter>();

                if (model.receive_date != null)
                {
                    builder.Append(" AND convert(VARCHAR, receive_date, 23) = @receive_date" + Environment.NewLine);
                    parameters.Add(new SqlParameter { ParameterName = "receive_date", Value = model.receive_date.Value.ToString("yyyy-MM-dd") });
                }

                if (!string.IsNullOrWhiteSpace(model.customer_idcard_no))
                {
                    builder.Append(" AND customer_idcard_no = @customer_idcard_no" + Environment.NewLine);
                    parameters.Add(new SqlParameter { ParameterName = "customer_idcard_no", Value = model.customer_idcard_no });
                }

                if (!string.IsNullOrWhiteSpace(model.receive_staff))
                {
                    builder.Append(" AND receive_staff = @receive_staff" + Environment.NewLine);
                    parameters.Add(new SqlParameter { ParameterName = "receive_staff", Value = model.receive_staff });
                }

                if ( !string.IsNullOrWhiteSpace(model.receive_status))
                {
                    builder.Append(" AND receive_status = @receive_status" + Environment.NewLine);
                    parameters.Add(new SqlParameter { ParameterName = "receive_status", Value = model.receive_status });
                }

                if (model.search_isAppropriation)
                {
                    builder.Append(" AND appropriation_status IS NOT NULL" + Environment.NewLine);
                }

                if (model.isResend)
                {
                    builder.Append(" AND receive_status IN (N'待補', N'自退', N'核准', N'婉拒', N'附條件')" + Environment.NewLine);
                }

                if (model.guarantor_idcard_no != null)
                {
                    builder.Append(" AND guarantor_idcard_no = @guarantor_idcard_no" + Environment.NewLine);
                    parameters.Add(new SqlParameter { ParameterName = "guarantor_idcard_no", Value = model.guarantor_idcard_no });
                }

                //if (model.start.HasValue)
                //{
                //    builder.Append(" ORDER BY receive_date DESC OFFSET @skip ROWS");
                //    parameters.Add(new SqlParameter("@skip", DbType.Int32) { Value = model.start.Value });
                //    if (model.length.HasValue)
                //    {
                //        builder.Append(" FETCH NEXT @take ROWS ONLY");
                //        parameters.Add(new SqlParameter("@take", DbType.Int32) { Value = model.length.Value });
                //    }
                //}

                return new baseRepository<viewModelReceiveCases>(new List<string> { builder.ToString() }, new List<List<SqlParameter>> { parameters }).GetList().ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }            
        }

        public viewModelReceiveCases GetOnly(viewModelReceiveCases model)
        {
            try
            {
                StringBuilder builder = new StringBuilder();
                builder.Append("SELECT * FROM receiveCases WHERE 1 = 1" + Environment.NewLine);
                List<SqlParameter> parameters = new List<SqlParameter>();

                if (model.search_receive_id != null)
                {
                    builder.Append(" AND receive_id = @receive_id" + Environment.NewLine);
                    parameters.Add(new SqlParameter("@receive_id", DbType.Guid) { Value = model.search_receive_id });
                }

                if (model.search_examine_no != null)
                {
                    builder.Append(" AND examine_no = @examine_no" + Environment.NewLine);
                    parameters.Add(new SqlParameter("@examine_no", DbType.Guid) { Value = model.search_examine_no });
                }

                return new baseRepository<viewModelReceiveCases>(new List<string> { builder.ToString() }, new List<List<SqlParameter>> { parameters }).GetOnly();
            }
            catch (Exception ex)
            {
                throw ex;
            }            
        }

        public viewModelReceiveCases Save(viewModelReceiveCases model)
        {
            try
            {
                // 改用Entity，所以隨便傳入泛型
                int result = new baseRepository<object>(null).SaveEntity<viewModelReceiveCases,receiveCases>(model, model.saveAction);
                if (result > 0)
                {
                    return new viewModelReceiveCases { replyResult = true };
                }
                else
                {
                    return new viewModelReceiveCases { replyResult = false, replyMsg = "存檔錯誤" };
                }
            }
            catch (Exception ex)
            {
                return new viewModelReceiveCases { replyResult = false, replyMsg = ex.Message };
            }
        }
    }
}