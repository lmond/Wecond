using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Wecond.Models;

namespace Wecond.Clients
{
    class News
    {
        public static async Task<NewsResults> GetNewsData(string Language) {
            string url = string.Empty; 
            if (Language == "en")
            {
                url = "https://newsapi.org/v2/everything?domains=weather.com&apiKey=02ca6c297f4b40bf8a6906a97dabc7d5&q=weather%20news";
            }
            else if (Language == "ru") {
                url = "https://newsapi.org/v2/everything?domains=lenta.ru&apiKey=02ca6c297f4b40bf8a6906a97dabc7d5&q=%D0%BF%D0%BE%D0%B3%D0%BE%D0%B4%D0%B0";
            }
            var client = new HttpClient();
            var response = await client.GetStringAsync(string.Format(url));
            var results = JsonConvert.DeserializeObject<NewsResults>(response);
            results.Language = Language;
            return results;
        }
    }
}
