using Windows.UI.Xaml.Controls;
using System;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml;
using System.Diagnostics;
using Wecond.Clients;
using Wecond.Helpers;
using System.Collections.Generic;
using Microsoft.Toolkit.Uwp.UI.Animations;
using Wecond.Models;
using Windows.Networking.Connectivity;

namespace Wecond.Views
{
    public sealed partial class FirstUse : Page
    {
        private PlaceInfo ChoosenPlace { get; set; }
        public FirstUse()
        {
            this.InitializeComponent();
            ChoosenPlace = null;
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            BackgroundGrid.Blur(20, 0, 0).StartAsync();

            var settings = new Data.Settings().GetSettings();
            var _Languages = new Data.Settings().GetSettings()[4].values;
            Languages.ItemsSource = _Languages;
            Languages.SelectedIndex =  0;
            Theme.ItemsSource = settings[0].values;
            SaveData.ItemsSource = settings[1].values;
            Units.ItemsSource = settings[2].values;
            TimeFormat.ItemsSource = settings[3].values;
        }
        private async void StartApp_Click(object sender, RoutedEventArgs e)
        {
            ErrorText.Visibility = Visibility.Collapsed;

            if (ChoosenPlace != null && SaveData.SelectedIndex != -1 && Theme.SelectedIndex != -1 && Units.SelectedIndex != -1 && TimeFormat.SelectedIndex != -1 && Languages.SelectedIndex != -1)
            {
                var checkLocation = AlwaysDetect.IsChecked == true ? 1 : 0;
                var theme = (Theme.SelectedItem as Data.ComboBoxItem).Key;
                var units = (Units.SelectedItem as Data.ComboBoxItem).Key;
                var time = (TimeFormat.SelectedItem as Data.ComboBoxItem).Key;
                var savedata = (SaveData.SelectedItem as Data.ComboBoxItem).Key;
                
                UserSettings _Settings = new UserSettings();
                _Settings = new UserSettings() {
                    Theme = theme,
                    Language = (Languages.SelectedItem as Data.ComboBoxItem).Key,
                    CheckLocation = checkLocation,
                    DataFormat =  new DataFormat() {
                        UnitsFormat = units,
                        TimeFormat = time
                    },
                    SaveData = savedata,
                    DefaultLocation = ChoosenPlace
                };

                var WriteToFile = (await UserDataHelper.WriteFile("Settings.json", _Settings));
                if (WriteToFile == true) {
                    Frame.Navigate(typeof(ShellPage));
                }
            }
            else
            {
                ErrorText.Visibility = Visibility.Visible;
            }
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

        private async void DetectLocation_Click(object sender, RoutedEventArgs e)
        {
            LocationInfoStack.Visibility = Visibility.Visible;
            LocationInfoText.Text = "Locating Your device";
            LocationProgress.Visibility = Visibility.Visible;

            var coords = await Geolocation.GetLocationInfo();
            if (coords.Latitude == 0 && coords.Longitude == 0)
            {
                LocationProgress.Visibility = Visibility.Collapsed;
                LocationInfoText.Text = "Can't Locate You!";
            }
            else
            {
                if (NetworkInformation.GetInternetConnectionProfile() != null)
                {
                    var plid = await PlacesSearch.GetCityNameByCoordinate(coords.Latitude, coords.Longitude);
                    if (plid != null)
                    {
                        ChoosenPlace = new PlaceInfo() { Latitude = plid.Latitude, Longitude = plid.Longitude, DisplayName = plid.DisplayName, PlaceId = plid.PlaceId };
                        DefaultLocation.Text = plid.DisplayName;
                    }
                    else {
                        LocationInfoText.Text = "Can't Locate You!";
                    }
                }
                else {
                    LocationInfoText.Text = "Can't Locate You!";
                }                
                
                LocationInfoStack.Visibility = Visibility.Collapsed;
            }
        }
    }
}
