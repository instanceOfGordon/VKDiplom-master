using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using HermiteInterpolation.Utils;
using Color = Microsoft.Xna.Framework.Color;

namespace VKDiplom
{
    public sealed class ColorSlider : Slider
    {
        public ColorSlider()
        {
            Minimum = 0;
            Maximum = 359;
            Value = 179;
            var colorXna = ColorUtils.FromHsv((float)Value, 1, 1, 1);
            var color = new SolidColorBrush(System.Windows.Media.Color.FromArgb(colorXna.A, colorXna.R, colorXna.G, colorXna.B));
            Background = color;
            ValueChanged += OnValueChanged;
        }

        private void OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> args)
        {
            var colorSlider = sender as Slider;
            if (colorSlider == null) return;
            if (!colorSlider.IsEnabled) return;       
            var colorXna = ColorUtils.FromHsv((float)colorSlider.Value, 1, 1, 1);
            var color = new SolidColorBrush(System.Windows.Media.Color.FromArgb(colorXna.A, colorXna.R, colorXna.G, colorXna.B));
            Background = color;
        }
    }
}