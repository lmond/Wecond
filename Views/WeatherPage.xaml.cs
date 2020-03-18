using Windows.UI.Xaml.Controls;
using System;
using Windows.UI.Xaml.Navigation;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using System.Diagnostics;
using Wecond.Models;
using Wecond.Clients;
using Wecond.Helpers;
using System.Linq;
using Windows.UI.Xaml.Media.Imaging;
using Microsoft.Toolkit.Uwp.Helpers;
using Microsoft.Toolkit.Uwp.UI.Animations;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using Windows.Storage.Pickers;
using Windows.Storage;
using Windows.UI.StartScreen;
using Windows.UI.Notifications;
using Windows.UI.Core;
using System.Collections.Generic;
using Windows.ApplicationModel.Resources;
using Windows.System;

namespace Wecond.Views
{
    public sealed partial class WeatherPage : Page
    {
        private static CityData _CityData { get; set; }
        private static NewsResults _NewsResults { get; set; }
        private static UserSettings _Settings { get; set; }
        private static DailyForecastResult DailyForecastResult { get; set; }
        private static CurrentWeatherResult CurrentWeatherResult { get; set; }
        private static string CoverLink { get; set; }
        private static PlaceInfo PlaceInfo { get; set; }
        public static FlickrImageResults _FlickrImages { get; set; }
        public static bool IsSettingsChanged { get; set; }
        public static bool IsFavoritesChanged { get; set; }
        public static List<PlaceInfo> _LocationHistory { get; set; }

        public WeatherPage()
        {
            this.InitializeComponent();
            string DeviceFamily = SystemInformation.DeviceFamily;
            if (DeviceFamily == "Windows.Mobile")
            {
                MobileDeviceLayout();
                this.SizeChanged += (object sender, SizeChangedEventArgs e) => MobileDeviceLayout();
            }
            this.SizeChanged += (object sender, SizeChangedEventArgs e) => PageSizeChnaged();
            SystemNavigationManager.GetForCurrentView().BackRequested += App_BackRequested;
        }
        private void MobileDeviceLayout()
        {
            AppNameGrid.Visibility = Visibility.Collapsed;
            DisplayInformation displayInfo = DisplayInformation.GetForCurrentView();
            if (displayInfo.CurrentOrientation == DisplayOrientations.Portrait)
            {
                PageGrid.Margin = new Thickness(0, -45, 0, 0);
                TopBarContent.Margin = new Thickness(0);
                //CoverCoprightInfoButton.Margin = new Thickness(20, 10, 20, 10);
            }
            else if (displayInfo.CurrentOrientation == DisplayOrientations.Landscape)
            {
                PageGrid.Margin = new Thickness(-45, -40, 0, 0);
                TopBarContent.Margin = new Thickness(15, 0, 0, 0);
                //CoverCoprightInfoButton.Margin = new Thickness(35, 10, 20, 10);
                CitiesSearch.Text = CurrentPlaceName.Text;
            }
            else if (displayInfo.CurrentOrientation == DisplayOrientations.LandscapeFlipped)
            {
                PageGrid.Margin = new Thickness(0, -40, -45, 0);
                TopBarContent.Margin = new Thickness(0, 0, 15, 0);
                //CoverCoprightInfoButton.Margin = new Thickness(20, 10, 20, 10);
                CitiesSearch.Text = CurrentPlaceName.Text;
            }
        }
        private void PageSizeChnaged()
        {
            var bounds = ApplicationView.GetForCurrentView().VisibleBounds;
            HourlyItemsPanel.Width = ForecastItemsPanel.Width = NewsItemsPanel.Width = GalleryItemsPanel.Width = bounds.Width / 3;
        }
        private void App_BackRequested(object sender, BackRequestedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            if (e.Handled == true)
                return;
            if (_LocationHistory.Count > 1)
            {
                rootFrame.Navigate(typeof(ShellPage), _LocationHistory[_LocationHistory.Count - 2]);
                _LocationHistory.Remove(_LocationHistory[_LocationHistory.Count - 1]);
                e.Handled = true;
            }
        }
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            _CityData = e.Parameter as CityData;

