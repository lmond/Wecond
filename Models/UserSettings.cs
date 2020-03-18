using System.Collections.Generic;

namespace Wecond.Models
{
    class UserData
    {
        public List<UserSettings> Settings { get; set; }
        public List<PlaceInfo> SavedPlaces{ get; set; }
    }
    class UserSettings {
        public int CheckLocation { get; set; }
        public PlaceInfo DefaultLocation { get; set; }
        public string SaveData { get; set; }
        public string Theme { get; set; }
        public DataFormat DataFormat { get; set; }
        public string Language { get; set; }
    }
    class DataFormat
    {
        public string TimeFormat { get; set; }
        public string UnitsFormat { get; set; }
    }
}
