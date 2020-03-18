using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Wecond.Models;
using Windows.UI.Notifications;
using Windows.UI.StartScreen;

namespace Wecond.Helpers
{
    class LiveTile
    {
        public static void UpdateTile(PlaceInfo _PlaceInfo, CurrentWeatherResult _CurrentWeatherResult, DailyForecastResult _DailyForecastResult, string _CoverImage)
        {
            try
            {
                var _CityData = new CityData() { PlaceInfo = _PlaceInfo, Current = _CurrentWeatherResult, DailyForecast = _DailyForecastResult, CoverImage = _CoverImage };
                var tileContent = SetTileContent(_CityData);

                var tileNotif = new TileNotification(tileContent.GetXml());
                TileUpdateManager.CreateTileUpdaterForApplication().Update(tileNotif);
            }
            catch (Exception) { }
        } 
        public static async Task<bool> UpdateCustomTile(PlaceInfo _PlaceInfo, CurrentWeatherResult _CurrentWeatherResult, DailyForecastResult _DailyForecastResult, string _CoverImage)
        {
            var _CityData = new CityData() { PlaceInfo = _PlaceInfo, Current = _CurrentWeatherResult, DailyForecast = _DailyForecastResult, CoverImage = _CoverImage };
            if (!SecondaryTile.Exists(_PlaceInfo.PlaceId))
            {
                string tileId = _CityData.PlaceInfo.PlaceId;
                string displayName = "Wecond";
                string arguments = "PlaceId=" + _CityData.PlaceInfo.PlaceId + "&Latitude=" + _CityData.PlaceInfo.Latitude + "&Longitude=" + _CityData.PlaceInfo.Longitude + "&DisplayName=" + _CityData.PlaceInfo.DisplayName;

                SecondaryTile tile = new SecondaryTile(
                    tileId,
                    displayName,
                    arguments,
                    new Uri("ms-appx:///Assets/Square150x150Logo.png"),
                    TileSize.Default);

                tile.VisualElements.Wide310x150Logo = new Uri("ms-appx:///Assets/Wide310x150Logo.png");
                tile.VisualElements.Square310x310Logo = new Uri("ms-appx:///Assets/Square310x310Logo.png");
                tile.VisualElements.Square71x71Logo = new Uri("ms-appx:///Assets/Square71x71Logo.png");
                tile.VisualElements.Square44x44Logo = new Uri("ms-appx:///Assets/Square44x44Logo.png");

                tile.VisualElements.ShowNameOnSquare150x150Logo = true;
                tile.VisualElements.ShowNameOnWide310x150Logo = true;
                tile.VisualElements.ShowNameOnSquare310x310Logo = true;

                bool isPinned = await tile.RequestCreateAsync();

                if (isPinned != true)
                    return false;
            }

            var tileContent = SetTileContent(_CityData);
            Debug.Write(tileContent.ToString());
            var tileNotif = new TileNotification(tileContent.GetXml());
            TileUpdateManager.CreateTileUpdaterForSecondaryTile(_CityData.PlaceInfo.PlaceId).Update(tileNotif);
            return true;
        }
        public static async Task<bool> UnpinCustomTile(string TileId) {
            if (!SecondaryTile.Exists(TileId))
                return true;
            SecondaryTile toBeDeleted = new SecondaryTile(TileId);
            var IsDeleted = await toBeDeleted.RequestDeleteAsync();
            if (IsDeleted == true) {
                return true;
            }
            return false;
        }
        public static TileContent SetTileContent(CityData _CityData) {
            var tileContent = new TileContent()
            {
                Visual = new TileVisual()
                {
                    DisplayName = _CityData.PlaceInfo.DisplayName,
                    TileSmall = new TileBinding()
                    {
                        Content = new TileBindingContentAdaptive()
                        {
                            TextStacking = TileTextStacking.Center,
                            Children =
                                {
                                    new AdaptiveText()
                                    {
                                        Text = _CityData.DailyForecast.list[0].Day,
                                        HintStyle = AdaptiveTextStyle.Body,
                                        HintAlign = AdaptiveTextAlign.Center
                                    },
                                    new AdaptiveText()
                                    {
                                        Text = _CityData.Current.main.temp + "°",
                                        HintStyle = AdaptiveTextStyle.Base,
                                        HintAlign = AdaptiveTextAlign.Center
                                    }
                                },
                            BackgroundImage = new TileBackgroundImage()
                            {
                                Source = _CityData.CoverImage,
                                HintOverlay = 60
                            }
                        }
                    },
                    TileMedium = new TileBinding()
                    {
                        Branding = TileBranding.NameAndLogo,
                        Content = new TileBindingContentAdaptive()
                        {
                            Children =
                            {
                                new AdaptiveGroup()
                                {
                                    Children =
                                    {
                                        new AdaptiveSubgroup()
                                        {
                                            HintWeight = 1,
                                            Children =
                                            {
                                                new AdaptiveText()
                                                {
                                                    Text = _CityData.DailyForecast.list[0].Day,
                                                    HintAlign = AdaptiveTextAlign.Center
                                                },
                                                new AdaptiveImage()
                                                {
                                                    HintRemoveMargin = true,
                                                    Source = _CityData.DailyForecast.list[0].Image
                                                },
                                                new AdaptiveText()
                                                {
                                                    Text = _CityData.DailyForecast.list[0].temp.max + "°",
                                                    HintAlign = AdaptiveTextAlign.Center
                                                },
                                                new AdaptiveText()
                                                {
                                                    Text = _CityData.DailyForecast.list[0].temp.min + "°",
                                                    HintStyle = AdaptiveTextStyle.CaptionSubtle,
                                                    HintAlign = AdaptiveTextAlign.Center
                                                }
                                            }
                                        },
                                        new AdaptiveSubgroup()
                                        {
                                            HintWeight = 1,
                                            Children =
                                            {
                                                new AdaptiveText()
                                                {
                                                    Text = _CityData.DailyForecast.list[1].Day,
                                                    HintAlign = AdaptiveTextAlign.Center
                                                },
                                                new AdaptiveImage()
                                                {
                                                    HintRemoveMargin = true,
                                                    Source = _CityData.DailyForecast.list[1].Image
                                                },
                                                new AdaptiveText()
                                                {
                                                    Text = _CityData.DailyForecast.list[1].temp.max + "°",
                                                    HintAlign = AdaptiveTextAlign.Center
                                                },
                                                new AdaptiveText()
                                                {
                                                    Text = _CityData.DailyForecast.list[1].temp.min + "°",
                                                    HintStyle = AdaptiveTextStyle.CaptionSubtle,
                                                    HintAlign = AdaptiveTextAlign.Center
                                                }
                                            }
                                        },
                                        new AdaptiveSubgroup()
                                        {
                                            HintWeight = 1,
                                            Children =
                                            {
                                                new AdaptiveText()
                                                {
                                                    Text = _CityData.DailyForecast.list[2].Day,
                                                    HintAlign = AdaptiveTextAlign.Center
                                                },
                                                new AdaptiveImage()
                                                {
                                                    HintRemoveMargin = true,
                                                    Source = _CityData.DailyForecast.list[2].Image
                                                },
                                                new AdaptiveText()
                                                {
                                                    Text = _CityData.DailyForecast.list[2].temp.max + "°",
                                                    HintAlign = AdaptiveTextAlign.Center
                                                },
                                                new AdaptiveText()
                                                {
                                                    Text = _CityData.DailyForecast.list[2].temp.min + "°",
                                                    HintStyle = AdaptiveTextStyle.CaptionSubtle,
                                                    HintAlign = AdaptiveTextAlign.Center
                                                }
                                            }
                                        }
                                    }
                                }
                            },
                            BackgroundImage = new TileBackgroundImage()
                            {
                                Source = _CityData.CoverImage,
                                HintOverlay = 60
                            }
                        }
                    },
                    TileWide = new TileBinding()
                    {
                        Branding = TileBranding.NameAndLogo,
                        Content = new TileBindingContentAdaptive()
                        {
                            Children =
                            {
                                new AdaptiveGroup()
                                {
                                    Children =
                                    {
                                        new AdaptiveSubgroup()
                                        {
                                            HintWeight = 1,
                                            Children =
                                            {
                                                new AdaptiveText()
                                                {
                                                    Text = _CityData.DailyForecast.list[0].Day,
                                                    HintAlign = AdaptiveTextAlign.Center
                                                },
                                                new AdaptiveImage()
                                                {
                                                    HintRemoveMargin = true,
                                                    Source = _CityData.DailyForecast.list[0].Image
                                                },
                                                new AdaptiveText()
                                                {
                                                    Text = _CityData.DailyForecast.list[0].temp.max + "°",
                                                    HintAlign = AdaptiveTextAlign.Center
                                                },
                                                new AdaptiveText()
                                                {
                                                    Text = _CityData.DailyForecast.list[0].temp.min + "°",
                                                    HintStyle = AdaptiveTextStyle.CaptionSubtle,
                                                    HintAlign = AdaptiveTextAlign.Center
                                                }
                                            }
                                        },
                                        new AdaptiveSubgroup()
                                        {
                                            HintWeight = 1,
                                            Children =
                                            {
                                                new AdaptiveText()
                                                {
                                                    Text = _CityData.DailyForecast.list[1].Day,
                                                    HintAlign = AdaptiveTextAlign.Center
                                                },
                                                new AdaptiveImage()
                                                {
                                                    HintRemoveMargin = true,
                                                    Source = _CityData.DailyForecast.list[1].Image
                                                },
                                                new AdaptiveText()
                                                {
                                                    Text = _CityData.DailyForecast.list[1].temp.max + "°",
                                                    HintAlign = AdaptiveTextAlign.Center
                                                },
                                                new AdaptiveText()
                                                {
                                                    Text = _CityData.DailyForecast.list[1].temp.min + "°",
                                                    HintStyle = AdaptiveTextStyle.CaptionSubtle,
                                                    HintAlign = AdaptiveTextAlign.Center
                                                }
                                            }
                                        },
                                        new AdaptiveSubgroup()
                                        {
                                            HintWeight = 1,
                                            Children =
                                            {
                                                new AdaptiveText()
                                                {
                                                    Text = _CityData.DailyForecast.list[2].Day,
                                                    HintAlign = AdaptiveTextAlign.Center
                                                },
                                                new AdaptiveImage()
                                                {
                                                    HintRemoveMargin = true,
                                                    Source = _CityData.DailyForecast.list[2].Image
                                                },
                                                new AdaptiveText()
                                                {
                                                    Text = _CityData.DailyForecast.list[2].temp.max + "°",
                                                    HintAlign = AdaptiveTextAlign.Center
                                                },
                                                new AdaptiveText()
                                                {
                                                    Text = _CityData.DailyForecast.list[2].temp.min + "°",
                                                    HintStyle = AdaptiveTextStyle.CaptionSubtle,
                                                    HintAlign = AdaptiveTextAlign.Center
                                                }
                                            }
                                        },
                                        new AdaptiveSubgroup()
                                        {
                                            HintWeight = 1,
                                            Children =
                                            {
                                                new AdaptiveText()
                                                {
                                                    Text = _CityData.DailyForecast.list[3].Day,
                                                    HintAlign = AdaptiveTextAlign.Center
                                                },
                                                new AdaptiveImage()
                                                {
                                                    HintRemoveMargin = true,
                                                    Source = _CityData.DailyForecast.list[3].Image
                                                },
                                                new AdaptiveText()
                                                {
                                                    Text = _CityData.DailyForecast.list[3].temp.max + "°",
                                                    HintAlign = AdaptiveTextAlign.Center
                                                },
                                                new AdaptiveText()
                                                {
                                                    Text = _CityData.DailyForecast.list[3].temp.min + "°",
                                                    HintStyle = AdaptiveTextStyle.CaptionSubtle,
                                                    HintAlign = AdaptiveTextAlign.Center
                                                }
                                            }
                                        },
                                        new AdaptiveSubgroup()
                                        {
                                            HintWeight = 1,
                                            Children =
                                            {
                                                new AdaptiveText()
                                                {
                                                    Text = _CityData.DailyForecast.list[4].Day,
                                                    HintAlign = AdaptiveTextAlign.Center
                                                },
                                                new AdaptiveImage()
                                                {
                                                    HintRemoveMargin = true,
                                                    Source = _CityData.DailyForecast.list[4].Image
                                                },
                                                new AdaptiveText()
                                                {
                                                    Text = _CityData.DailyForecast.list[4].temp.max + "°",
                                                    HintAlign = AdaptiveTextAlign.Center
                                                },
                                                new AdaptiveText()
                                                {
                                                    Text = _CityData.DailyForecast.list[4].temp.min + "°",
                                                    HintStyle = AdaptiveTextStyle.CaptionSubtle,
                                                    HintAlign = AdaptiveTextAlign.Center
                                                }
                                            }
                                        },
                                        new AdaptiveSubgroup()
                                        {
                                            HintWeight = 1,
                                            Children =
                                            {
                                                new AdaptiveText()
                                                {
                                                    Text = _CityData.DailyForecast.list[5].Day,
                                                    HintAlign = AdaptiveTextAlign.Center
                                                },
                                                new AdaptiveImage()
                                                {
                                                    HintRemoveMargin = true,
                                                    Source = _CityData.DailyForecast.list[5].Image
                                                },
                                                new AdaptiveText()
                                                {
                                                    Text = _CityData.DailyForecast.list[5].temp.max + "°",
                                                    HintAlign = AdaptiveTextAlign.Center
                                                },
                                                new AdaptiveText()
                                                {
                                                    Text = _CityData.DailyForecast.list[5].temp.min + "°",
                                                    HintStyle = AdaptiveTextStyle.CaptionSubtle,
                                                    HintAlign = AdaptiveTextAlign.Center
                                                }
                                            }
                                        }
                                    }
                                }
                            },
                            BackgroundImage = new TileBackgroundImage()
                            {
                                Source = _CityData.CoverImage,
                                HintOverlay = 60
                            }
                        }
                    },
                    TileLarge = new TileBinding()
                    {
                        Branding = TileBranding.NameAndLogo,
                        Content = new TileBindingContentAdaptive()
                        {
                            Children =
                            {
                                new AdaptiveGroup()
                                {
                                    Children =
                                    {
                                        new AdaptiveSubgroup()
                                        {
                                            HintWeight = 30,
                                            Children =
                                            {
                                                new AdaptiveImage()
                                                {
                                                    Source = _CityData.Current.Image
                                                }
                                            }
                                        },
                                        new AdaptiveSubgroup()
                                        {
                                            Children =
                                            {
                                                new AdaptiveText()
                                                {
                                                    Text = _CityData.Current.Date,
                                                    HintStyle = AdaptiveTextStyle.Base
                                                },
                                                new AdaptiveText()
                                                {
                                                    Text = _CityData.Current.main.temp + "°"
                                                },
                                                new AdaptiveText()
                                                {
                                                    Text = "Humidity " + _CityData.Current.main.humidity + "%",
                                                    HintStyle = AdaptiveTextStyle.CaptionSubtle
                                                },
                                                new AdaptiveText()
                                                {
                                                    Text = "Wind speed " + _CityData.Current.wind.speed,
                                                    HintStyle = AdaptiveTextStyle.CaptionSubtle
                                                }
                                            }
                                        }
                                    }
                                },
                                new AdaptiveText()
                                {
                                    Text = ""
                                },
                                new AdaptiveGroup()
                                {
                                    Children =
                                    {
                                        new AdaptiveSubgroup()
                                        {
                                            HintWeight = 1,
                                            Children =
                                            {
                                                new AdaptiveText()
                                                {
                                                    Text = _CityData.DailyForecast.list[0].Day,
                                                    HintAlign = AdaptiveTextAlign.Center
                                                },
                                                new AdaptiveImage()
                                                {
                                                    Source = _CityData.DailyForecast.list[0].Image
                                                },
                                                new AdaptiveText()
                                                {
                                                    Text =  _CityData.DailyForecast.list[0].temp.max + "°",
                                                    HintAlign = AdaptiveTextAlign.Center
                                                },
                                                new AdaptiveText()
                                                {
                                                    Text =  _CityData.DailyForecast.list[0].temp.min + "°",
                                                    HintStyle = AdaptiveTextStyle.CaptionSubtle,
                                                    HintAlign = AdaptiveTextAlign.Center
                                                }
                                            }
                                        },
                                        new AdaptiveSubgroup()
                                        {
                                            HintWeight = 1,
                                            Children =
                                            {
                                                new AdaptiveText()
                                                {
                                                    Text = _CityData.DailyForecast.list[1].Day,
                                                    HintAlign = AdaptiveTextAlign.Center
                                                },
                                                new AdaptiveImage()
                                                {
                                                    Source = _CityData.DailyForecast.list[1].Image
                                                },
                                                new AdaptiveText()
                                                {
                                                    Text =  _CityData.DailyForecast.list[1].temp.max + "°",
                                                    HintAlign = AdaptiveTextAlign.Center
                                                },
                                                new AdaptiveText()
                                                {
                                                    Text =  _CityData.DailyForecast.list[1].temp.min + "°",
                                                    HintStyle = AdaptiveTextStyle.CaptionSubtle,
                                                    HintAlign = AdaptiveTextAlign.Center
                                                }
                                            }
                                        },
                                        new AdaptiveSubgroup()
                                        {
                                            HintWeight = 1,
                                            Children =
                                            {
                                                new AdaptiveText()
                                                {
                                                    Text = _CityData.DailyForecast.list[2].Day,
                                                    HintAlign = AdaptiveTextAlign.Center
                                                },
                                                new AdaptiveImage()
                                                {
                                                    Source = _CityData.DailyForecast.list[2].Image
                                                },
                                                new AdaptiveText()
                                                {
                                                    Text =  _CityData.DailyForecast.list[2].temp.max + "°",
                                                    HintAlign = AdaptiveTextAlign.Center
                                                },
                                                new AdaptiveText()
                                                {
                                                    Text =  _CityData.DailyForecast.list[2].temp.min + "°",
                                                    HintStyle = AdaptiveTextStyle.CaptionSubtle,
                                                    HintAlign = AdaptiveTextAlign.Center
                                                }
                                            }
                                        },
                                        new AdaptiveSubgroup()
                                        {
                                            HintWeight = 1,
                                            Children =
                                            {
                                                new AdaptiveText()
                                                {
                                                    Text = _CityData.DailyForecast.list[3].Day,
                                                    HintAlign = AdaptiveTextAlign.Center
                                                },
                                                new AdaptiveImage()
                                                {
                                                    Source = _CityData.DailyForecast.list[3].Image
                                                },
                                                new AdaptiveText()
                                                {
                                                    Text =  _CityData.DailyForecast.list[3].temp.max + "°",
                                                    HintAlign = AdaptiveTextAlign.Center
                                                },
                                                new AdaptiveText()
                                                {
                                                    Text =  _CityData.DailyForecast.list[3].temp.min + "°",
                                                    HintStyle = AdaptiveTextStyle.CaptionSubtle,
                                                    HintAlign = AdaptiveTextAlign.Center
                                                }
                                            }
                                        },
                                        new AdaptiveSubgroup()
                                        {
                                            HintWeight = 1,
                                            Children =
                                            {
                                                new AdaptiveText()
                                                {
                                                    Text = _CityData.DailyForecast.list[4].Day,
                                                    HintAlign = AdaptiveTextAlign.Center
                                                },
                                                new AdaptiveImage()
                                                {
                                                    Source = _CityData.DailyForecast.list[4].Image
                                                },
                                                new AdaptiveText()
                                                {
                                                    Text =  _CityData.DailyForecast.list[4].temp.max + "°",
                                                    HintAlign = AdaptiveTextAlign.Center
                                                },
                                                new AdaptiveText()
                                                {
                                                    Text =  _CityData.DailyForecast.list[4].temp.min + "°",
                                                    HintStyle = AdaptiveTextStyle.CaptionSubtle,
                                                    HintAlign = AdaptiveTextAlign.Center
                                                }
                                            }
                                        },
                                        new AdaptiveSubgroup()
                                        {
                                            HintWeight = 1,
                                            Children =
                                            {
                                                new AdaptiveText()
                                                {
                                                    Text = _CityData.DailyForecast.list[5].Day,
                                                    HintAlign = AdaptiveTextAlign.Center
                                                },
                                                new AdaptiveImage()
                                                {
                                                    Source = _CityData.DailyForecast.list[5].Image
                                                },
                                                new AdaptiveText()
                                                {
                                                    Text =  _CityData.DailyForecast.list[5].temp.max + "°",
                                                    HintAlign = AdaptiveTextAlign.Center
                                                },
                                                new AdaptiveText()
                                                {
                                                    Text =  _CityData.DailyForecast.list[5].temp.min + "°",
                                                    HintStyle = AdaptiveTextStyle.CaptionSubtle,
                                                    HintAlign = AdaptiveTextAlign.Center
                                                }
                                            }
                                        }
                                    }
                                }
                            },
                            BackgroundImage = new TileBackgroundImage()
                            {
                                Source = _CityData.CoverImage,
                                HintOverlay = 60
                            }
                        }
                    }
                }
            };
            return tileContent;
        }
    }
}
