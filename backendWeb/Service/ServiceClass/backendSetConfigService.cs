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
    public class backendSetConfigService : IBaseCrudService<viewModelBackendSetConfig>
    {
        public viewModelBackendSetConfig Delete(viewModelBackendSetConfig model)
        {
            throw new NotImplementedException();
        }

        public IList<viewModelBackendSetConfig> GetList(viewModelBackendSetConfig model)
        {
            try
            {
                StringBuilder builder = new StringBuilder();
                builder.Append("SELECT * FROM backendSetConfig WHERE 1 = 1" + Environment.NewLine);
                List<SqlParameter> parameters = new List<SqlParameter>();
                
                if (!string.IsNullOrEmpty(model.search_config_tag))
                {
                    builder.Append(" AND config_tag = @config_tag ");
                    parameters.Add(new SqlParameter("@config_tag", DbType.String) { Value = model.search_config_tag });
                }
                if (!string.IsNullOrEmpty(model.search_config_code))
                {
                    builder.Append(" AND config_code = @config_code ");
                    parameters.Add(new SqlParameter("@config_code", DbType.String) { Value = model.search_config_code });
                }
                if (model.search_is_enable.HasValue)
                {
                    builder.Append(" AND is_enable = @is_enable ");
                    parameters.Add(new SqlParameter("@is_enable", DbType.Boolean) { Value = model.search_is_enable.Value });
                }
                return new baseRepository<viewModelBackendSetConfig>(new List<string> { builder.ToString() }, new List<List<SqlParameter>> { parameters }).GetList().ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public viewModelBackendSetConfig GetOnly(viewModelBackendSetConfig model)
        {
            try
            {
                StringBuilder builder = new StringBuilder();
                builder.Append("SELECT * FROM backendSetConfig WHERE 1 = 1" + Environment.NewLine);
                List<SqlParameter> parameters = new List<SqlParameter>();

                if (!string.IsNullOrEmpty(model.search_config_tag))
                {
                    builder.Append(" AND config_tag = @config_tag ");
                    parameters.Add(new SqlParameter("@config_tag", DbType.String) { Value = model.search_config_tag });
                }
                if (!string.IsNullOrEmpty(model.search_config_code))
                {
                    builder.Append(" AND config_code = @config_code ");
                    parameters.Add(new SqlParameter("@config_code", DbType.String) { Value = model.search_config_code });
                }

                return new baseRepository<viewModelBackendSetConfig>(new List<string> { builder.ToString() }, new List<List<SqlParameter>> { parameters }).GetOnly();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public viewModelBackendSetConfig Save(viewModelBackendSetConfig model)
        {
            throw new NotImplementedException();
        }
    }
}