            /*set country flag*/
            /*string _CountryCode = _CityData.PlaceInfo.DisplayName;
            _CountryCode = _CountryCode.Substring(_CountryCode.LastIndexOf(",") + 2);
            StorageFolder _assets = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync("Assets");
            StorageFolder _flags = await _assets.GetFolderAsync("CountriesFlag");

            if (_flags.TryGetItemAsync(_CountryCode + ".png") != null)
            {
                CurrentPlaceName.Text = _CityData.PlaceInfo.DisplayName.Substring(0, _CityData.PlaceInfo.DisplayName.LastIndexOf(","));
                BitmapImage image = new BitmapImage(new Uri("ms-appx:///Assets/CountriesFlag/" + _CountryCode + ".png"));
                CountryFlag.Source = image;
            }
            else {
                CurrentPlaceName.Text = _CityData.PlaceInfo.DisplayName;
            }*/
            /*set country flag*/

            CurrentPlaceName.Text = _CityData.PlaceInfo.DisplayName;

            this.DataContext = _CityData;
            DailyForecastResult = _CityData.DailyForecast;
            CurrentWeatherResult = _CityData.Current;
            PlaceInfo = _CityData.PlaceInfo;
            CoverLink = _CityData.CoverImage;

            /*set back navigation arguments*/
            if (_LocationHistory == null) {
                _LocationHistory = new List<PlaceInfo>();
                _LocationHistory.Add(_CityData.PlaceInfo);
            }

            if (_LocationHistory[_LocationHistory.Count - 1].PlaceId != _CityData.PlaceInfo.PlaceId)
                _LocationHistory.Add(_CityData.PlaceInfo);
                                                          
            if (_LocationHistory.Count > 1)
            {
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
                AppNameGrid.Margin = new Thickness(55, 10, 0, 10);
            }
            else {
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
                AppNameGrid.Margin = new Thickness(20, 10, 0, 10);
            }
            /*set back navigation arguments*/
            _Settings = await UserDataHelper.GetSettings("Settings.json");

            if (_Settings.SaveData == "1" || _CityData.IsLocalData == true) { 
                BottomItemsPivot.Items.Remove(BottomItemsPivot.Items.Single(p => ((PivotItem)p).Name == "NewsPivot"));
                BottomItemsPivot.Items.Remove(BottomItemsPivot.Items.Single(p => ((PivotItem)p).Name == "GalleryPivot"));
                _CityData.CoverImage = "ms-appx:///Assets/Weather/11.jpg";
                if (_CityData.CoverImage != null) {
                    BitmapImage image = new BitmapImage(new Uri(_CityData.CoverImage));
                    Cover.Source = image;
                }
            }
            else {
                int _CheckFileAge = await ApplicationData.Current.LocalFolder.TryGetItemAsync("News.json") != null ? await UserDataHelper.CheckFileAge("News.json", "local") : 1441;
                string Language = _Settings.Language ?? "en";

                if (_CheckFileAge >= 1440)
                {
                    _NewsResults = await News.GetNewsData(Language);
                    bool _SaveNews = await UserDataHelper.SaveNewsData(_NewsResults);
                }
                else {
                    _NewsResults = await UserDataHelper.GetSavedNews();
                    if (_NewsResults.Language != _Settings.Language) { 
                        _NewsResults = await News.GetNewsData(Language);
                        bool _SaveNews = await UserDataHelper.SaveNewsData(_NewsResults);
                    }
                }
                NewsData.ItemsSource = _NewsResults.articles;

                await GetCover(_CityData.PlaceInfo.DisplayName);
            }

            CoverLink = _CityData.CoverImage;

            var favorites = (await UserDataHelper.GetFavorites("Favorites.json"));
            var _CurrentPlace = favorites.Where(x => x.PlaceId == _CityData.PlaceInfo.PlaceId).ToList();
            if (_CurrentPlace.Count > 0)
            {
                SaveToFavorites.Visibility = Visibility.Collapsed;
                RemoveFromFavorites.Visibility = Visibility.Visible;
            }

            SetAsHome.IsEnabled = _CityData.PlaceInfo.PlaceId == _Settings.DefaultLocation.PlaceId ? false : true;

