using Wecond.Helpers;
using Wecond.Models;
using Windows.UI.Xaml;
using System.Linq;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Wecond.Views
{
    public sealed partial class Favorites : Page
    {
        private static List<CityData> FavoritePlaces { get; set; }

        public Favorites()
        {
            this.InitializeComponent();
        }
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            var favorites = (await UserDataHelper.GetFavorites("Favorites.json"));

            FavoritePlaces = new List<CityData>();
            foreach (var item in favorites) {
                var _data = await UserDataHelper.GetFavoriteWeather(item.PlaceId);
                if (_data != null)
                    FavoritePlaces.Add(_data);
                else
                    FavoritePlaces.Add(new CityData() { CoverImage = "ms-appx:///Assets/Weather/11.jpg", PlaceInfo = item });
            }
            var _Count = FavoritePlaces.Count();
            switch (_Count) {
                case 0:
                    NoPlaces.Visibility = Visibility.Visible;
                    break;
                case 1:
                    SavedPlacesGridView.MaxWidth = 250;
                    break;
                case 2:
                    SavedPlacesGridView.MaxWidth = 500;
                    break;
                case 3:
                    SavedPlacesGridView.MaxWidth = 750;
                    break;
                case 4:
                    SavedPlacesGridView.MaxWidth = 1000;
                    break;
                case 5:
                    SavedPlacesGridView.MaxWidth = 1250;
                    break;
                case 6:
                    SavedPlacesGridView.MaxWidth = 1500;
                    break;
                case 7:
                    SavedPlacesGridView.MaxWidth = 1750;
                    break;
                case 8:
                    SavedPlacesGridView.MaxWidth = 2000;
                    break;
                case 9:
                    SavedPlacesGridView.MaxWidth = 2250;
                    break;
                case 10:
                    SavedPlacesGridView.MaxWidth = 2500;
                    break;
            }
            SavedPlacesGridView.ItemsSource = FavoritePlaces;
        }

        private void SavedPlacesGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var rootFrame = new Frame();
            Window.Current.Content = rootFrame;
            var _CityData = e.ClickedItem as CityData;
            rootFrame.Navigate(typeof(ShellPage), _CityData.PlaceInfo);
        }

        private async void RemovePlaceFormFavorites(object sender, RoutedEventArgs e)
        {
            var _PlaceId = (sender as Button).Tag.ToString();
            var _CityData = new CityData() { PlaceInfo = new PlaceInfo() { PlaceId = _PlaceId } };

            bool WriteToFile = (await UserDataHelper.RemoveFromFavorites("Favorites.json", _CityData.PlaceInfo));
            if (WriteToFile == true)
            {
                WeatherPage.IsFavoritesChanged = true;
                await UserDataHelper.RemoveFavoriteWeather(_CityData.PlaceInfo.PlaceId);
                FavoritePlaces = FavoritePlaces.Where(x => x.PlaceInfo.PlaceId != _PlaceId).ToList();
                SavedPlacesGridView.ItemsSource = FavoritePlaces;

                var _Count = FavoritePlaces.Count();
                switch (_Count)
                {
                    case 0:
                        NoPlaces.Visibility = Visibility.Visible;
                        break;
                    case 1:
                        SavedPlacesGridView.MaxWidth = 250;
                        break;
                    case 2:
                        SavedPlacesGridView.MaxWidth = 500;
                        break;
                    case 3:
                        SavedPlacesGridView.MaxWidth = 750;
                        break;
                    case 4:
                        SavedPlacesGridView.MaxWidth = 1000;
                        break;
                    case 5:
                        SavedPlacesGridView.MaxWidth = 1250;
                        break;
                    case 6:
                        SavedPlacesGridView.MaxWidth = 1500;
                        break;
                    case 7:
                        SavedPlacesGridView.MaxWidth = 1750;
                        break;
                    case 8:
                        SavedPlacesGridView.MaxWidth = 2000;
                        break;
                    case 9:
                        SavedPlacesGridView.MaxWidth = 2250;
                        break;
                    case 10:
                        SavedPlacesGridView.MaxWidth = 2500;
                        break;
                }
            }
        }
    }
}