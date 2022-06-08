using backendWeb.Models.Repositories;
using System;
using System.Data.SqlClient;

namespace backendWeb.Service.ServiceClass
{
    public class LogUtil
    {
        public void OutputLog(string LogMethod, string LogMessage)
        {
            using (var context = new RYMimoneyEntities())
            {
                using (var dbContextTransaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        SystemLog systemLog = new SystemLog
                        {
                            LogTime = DateTime.Now,
                            LogMethod = LogMethod,
                            LogMessage = LogMessage
                        };

                        context.Set<SystemLog>().Add(systemLog);

                        context.SaveChanges();
                        dbContextTransaction.Commit();
                    }
                    catch (SqlException ex)
                    {
                        dbContextTransaction.Rollback();
                        throw ex;
                    }
                }
            }
        }
    }
}