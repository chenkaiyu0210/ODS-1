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
    public class backendUserService : IBaseCrudService<viewModelBackendUser>
    {
        public backendUserService()
        {
        }
        public viewModelBackendUser Delete(viewModelBackendUser model)
        {
            throw new NotImplementedException();
        }

        IList<viewModelBackendUser> IBaseCrudService<viewModelBackendUser>.GetList(viewModelBackendUser model)
        {
            StringBuilder builder = new StringBuilder();
            StringBuilder builderCount = new StringBuilder();
            builder.Append("SELECT * @count FROM backendUser WHERE 1 = 1" + Environment.NewLine);
            builderCount.Append("SELECT count(1) FROM backendUser WHERE 1 = 1" + Environment.NewLine);
            List<SqlParameter> parameters = new List<SqlParameter>();
            if (!string.IsNullOrWhiteSpace(model.search_account))
            {
                builder.Append(" AND account = @account" + Environment.NewLine);
                builderCount.Append(" AND account = @account" + Environment.NewLine);
                parameters.Add(new SqlParameter { ParameterName = "account", Value = model.search_account });
            }
            if (!string.IsNullOrWhiteSpace(model.search_name))
            {
                builder.Append(" AND [name] = @name" + Environment.NewLine);
                builderCount.Append(" AND name = @name" + Environment.NewLine);
                parameters.Add(new SqlParameter { ParameterName = "name", Value = model.search_name });
            }
            if (model.search_enable.HasValue)
            {
                builder.Append(" AND is_enable = @is_enable" + Environment.NewLine);
                builderCount.Append(" AND is_enable = @is_enable" + Environment.NewLine);
                parameters.Add(new SqlParameter { ParameterName = "is_enable", DbType= DbType.Boolean, Value = model.search_enable.Value });
            }
            if (model.start.HasValue)
            {
                int countModel = new baseRepository<int>(new List<string> { builderCount.ToString() }, new List<List<SqlParameter>> { parameters }).GetOnly();
                builder.Append(" ORDER BY register_time DESC OFFSET @skip ROWS");
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
            return new baseRepository<viewModelBackendUser>(new List<string> { builder.ToString() }, new List<List<SqlParameter>> { parameters }).GetList().ToList();
        }

        public viewModelBackendUser GetOnly(viewModelBackendUser model)
        {
            StringBuilder builder = new StringBuilder();
            List<SqlParameter> parameters = new List<SqlParameter>();
            builder.Append("SELECT * FROM backendUser WHERE 1 = 1");
            if (!string.IsNullOrWhiteSpace(model.search_account))
            {
                builder.Append(" AND account = @account");
                parameters.Add(new SqlParameter { ParameterName = "account", Value = model.search_account });
            }
            return new baseRepository<viewModelBackendUser>(new List<string> { builder.ToString() }, new List<List<SqlParameter>> { parameters }).GetOnly();
        }

        public viewModelBackendUser Save(viewModelBackendUser model)
        {
            try
            {
                StringBuilder builder = new StringBuilder();
                List<SqlParameter> parameters = new List<SqlParameter>();
                if (model.saveAction == "Create")
                {
                    builder.AppendLine(@"INSERT INTO [dbo].[backendUser]
                                   ([account]
                                   ,[name]
                                   ,[password]
                                   ,[salt_key]
                                   ,[register_user]
                                   ,[register_time]
                                   ,[is_enable]
                                   ,[role_group_codes])
                             VALUES");
                    builder.AppendLine(@"(
                                    @account
                                   ,@name
                                   ,@password
                                   ,@salt_key
                                   ,@register_user
                                   ,GETDATE()
                                   ,@is_enable
                                   ,@role_group_codes )");
                    parameters.Add(new SqlParameter { ParameterName = "account", Value = model.account });
                    parameters.Add(new SqlParameter { ParameterName = "name", Value = model.name });
                    parameters.Add(new SqlParameter { ParameterName = "password", Value = model.password });
                    parameters.Add(new SqlParameter { ParameterName = "salt_key", Value = model.salt_key });
                    parameters.Add(new SqlParameter { ParameterName = "register_user", Value = model.register_user });
                    parameters.Add(new SqlParameter { ParameterName = "is_enable", DbType = DbType.Boolean, Value = model.is_enable.HasValue ? model.is_enable.Value : false });
                    parameters.Add(new SqlParameter { ParameterName = "role_group_codes", Value = model.role_group_codes });
                    object i = new baseRepository<viewModelBackendUser>(new List<string> { builder.ToString() }, new List<List<SqlParameter>> { parameters }).Save();
                    int.TryParse(i.ToString(), out int result);

                    return result > 0 ? new viewModelBackendUser { replyResult = true } : new viewModelBackendUser { replyResult = false, replyMsg = "存檔失敗" };
                }
                else
                {
                    builder.Append(@"UPDATE [dbo].[backendUser] SET [name] = @name ,[role_group_codes] = @role_group_codes");
                    if (!string.IsNullOrWhiteSpace(model.password))
                    {
                        builder.Append(@" ,[password] = @password");
                        builder.Append(@" ,[salt_key] = @salt_key");
                        parameters.Add(new SqlParameter { ParameterName = "password", Value = model.password });
                        parameters.Add(new SqlParameter { ParameterName = "salt_key", Value = model.salt_key });
                    }
                    if (model.is_enable.HasValue)
                    {
                        builder.Append(@" ,[is_enable] = 1");                        
                    }
                    else
                    {
                        builder.Append(@" ,[is_enable] = 0");
                        builder.Append(@" ,[disable_time] = GETDATE() ,[disable_user]=@disable_user");
                        parameters.Add(new SqlParameter { ParameterName = "disable_user", Value = model.disable_user });
                    }

                    builder.AppendLine(@" WHERE account = @account");
                    parameters.Add(new SqlParameter { ParameterName = "account", Value = model.account });
                    parameters.Add(new SqlParameter { ParameterName = "name", Value = model.name });
                    parameters.Add(new SqlParameter { ParameterName = "role_group_codes", Value = model.role_group_codes });
                    object i = new baseRepository<viewModelBackendUser>(new List<string> { builder.ToString() }, new List<List<SqlParameter>> { parameters }).Save();
                    int.TryParse(i.ToString(), out int result);

                    return result > 0 ? new viewModelBackendUser { replyResult = true } : new viewModelBackendUser { replyResult = false, replyMsg = "存檔失敗" };
                }
            }
            catch (Exception ex)
            {
                return new viewModelBackendUser { replyResult = false, replyMsg = ex.Message };
            }
        }
    }
}