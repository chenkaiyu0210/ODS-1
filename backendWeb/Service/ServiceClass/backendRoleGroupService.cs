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
    public class backendRoleGroupService : IBaseCrudService<viewModelBackendRoleGroup>
    {
        public viewModelBackendRoleGroup Delete(viewModelBackendRoleGroup model)
        {
            throw new NotImplementedException();
        }

        public IList<viewModelBackendRoleGroup> GetList(viewModelBackendRoleGroup model)
        {
            StringBuilder builder = new StringBuilder();
            StringBuilder builderCount = new StringBuilder();
            builder.Append("SELECT * @count FROM backendRoleGroup WHERE 1 = 1");
            builderCount.Append("SELECT count(1) FROM backendRoleGroup WHERE 1 = 1" + Environment.NewLine);
            List<SqlParameter> parameters = new List<SqlParameter>();
            if (!string.IsNullOrWhiteSpace(model.searchIn_role_group_code))
            {
                builder.Append(" AND role_group_code in (");
                string[] group_codes = model.searchIn_role_group_code.Split(new char[] { ',' });
                for (int i = 0; i < group_codes.Length; i++)
                {
                    builder.Append($"@roleCode{(i == group_codes.Length - 1 ? i.ToString() : i.ToString() + ",")}");
                    parameters.Add(new SqlParameter { ParameterName = $"roleCode{i}", Value = group_codes[i] });
                }
                builder.Append(")");
            }
            if (!string.IsNullOrWhiteSpace(model.search_role_group_code))
            {
                builder.Append(" AND role_group_code = @role_group_code" + Environment.NewLine);
                builderCount.Append(" AND role_group_code = @role_group_code" + Environment.NewLine);
                parameters.Add(new SqlParameter { ParameterName = "role_group_code", Value = model.search_role_group_code });
            }
            if (!string.IsNullOrWhiteSpace(model.search_role_group_name))
            {
                builder.Append(" AND [role_group_name] = @role_group_name" + Environment.NewLine);
                builderCount.Append(" AND role_group_name = @role_group_name" + Environment.NewLine);
                parameters.Add(new SqlParameter { ParameterName = "role_group_name", Value = model.search_role_group_name });
            }
            //if (model.search_enable.HasValue)
            //{
            //    builder.Append(" AND is_enable = @is_enable" + Environment.NewLine);
            //    builderCount.Append(" AND is_enable = @is_enable" + Environment.NewLine);
            //    parameters.Add(new SqlParameter { ParameterName = "is_enable", DbType = DbType.Boolean, Value = model.search_enable.Value });
            //}
            if (model.start.HasValue)
            {
                int countModel = new baseRepository<int>(new List<string> { builderCount.ToString() }, new List<List<SqlParameter>> { parameters }).GetOnly();
                builder.Append(" ORDER BY create_time DESC OFFSET @skip ROWS");
                parameters.Add(new SqlParameter("@skip", DbType.Int32) { Value = model.start.Value });
                if (model.length.HasValue)
                {
                    builder.Append(" FETCH NEXT @take ROWS ONLY");
                    parameters.Add(new SqlParameter("@take", DbType.Int32) { Value = model.length.Value });
                }

                builder = builder.Replace("@count", "," + countModel.ToString() + "as [numCount]");
            }
            else
            {
                builder = builder.Replace("@count", "");
            }
            return new baseRepository<viewModelBackendRoleGroup>(new List<string> { builder.ToString() } , new List<List<SqlParameter>> { parameters }).GetList().ToList();
        }

        public viewModelBackendRoleGroup GetOnly(viewModelBackendRoleGroup model)
        {
            StringBuilder builder = new StringBuilder();
            List<SqlParameter> parameters = new List<SqlParameter>();
            builder.Append("SELECT * FROM backendRoleGroup WHERE 1 = 1");
            if (!string.IsNullOrWhiteSpace(model.search_role_group_code))
            {
                builder.Append(" AND role_group_code = @role_group_code");
                parameters.Add(new SqlParameter { ParameterName = "role_group_code", Value = model.search_role_group_code });
            }
            return new baseRepository<viewModelBackendRoleGroup>(new List<string> { builder.ToString() }, new List<List<SqlParameter>> { parameters }).GetOnly();
        }

        public viewModelBackendRoleGroup Save(viewModelBackendRoleGroup model)
        {
            try
            {
                StringBuilder builder = new StringBuilder();
                List<SqlParameter> parameters = new List<SqlParameter>();
                if (model.saveAction == "Create")
                {
                    builder.AppendLine(@"INSERT INTO [dbo].[backendRoleGroup]
                                   ([role_group_code]
                                   ,[role_group_name]
                                   ,[is_enable]
                                   ,[authorize_codes]
                                   ,[create_time])
                             VALUES");
                    builder.AppendLine(@"(
                                    @role_group_code
                                   ,@role_group_name
                                   ,@is_enable
                                   ,@authorize_codes
                                   ,GETDATE())");
                    parameters.Add(new SqlParameter { ParameterName = "role_group_code", Value = model.role_group_code });
                    parameters.Add(new SqlParameter { ParameterName = "role_group_name", Value = model.role_group_name });
                    parameters.Add(new SqlParameter { ParameterName = "is_enable", DbType = DbType.Boolean, Value = model.is_enable });
                    parameters.Add(new SqlParameter { ParameterName = "authorize_codes", Value = model.authorize_codes });                    
                    object i = new baseRepository<viewModelBackendRoleGroup>(new List<string> { builder.ToString() }, new List<List<SqlParameter>> { parameters }).Save();
                    int.TryParse(i.ToString(), out int result);

                    return result > 0 ? new viewModelBackendRoleGroup { replyResult = true } : new viewModelBackendRoleGroup { replyResult = false, replyMsg = "存檔失敗" };
                }
                else
                {
                    builder.Append(@"UPDATE [dbo].[backendRoleGroup] SET [role_group_name] = @role_group_name ,[authorize_codes] = @authorize_codes");
                    if (model.is_enable.HasValue)
                        builder.Append(@" ,[is_enable] = 1");
                    else
                        builder.Append(@" ,[is_enable] = 0");

                    builder.AppendLine(@" WHERE role_group_code = @role_group_code");
                    parameters.Add(new SqlParameter { ParameterName = "role_group_code", Value = model.role_group_code });
                    parameters.Add(new SqlParameter { ParameterName = "role_group_name", Value = model.role_group_name });
                    parameters.Add(new SqlParameter { ParameterName = "authorize_codes", Value = model.authorize_codes });
                    object i = new baseRepository<viewModelBackendRoleGroup>(new List<string> { builder.ToString() }, new List<List<SqlParameter>> { parameters }).Save();
                    int.TryParse(i.ToString(), out int result);

                    return result > 0 ? new viewModelBackendRoleGroup { replyResult = true } : new viewModelBackendRoleGroup { replyResult = false, replyMsg = "存檔失敗" };
                }
            }
            catch (Exception ex)
            {
                return new viewModelBackendRoleGroup { replyResult = false, replyMsg = ex.Message };
            }
        }

        //public string GetRoleGroupCodes(viewModelBackendRoleGroup model)
        //{
        //    try
        //    {
        //        StringBuilder builder = new StringBuilder();
        //        StringBuilder queryStr = new StringBuilder();
        //        builder.Append(@"SELECT TRIM(',' FROM (
        //                    SELECT distinct col + ',' FROM f_StringToSplit(TRIM(',' FROM(
        //                    SELECT authorize_codes + ',' FROM backendRoleGroup @queryStr@
        //                    FOR XML PATH('')
        //                    )), ',') FOR XML PATH('')
        //                    )) as [authorize_codes]");
        //        List<SqlParameter> parameters = null;
        //        if (!string.IsNullOrWhiteSpace(model.searchIn_role_group_code))
        //        {
        //            parameters = new List<SqlParameter>();
        //            queryStr.Append("WHERE role_group_code in (");
        //            string[] group_codes = model.searchIn_role_group_code.Split(new char[] { ',' });
        //            for (int i = 0; i < group_codes.Length; i++)
        //            {
        //                queryStr.Append($"@roleCode{(i == group_codes.Length - 1 ? i.ToString() : i.ToString() + ",")}");
        //                parameters.Add(new SqlParameter { ParameterName = $"roleCode{i}", Value = group_codes[i] });
        //            }
        //            queryStr.Append(")");
        //        }
        //        viewModelBackendRoleGroup item = new baseRepository<viewModelBackendRoleGroup>(new List<string> { builder.ToString().Replace("@queryStr@", queryStr.ToString()) }, new List<List<SqlParameter>> { parameters }).GetOnly();
        //        return item == null ? "" : item.authorize_codes;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }            
        //}
    }
}