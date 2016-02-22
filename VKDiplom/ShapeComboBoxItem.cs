using HermiteInterpolation.Utils;
using Microsoft.Xna.Framework;

namespace VKDiplom
{
    public class ShapeComboBoxItem
    {
        public string Content { get; set; }
        public string Color { get; set; }

        public ShapeComboBoxItem()
        {
            
        }

        public ShapeComboBoxItem(string content, Color color)
        {
            Content = content;
            color = AddBrightness(color);
            Color = "#"+ $"{color.A:X2}"+ $"{color.R:X2}"+ $"{color.G:X2}"+ $"{color.B:X2}";
        }

        public ShapeComboBoxItem(string content, System.Windows.Media.Color color)
        {
            Content = content;
            var xnaCol = AddBrightness(color.WindowsColorToXnaColor());
            Color = "#" + $"{xnaCol.A:X2}" + $"{xnaCol.R:X2}" + $"{xnaCol.G:X2}" + $"{xnaCol.B:X2}";
        }

        private Color AddBrightness(Color color)
        {
            float hue, sat, val, alpha;
            ColorUtils.ToHsv(color,out hue,out sat, out val,out alpha);
            val = 0.85f + (val/1)*(1 - 0.85f);
            return ColorUtils.FromHsv(hue, sat, val, alpha);
        }
    }
}
