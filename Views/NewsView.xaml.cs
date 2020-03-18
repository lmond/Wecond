using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Wecond.Views
{
    public sealed partial class NewsView : Page
    {
        public NewsView()
        {
            this.InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            NewsItemView.Navigate(new Uri(e.Parameter.ToString()));
        }
    }
}
