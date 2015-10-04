using System.Collections.Generic;
using System.Linq;
using HermiteInterpolation.Utils;
using Microsoft.Xna.Framework;

namespace HermiteInterpolation.Shapes
{
    public abstract class CompositeSurface : ISurface
    {
        protected static readonly Color DefaultColor = Color.FromNonPremultiplied(128, 128, 128, 255);
        protected static readonly Vector3 DefaultNormal = Vector3.Zero;

        public string Name { get; set; } //= ToString();
        private Color _color = DefaultColor;

        public Color Color
        {
            get { return _color; }
            set
            {
                _color = value;
                ColoredByShades(value);
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
            _color = color;
            foreach (var segment in Segments)
            {
                segment.ColoredSimple(color);
            }
        }

        public void ColoredHeight()
        {
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

        public virtual void ColoredBySegment(ColorUtils.SeedColor seedFunction, params object[] parameters)
        {
            foreach (var segment in Segments)
            {
                segment.ColoredSimple(seedFunction(parameters));
            }
        }

        public void ColoredBySegment()
        {
            ColoredBySegment(x => ColorUtils.Random());         
        }

        public void ColoredByShades(Color baseColor)
        {
            ColoredBySegment(color => ColorUtils.RandomShade((Color)color[0]), baseColor);
        }

        public void ColoredByShades(float baseHue)
        {
            ColoredBySegment(hue => ColorUtils.RandomShade((float)hue[0]), baseHue);
        }

        public void ColoredSimple(int r, int g, int b, int a)
        {
            ColoredSimple(Color.FromNonPremultiplied(r, g, b, a));
        }

    }
}