            if (SecondaryTile.Exists(_CityData.PlaceInfo.PlaceId))
            {
                PinToStart.Visibility = Visibility.Collapsed;
                UnPinFromStart.Visibility = Visibility.Visible;
                await LiveTile.UpdateCustomTile(PlaceInfo, CurrentWeatherResult, DailyForecastResult, CoverLink);
            }

            if (_CityData.PlaceInfo.PlaceId == _Settings.DefaultLocation.PlaceId)
                LiveTile.UpdateTile(PlaceInfo, CurrentWeatherResult, DailyForecastResult, CoverLink);    

            if(favorites.ToList().Where(x => x.PlaceId == _CityData.PlaceInfo.PlaceId).FirstOrDefault() != null)
                await UserDataHelper.SaveFavoriteWeather(_CityData);
        }
        private async Task GetCover(string PlaceName)
        {
            _FlickrImages = (await FlickrData.GetFlickrImages(PlaceName));
            if (_FlickrImages.stat == "ok")
            {
                //new Random().Next(_FlickrImages.query.results.photo.Count);
                var CoverObj = _FlickrImages.photos.photo[new Random().Next(_FlickrImages.photos.photo.Count)];
                SetCover(CoverObj);
                //CoverCoprightInfoButton.Visibility = Visibility.Visible;

                _CityData.CoverImage = CoverObj.ImageAddress;
                CityImages.ItemsSource = _FlickrImages.photos.photo;
            }
        }
        public void SetCover(Photo CoverObj)
        {
            Cover.Source = new BitmapImage();
            /*var t = Task.Run(() => FlickrData.GetFlickrImageOwnerInfo(CoverObj.owner));
            t.Wait();*/

            BitmapImage image = new BitmapImage(new Uri(CoverObj.ImageAddress));
            Cover.Source = image;
            //Cover.Blur(0.5, 0, 0).StartAsync();

            /*UserLink.Content = t.Result.query.results.person.username;
            UserLink.NavigateUri = new Uri(t.Result.query.results.person.profileurl);*/
        }
        private void CityImages_ItemClick(object sender, ItemClickEventArgs e)
        {
            SetCover(e.ClickedItem as Photo);
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
            if (args.ChosenSuggestion != null) {
                var selected = args.ChosenSuggestion as SinglePlace;
                var result = (await PlacesSearch.PlaceDetails(selected.PlaceId));
                PlaceInfo _PlaceInfo = new PlaceInfo() {
                    Latitude = result.result.geometry.location.lat,
                    Longitude = result.result.geometry.location.lng,
                    DisplayName = result.result.FormmatedName,
                    PlaceId = result.result.place_id
                };
                Frame.Navigate(typeof(ShellPage), _PlaceInfo);
            }
        }
        private void GoToHome_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(ShellPage));
        }
        private void NewsData_ItemClick(object sender, ItemClickEventArgs e)
        {
            var uri = new Uri((e.ClickedItem as Article).url);
            SettingsPageGrid.Visibility = Visibility.Visible;
            SettingsPage.Navigate(typeof(NewsView), uri);
        }
        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            SettingsPageGrid.Visibility = Visibility.Visible;
            SettingsPage.Navigate(typeof(Settings));
        }
        private void FavoritesButton_Click(object sender, RoutedEventArgs e)
        {
            SettingsPageGrid.Visibility = Visibility.Visible;
            SettingsPage.Navigate(typeof(Favorites));
        }
        private async void CloseSettings_Click(object sender, RoutedEventArgs e)
        {
            SettingsPageGrid.Visibility = Visibility.Collapsed;
            SettingsPage.Content = "";
            if (IsFavoritesChanged == true) {
                IsFavoritesChanged = false;
                var favorites = (await UserDataHelper.GetFavorites("Favorites.json"));

                var _CurrentPlace = favorites.Where(x => x.PlaceId == _CityData.PlaceInfo.PlaceId).ToList();
                if (_CurrentPlace.Count == 0)
                {
                    SaveToFavorites.Visibility = Visibility.Visible;
                    RemoveFromFavorites.Visibility = Visibility.Collapsed;
                }
            }
            if (IsSettingsChanged == true) {
                IsSettingsChanged = false;
                PlaceInfo _argument = null;
                var rootFrame = Window.Current.Content as Frame;
                if (rootFrame == null)
                    rootFrame = new Frame();

                if (_CityData.PlaceInfo.PlaceId != _Settings.DefaultLocation.PlaceId)
                    _argument = _CityData.PlaceInfo;
                rootFrame.Navigate(typeof(ShellPage), _argument);
            }
        }
        private async void AvailableActions_ItemClick(object sender, ItemClickEventArgs e)
        {
            switch ((e.ClickedItem as StackPanel).Tag.ToString()) {
                case "SetHome":
                    var settings = await UserDataHelper.GetSettings("Settings.json");
                    settings.DefaultLocation = _CityData.PlaceInfo;
                    await UserDataHelper.WriteFile("Settings.json", settings);
                    SetAsHome.IsEnabled = false;
                    ShowToolTip("SavedAsHome", 2000);
                    break;

                case "Favorite":
                    var IsFavorited = (await UserDataHelper.SaveToFavorites("Favorites.json", _CityData.PlaceInfo));
                    if (IsFavorited == true)
                    {
                        await UserDataHelper.SaveFavoriteWeather(_CityData);
                        SaveToFavorites.Visibility = Visibility.Collapsed;
                        RemoveFromFavorites.Visibility = Visibility.Visible;
                        ShowToolTip("Favorited", 2000);
                    }
                    break;

                case "UnFavorite":
                    var IsUnfavorited = (await UserDataHelper.RemoveFromFavorites("Favorites.json", _CityData.PlaceInfo));
                    if (IsUnfavorited == true)
                    {
                        await UserDataHelper.RemoveFavoriteWeather(_CityData.PlaceInfo.PlaceId);
                        SaveToFavorites.Visibility = Visibility.Visible;
                        RemoveFromFavorites.Visibility = Visibility.Collapsed;
                        ShowToolTip("Unfavorited", 2000);
                    }
                    break;

                case "Pin":
                    ShowToolTip("Pinning", 2000);
                    var IsPined = await LiveTile.UpdateCustomTile(PlaceInfo, CurrentWeatherResult, DailyForecastResult, CoverLink);
                    if (IsPined == true)
                    {
                        PinToStart.Visibility = Visibility.Collapsed;
                        UnPinFromStart.Visibility = Visibility.Visible;
                        ShowToolTip("Pined", 2000);
                    }
                    break;

                case "UnPin":
                    ShowToolTip("Unpinning", 2000);
                    var IsUnPined =  await LiveTile.UnpinCustomTile(_CityData.PlaceInfo.PlaceId);
                    if (IsUnPined == true) {
                        PinToStart.Visibility = Visibility.Visible;
                        UnPinFromStart.Visibility = Visibility.Collapsed;
                        ShowToolTip("Unpined", 2000);
                    }
                    break;

                case "Download":
                    ShowToolTip("DownloadingImage", 2000);
                    BitmapImage bitMap = Cover.Source as BitmapImage;
                    string uri = bitMap?.UriSource.ToString();
                    await UserDataHelper.DownloadAsset(uri);
                    ShowToolTip("Downloaded", 2000);
                    break;

                case "Rate":
                    await Launcher.LaunchUriAsync(new Uri(string.Format("ms-windows-store:REVIEW?PFN={0}", Windows.ApplicationModel.Package.Current.Id.FamilyName)));
                    break;

               case "Feedback":
                    await Microsoft.Services.Store.Engagement.StoreServicesFeedbackLauncher.GetDefault().LaunchAsync();
                    break;

                default:
                    break;
            }
        }
        private async void DownloadImage_Click(object sender, RoutedEventArgs e)
        {
            ShowToolTip("DownloadingImage", 2000);
            var url = (sender as Button).Tag.ToString();
            await UserDataHelper.DownloadAsset(url);
            ShowToolTip("Downloaded", 2000);
        }
        private async void ShowToolTip(string _ResourceKey, int _Delay) {
            var loader = new ResourceLoader();
            ActionText.Text = loader.GetString(_ResourceKey);
            ActionTextGrid.Visibility = Visibility.Visible;
            await Task.Delay(_Delay);
            ActionTextGrid.Visibility = Visibility.Collapsed;
        }
    }
}