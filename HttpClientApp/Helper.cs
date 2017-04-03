using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HttpClientApp
{
    public class Helper
    {
        public static async Task<string> PostJsonData(string url, string json)
        {
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    httpClient.Timeout = TimeSpan.FromMilliseconds(Timeout.Infinite);

                    //string body = "{" + "'FirstName':'Kenan','LastName':'Hancer'" + "}\r\n\r\n";

                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    HttpResponseMessage httpResponseMessage = await httpClient.PostAsync(url, content);
                    {
                        string result = await httpResponseMessage.Content.ReadAsStringAsync();

                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return null;
        }

        public static async Task<string> RequestPage(string url)
        {
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    httpClient.Timeout = TimeSpan.FromMilliseconds(Timeout.Infinite);

                    httpClient.DefaultRequestHeaders.Accept.TryParseAdd("text/html");

                    using (HttpResponseMessage httpResponseMessage = await httpClient.GetAsync(url))
                    {
                        string result = await httpResponseMessage.Content.ReadAsStringAsync();

                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return null;
        }
    }
}