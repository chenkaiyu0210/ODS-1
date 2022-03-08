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
    public class postfileService : IBaseCrudService<viewModelPostfile>
    {
        public viewModelPostfile Delete(viewModelPostfile model)
        {
            throw new NotImplementedException();
        }

        public IList<viewModelPostfile> GetList(viewModelPostfile model)
        {
            try
            {
                StringBuilder builder = new StringBuilder();
                builder.Append("SELECT * FROM postfile WHERE 1 = 1" + Environment.NewLine);
                List<SqlParameter> parameters = new List<SqlParameter>();               
                if (!string.IsNullOrEmpty(model.search_city_name))
                {
                    builder.Append(" AND city_name = @city_name");
                    parameters.Add(new SqlParameter("@city_name", DbType.String) { Value = model.search_city_name });                   
                }

                return new baseRepository<viewModelPostfile>(new List<string> { builder.ToString() }, new List<List<SqlParameter>> { parameters }).GetList().ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public viewModelPostfile GetOnly(viewModelPostfile model)
        {
            try
            {
                StringBuilder builder = new StringBuilder();
                builder.Append("SELECT * FROM postfile WHERE 1 = 1" + Environment.NewLine);
                List<SqlParameter> parameters = new List<SqlParameter>();
                if (!string.IsNullOrEmpty(model.search_city_name))
                {
                    builder.Append(" AND city_name = @city_name");
                    parameters.Add(new SqlParameter("@city_name", DbType.String) { Value = model.search_city_name });
                }
                if (!string.IsNullOrEmpty(model.search_town_name))
                {
                    builder.Append(" AND town_name = @town_name");
                    parameters.Add(new SqlParameter("@town_name", DbType.String) { Value = model.search_town_name });
                }

                return new baseRepository<viewModelPostfile>(new List<string> { builder.ToString() }, new List<List<SqlParameter>> { parameters }).GetOnly();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public viewModelPostfile Save(viewModelPostfile model)
        {
            throw new NotImplementedException();
        }
    }
}