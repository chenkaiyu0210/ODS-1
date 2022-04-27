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
    public class backendBusTypeService : IBaseCrudService<viewModelBackendBusType>
    {
        public viewModelBackendBusType Delete(viewModelBackendBusType model)
        {
            throw new NotImplementedException();
        }

        public IList<viewModelBackendBusType> GetList(viewModelBackendBusType model)
        {
            try
            {
                StringBuilder builder = new StringBuilder();
                builder.Append("SELECT * FROM backendBusType WHERE 1 = 1" + Environment.NewLine);
                List<SqlParameter> parameters = new List<SqlParameter>();

                if (!string.IsNullOrEmpty(model.search_bus_type))
                {
                    builder.Append(" AND bus_type = @bus_type ");
                    parameters.Add(new SqlParameter("@bus_type", DbType.String) { Value = model.search_bus_type });
                }
                

                return new baseRepository<viewModelBackendBusType>(new List<string> { builder.ToString() }, new List<List<SqlParameter>> { parameters }).GetList().ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public viewModelBackendBusType GetOnly(viewModelBackendBusType model)
        {
            try
            {
                StringBuilder builder = new StringBuilder();
                builder.Append("SELECT * FROM backendBusType WHERE 1 = 1" + Environment.NewLine);
                List<SqlParameter> parameters = new List<SqlParameter>();

                if (!string.IsNullOrEmpty(model.search_bus_type))
                {
                    builder.Append(" AND bus_type = @bus_type ");
                    parameters.Add(new SqlParameter("@bus_type", DbType.String) { Value = model.search_bus_type });
                }

                return new baseRepository<viewModelBackendBusType>(new List<string> { builder.ToString() }, new List<List<SqlParameter>> { parameters }).GetOnly();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public viewModelBackendBusType Save(viewModelBackendBusType model)
        {
            throw new NotImplementedException();
        }
    }
}