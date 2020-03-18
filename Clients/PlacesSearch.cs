using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Wecond.Models;

namespace Wecond.Clients
{
    class PlacesSearch
    {
        public static async Task<List<SinglePlace>> PlaceAutocomplate(string term)
        {
            List<SinglePlace> result = new List<SinglePlace>();
            var url = "https://maps.googleapis.com/maps/api/place/autocomplete/json?input=" + term + "&types=(cities)&key=AIzaSyAGI9M_4xntR5e17s3FM62pxst17ENa-a4";
            try
            {
                var client = new HttpClient();
                var response = await client.GetStringAsync(string.Format(url));
                var results = JsonConvert.DeserializeObject<PlaceResults>(response);

                foreach (var item in results.predictions) {
                    result.Add(new SinglePlace() { CityName = item.terms[0].value + ", " + item.terms[item.terms.Count - 1].value, CityDisplayName = item.description, PlaceId = item.place_id });
                }
            }
            catch (Exception)
            {
                
            }
            return result;
        }
        public static async Task<PlacesResults> PlaceDetails(string PlaceId)
        {
            PlacesResults result = null;
            var url = "https://maps.googleapis.com/maps/api/place/details/json?placeid=" + PlaceId + "&key=AIzaSyAGI9M_4xntR5e17s3FM62pxst17ENa-a4";
            try
            {
                var client = new HttpClient();
                var response = await client.GetStringAsync(string.Format(url));
                result = JsonConvert.DeserializeObject<PlacesResults>(response);

                result.result.FormmatedName = result.result.address_components[0].short_name;
                foreach (var item in result.result.address_components) {
                    if (item.types[0] == "country")
                    {
                        result.result.FormmatedName += ", " + item.short_name;
                    }
                }
            }
            catch (Exception)
            {

            }
            return result;
        }
        public static async Task<PlaceInfo> GetCityNameByCoordinate(double Latitude, double Longitude) {
            PlaceInfo result = null;
            var _Latitude = Latitude.ToString();
            var _Longitude = Longitude.ToString();
            if (_Latitude.Contains(","))
                _Latitude = _Latitude.Replace(",", ".");
            if (_Longitude.Contains(","))
                _Longitude = _Longitude.Replace(",", ".");
            try
            {
                var client = new HttpClient();
                var url = "https://maps.googleapis.com/maps/api/geocode/json?latlng=" + _Latitude + "," + _Longitude + "&result_type=locality&sensor=true&key=AIzaSyAGI9M_4xntR5e17s3FM62pxst17ENa-a4&types=(cities)";
                var response = await client.GetStringAsync(string.Format(url));
                var results = JsonConvert.DeserializeObject<PlacesList>(response);

                results.results[results.results.Count - 1].formatted_address = results.results[results.results.Count - 1].address_components[0].short_name;
                foreach (var item in results.results[results.results.Count - 1].address_components)
                {
                    if (item.types[0] == "country")
                    {
                        results.results[results.results.Count - 1].formatted_address += ", " + item.short_name;
                    }
                }

                result = new PlaceInfo() { DisplayName = results.results[results.results.Count - 1].formatted_address, Latitude = Latitude, Longitude = Longitude, PlaceId = results.results[results.results.Count - 1].place_id };
            }
            catch (Exception) {

            }
            return result;
        }
    }
}
