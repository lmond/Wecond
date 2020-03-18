using System.Collections.Generic;

namespace Wecond.Models
{
    class WeatherResults
    {
        public CurrentWeatherResult Current { get; set; }
        public List<List> DailyForecast { get; set; }
        public string CoverImage { get; set; }
        public PlaceInfo PlaceInfo { get; set; }
    }
    class CityData
    {
        public CurrentWeatherResult Current { get; set; }
        public HourlyResult Hourly { get; set; }
        public DailyForecastResult DailyForecast { get; set; }        
        public string CoverImage { get; set; }
        public PlaceInfo PlaceInfo { get; set; }
        public bool IsLocalData { get; set; }
        public string LastUpdate { get; set; }
    }
    public class PlaceInfo
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string DisplayName { get; set; }
        public string PlaceId { get; set; }
    }
    public class CurrentWeatherResult
    {
        public Coord coord { get; set; }
        public List<Weather> weather { get; set; }
        public string @base { get; set; }
        public Main main { get; set; }
        public string visibility { get; set; }
        public Wind wind { get; set; }
        public Clouds clouds { get; set; }
        public int dt { get; set; }
        public Sys sys { get; set; }
        public int id { get; set; }
        public string name { get; set; }
        public int cod { get; set; }
        public string Date { get; set; }
        public string Sunrise { get; set; }
        public string Sunset { get; set; }
        public string SunriseSunset { get; set; }
        public string Image { get; set; }
        public string CoverUrl { get; set; }
        public string CoverOwnerUsername { get; set; }
        public string CoverOwnerProfileLink { get; set; }
    }
    public class Coord
    {
        public double lon { get; set; }
        public double lat { get; set; }
    }

    public class Weather
    {
        public double id { get; set; }
        public string main { get; set; }
        public string description { get; set; }
        public string icon { get; set; }
    }

    public class Main
    {
        public double temp { get; set; }
        public string pressure { get; set; }
        public double humidity { get; set; }
        public double temp_min { get; set; }
        public double temp_max { get; set; }
    }

    public class Wind
    {
        public string speed { get; set; }
        public double deg { get; set; }
    }

    public class Clouds
    {
        public double all { get; set; }
    }

    public class Sys
    {
        public double type { get; set; }
        public double id { get; set; }
        public double message { get; set; }
        public string country { get; set; }
        public double sunrise { get; set; }
        public double sunset { get; set; }
    }




    public class HourlyList
    {
        public int dt { get; set; }
        public Main main { get; set; }
        public List<Weather> weather { get; set; }
        public Clouds clouds { get; set; }
        public Wind wind { get; set; }
        public Sys sys { get; set; }
        public string dt_txt { get; set; }
        public string Date { get; set; }
        public string Image { get; set; }
    }
    public class HourlyResult
    {
        public int cod { get; set; }
        public double message { get; set; }
        public int cnt { get; set; }
        public List<HourlyList> list { get; set; }
        public City city { get; set; }
    }




    public class DailyForecastResult
    {
        public City city { get; set; }
        public int cod { get; set; }
        public double message { get; set; }
        public int cnt { get; set; }
        public List<List> list { get; set; }
    }
    public class City
    {
        public int id { get; set; }
        public string name { get; set; }
        public Coord coord { get; set; }
        public string country { get; set; }
        public double population { get; set; }
    }

    public class Temp
    {
        public double day { get; set; }
        public double min { get; set; }
        public double max { get; set; }
        public double night { get; set; }
        public double eve { get; set; }
        public double morn { get; set; }
    }

    public class List
    {
        public double dt { get; set; }
        public Temp temp { get; set; }
        public double pressure { get; set; }
        public double humidity { get; set; }
        public List<Weather> weather { get; set; }
        public double speed { get; set; }
        public double deg { get; set; }
        public double clouds { get; set; }
        public double snow { get; set; }
        public string Day { get; set; }
        public string Image { get; set; }
    }
}