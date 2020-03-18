using System;
using System.Collections.Specialized;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Storage;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Wecond
{
    sealed partial class App : Application
    {
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            ApplicationView.GetForCurrentView().TitleBar.ButtonBackgroundColor = Colors.Transparent;
            CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;



            Frame rootFrame = Window.Current.Content as Frame;

            if (rootFrame == null)
            {
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    
                }

                Window.Current.Content = rootFrame;
            }

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    var _PlaceInfo = new Models.PlaceInfo();
                    if (e.Arguments.ToString() != "") { 
                        var arr = e.Arguments.ToString().Split('&');

                        var _DisplayName = arr[3].Substring(arr[3].LastIndexOf('=') + 1);
                        var _Latitude = double.Parse(arr[1].Substring(arr[1].LastIndexOf('=') + 1), System.Globalization.CultureInfo.InvariantCulture);
                        var _Longitude = double.Parse(arr[2].Substring(arr[2].LastIndexOf('=') + 1), System.Globalization.CultureInfo.InvariantCulture);
                        var _PlaceId = arr[0].Substring(arr[0].LastIndexOf('=') + 1);

                        _PlaceInfo = new Models.PlaceInfo() { DisplayName = _DisplayName, Latitude = _Latitude, Longitude = _Longitude, PlaceId = _PlaceId };
                        rootFrame.Navigate(typeof(Views.ShellPage), _PlaceInfo);
                    } else
                        rootFrame.Navigate(typeof(Views.ShellPage));
                    
                    //rootFrame.Navigate(typeof(MainPage), e.Arguments);
                }
                Window.Current.Activate();
            }
        }

        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }
        
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            deferral.Complete();
        }
    }
}
