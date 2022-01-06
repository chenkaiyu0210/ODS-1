using backendWeb.Models.Repositories;
using backendWeb.Models.ViewModel;
using backendWeb.Service.InterFace;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;

namespace backendWeb.Service.ServiceClass
{
    public class backendMenuService : IBaseCrudService<viewModelBackendMenu>
    {
        public viewModelBackendMenu Delete(viewModelBackendMenu model)
        {
            throw new NotImplementedException();
        }

        public IList<viewModelBackendMenu> GetList(viewModelBackendMenu model)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("SELECT * FROM backendMenu WHERE 1 = 1");
            List<SqlParameter> parameters = new List<SqlParameter>();
            if (!string.IsNullOrWhiteSpace(model.searchIn_authorize_code))
            {
                builder.Append(" AND authorize_code in (");
                string[] group_codes = model.searchIn_authorize_code.Split(new char[] { ',' });
                for (int i = 0; i < group_codes.Length; i++)
                {
                    builder.Append($"@authCode{(i == group_codes.Length - 1 ? i.ToString() : i.ToString() + ",")}");
                    parameters.Add(new SqlParameter { ParameterName = $"authCode{i}", Value = group_codes[i] });
                }
                builder.Append(")");
            }
            if (model.search_hasAuthorize.HasValue)
            {
                builder.Append(" AND ISNULL(authorize_code,'') <> ''" );
            }
            return new baseRepository<viewModelBackendMenu>(new List<string> { builder.ToString() }).GetList().ToList();
        }

        public viewModelBackendMenu GetOnly(viewModelBackendMenu model)
        {
            throw new NotImplementedException();
        }

        public viewModelBackendMenu Save(viewModelBackendMenu model)
        {
            throw new NotImplementedException();
        }
        public IList<viewModelBackendMenu> GetMenu(viewModelBackendMenu model)
        {
            try
            {
                StringBuilder builder = new StringBuilder();
                StringBuilder queryStr = new StringBuilder();
                builder.Append(@"WITH tmpTree (app_id,func_id, parent_func_id) AS (
                                SELECT app_id,func_id, parent_func_id FROM backendMenu WHERE authorize_code IN (
                                SELECT distinct col FROM f_StringToSplit(TRIM (',' FROM (
                                SELECT  authorize_codes + ',' FROM backendRoleGroup @queryStr@
                                FOR XML PATH('')
                                )),','))
                                UNION All
                                SELECT a.app_id,a.func_id, a.parent_func_id FROM backendMenu a
                                INNER JOIN tmpTree b on a.func_id=b.parent_func_id )
                                SELECT distinct a.*  FROM backendMenu a INNER JOIN tmpTree b on a.func_id = b.func_id AND a.app_id = b.app_id
                                ORDER BY a.sort_order");
                List<SqlParameter> parameters = null;
                if (!string.IsNullOrWhiteSpace(model.searchIn_role_group_code))
                {
                    parameters = new List<SqlParameter>();
                    queryStr.Append("WHERE role_group_code in (");
                    string[] group_codes = model.searchIn_role_group_code.Split(new char[] { ',' });
                    for (int i = 0; i < group_codes.Length; i++)
                    {
                        queryStr.Append($"@roleCode{(i == group_codes.Length - 1 ? i.ToString() : i.ToString() + ",")}");
                        parameters.Add(new SqlParameter { ParameterName = $"roleCode{i}", Value = group_codes[i] });
                    }
                    queryStr.Append(")");
                }
                return new baseRepository<viewModelBackendMenu>(new List<string> { builder.ToString().Replace("@queryStr@", queryStr.ToString()) }, new List<List<SqlParameter>> { parameters }).GetList().OrderBy(o => o.sort_order).ToList();          
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}