using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Wecond.Common
{
    public sealed partial class AcrylicBrush : UserControl
    {
        public AcrylicBrush()
        {
            this.InitializeComponent();
        }
        public static readonly DependencyProperty ColorProperty = DependencyProperty.Register("Color", typeof(object), typeof(AcrylicBrush), null);

        public object Color
        {
            get { return GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }
        public string ColorCode { get
            {
                if (ColorProperty == null)
                    return "#FFFFFF";
                return Color.ToString();
            }
        }
        /*public static readonly DependencyProperty ColorProperty = DependencyProperty.Register("Color", typeof(Color), typeof(AcrylicBrush), new PropertyMetadata(null));

        public Color Color
        {
            get {
                string colorStr = GetValue(ColorProperty).ToString();
                Color color = Windows.UI.Color.FromArgb(255, 255, 255, 255);
                if (colorStr != string.Empty) {
                    colorStr = colorStr.Replace("#", string.Empty);
                    var r = (byte)System.Convert.ToUInt32(colorStr.Substring(0, 2), 16);
                    var g = (byte)System.Convert.ToUInt32(colorStr.Substring(2, 2), 16);
                    var b = (byte)System.Convert.ToUInt32(colorStr.Substring(4, 2), 16);
                    color = Windows.UI.Color.FromArgb(255, r, g, b);
                }
                
                return color;
            }
            set { SetValue(ColorProperty, value); }
        }*/
    }
}
