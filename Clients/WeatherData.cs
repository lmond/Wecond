using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Wecond.Models;
using Windows.ApplicationModel.Resources;
using Windows.Storage;

namespace Wecond.Clients
{
    class WeatherData
    {
        public static async Task<CityData> GetWeather(PlaceInfo _PlaceInfo, DataFormat _DataFormat, string Language) {
            string url = string.Empty;
            string buildUrl = string.Empty;
            string dataType = string.Empty;
            CityData _CityData = new CityData() { PlaceInfo = _PlaceInfo};

            if (_PlaceInfo.Latitude != 0 && _PlaceInfo.Longitude != 0)
                buildUrl = "lat=" + _PlaceInfo.Latitude + "&lon=" + _PlaceInfo.Longitude;
            else buildUrl = "q=" + _PlaceInfo.DisplayName;

            dataType = "weather?";
            url = "http://api.openweathermap.org/data/2.5/" + dataType + buildUrl + "&lang=" + Language + "&units=" + _DataFormat.UnitsFormat + "&type=like&appid=0b674dcfb7fe95180a52c68c15b40809";
            _CityData.Current = await GetCurrentData(url);

            if (_CityData.Current.cod != 404) {
                dataType = "forecast?";
                url = "http://api.openweathermap.org/data/2.5/" + dataType + buildUrl + "&lang=" + Language + "&units=" + _DataFormat.UnitsFormat + "&cnt=9&appid=0b674dcfb7fe95180a52c68c15b40809";
                _CityData.Hourly = await GetHourlyResult(url);
                if (_CityData.Hourly.cod != 404)
                {
                    dataType = "forecast/daily?";
                    url = "http://api.openweathermap.org/data/2.5/" + dataType + buildUrl + "&lang=" + Language + "&units=" + _DataFormat.UnitsFormat + "&cnt=10&type=like&appid=0b674dcfb7fe95180a52c68c15b40809";
                    _CityData.DailyForecast = await GetForecatData(url);
                }
            }
            _CityData = await FormatResult(_CityData, _DataFormat);
            return _CityData;
        }
        public static async Task<CurrentWeatherResult> GetCurrentData(string url)
        {
            CurrentWeatherResult result = null;
            try
            {
                var client = new HttpClient();
                var response = await client.GetStringAsync(string.Format(url));
                result = JsonConvert.DeserializeObject<CurrentWeatherResult>(response);
            }
            catch (Exception)
            {
                result = new CurrentWeatherResult() { cod = 404 };
            }
            return result;
        }
        public static async Task<DailyForecastResult> GetForecatData(string url)
        {
            DailyForecastResult result = null;
            try
            {
                var client = new HttpClient();
                var response = await client.GetStringAsync(string.Format(url));
                result = JsonConvert.DeserializeObject<DailyForecastResult>(response);
            }
            catch (Exception)
            {
                result = new DailyForecastResult() { cod = 404 };
            }
            return result;
        }
        public static async Task<HourlyResult> GetHourlyResult(string url)
        {
            HourlyResult result = null;
            try
            {
                var client = new HttpClient();
                var response = await client.GetStringAsync(string.Format(url));
                result = JsonConvert.DeserializeObject<HourlyResult>(response);
            }
            catch (Exception)
            {
                result = new HourlyResult() { cod = 404 };
            }
            return result;
        }
        private static async Task<CityData> FormatResult(CityData _CityData, DataFormat _DataFormat)
        {
            var loader = new ResourceLoader();
            var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var _Visibility = Convert.ToDouble(_CityData.Current.visibility) / 1000;

            _CityData.Current.Date = dateTime.AddSeconds(_CityData.Current.dt).ToLocalTime().ToString("ddd, dd MMM");

            if (_DataFormat.TimeFormat == "24h")
                _CityData.Current.SunriseSunset = dateTime.AddSeconds(_CityData.Current.sys.sunrise).ToLocalTime().ToString("H:mm") + "/" + dateTime.AddSeconds(_CityData.Current.sys.sunset).ToLocalTime().ToString("H:mm");
            else
                _CityData.Current.SunriseSunset = dateTime.AddSeconds(_CityData.Current.sys.sunrise).ToLocalTime().ToString("h:mmtt") + "/" + dateTime.AddSeconds(_CityData.Current.sys.sunset).ToLocalTime().ToString("h:mmtt");

            if (_DataFormat.UnitsFormat == "metric") {
                _CityData.Current.wind.speed = _CityData.Current.wind.speed + loader.GetString("MeterSec");
                _CityData.Current.visibility = _Visibility + loader.GetString("Kilometer");
            } else {
                _CityData.Current.wind.speed = _CityData.Current.wind.speed + loader.GetString("MileHour");
                _CityData.Current.visibility = Math.Round(_Visibility * 0.62, 0) + loader.GetString("Mile");
            }

            StorageFolder _assets = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync("Assets");
            StorageFolder _icons = await _assets.GetFolderAsync("Icons");

            _CityData.Current.main.temp = Math.Round(_CityData.Current.main.temp, 0);
            _CityData.Current.main.pressure = _CityData.Current.main.pressure + loader.GetString("PressureUnit");
            _CityData.Current.weather[0].description = char.ToUpper(_CityData.Current.weather[0].description[0]) + _CityData.Current.weather[0].description.Substring(1);
            if (await _icons.TryGetItemAsync(_CityData.Current.weather[0].icon + ".png") != null)
            {
                _CityData.Current.Image = "ms-appx:///Assets/Icons/" + _CityData.Current.weather[0].icon + ".png";
            }
            else {
                _CityData.Current.Image = "ms-appx:///Assets/Icons/02n.png";
            }

            foreach (var item in _CityData.Hourly.list)
            {
                item.Date = dateTime.AddSeconds(item.dt).ToLocalTime().ToString("ddd, H:mm");
                if (await _icons.TryGetItemAsync(item.weather[0].icon + ".png") != null)
                {
                    item.Image = "ms-appx:///Assets/Icons/" + item.weather[0].icon + ".png";
                }
                else {
                    item.Image = "ms-appx:///Assets/Icons/02n.png";
                }
                    
                item.main.temp = Math.Round(item.main.temp, 1);
                item.weather[0].description = char.ToUpper(item.weather[0].description[0]) + item.weather[0].description.Substring(1);
            }

            foreach (var item in _CityData.DailyForecast.list)
            {
                item.temp.min = Math.Round(item.temp.min, 0);
                item.temp.max = Math.Round(item.temp.max, 0);
                item.Day = dateTime.AddSeconds(item.dt).ToLocalTime().ToString("ddd");
                if (await _icons.TryGetItemAsync(item.weather[0].icon + ".png") != null)
                {
                    item.Image = "ms-appx:///Assets/Icons/" + item.weather[0].icon + ".png";
                }
                else {
                    item.Image = "ms-appx:///Assets/Icons/02n.png";
                }
                
                item.weather[0].description = char.ToUpper(item.weather[0].description[0]) + item.weather[0].description.Substring(1);
            }

            return _CityData;
        }
    }
}