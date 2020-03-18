using System;
using System.Collections.Generic;

namespace Wecond.Data
{
    class ComboBoxItem
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
    class SettingsData
    {
        public string name { get; set; }
        public List<ComboBoxItem> values { get; set; }
    }
    class Settings
    {
        public List<SettingsData> GetSettings() {
            List<ComboBoxItem> themes = new List<ComboBoxItem>() { new ComboBoxItem() { Key = "Colored", Value = "Colored" }, new ComboBoxItem() { Key = "Light", Value = "Light" }, new ComboBoxItem() { Key = "Dark", Value = "Dark" } };
            List<ComboBoxItem> savedata = new List<ComboBoxItem>() { new ComboBoxItem() { Key = "0", Value = "No" }, new ComboBoxItem() { Key = "1", Value = "Yes" } };
            List<ComboBoxItem> unitsformat = new List<ComboBoxItem>() { new ComboBoxItem() { Key = "metric", Value = "Metric" }, new ComboBoxItem() { Key = "imperial", Value = "Imperial" } };
            List<ComboBoxItem> timeformat = new List<ComboBoxItem>() { new ComboBoxItem() { Key = "24h", Value = "24h" }, new ComboBoxItem() { Key = "12h", Value = "12h" } };
            List<ComboBoxItem> languages = new List<ComboBoxItem>() { new ComboBoxItem() { Key = "en", Value = "English" }, new ComboBoxItem() { Key = "ru", Value = "Русский" } };

            List<SettingsData> settings = new List<SettingsData>() {
                new SettingsData() { name = "Themes", values = themes },
                new SettingsData() { name = "SaveData", values = savedata },
                new SettingsData() { name = "UnitsFormat", values = unitsformat },
                new SettingsData() { name = "TimeFormat", values = timeformat },
                new SettingsData() { name = "Languages", values = languages }
            };
            return settings;
        }
    }
}
