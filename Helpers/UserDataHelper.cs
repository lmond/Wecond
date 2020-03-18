using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Wecond.Models;
using Windows.Graphics.Imaging;
using Windows.Networking.BackgroundTransfer;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace Wecond.Helpers
{
    class UserDataHelper
    {
        public static async Task<bool> WriteFile(string filename, UserSettings _UserSettings) {
            try
            {
                StorageFile file = await ApplicationData.Current.RoamingFolder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);
                string data = JsonConvert.SerializeObject(_UserSettings);
                await FileIO.WriteTextAsync(file, data);
                return true;
            }
            catch (Exception) {
                return false;
            }
        }
        public static async Task<UserSettings> GetSettings(string filename)
        {
            UserSettings result = new UserSettings();
            try
            {
                var file = await ApplicationData.Current.RoamingFolder.GetFileAsync(filename);
                string content = await FileIO.ReadTextAsync(file); ;
                result = JsonConvert.DeserializeObject<UserSettings>(content);
            }
            catch (Exception)
            {
                result = null;
            }
            return result;
        }
        public static async Task<List<PlaceInfo>> GetFavorites(string filename)
        {
            var result = new List<PlaceInfo>();
            try
            {
                var file = await ApplicationData.Current.RoamingFolder.GetFileAsync(filename);
                string content = await FileIO.ReadTextAsync(file);
                result = JsonConvert.DeserializeObject<List<PlaceInfo>>(content);
            }
            catch (Exception)
            {
                
            }
            return result;
        }
        public static async Task<bool> SaveToFavorites(string filename, PlaceInfo _PlaceInfo)
        {
            try
            {
                var favorites = await GetFavorites(filename);
                if (favorites == null) {
                    favorites = new List<PlaceInfo>();
                }
                favorites.Add(_PlaceInfo);

                StorageFile file = await ApplicationData.Current.RoamingFolder.CreateFileAsync("Favorites.json", CreationCollisionOption.ReplaceExisting);
                string data = JsonConvert.SerializeObject(favorites);
                await FileIO.WriteTextAsync(file, data);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public static async Task<bool> RemoveFromFavorites(string filename, PlaceInfo _PlaceInfo)
        {
            try
            {
                var favorites = await GetFavorites(filename);
                foreach (var item in favorites.ToArray()) {
                    if (item.PlaceId == _PlaceInfo.PlaceId)
                        favorites.Remove(item);
                }
                
                StorageFile file = await ApplicationData.Current.RoamingFolder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);
                string data = JsonConvert.SerializeObject(favorites);
                await FileIO.WriteTextAsync(file, data);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public static async Task<bool> SaveFavoriteWeather(CityData _CityData)
        {
            try
            {
                if (ApplicationData.Current.LocalFolder.TryGetItemAsync("FavoritesData") == null)
                    await ApplicationData.Current.LocalFolder.CreateFolderAsync("FavoritesData");
                
                StorageFile file = await ApplicationData.Current.LocalFolder.CreateFileAsync(@"FavoritesData\" + _CityData.PlaceInfo.PlaceId + ".json", CreationCollisionOption.ReplaceExisting);
                _CityData.DailyForecast = null;
                string data = JsonConvert.SerializeObject(_CityData);
                await FileIO.WriteTextAsync(file, data);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public static async Task<bool> RemoveFavoriteWeather(string _CityId)
        {
            try
            {
                var file = await ApplicationData.Current.LocalFolder.GetFileAsync(@"FavoritesData\" + _CityId + ".json");
                await file.DeleteAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public static async Task<CityData> GetFavoriteWeather(string PlaceId)
        {
            CityData result = new CityData();
            try
            {
                var file = await ApplicationData.Current.LocalFolder.GetFileAsync(@"FavoritesData\" + PlaceId + ".json");
                string content = await FileIO.ReadTextAsync(file);
                result = JsonConvert.DeserializeObject<CityData>(content);
            }
            catch (Exception)
            {
                result = null;
            }
            return result;
        }
        public static async Task<bool> SaveHomeWeather(CityData _CityData)
        {
            try
            {
                StorageFile file = await ApplicationData.Current.LocalFolder.CreateFileAsync("HomeWeather.json", CreationCollisionOption.ReplaceExisting);
                string data = JsonConvert.SerializeObject(_CityData);
                await FileIO.WriteTextAsync(file, data);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public static async Task<CityData> GetSavedHomeWeather()
        {
            CityData result = new CityData();
            try
            {
                var file = await ApplicationData.Current.LocalFolder.GetFileAsync("HomeWeather.json");
                string content = await FileIO.ReadTextAsync(file);
                result = JsonConvert.DeserializeObject<CityData>(content);
            }
            catch (Exception)
            {
                result = null;
            }
            return result;
        }
        public static async Task<int> CheckFileAge(string filename, string folder) {
            StorageFile _File = null;
            if (folder == "local") {
                _File = await ApplicationData.Current.LocalFolder.GetFileAsync(filename);
            }else if (folder == "roaming")
                _File = await ApplicationData.Current.RoamingFolder.GetFileAsync(filename);

            double _FileAge = Math.Round(DateTime.Now.Subtract(System.IO.File.GetLastWriteTime(_File.Path)).TotalMinutes);
            int age = (int)_FileAge;
            return age;
        }
        public static async Task<bool> SaveNewsData(NewsResults _News)
        {
            try
            {
                StorageFile file = await ApplicationData.Current.LocalFolder.CreateFileAsync("News.json", CreationCollisionOption.ReplaceExisting);
                string data = JsonConvert.SerializeObject(_News);
                await FileIO.WriteTextAsync(file, data);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public static async Task<NewsResults> GetSavedNews()
        {
            NewsResults result = new NewsResults();
            try
            {
                var file = await ApplicationData.Current.LocalFolder.GetFileAsync("News.json");
                string content = await FileIO.ReadTextAsync(file);
                result = JsonConvert.DeserializeObject<NewsResults>(content);
            }
            catch (Exception)
            {
                result = null;
            }
            return result;
        }
        public static async Task DownloadAsset(string url)
        {
            try
            {
                BackgroundDownloader _downloader = new BackgroundDownloader(); ;
                String fileName = url.Substring(url.LastIndexOf("/") + 1);
                IStorageFile newFile = await DownloadsFolder.CreateFileAsync(fileName, CreationCollisionOption.GenerateUniqueName);
                var newDownload = _downloader.CreateDownload(new Uri(url), newFile);
                await newDownload.StartAsync();
            }
            catch (Exception)
            {

            }
            /*try
            {                  
                StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
                
                FileSavePicker picker = new FileSavePicker();
                picker.FileTypeChoices.Add("JPG File", new List<string>() { ".jpg" });
                StorageFile savefile = await picker.PickSaveFileAsync();
                if (savefile == null)
                    return;
                IRandomAccessStream stream = await savefile.OpenAsync(FileAccessMode.ReadWrite);
                StorageFile sampleFile = await storageFolder.CreateFileAsync("123.jpg", CreationCollisionOption.ReplaceExisting);
                var cli = new HttpClient();
                var str = await cli.GetStreamAsync(new Uri(url));
                var dst = await sampleFile.OpenStreamForWriteAsync();
                await str.AsInputStream().AsStreamForRead().CopyToAsync(dst);         
            }
            catch (Exception)
            {

            }*/
        }
    }
}
