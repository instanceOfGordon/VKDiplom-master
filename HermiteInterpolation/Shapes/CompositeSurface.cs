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
            //if (_segments == null)
            //    _segments = CreateMesh();
            //_segments.Draw();
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
                //_vertices.Min(vertex => vertex.Position.Z);
                return _minHeight.Value;
            }
            //protected set { _minHeight = value; }
        }

        public float MaxHeight
        {
            get
            {
                if (!_maxHeight.HasValue) _maxHeight = Segments.Max(segment => segment.MaxHeight);
                return _maxHeight.Value;
            }
            //  protected set { _maxHeight = value; }
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
            //Segments.ForEach(segment => segment.ColoredSimple(seedFunction(parameters)));
        }

        public void ColoredBySegment()
        {
            //            var colors = new[] { new Color(237, 28, 36), new Color(255, 127, 39), new Color(255,242,0), new Color(34, 177, 76), 
            //                new Color(63, 72, 204), new Color(163, 73, 164)};

            //new Color(255, 201, 14)};
            //var colors = new[] { new Color(237, 28, 36), new Color(6, 128, 64), new Color(63, 72, 204), new Color(255, 201, 14) };
            // new Color(63, 72, 204), new Color(163, 73, 164)};
            ColoredBySegment(x => ColorUtils.Random());
            //Segments.ForEach(segment => segment.ColoredSimple(ColorUtils.Random()));

            //            var colors = new[] { new Color(255, 6, 6), new Color(6, 255, 6), new Color(6, 6, 255), new Color(255, 6, 255) };
            //            var ccount = colors.Length;
            //            var cidx = 0;
            //            for (var i = 0; i < ukcmo; i++)
            //            {
            //                for (var j = 0; j < vkcmo; j++)
            //                {
            //                    var idx = i*vkcmo + j;
            //                    var idxcolor = cidx%ccount;
            //                    Segments[idx].ColoredSimple(colors[idxcolor]);
            //                    ++cidx;
            //                }
            //                cidx = vkcmo != ccount ? cidx:cidx+1;
            //           
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