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
                builder.Append("SELECT * FROM receiveCases WHERE 1 = 1" + Environment.NewLine);
                List<SqlParameter> parameters = new List<SqlParameter>();
                //if (!string.IsNullOrWhiteSpace(model.searchIn_authorize_code))
                //{
                //    builder.Append(" AND authorize_code in (");
                //    string[] group_codes = model.searchIn_authorize_code.Split(new char[] { ',' });
                //    for (int i = 0; i < group_codes.Length; i++)
                //    {
                //        builder.Append($"@authCode{(i == group_codes.Length - 1 ? i.ToString() : i.ToString() + ",")}");
                //        parameters.Add(new SqlParameter { ParameterName = $"authCode{i}", Value = group_codes[i] });
                //    }
                //    builder.Append(")");
                //}
                if (model.start.HasValue)
                {
                    builder.Append(" ORDER BY receive_date DESC OFFSET @skip ROWS");
                    parameters.Add(new SqlParameter("@skip", DbType.Int32) { Value = model.start.Value });
                    if (model.length.HasValue)
                    {
                        builder.Append(" FETCH NEXT @take ROWS ONLY");
                        parameters.Add(new SqlParameter("@take", DbType.Int32) { Value = model.length.Value });
                    }
                }

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
                builder.Append(" AND receive_id = @receive_id" + Environment.NewLine);
                parameters.Add(new SqlParameter("@receive_id", DbType.Guid) { Value = model.search_receive_id });
                return new baseRepository<viewModelReceiveCases>(new List<string> { builder.ToString() }, new List<List<SqlParameter>> { parameters }).GetOnly();
            }
            catch (Exception ex)
            {
                throw ex;
            }            
        }

        public viewModelReceiveCases Save(viewModelReceiveCases model)
        {
            throw new NotImplementedException();
        }
    }
}