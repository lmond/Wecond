using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wecond.Models
{
    public class PlaceResults
    {
        public List<Prediction> predictions { get; set; }
        public string status { get; set; }
    }
    public class Term
    {
        public int offset { get; set; }
        public string value { get; set; }
    }
    public class Prediction
    {
        public string description { get; set; }
        public string place_id { get; set; }
        public List<Term> terms { get; set; }
    }


    public class PlacesResults
    {
        public Result result { get; set; }
        public string status { get; set; }
    }
    public class Result
    {
        public List<AddressComponent> address_components { get; set; }
        public string formatted_address { get; set; }
        public string FormmatedName { get; set; }
        public string place_id { get; set; }
        public Geometry geometry { get; set; }
    }
    public class Geometry
    {
        public Location location { get; set; }
        public string location_type { get; set; }
    }
    public class Location
    {
        public double lat { get; set; }
        public double lng { get; set; }
    }
    public class AddressComponent
    {
        public string long_name { get; set; }
        public string short_name { get; set; }
        public List<string> types { get; set; }
    }
    public class PlacesList
    {
        public List<Result> results { get; set; }
        public string status { get; set; }
    }
    public class SinglePlace
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Altitude { get; set; }
        public string CityName { get; set; }
        public string CityDisplayName { get; set; }
        public string PlaceId { get; set; }
    }
}
