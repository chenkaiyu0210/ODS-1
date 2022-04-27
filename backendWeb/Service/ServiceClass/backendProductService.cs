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
    public class backendProductService : IBaseCrudService<viewModelBackendProduct>
    {
        public viewModelBackendProduct Delete(viewModelBackendProduct model)
        {
            throw new NotImplementedException();
        }

        public IList<viewModelBackendProduct> GetList(viewModelBackendProduct model)
        {
            try
            {
                StringBuilder builder = new StringBuilder();
                builder.Append("SELECT * FROM backendProduct WHERE 1 = 1" + Environment.NewLine);
                List<SqlParameter> parameters = new List<SqlParameter>();

                if (!string.IsNullOrEmpty(model.search_bus_type))
                {
                    builder.Append(" AND bus_type = @bus_type ");
                    parameters.Add(new SqlParameter("@bus_type", DbType.String) { Value = model.search_bus_type });
                }
                if (!string.IsNullOrEmpty(model.search_product_brand))
                {
                    builder.Append(" AND product_brand = @product_brand ");
                    parameters.Add(new SqlParameter("@product_brand", DbType.String) { Value = model.search_product_brand });
                }

                return new baseRepository<viewModelBackendProduct>(new List<string> { builder.ToString() }, new List<List<SqlParameter>> { parameters }).GetList().ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public viewModelBackendProduct GetOnly(viewModelBackendProduct model)
        {
            try
            {
                StringBuilder builder = new StringBuilder();
                builder.Append("SELECT * FROM backendProduct WHERE 1 = 1" + Environment.NewLine);
                List<SqlParameter> parameters = new List<SqlParameter>();

                if (!string.IsNullOrEmpty(model.search_bus_type))
                {
                    builder.Append(" AND bus_type = @bus_type ");
                    parameters.Add(new SqlParameter("@bus_type", DbType.String) { Value = model.search_bus_type });
                }
                if (!string.IsNullOrEmpty(model.search_product_brand))
                {
                    builder.Append(" AND product_brand = @product_brand ");
                    parameters.Add(new SqlParameter("@product_brand", DbType.String) { Value = model.search_product_brand });
                }

                return new baseRepository<viewModelBackendProduct>(new List<string> { builder.ToString() }, new List<List<SqlParameter>> { parameters }).GetOnly();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public viewModelBackendProduct Save(viewModelBackendProduct model)
        {
            throw new NotImplementedException();
        }
    }
}