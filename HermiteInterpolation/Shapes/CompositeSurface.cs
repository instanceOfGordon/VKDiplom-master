using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using HermiteInterpolation.Utils;
using Microsoft.Xna.Framework;

namespace HermiteInterpolation.Shapes
{
    public abstract class CompositeSurface : ISurface
    {
        public string Name { get; set; }
        private Color _color = Properties.Constants.DefaultColor;

        public bool IsHeightColored { get; private set; } = false;

        public Color Color
        {
            get { return _color; }
            set
            {
                _color = value;
                ColoredSimple(value);
            }
        }
        protected IEnumerable<ISurface> Segments { get; set; }
        private float? _minHeight;
        private float? _maxHeight;

        public void Draw()
        {
            foreach (var segment in Segments)
            {
                segment.Draw();
            }
        }

        public DrawStyle DrawStyle { get; set; }
        public float MinHeight
        {
            get
            {
                if (!_minHeight.HasValue)
                    _minHeight = Segments.Min(segment => segment.MinHeight);           
                return _minHeight.Value;
            }
        }

        public float MaxHeight
        {
            get
            {
                if (!_maxHeight.HasValue) _maxHeight = Segments.Max(segment => segment.MaxHeight);
                return _maxHeight.Value;
            }
        }
        public void ColoredSimple(Color color)
        {
            IsHeightColored = false;
            _color = color;
            foreach (var segment in Segments)
            {
                segment.ColoredSimple(color);
            }
        }

        public void ColoredHeight()
        {
            IsHeightColored = true;
            _color = Color.FromNonPremultiplied(0, 0, 0, 0);
            var minZ = MinHeight;
            var maxZ = MaxHeight;

            var dZ = maxZ - minZ;

            foreach (var segment in Segments)
            {
                var fromHue = ((segment.MinHeight - minZ) / dZ) * 300f;
                var toHue = ((segment.MaxHeight - minZ) / dZ) * 300f;
                segment.ColoredHeight(fromHue, toHue);
            }
        }

        public void ColoredHeight(float fromHue, float toHue)
        {
            IsHeightColored = true;
            _color = Color.FromNonPremultiplied(0, 0, 0, 0);
            var minZ = MinHeight;
            var maxZ = MaxHeight;

            var dZ = maxZ - minZ;

            foreach (var segment in Segments)
            {
                var frHue = ((segment.MinHeight - minZ) / dZ) * toHue - fromHue;
                var tHue = ((segment.MaxHeight - minZ) / dZ) * toHue;
                segment.ColoredHeight(frHue, tHue);
            }
        }

        public virtual void ColoredBySegment(ColorUtils.ColorSeedFunction colorSeedFunction, params object[] parameters)
        {
            IsHeightColored = false;
            foreach (var segment in Segments)
            {
                segment.ColoredSimple(colorSeedFunction(parameters));
            }
        }

        public void ColoredBySegment()
        {
            IsHeightColored = false;
            ColoredBySegment(x => ColorUtils.Random());         
        }

        public void ColoredByShades(Color baseColor)
        {
            IsHeightColored = false;
            _color = baseColor;
            ColoredBySegment(color => ColorUtils.RandomShade((Color)color[0]), baseColor);
        }

        public void ColoredByShades(float baseHue)
        {
            IsHeightColored = false;
            _color = ColorUtils.RandomShade(baseHue);
            ColoredBySegment(hue => ColorUtils.RandomShade((float)hue[0]), baseHue);
        }

        public void ColoredSimple(int r, int g, int b, int a)
        {
            IsHeightColored = false;
            _color = Color.FromNonPremultiplied(r, g, b, a);
            ColoredSimple(_color);
        }

    }
}