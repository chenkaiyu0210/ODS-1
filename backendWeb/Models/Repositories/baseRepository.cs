using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace backendWeb.Models.Repositories
{
    public class baseRepository<T> /*where T : class*/
    {
        private List<string> sqlQuery;
        private List<List<SqlParameter>> sqlPar;
        public baseRepository(List<string> sql, List<List<SqlParameter>> par = null)
        {
            sqlQuery = sql;
            sqlPar = par;
        }
        /// <summary>
        /// Select List
        /// </summary>
        /// <returns></returns>
        public IEnumerable<T> GetList()
        {
            try
            {
                using (var context = new RYMimoneyEntities())
                {
                    IEnumerable<T> Results = null;
                    if (sqlPar != null)
                        Results = context.Database.SqlQuery<T>(sqlQuery[0], sqlPar[0].Select(c => ((ICloneable)c).Clone()).ToArray()).ToList();
                    else
                        Results = context.Database.SqlQuery<T>(sqlQuery[0]).ToList();
                    return Results;
                }
            }
            catch (SqlException ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// Select One
        /// </summary>
        /// <returns></returns>
        public T GetOnly()
        {
            try
            {
                using (var context = new RYMimoneyEntities())
                {
                    T Results; //= null;
                    if (sqlPar != null)
                        Results = context.Database.SqlQuery<T>(sqlQuery[0], sqlPar[0].Select(c => ((ICloneable)c).Clone()).ToArray()).FirstOrDefault();
                    else
                        Results = context.Database.SqlQuery<T>(sqlQuery[0]).FirstOrDefault();
                    return Results;
                }
            }
            catch (SqlException ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// Insert OR Update
        /// </summary>
        /// <returns></returns>
        public object Save()
        {
            int Results = 0;
            using (var context = new RYMimoneyEntities())
            {
                using (var dbContextTransaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        for (int i = 0; i < sqlQuery.Count; i++)
                        {
                            Results += context.Database.ExecuteSqlCommand(sqlQuery[i], sqlPar[i].ToArray());
                        }

                        context.SaveChanges();
                        dbContextTransaction.Commit();
                    }
                    catch (SqlException ex)
                    {
                        Results = 0;
                        dbContextTransaction.Rollback();
                        throw ex;
                    }
                }
                return Results;
            }
        }
        /// <summary>
        /// Delete
        /// </summary>
        /// <returns></returns>
        public object Delete()
        {
            int Results = 0;
            using (var context = new RYMimoneyEntities())
            {
                using (var dbContextTransaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        for (int i = 0; i < sqlQuery.Count; i++)
                        {
                            Results += context.Database.ExecuteSqlCommand(sqlQuery[i], sqlPar[i].ToArray());
                        }

                        context.SaveChanges();

                        dbContextTransaction.Commit();
                    }
                    catch (SqlException ex)
                    {
                        Results = 0;
                        dbContextTransaction.Rollback();
                        throw ex;
                    }
                }
                return Results;
            }
        }
    }
}