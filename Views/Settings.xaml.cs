using System.Linq;
using Wecond.Clients;
using Wecond.Helpers;
using Wecond.Models;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Wecond.Views
{
    public sealed partial class Settings : Page
    {
        public Settings()
        {
            this.InitializeComponent();
        }
        private PlaceInfo ChoosenPlace { get; set; }
        private string _Themes { get; set; }
        private string _SaveData { get; set; }
        private string _UnitsFormat { get; set; }
        private string _TimeFormat { get; set; }
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            var settings = await UserDataHelper.GetSettings("Settings.json");

            DefaultLocation.Text = settings.DefaultLocation.DisplayName;
            ChoosenPlace = settings.DefaultLocation;

            var _Languages = new Data.Settings().GetSettings()[4].values;
            Languages.ItemsSource = _Languages;
            Languages.SelectedIndex = settings.Language != null ? _Languages.IndexOf(_Languages.Where(x => x.Key == settings.Language).ToList()[0]) : 0; 
            if (settings.CheckLocation == 1) AlwaysDetect.IsChecked = true;
            if (settings.Theme == "Colored") Themes0.IsChecked = true;
            else if (settings.Theme == "Light") Themes1.IsChecked = true;
            else Themes2.IsChecked = true;
            if (settings.SaveData == "0") SaveData0.IsChecked = true;
            else SaveData1.IsChecked = true;
            if (settings.DataFormat.UnitsFormat == "metric") Units0.IsChecked = true;
            else Units1.IsChecked = true;
            if (settings.DataFormat.TimeFormat == "24h") TimeFormat0.IsChecked = true;
            else TimeFormat1.IsChecked = true;
        }
        private async void DefaultLocation_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput && sender.Text.Length > 2)
            {
                var results = (await PlacesSearch.PlaceAutocomplate(sender.Text));
                sender.ItemsSource = results.ToArray();
            }
        }
        private async void DefaultLocation_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            if (args.SelectedItem != null)
            {
                var selected = args.SelectedItem as SinglePlace;
                var result = (await PlacesSearch.PlaceDetails(selected.PlaceId));
                PlaceInfo _PlaceInfo = new PlaceInfo() { Latitude = result.result.geometry.location.lat, Longitude = result.result.geometry.location.lng, DisplayName = result.result.FormmatedName, PlaceId = selected.PlaceId };
                ChoosenPlace = _PlaceInfo;
            }
        }
        private void RadioButtonChecked(object sender, RoutedEventArgs e)
        {
            string groupName = (sender as RadioButton).GroupName.ToString();
            switch (groupName) {
                case "Themes":
                    _Themes = (sender as RadioButton).Tag.ToString();
                    break;
                case "SaveData":
                    _SaveData = (sender as RadioButton).Tag.ToString();
                    break;
                case "UnitsFormat":
                    _UnitsFormat = (sender as RadioButton).Tag.ToString();
                    break;
                case "TimeFormat":
                    _TimeFormat = (sender as RadioButton).Tag.ToString();
                    break;
            }
        }
        private async void SaveData_Click(object sender, RoutedEventArgs e)
        {
            SavedText.Visibility = Visibility.Collapsed;
            if (ChoosenPlace != null && _Themes != null && _SaveData != null && _UnitsFormat != null && _TimeFormat != null && Languages.SelectedIndex != -1)
            {
                UserSettings _Settings = new UserSettings();
                var checkLocation = AlwaysDetect.IsChecked == true ? 1 : 0;
                _Settings = new UserSettings()
                {
                    Theme = _Themes,
                    Language = (Languages.SelectedItem as Data.ComboBoxItem).Key,
                    CheckLocation = checkLocation,
                    DataFormat = new DataFormat()
                    {
                        UnitsFormat = _UnitsFormat,
                        TimeFormat = _TimeFormat
                    },
                    SaveData = _SaveData,
                    DefaultLocation = ChoosenPlace
                };

                var WriteToFile = (await UserDataHelper.WriteFile("Settings.json", _Settings));
                WeatherPage.IsSettingsChanged = true;
                SavedText.Visibility = Visibility.Visible;
                DataError.Visibility = Visibility.Collapsed;
            }
            else {
                SavedText.Visibility = Visibility.Collapsed;
                DataError.Visibility = Visibility.Visible;
            }
        }
        private async void DetectLocation_Click(object sender, RoutedEventArgs e)
        {
            LocationInfoStack.Visibility = Visibility.Visible;
            LocatingYou.Visibility = Visibility.Visible;
            LocationProgress.Visibility = Visibility.Visible;

            var coords = await Geolocation.GetLocationInfo();
            if (coords.Latitude == 0 && coords.Longitude == 0)
            {
                LocationProgress.Visibility = Visibility.Collapsed;
                LocatingYou.Visibility = Visibility.Collapsed;
                CantLocateYou.Visibility = Visibility.Visible;
            }
            else
            {
                var plid = await PlacesSearch.GetCityNameByCoordinate(coords.Latitude, coords.Longitude);
                if (plid != null)
                {
                    ChoosenPlace = new PlaceInfo() { Latitude = plid.Latitude, Longitude = plid.Longitude, DisplayName = plid.DisplayName, PlaceId = plid.PlaceId };
                    DefaultLocation.Text = plid.DisplayName;
                    LocationInfoStack.Visibility = Visibility.Collapsed;
                }
                else {
                    CantLocateYou.Visibility = Visibility.Visible;
                }
            }
        }
    }
}
