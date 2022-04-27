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
    public class backendBankService : IBaseCrudService<viewModelBackendBank>
    {
        public viewModelBackendBank Delete(viewModelBackendBank model)
        {
            throw new NotImplementedException();
        }

        public IList<viewModelBackendBank> GetList(viewModelBackendBank model)
        {
            try
            {
                StringBuilder builder = new StringBuilder();
                builder.Append("SELECT * FROM backendBank WHERE 1 = 1" + Environment.NewLine);
                List<SqlParameter> parameters = new List<SqlParameter>();

                

                return new baseRepository<viewModelBackendBank>(new List<string> { builder.ToString() }, new List<List<SqlParameter>> { parameters }).GetList().ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public viewModelBackendBank GetOnly(viewModelBackendBank model)
        {
            try
            {
                StringBuilder builder = new StringBuilder();
                builder.Append("SELECT * FROM backendBank WHERE 1 = 1" + Environment.NewLine);
                List<SqlParameter> parameters = new List<SqlParameter>();

                if (!string.IsNullOrEmpty(model.search_bank_code))
                {
                    builder.Append(" AND bank_code = @bank_code ");
                    parameters.Add(new SqlParameter("@bank_code", DbType.String) { Value = model.search_bank_code });
                }


                return new baseRepository<viewModelBackendBank>(new List<string> { builder.ToString() }, new List<List<SqlParameter>> { parameters }).GetOnly();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public viewModelBackendBank Save(viewModelBackendBank model)
        {
            throw new NotImplementedException();
        }
    }
}