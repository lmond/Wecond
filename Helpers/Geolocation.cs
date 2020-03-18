using System;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Wecond.Models;
using System.Diagnostics;

namespace Wecond.Helpers
{
    class Geolocation
    {
        public static async Task<PlaceInfo> GetLocationInfo() {
            double _Latitude = 0;
            double _Longitude = 0;
            Geoposition pos = null;

            var accessStatus = await Geolocator.RequestAccessAsync();
            switch (accessStatus)
            {
                case GeolocationAccessStatus.Allowed:
                    Geolocator geolocator = new Geolocator { DesiredAccuracyInMeters = 0, DesiredAccuracy = PositionAccuracy.Default };
                    pos = await geolocator.GetGeopositionAsync();
                    break;
                case GeolocationAccessStatus.Denied:
                    break;
                case GeolocationAccessStatus.Unspecified:
                    break;
            }
            if (pos != null)
            {
                _Latitude = pos.Coordinate.Point.Position.Latitude;
                _Longitude = pos.Coordinate.Point.Position.Longitude;
            }

            var result = new PlaceInfo { Latitude = _Latitude, Longitude = _Longitude};
            
            return result;
        }
    }
}
