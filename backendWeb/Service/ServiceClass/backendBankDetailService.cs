using backendWeb.Models.Repositories;
using backendWeb.Models.ViewModel;
using backendWeb.Service.InterFace;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;

namespace backendWeb.Service.ServiceClass
{
    public class backendBankDetailService : IBaseCrudService<viewModelBackendBankDetail>
    {
        public viewModelBackendBankDetail Delete(viewModelBackendBankDetail model)
        {
            throw new NotImplementedException();
        }

        public IList<viewModelBackendBankDetail> GetList(viewModelBackendBankDetail model)
        {
            try
            {
                StringBuilder builder = new StringBuilder();
                builder.Append("SELECT * FROM backendBankDetail WHERE 1 = 1" + Environment.NewLine);
                List<SqlParameter> parameters = new List<SqlParameter>();

                if (!string.IsNullOrEmpty(model.search_bank_code))
                {
                    builder.Append(" AND bank_code = @bank_code ");
                    parameters.Add(new SqlParameter("@bank_code", DbType.String) { Value = model.search_bank_code });
                }                

                return new baseRepository<viewModelBackendBankDetail>(new List<string> { builder.ToString() }, new List<List<SqlParameter>> { parameters }).GetList().ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public viewModelBackendBankDetail GetOnly(viewModelBackendBankDetail model)
        {
            try
            {
                StringBuilder builder = new StringBuilder();
                builder.Append("SELECT * FROM backendBankDetail WHERE 1 = 1" + Environment.NewLine);
                List<SqlParameter> parameters = new List<SqlParameter>();
                if (!string.IsNullOrEmpty(model.search_bank_code))
                {
                    builder.Append(" AND bank_code = @bank_code ");
                    parameters.Add(new SqlParameter("@bank_code", DbType.String) { Value = model.search_bank_code });
                }
                if (!string.IsNullOrEmpty(model.search_fnctr_code))
                {
                    builder.Append(" AND fnctr_code = @fnctr_code ");
                    parameters.Add(new SqlParameter("@fnctr_code", DbType.String) { Value = model.search_fnctr_code });
                }

                return new baseRepository<viewModelBackendBankDetail>(new List<string> { builder.ToString() }, new List<List<SqlParameter>> { parameters }).GetOnly();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public viewModelBackendBankDetail Save(viewModelBackendBankDetail model)
        {
            throw new NotImplementedException();
        }
    }
}