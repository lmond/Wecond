using Windows.UI.Xaml.Controls;
using System;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml;
using System.Diagnostics;
using Wecond.Models;
using Wecond.Clients;
using Windows.Networking.Connectivity;
using Windows.UI.Popups;
using Wecond.Helpers;
using Windows.Storage;
using Windows.UI.Core;
using System.Collections.Generic;
using Windows.UI.StartScreen;
using Windows.UI.ViewManagement;
using Windows.Globalization;

namespace Wecond.Views
{
    public sealed partial class ShellPage : Page
    {
        private PlaceInfo _PlaceInfo = new PlaceInfo();
        private CityData _CityData { get; set; }

        public ShellPage()
        {
            this.InitializeComponent();
            SetTheme();
        }
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            Frame PageFrame = Window.Current.Content as Frame;
            if (PageFrame == null)
                PageFrame = new Frame();

            var settings = await ApplicationData.Current.RoamingFolder.TryGetItemAsync("Settings.json") == null ? null : await UserDataHelper.GetSettings("Settings.json");
            if (settings == null)
            {
                ApplicationLanguages.PrimaryLanguageOverride = "en";
                DefaultFrame.Navigate(typeof(FirstUse));
                LoadingControl.IsLoading = false;
                return;
            }

            string Language = settings.Language ?? "en";
            ApplicationLanguages.PrimaryLanguageOverride = Language;

            bool IsConnected = NetworkInformation.GetInternetConnectionProfile() != null ? true : false;

            string TimeFormat = settings.DataFormat.TimeFormat == "24h" ? "H:mm" : "h:mmtt";
            if (IsConnected == false && e.Parameter == null)
            {
                if (await ApplicationData.Current.LocalFolder.TryGetItemAsync("HomeWeather.json") == null)
                {
                    NoInternetGrid.Visibility = Visibility.Visible;
                    return;
                }
                _CityData = await UserDataHelper.GetSavedHomeWeather();
                _CityData.IsLocalData = true;
                var _File = await ApplicationData.Current.LocalFolder.GetFileAsync("HomeWeather.json");
                _CityData.LastUpdate = System.IO.File.GetLastWriteTime(_File.Path).ToString(TimeFormat);
                PageFrame.Navigate(typeof(WeatherPage), _CityData);
                return;
            }

            if (e.Parameter != null)
            {
                if (IsConnected == true)
                {
                    _PlaceInfo = e.Parameter as PlaceInfo;
                    _CityData = (await WeatherData.GetWeather(_PlaceInfo, settings.DataFormat, Language));

                    if (_CityData.Current.cod == 200)
                    {
                        _CityData.LastUpdate = DateTime.Now.ToLocalTime().ToString(TimeFormat);
                        PageFrame.Navigate(typeof(WeatherPage), _CityData);
                        return;
                    }
                }
                NoInternetGrid.Visibility = Visibility.Visible;
            }
            else
            {
                if (settings.CheckLocation == 1)
                {
                    var coords = await Geolocation.GetLocationInfo();
                    if (coords.Latitude == 0 && coords.Longitude == 0)
                    {
                        LoadingControl.IsLoading = false;
                        NotFoundGrid.Visibility = Visibility.Visible;
                        NotFoundText.Text = "Sorry! Can't locate you!";
                        NotFoudHint.Text = "You can try searching instead";
                        return;
                    }
                    else
                    {
                        var plid = await PlacesSearch.GetCityNameByCoordinate(coords.Latitude, coords.Longitude);
                        if (plid != null)
                        {
                            _PlaceInfo = new PlaceInfo() { Latitude = plid.Latitude, Longitude = plid.Longitude, DisplayName = plid.DisplayName, PlaceId = plid.PlaceId };
                        }
                        else {
                            LoadingControl.IsLoading = false;
                            NotFoundGrid.Visibility = Visibility.Visible;
                            NotFoundText.Text = "Sorry! Can't locate you!";
                            NotFoudHint.Text = "You can try searching instead";
                            return;
                        }
                    }
                }
                else
                {
                    _PlaceInfo = new PlaceInfo { Latitude = settings.DefaultLocation.Latitude, Longitude = settings.DefaultLocation.Longitude, DisplayName = settings.DefaultLocation.DisplayName, PlaceId = settings.DefaultLocation.PlaceId };
                }

                _CityData = (await WeatherData.GetWeather(_PlaceInfo, settings.DataFormat, Language));
                _CityData.LastUpdate = DateTime.Now.ToLocalTime().ToString(TimeFormat);
                bool _SaveData = await UserDataHelper.SaveHomeWeather(_CityData);
                PageFrame.Navigate(typeof(WeatherPage), _CityData);
            }
            LoadingControl.IsLoading = false;
            /*try
            {
            }
            catch (Exception)
            {
                await new MessageDialog("Unexpected exception occurred: 7221252017").ShowAsync();
            }*/
        }
        private async void CitiesSearch_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput && sender.Text.Length > 2)
            {
                var result = (await PlacesSearch.PlaceAutocomplate(sender.Text));
                sender.ItemsSource = result;
            }
        }
        private async void CitiesSearch_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (args.ChosenSuggestion != null)
            {
                var selected = args.ChosenSuggestion as SinglePlace;
                var result = (await PlacesSearch.PlaceDetails(selected.PlaceId));
                PlaceInfo _PlaceInfo = new PlaceInfo() { Latitude = result.result.geometry.location.lat, Longitude = result.result.geometry.location.lng, DisplayName = result.result.FormmatedName, PlaceId = selected.PlaceId };
                Frame.Navigate(typeof(ShellPage), _PlaceInfo);
            }
        }
        private void ReloadApp_Click(object sender, RoutedEventArgs e)
        {
            Frame PageFrame = Window.Current.Content as Frame;
            if (PageFrame == null)
                PageFrame = new Frame();
            PageFrame.Navigate(typeof(ShellPage));
        }
        public static async void SetTheme() {
            if (await ApplicationData.Current.RoamingFolder.TryGetItemAsync("Settings.json") != null)
            {
                var settings = await UserDataHelper.GetSettings("Settings.json");
                if (Application.Current.Resources.MergedDictionaries.Count > 1)
                {
                    Application.Current.Resources.MergedDictionaries.RemoveAt(1);
                }
                if (settings.Theme != null && settings.Theme != "Colored")
                {
                    Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri("ms-appx:///Styles/" + settings.Theme + ".xaml") });
                }
                var titleBar = ApplicationView.GetForCurrentView().TitleBar;
                if (settings.Theme == "Light")
                {
                    titleBar.ButtonForegroundColor = Windows.UI.Colors.Black;
                    titleBar.ButtonBackgroundColor = Windows.UI.Colors.Transparent;
                }
                else
                {
                    titleBar.ButtonForegroundColor = Windows.UI.Colors.White;
                    titleBar.ButtonBackgroundColor = Windows.UI.Colors.Transparent;
                }
            }
        }
    }
}