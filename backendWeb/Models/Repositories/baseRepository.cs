using backendWeb.Helpers;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace backendWeb.Models.Repositories
{
    public class baseRepository<T> /*where T : class, new()*/
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

        public DataTable DataTable()
        {
            using (var context = new RYMimoneyEntities())
            {
                DataTable dataTable = new DataTable();
                DbConnection connection = context.Database.Connection;
                DbProviderFactory dbFactory = DbProviderFactories.GetFactory(connection);
                using (var cmd = dbFactory.CreateCommand())
                {
                    cmd.Connection = connection;
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = sqlQuery[0];
                    if (sqlPar[0] != null)
                    {
                        foreach (var item in sqlPar[0])
                        {
                            cmd.Parameters.Add(item);
                        }
                    }
                    using (DbDataAdapter adapter = dbFactory.CreateDataAdapter())
                    {
                        adapter.SelectCommand = cmd;
                        adapter.Fill(dataTable);
                    }
                }
                return dataTable;
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
        /// <summary>
        /// Insert OR Update
        /// </summary>
        /// <returns></returns>
        public int SaveEntity<TSource, TTarget>(TSource sourceInstance, string behavior) where TSource : class, new ()
        {
            int Results = 0;
            using (var context = new RYMimoneyEntities())
            {
                using (var dbContextTransaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        if (behavior == "Create")
                            context.Entry(CommonHelpers.Migration<TSource, TTarget>(sourceInstance)).State = EntityState.Added;
                        else
                            context.Entry(CommonHelpers.Migration<TSource, TTarget>(sourceInstance)).State = EntityState.Modified;

                        context.SaveChanges();
                        dbContextTransaction.Commit();
                        Results = 1;
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