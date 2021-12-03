using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fontWebCore.Models
{
    public class settingConifgModel
    {
        public string every8dUrl { set; get; }
        /// <summary>
        /// 從組態讀取登入逾時設定
        /// </summary>
        public double loginExpireMinute { set; get; }
    }
}
