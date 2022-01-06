using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace backendWeb.Service.InterFace
{
    public interface IBaseCrudService<T> where T : class
    {
        /// <summary>
        /// 提供列表
        /// </summary>
        /// <typeparam name="T">帶入需傳入(取得)參數型別</typeparam>
        /// <param name="model">參數</param>
        /// <returns></returns>
        IList<T> GetList(T model);

        /// <summary>
        /// 提供唯一資料
        /// </summary>
        /// <typeparam name="T">帶入需傳入(取得)參數型別</typeparam>
        /// <param name="model">參數</param>
        /// <returns></returns>
        T GetOnly(T model);

        /// <summary>
        /// 提供儲存
        /// </summary>
        /// <typeparam name="T">帶入需傳入(取得)參數型別</typeparam>
        /// <param name="model">參數</param>
        /// <returns></returns>
        T Save(T model);

        /// <summary>
        /// 提供刪除
        /// </summary>
        /// <typeparam name="T">帶入需傳入(取得)參數型別</typeparam>
        /// <param name="model">參數</param>
        /// <returns></returns>
        T Delete(T model);
    }
}
