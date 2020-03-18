using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Wecond.Models;

namespace Wecond.Clients
{
    class FlickrData
    {
        public static async Task<FlickrImageResults> GetFlickrImages(string PlaceName)
        {
            PlaceName = PlaceName.Substring(0, PlaceName.LastIndexOf(','));
            var url = "https://api.flickr.com/services/rest/?method=flickr.photos.search&api_key=bdcfe11b48e198ee10fa2399daa3a0ea&text=" + PlaceName + "%20city%20landscape&format=json&per_page=11&nojsoncallback=1";
            FlickrImageResults result = null;
            try
            {
                var client = new HttpClient();
                var response = await client.GetStringAsync(string.Format(url));
                result = JsonConvert.DeserializeObject<FlickrImageResults>(response);
                foreach (var item in result.photos.photo)
                {
                    item.ImageAddress = "http://farm" + item.farm + ".staticflickr.com/" + item.server + "/" + item.id + "_" + item.secret + "_b.jpg";
                    item.ThumbnailAddress = "http://farm" + item.farm + ".staticflickr.com/" + item.server + "/" + item.id + "_" + item.secret + "_q.jpg";
                }
            }
            catch (Exception)
            {
                result = new FlickrImageResults() { };
            }
            return result;
        }
        /*public static async Task<FlickrUserResult> GetFlickrImageOwnerInfo(string OwnerId)
        {
            //var url = "https://query.yahooapis.com/v1/public/yql?q=select%20*%20from%20flickr.people.info2%20where%20user_id%3D%22" + OwnerId + "%22%20and%20api_key%3D%2292bd0de55a63046155c09f1a06876875%22&format=json&env=store%3A%2F%2Fdatatables.org%2Falltableswithkeys";
            var url = "https://api.flickr.com/services/rest/?method=flickr.photos.search&api_key=e323e04846805cdcaca5e8cfbd341967&user_id=" + OwnerId + "&format=json&nojsoncallback=1";
            Debug.WriteLine(url);
            FlickrUserResult result = null;
            try
            {
                var client = new HttpClient();
                var response = await client.GetStringAsync(string.Format(url));
                result = JsonConvert.DeserializeObject<FlickrUserResult>(response);
            }
            catch (Exception)
            {
                result = new FlickrUserResult() { };
            }

            return result;
        }*/
    }
}
