using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace backendWeb.Models
{
    public class settingConifgModel
    {
        public string every8dUrl { set; get; }
        /// <summary>
        /// 從組態讀取登入逾時設定
        /// </summary>
        public double loginExpireMinute { set; get; }
        /// <summary>
        /// mail設定檔
        /// </summary>
        public setMailModel setMail { set; get; }
        /// <summary>
        /// api設定檔
        /// </summary>
        public apiSettingModel apiSetting { set; get; }
    }

    public class setMailModel
    {
        public string mailSmtp { set; get; }
        public string mailUser { set; get; }
        public string mailAccount { set; get; }
        public string mailPassword { set; get; }
    }
    public class apiSettingModel
    {
        public List<apiUrlModel> apiUrls { set; get; }
        public apiKeyModel apiKey { set; get; }
    }
    public class apiKeyModel
    {
        public string aesIv { set; get; }
        public string aesKey { set; get; }
    }
    public class apiUrlModel
    {
        public string func { set; get; }
        public string url { set; get; }
        public string token { set; get; }
    }
}