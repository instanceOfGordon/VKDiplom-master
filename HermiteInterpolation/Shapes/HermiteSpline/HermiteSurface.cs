using System;
using System.Collections.Generic;
using System.Linq;
using HermiteInterpolation.Utils;
using Microsoft.Xna.Framework;

namespace HermiteInterpolation.Shapes.HermiteSpline
{
    public enum HermiteType
    {
        Bicubic,
        Biquartic
    }

    public class HermiteSurface : ISurface
    {
        //private readonly float _meshDensity;
        private readonly List<ISurface> _segments;
        // private readonly HermiteType _type;
        private readonly int _uCount;
        private readonly double _uMax;
        private readonly double _uMin;
        private readonly int _vCount;
        private readonly double _vMax;
        private readonly double _vMin;
        private DrawStyle _drawStyle;
//        public HermiteType Type {
//            get { return _type; }
//        }

        private float? _maxHeight;
        private float? _minHeight;

        internal HermiteSurface(double uMin, double uMax, int uCount, double vMin, double vMax, int vCount,
            List<ISurface> segments, Derivation derivation)
        {
            _segments = segments;
            //_meshDensity = 0.1f;
            _uMin = uMin;
            _uMax = uMax;

            _uCount = uCount;
            _vMin = vMin;
            _vMax = vMax;


            _vCount = vCount;

            Derivation = derivation;
        }

        public double UMinKnot
        {
            get { return _uMin; }
        }

        public double UMaxKnot
        {
            get { return _uMax; }
        }

        public int UKnotsCount
        {
            get { return _uCount; }
        }

        public double VMinKnot
        {
            get { return _vMin; }
        }

        public double VMaxKnot
        {
            get { return _vMax; }
        }

        public int VKnotsCount
        {
            get { return _vCount; }
        }

        public double UKnotsDistance
        {
            get { return Math.Abs(_uMax - _uMin)/(_uCount - 1); }
        }

        public double VKnotsDistance
        {
            get { return Math.Abs(_vMax - _vMin)/(_vCount - 1); }
        }

        protected List<ISurface> Segments
        {
            get { return _segments; }
        }

        public Derivation Derivation { get; protected set; }
//        public float MeshDensity
//        {
//            get { return _meshDensity; }
////            set
////            {
////                _meshDensity = value;
////                GenerateMesh();
////            }
//        }

        public DrawStyle DrawStyle
        {
            get { return _drawStyle; }
            set
            {
                _drawStyle = value;
                foreach (var segment in Segments)
                {
                    segment.DrawStyle = value;
                }
            }
        }

//        public static HermiteSurface CreateBicubic(double uMin, double uMax, int uCount, double vMin, double vMax, int vCount, InterpolatedFunction aproximationFunction)
//        {
//            return new HermiteSurface(uMin, uMax, uCount, vMin, vMax, vCount, aproximationFunction, Derivation.Zero, HermiteType.Bicubic);
//        }
//
//        public static HermiteSurface CreateBiquartic(double uMin, double uMax, int uCount, double vMin, double vMax, int vCount, InterpolatedFunction aproximationFunction)
//        {
//            return new HermiteSurface(uMin, uMax, uCount, vMin, vMax, vCount, aproximationFunction, Derivation.Zero, HermiteType.Biquartic);
//        }
//
//        public static HermiteSurface CreateBicubic(double uMin, double uMax, int uCount, double vMin, double vMax, int vCount, InterpolatedFunction aproximationFunction, Derivation derivation)
//        {
//            return new HermiteSurface(uMin, uMax, uCount, vMin, vMax, vCount, aproximationFunction, derivation, HermiteType.Bicubic);
//        }
//
//        public static HermiteSurface CreateBiquartic(double uMin, double uMax, int uCount, double vMin, double vMax, int vCount, InterpolatedFunction aproximationFunction, Derivation derivation)
//        {
//            return new HermiteSurface(uMin, uMax, uCount, vMin, vMax, vCount, aproximationFunction, derivation, HermiteType.Biquartic);
//        }


//        protected HermiteSurface(double uMin, double uKnotsDistance, int uCount, double vMin,double vKnotsDistance, int vCount, InterpolatedFunction aproximationFunction,
//            Derivation derivation, HermiteType type)
//        {
//            _meshDensity = 0.1f;
//            _uMin = uMin;
//            _uKnotsDistance = uKnotsDistance;
//            _uCount = uCount;
//            _vMin = vMin;
//            _vKnotsDistance = vKnotsDistance;
//
//            _vCount = vCount;
//            _type = type;
//            Derivation = derivation;
//            GenerateMesh(aproximationFunction);
//        }

        public void ColoredSimple(Color color)
        {
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
                var fromHue = ((segment.MinHeight - minZ)/dZ)*300f;
                var toHue = ((segment.MaxHeight - minZ)/dZ)*300f;
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
                var frHue = ((segment.MinHeight - minZ)/dZ)*toHue - fromHue;
                var tHue = ((segment.MaxHeight - minZ)/dZ)*toHue;
                segment.ColoredHeight(frHue, tHue);
            }
        }

        public float MinHeight
        {
            get
            {
                if (!_minHeight.HasValue)
                    _minHeight = Segments.Min(segment => segment.MinHeight);
                //_vertices.Min(vertex => vertex.Position.Z);
                return _minHeight.Value;
            }
            protected set { _minHeight = value; }
        }

        public float MaxHeight
        {
            get
            {
                if (!_maxHeight.HasValue) _maxHeight = Segments.Max(segment => segment.MaxHeight);
                return _maxHeight.Value;
            }
            protected set { _maxHeight = value; }
        }

        public virtual void Draw()
        {
            for (var i = 0; i < Segments.Count; i++)
            {
                Segments[i].Draw();
            }
        }

//        protected void GenerateMesh(InterpolatedFunction aproximationFunction)
//        {
//            var generator = HermiteMeshGeneratorFactory.CreateHermiteMeshGenerator(this,aproximationFunction);
//            Segments = generator.CreateMesh();
//
//            //Segments = HermiteMeshGeneratorFactory.CreateHermiteMeshGenerator(this, aproximationFunction).CreateMesh();
//        }

        public virtual void ColoredBySegment()
        {
            //            var colors = new[] { new Color(237, 28, 36), new Color(255, 127, 39), new Color(255,242,0), new Color(34, 177, 76), 
            //                new Color(63, 72, 204), new Color(163, 73, 164)};

            //new Color(255, 201, 14)};
            //var colors = new[] { new Color(237, 28, 36), new Color(6, 128, 64), new Color(63, 72, 204), new Color(255, 201, 14) };
            // new Color(63, 72, 204), new Color(163, 73, 164)};

            Segments.ForEach(segment => segment.ColoredSimple(ColorUtils.Random()));

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
//            }
        }

        public void ColoredSimple(int r, int g, int b, int a)
        {
            ColoredSimple(Color.FromNonPremultiplied(r, g, b, a));
        }
    }
}