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
    public class backendPromotionService : IBaseCrudService<viewModelBackendPromotion>
    {
        public viewModelBackendPromotion Delete(viewModelBackendPromotion model)
        {
            throw new NotImplementedException();
        }

        public IList<viewModelBackendPromotion> GetList(viewModelBackendPromotion model)
        {
            try
            {
                StringBuilder builder = new StringBuilder();
                builder.Append("SELECT * FROM backendPromotion WHERE 1 = 1 AND is_enable = 1 " + Environment.NewLine);
                List<SqlParameter> parameters = new List<SqlParameter>();

                if (!string.IsNullOrEmpty(model.search_bus_type))
                {
                    builder.Append(" AND bus_type = @bus_type ");
                    parameters.Add(new SqlParameter("@bus_type", DbType.String) { Value = model.search_bus_type });
                }

                return new baseRepository<viewModelBackendPromotion>(new List<string> { builder.ToString() }, new List<List<SqlParameter>> { parameters }).GetList().ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public viewModelBackendPromotion GetOnly(viewModelBackendPromotion model)
        {
            try
            {
                StringBuilder builder = new StringBuilder();
                builder.Append("SELECT * FROM backendPromotion WHERE 1 = 1" + Environment.NewLine);
                List<SqlParameter> parameters = new List<SqlParameter>();

                if (!string.IsNullOrEmpty(model.search_bus_type))
                {
                    builder.Append(" AND bus_type = @bus_type ");
                    parameters.Add(new SqlParameter("@bus_type", DbType.String) { Value = model.search_bus_type });
                }
                if (!string.IsNullOrEmpty(model.search_promo_no))
                {
                    builder.Append(" AND promo_no = @promo_no ");
                    parameters.Add(new SqlParameter("@promo_no", DbType.String) { Value = model.search_promo_no });
                }
                return new baseRepository<viewModelBackendPromotion>(new List<string> { builder.ToString() }, new List<List<SqlParameter>> { parameters }).GetOnly();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public viewModelBackendPromotion Save(viewModelBackendPromotion model)
        {
            throw new NotImplementedException();
        }
    }
}