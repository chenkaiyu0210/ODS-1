using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;

namespace backendWeb.Helpers
{
    /// <summary>
    /// Http Api呼叫功能
    /// </summary>
    public class HttpHelpers
    {
        /// <summary>
        /// HttpWebRequest Api Post功能
        /// </summary>
        /// <typeparam name="T">物件本體</typeparam>
        /// <param name="Model">物件本體</param>
        /// <param name="targetUrl">Api位置</param>
        /// <param name="WebAPIToken">有Token帶入即可</param>
        /// <returns>Api結果</returns>
        public static string PostJsonData<T>(T Model, string targetUrl, string WebAPIToken = "")
        {
            string s = JsonConvert.SerializeObject(Model);
            byte[] bytes = Encoding.UTF8.GetBytes(s);
            HttpWebRequest httpWebRequest = WebRequest.Create(targetUrl) as HttpWebRequest;
            httpWebRequest.Headers.Add(HttpRequestHeader.Authorization, WebAPIToken);
            httpWebRequest.Method = "POST";
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Timeout = 30000;
            httpWebRequest.ContentLength = (long)bytes.Length;
            using (Stream requestStream = httpWebRequest.GetRequestStream())
            {
                requestStream.Write(bytes, 0, bytes.Length);
            }
            string result = string.Empty;
            using (HttpWebResponse httpWebResponse = httpWebRequest.GetResponse() as HttpWebResponse)
            {
                using (StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                {
                    result = streamReader.ReadToEnd();
                }
            }
            return result;
        }
        /// <summary>
        /// HttpWebRequest Api Post功能
        /// </summary>        
        /// <param name="Model">Json格式字串</param>
        /// <param name="targetUrl">Api位置</param>
        /// <param name="WebAPIToken">有Token帶入即可</param>
        /// <returns>Api結果</returns>
        public static string PostJsonData(string Model, string targetUrl, string WebAPIToken = "")
        {
            byte[] bytes = Encoding.UTF8.GetBytes(Model);
            HttpWebRequest httpWebRequest = WebRequest.Create(targetUrl) as HttpWebRequest;
            httpWebRequest.Headers.Set(HttpRequestHeader.Authorization, WebAPIToken);
            httpWebRequest.Method = "POST";
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Timeout = 30000;
            httpWebRequest.ContentLength = (long)bytes.Length;
            using (Stream requestStream = httpWebRequest.GetRequestStream())
            {
                requestStream.Write(bytes, 0, bytes.Length);
            }
            string result = string.Empty;
            using (HttpWebResponse httpWebResponse = httpWebRequest.GetResponse() as HttpWebResponse)
            {
                using (StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                {
                    result = streamReader.ReadToEnd();
                }
            }
            return result;
        }
        /// <summary>
        /// HttpClient Api Post功能
        /// </summary>
        /// <typeparam name="T">物件本體</typeparam>
        /// <param name="Model">物件本體</param>
        /// <param name="targetUrl">Api位置</param>
        /// <param name="WebAPIToken">有Token帶入即可</param>
        /// <param name="timeOutSecond">Api回應時間(預設3分鐘)</param>
        /// <returns>Api結果</returns>
        public static HttpResponseMessage PostHttpClient<T>(T Model, string targetUrl, string WebAPIToken = "", int? timeOutSecond = null)
        {
            string s = JsonConvert.SerializeObject(Model);
            HttpClient _client = new HttpClient();
            _client.BaseAddress = new Uri(targetUrl);
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", WebAPIToken);
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, targetUrl);
            request.Content = new StringContent(s, Encoding.UTF8, "application/json");//CONTENT-TYPE header            
            _client.Timeout = timeOutSecond.HasValue ? TimeSpan.FromSeconds(timeOutSecond.Value) : TimeSpan.FromSeconds(180);
            return _client.SendAsync(request).Result;
        }
        /// <summary>
        /// HttpClient Api Post功能
        /// </summary>        
        /// <param name="Model">Json格式文字</param>
        /// <param name="targetUrl">Api位置</param>
        /// <param name="WebAPIToken">有Token帶入即可</param>
        /// <param name="timeOutSecond">Api回應時間(預設3分鐘)</param>
        /// <returns>Api結果</returns>
        public static HttpResponseMessage PostHttpClient(string Model, string targetUrl, string WebAPIToken = "", int? timeOutSecond = null)
        {
            HttpClient _client = new HttpClient();
            _client.BaseAddress = new Uri(targetUrl);
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", WebAPIToken);
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, targetUrl);
            request.Content = new StringContent(Model, Encoding.UTF8, "application/json");//CONTENT-TYPE header
            _client.Timeout = timeOutSecond.HasValue ? TimeSpan.FromSeconds(timeOutSecond.Value) : TimeSpan.FromSeconds(180);
            return _client.SendAsync(request).Result;
        }
        /// <summary>
        /// HttpClient Api Get功能
        /// </summary>        
        /// <param name="Model">Json格式文字</param>
        /// <param name="targetUrl">Api位置</param>
        /// <param name="WebAPIToken">有Token帶入即可</param>        
        /// <returns>Api結果</returns>
        public static HttpResponseMessage GetHttpClient(string Model, string targetUrl, string WebAPIToken = "")
        {
            HttpClient _client = new HttpClient();
            _client.BaseAddress = new Uri(targetUrl);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", WebAPIToken);
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, targetUrl);
            return _client.SendAsync(request).Result;
        }
    }
}