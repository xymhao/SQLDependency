using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Utils
{
    public class BaseInput
    {
        public object input
        {
            get;
            set;
        }

        public BaseInput(object obj)
        {
            input = obj;
        }
    }

    public static class HttpClientUtility
    {
        public static string _qdtAddress = ConfigurationManager.AppSettings["qdtServiceAddress"].ToString();

        public static void Post(string func, BaseInput input)
        {
            try
            {
                HttpClient client = new HttpClient();
                string uri = _qdtAddress + func;
                string json = JsonConvert.SerializeObject(input);
                client.BaseAddress = new Uri(_qdtAddress);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                StringContent theContent = new StringContent(json, Encoding.UTF8, "application/json");
                var response = client.PostAsync(uri, theContent);
            }
            catch (Exception e)
            {
                LogUtils.Error("Post:", e.Message);
                LogUtils.Error(e.StackTrace);
            } 
        }

        public static void PostResult(string func, BaseInput input)
        {
            try
            {
                HttpClient client = new HttpClient();
                string uri = _qdtAddress + func;
                string json = JsonConvert.SerializeObject(input);
                client.BaseAddress = new Uri(_qdtAddress);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                StringContent theContent = new StringContent(json, Encoding.UTF8, "application/json");
                var response = client.PostAsync(uri, theContent).Result;
            }
            catch (Exception e)
            {
                LogUtils.Error("Post:", e.Message);
                LogUtils.Error(e.StackTrace);
            }
        }
    }
}
